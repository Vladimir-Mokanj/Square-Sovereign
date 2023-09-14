using Godot;

namespace FT.Terrain;

public class TerrainVertices
{
	private readonly float _cellSize;

	public TerrainVertices(float cellSize)
	{
		_cellSize = cellSize;
	}
	
	public Vector3[] GenerateVertexPositions(ushort rows, ushort cols, int seed)
	{
		FastNoiseLite noise = new();
		noise.Seed = seed;

		Vector3[] vertices = new Vector3[(rows + 1) * (cols + 1)];
		uint index = 0;
		for (ushort x = 0; x <= rows; x++)
			for (ushort z = 0; z <= cols; z++)
			{
				vertices[index] = new Vector3(x * _cellSize, GenerateVertexHeight(noise.GetNoise2D(x, z) + 1), z * _cellSize);
				index++;
			}
            
		return vertices;
	}

	private float GenerateVertexHeight(float height) =>
		height switch
		{
			< 0.8f => -_cellSize,
			< 0.9f => 0,
			_ => _cellSize
		};
}