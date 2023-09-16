using System.Linq;
using FT.Data;
using Godot;

namespace FT.Terrain;

public class GenerateTerrain
{
	private readonly Vector3[] vertices;
	private readonly int[] indices;
	private readonly Vector3[] vertexNormals;
	private readonly Vector2[] uvs;

	public GenerateTerrain(TerrainGenerationData tgd)
	{
		vertices = new TerrainVertices(tgd.CellSize).GenerateVertexPositions(tgd.Rows, tgd.Cols, tgd.Seed);
		indices = new TerrainIndices().GenerateConnections(tgd.Rows, tgd.Cols, vertices.Select(vertex => vertex.Y).ToArray());
		vertexNormals = new TerrainNormals().GenerateVertexNormals(vertices.Length);
		uvs = new TerrainTexture().GenerateUVs(tgd.Rows, tgd.Cols);
	}
	
	public void GenerateMesh(Node parentNode, TerrainGenerationData tgd)
	{
		MeshInstance3D meshInstance = new();

		meshInstance.Mesh = GenerateArrayMesh();
		meshInstance.SetSurfaceOverrideMaterial(0, GenerateShaderMaterial(tgd));
		
		parentNode.AddChild(meshInstance);
	}

	private ArrayMesh GenerateArrayMesh()
	{
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

		return arrayMesh;
	}
	
	private ShaderMaterial GenerateShaderMaterial(TerrainGenerationData tgd)
	{
		ShaderMaterial shaderMaterial = new();
		shaderMaterial.Shader = tgd.Shader;
		shaderMaterial.SetShaderParameter(nameof(tgd.WaterTexture), tgd.WaterTexture);
		shaderMaterial.SetShaderParameter(nameof(tgd.DirtTexture), tgd.DirtTexture);
		shaderMaterial.SetShaderParameter(nameof(tgd.StoneTexture), tgd.StoneTexture);
		shaderMaterial.SetShaderParameter(nameof(tgd.CellSize), tgd.CellSize);

		return shaderMaterial;
	}
}
