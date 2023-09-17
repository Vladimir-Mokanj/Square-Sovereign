using System.Linq;
using FT.Data;
using Godot;

namespace FT.Terrain;

public class GenerateTerrain
{
	private readonly TerrainGenerationData _tgd;
	
	private readonly Vector3[] vertices;
	private readonly int[] indices;
	private readonly Vector3[] vertexNormals;
	private readonly Vector2[] uvs;

	public GenerateTerrain(ref TerrainGenerationData tgd)
	{
		_tgd = tgd;
		
		vertices = new TerrainVertices(tgd.CellSize).GenerateVertexPositions(tgd.Rows, tgd.Cols, tgd.Seed);
		indices = new TerrainIndices().GenerateConnections(tgd.Rows, tgd.Cols, vertices.Select(vertex => vertex.Y).ToArray());
		vertexNormals = new TerrainNormals().GenerateVertexNormals(vertices.Length);
		uvs = new TerrainTexture().GenerateUVs(tgd.Rows, tgd.Cols);
	}
	
	public float[] GetYHeights => vertices.Select(vertex => vertex.Y).ToArray();

	public void GenerateMesh(Node parentNode)
	{
		MeshInstance3D meshInstance = new();

		meshInstance.Mesh = GenerateArrayMesh();
		meshInstance.SetSurfaceOverrideMaterial(0, GenerateShaderMaterial());
		
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
	
	private ShaderMaterial GenerateShaderMaterial()
	{
		ShaderMaterial shaderMaterial = new();
		shaderMaterial.Shader = _tgd.Shader;
		shaderMaterial.SetShaderParameter(nameof(_tgd.WaterTexture), _tgd.WaterTexture);
		shaderMaterial.SetShaderParameter(nameof(_tgd.DirtTexture), _tgd.DirtTexture);
		shaderMaterial.SetShaderParameter(nameof(_tgd.StoneTexture), _tgd.StoneTexture);
		shaderMaterial.SetShaderParameter(nameof(_tgd.CellSize), _tgd.CellSize);

		return shaderMaterial;
	}
}
