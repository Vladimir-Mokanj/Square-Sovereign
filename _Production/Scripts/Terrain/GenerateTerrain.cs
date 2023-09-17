using System;
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
	
	/// Get highest possible y vertex of every cell  
	public float[] GetCellMaxYVertexHeight => FindHighestYVertexOfTheCell();
	
	/// Generate the procedural terrain
	/// <param name="parentNode">Parent node for the generated mesh</param>
	public void GenerateMesh(Node parentNode)
	{
		MeshInstance3D meshInstance = new();
		meshInstance.Mesh = GenerateArrayMesh();
		meshInstance.SetSurfaceOverrideMaterial(0, GenerateShaderMaterial());
		
		parentNode.AddChild(meshInstance);
	}

	private ArrayMesh GenerateArrayMesh()
	{
		Godot.Collections.Array arrays = new();
		arrays.Resize((int)Mesh.ArrayType.Max);
			
		arrays[(int)Mesh.ArrayType.Vertex] = vertices;
		arrays[(int)Mesh.ArrayType.Index] = indices;
		arrays[(int)Mesh.ArrayType.Normal] = vertexNormals;
		arrays[(int)Mesh.ArrayType.TexUV] = uvs;
		
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

	private float[] FindHighestYVertexOfTheCell()
	{
		const float tolerance = 0.05f;
		int rows = _tgd.Rows;
		int cols = _tgd.Cols;
		float cellSize = _tgd.CellSize;
    
		float[] cellHeight = new float[rows * cols];
		Array.Fill(cellHeight, cellSize);

		int[] offsets = {0, 1, cols + 1, cols + 2};
		for (byte x = 0, idx = 0; x < rows; x++)
			for (byte z = 0; z < cols; z++, idx++)
			{
				int tl = x * (cols + 1) + z;
				foreach (int offset in offsets)
				{
					float height = vertices[tl + offset].Y;
					if (!(Math.Abs(height - cellSize) >= tolerance)) 
						continue;
					
					cellHeight[idx] = height;
					break;
				}
			}
		
		return cellHeight;
	}
}