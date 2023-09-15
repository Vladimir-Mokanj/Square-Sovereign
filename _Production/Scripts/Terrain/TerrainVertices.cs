using Godot;

namespace FT.Terrain;

public class TerrainVertices
{
	private readonly float _cellSize;

	public TerrainVertices(float cellSize)
	{
		_cellSize = cellSize;
	}
	
	public Vector3[] GenerateVertexPositions(byte rows, byte cols, int seed)
	{
		FastNoiseLite noise = new();
		noise.Seed = seed;
		
		Vector3[] vertices = new Vector3[(rows + 1) * (cols + 1)];
		ushort index = 0;
		for (byte x = 0; x <= rows; x++)
			for (byte z = 0; z <= cols; z++)
			{
				vertices[index] = new Vector3(x * _cellSize, GenerateVertexHeight(noise.GetNoise2D(x, z) + 1), z * _cellSize);
				index++;
			}
            
		return vertices;
	}

	private float GenerateVertexHeight(float height) =>
		height switch
		{
			< 0.8f => 0,
			< 0.9f => _cellSize,
			_ => 2 * _cellSize
		};
}