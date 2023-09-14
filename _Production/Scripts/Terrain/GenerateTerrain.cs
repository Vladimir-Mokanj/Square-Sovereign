using System.Linq;
using FT.Data;
using Godot;

namespace FT.Terrain;

public partial class GenerateTerrain : Node
{
	[Export] private TerrainGenerationData _tgd;
	[Export] private Shader _shader;

	public override void _Ready()
	{
		// Generate vertices
		Vector3[] vertices = new TerrainVertices(_tgd.CellSize).GenerateVertexPositions(_tgd.Rows, _tgd.Cols, _tgd.Seed);
		
		// Generate indices
		int[] indices = new TerrainIndices().GenerateConnections(_tgd.Rows, _tgd.Cols);

		// Generate normals
		Vector3[] vertexNormals = new TerrainNormals().GenerateVertexNormals(vertices.Length);
		
		// Generate Colors
		//Color[] vertexColor = new TerrainColors().SetVertexColor((uint)vertices.Length, vertices.Select(vertex => vertex.Y).ToArray());
		
		// Generate Terrain
		GenerateMesh(vertices, indices, vertexNormals);
		GenerateWireframeMesh(new GenerateWireframe().GenerateAndConnectWireframeMesh(_tgd.Rows, _tgd.Cols, vertices));
	}
	
	private void GenerateMesh(Vector3[] vertices, int[] indices, Vector3[] vertexNormals)
	{
		MeshInstance3D meshInstance = new();
		AddChild(meshInstance);

		// Set the arrays into Godot's Array object
		Godot.Collections.Array arrays = new();
		arrays.Resize((int)Mesh.ArrayType.Max);
			
		arrays[(int)Mesh.ArrayType.Vertex] = vertices;
		arrays[(int)Mesh.ArrayType.Index] = indices;
		arrays[(int)Mesh.ArrayType.Normal] = vertexNormals;

		// Create the mesh surface
		ArrayMesh arrayMesh = new();
		arrayMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
			
		// Assign the ArrayMesh to the MeshInstance
		meshInstance.Mesh = arrayMesh;

		// Create a new StandardMaterial3D and set it as the surface material
		ShaderMaterial shaderMaterial = new();
		shaderMaterial.Shader = _shader;
		meshInstance.SetSurfaceOverrideMaterial(0, shaderMaterial);
	}

	private void GenerateWireframeMesh(Mesh arrayMesh)
	{
		MeshInstance3D wireframeMesh = new();
		AddChild(wireframeMesh);
		
		wireframeMesh.Mesh = arrayMesh;
	}
}
