using System.Collections.Generic;
using System.Linq;
using FT.Data;
using FT.Managers;
using Godot;

namespace FT.Terrain;

public partial class GenerateTerrain : Node
{
	[Export] private TerrainGenerationData _tgd;
	[Export] private Shader _shader;

	public CellManager _cellManager;
	
	public override void _Ready()
	{
		_cellManager = new CellManager(_tgd.Rows, _tgd.Cols);
		
		// Generate vertices
		Vector3[] vertices = new TerrainVertices(_tgd.CellSize).GenerateVertexPositions(_tgd.Rows, _tgd.Cols, _tgd.Seed);
		
		// Generate indices
		int[] indices = new TerrainIndices().GenerateConnections(_tgd.Rows, _tgd.Cols, ref _cellManager, vertices.Select(vertex => vertex.Y).ToArray());

		// Generate normals
		Vector3[] vertexNormals = new TerrainNormals().GenerateVertexNormals(vertices.Length);

		// Generate UVs
		Vector2[] uvs = new TerrainTexture().GenerateUVs(_tgd.Rows, _tgd.Cols);

		// Generate Terrain
		GenerateMesh(vertices, indices, vertexNormals, uvs);
		
		if (_tgd.HasWireframe)
			GenerateWireframeMesh(new GenerateWireframe().GenerateAndConnectWireframeMesh(_tgd.Rows, _tgd.Cols, vertices));
	}
	
	private void GenerateMesh(Vector3[] vertices, int[] indices, Vector3[] vertexNormals, Vector2[] uvs)
	{
		MeshInstance3D meshInstance = new();
		AddChild(meshInstance);

		// Set the arrays into Godot's Array object
		Godot.Collections.Array arrays = new();
		arrays.Resize((int)Mesh.ArrayType.Max);
			
		arrays[(int)Mesh.ArrayType.Vertex] = vertices;
		arrays[(int)Mesh.ArrayType.Index] = indices;
		arrays[(int)Mesh.ArrayType.Normal] = vertexNormals;
		arrays[(int)Mesh.ArrayType.TexUV] = uvs;

		// Create the mesh surface
		ArrayMesh arrayMesh = new();
		arrayMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
			
		// Assign the ArrayMesh to the MeshInstance
		meshInstance.Mesh = arrayMesh;

		// Create a new StandardMaterial3D and set it as the surface material
		ShaderMaterial shaderMaterial = new();
		shaderMaterial.Shader = _shader;
		shaderMaterial.SetShaderParameter("waterTexture", _tgd.WaterTexture);
		shaderMaterial.SetShaderParameter("dirtTexture", _tgd.DirtTexture);
		shaderMaterial.SetShaderParameter("stoneTexture", _tgd.StoneTexture);
		shaderMaterial.SetShaderParameter("cellSize", _tgd.CellSize);
		
		meshInstance.SetSurfaceOverrideMaterial(0, shaderMaterial);
		
		// Create and configure CollisionShape
		CollisionShape3D collisionShape = new();
		ConcavePolygonShape3D shape = new();
		shape.Data = CreateTriangleListFromGrid(vertices, _tgd.Rows, _tgd.Cols); 
		collisionShape.Shape = shape;

		// Create StaticBody and add CollisionShape as a child
		StaticBody3D staticBody = new();
		AddChild(staticBody);
		staticBody.AddChild(collisionShape);
	}

	private void GenerateWireframeMesh(Mesh arrayMesh)
	{
		MeshInstance3D wireframeMesh = new();
		AddChild(wireframeMesh);
		
		wireframeMesh.Mesh = arrayMesh;
	}
	
	private Vector3[] CreateTriangleListFromGrid(Vector3[] gridVertices, int rows, int cols)
	{
		List<Vector3> triangleList = new();
    
		// Loop over each cell in the grid
		for (int x = 0; x < rows; x++)
		{
			for (int z = 0; z < cols; z++)
			{
				// Compute indices for the four corners of the current grid cell
				int topLeft = x * (cols + 1) + z;
				int topRight = topLeft + 1;
				int bottomLeft = (x + 1) * (cols + 1) + z;
				int bottomRight = bottomLeft + 1;

				// Create two triangles to fill the grid cell
				// Triangle 1 (top-left, bottom-left, top-right)
				triangleList.Add(gridVertices[topLeft]);
				triangleList.Add(gridVertices[bottomLeft]);
				triangleList.Add(gridVertices[topRight]);

				// Triangle 2 (top-right, bottom-left, bottom-right)
				triangleList.Add(gridVertices[topRight]);
				triangleList.Add(gridVertices[bottomLeft]);
				triangleList.Add(gridVertices[bottomRight]);
			}
		}
    
		return triangleList.ToArray();
	}
}
