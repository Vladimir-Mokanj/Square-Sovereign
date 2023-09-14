using Godot;

namespace FT.Terrain;

public partial class GenerateTerrain : Node
{
	[Export] private bool isRandomColor;

	[Export] private int _seed = 123456789;
	[Export] private ushort _cellSize = 5;
	[Export] private ushort _rows = 10;
	[Export] private ushort _cols = 10;
	
	public override void _Ready()
	{
		// Generate vertices
		Vector3[] vertices = new TerrainVertices(_cellSize).GenerateVertexPositions(_rows, _cols, _seed);
		
		// Generate indices
		int[] indices = new TerrainIndices().GenerateConnections(_rows, _cols);

		// Generate normals
		Vector3[] normals = new TerrainNormals().GenerateVertexNormals(vertices.Length);
		
		// Generate Colors
		Color[] vertexColor = new TerrainColors().SetVertexColor((uint)vertices.Length, isRandomColor);
		
		// Generate Terrain
		GenerateMesh(vertices, indices, normals, vertexColor);
		GenerateWireframeMesh(new GenerateWireframe().GenerateAndConnectWireframeMesh(_rows, _cols, vertices));
	}
	
	private void GenerateMesh(Vector3[] vertices, int[] indices, Vector3[] normals, Color[] vertexColor)
	{
		MeshInstance3D meshInstance = new();
		AddChild(meshInstance);

		// Set the arrays into Godot's Array object
		Godot.Collections.Array arrays = new();
		arrays.Resize((int)Mesh.ArrayType.Max);
			
		arrays[(int)Mesh.ArrayType.Vertex] = vertices;
		arrays[(int)Mesh.ArrayType.Index] = indices;
		arrays[(int)Mesh.ArrayType.Normal] = normals;
		arrays[(int)Mesh.ArrayType.Color] = vertexColor;

		// Create the mesh surface
		ArrayMesh arrayMesh = new();
		arrayMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
			
		// Assign the ArrayMesh to the MeshInstance
		meshInstance.Mesh = arrayMesh;

		// Create a new StandardMaterial3D and set it as the surface material
		StandardMaterial3D material = new();
		material.VertexColorUseAsAlbedo = true;
		meshInstance.SetSurfaceOverrideMaterial(0, material);
	}

	private void GenerateWireframeMesh(Mesh arrayMesh)
	{
		MeshInstance3D wireframeMesh = new();
		AddChild(wireframeMesh);
		
		wireframeMesh.Mesh = arrayMesh;
	}
}
