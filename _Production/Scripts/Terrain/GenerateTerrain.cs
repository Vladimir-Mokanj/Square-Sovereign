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
	public byte[] GetCellMaxYVertexHeight => FindHighestYVertexOfTheCell();
	
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

	private byte[] FindHighestYVertexOfTheCell()
	{
		byte[] cellHeight = new byte[_tgd.Rows * _tgd.Cols];
		Array.Fill(cellHeight, _tgd.CellSize);
		
		for (byte x = 0; x < _tgd.Rows; x++)
		for (byte z = 0; z < _tgd.Cols; z++)
		{
			int index = x * _tgd.Cols + z;
			int tl = x * (_tgd.Cols + 1) + z,
				tr = tl + 1,
				bl = tl + _tgd.Cols + 1,
				br = tl + _tgd.Cols + 2;
				
			foreach (float height in new[]{vertices[tl].Y, vertices[tr].Y, vertices[bl].Y, vertices[br].Y})
			{
				if (Math.Abs(height - _tgd.CellSize) < 0.05f)
					continue;

				cellHeight[index] = (byte)height;
			}
		}

		return cellHeight;
	}
}