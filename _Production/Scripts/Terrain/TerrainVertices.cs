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
		noise.FractalOctaves = 4;
		noise.Frequency = 0.03f;

		Vector3[] vertices = new Vector3[(rows + 1) * (cols + 1)];
		ushort index = 0;
		for (byte x = 0; x <= rows; x++)
			for (byte z = 0; z <= cols; z++)
			{
				vertices[index] = new Vector3(x * _cellSize, GenerateVertexHeight(noise.GetNoise2D(x, z) + 1), z * _cellSize);
				index++;
			}
		
		int colsPlusOne = cols + 1;
		for (byte x = 0; x <= rows; x++)
			for (byte z = 0; z <= cols; z++)
			{
				int currentVertex = x * colsPlusOne + z;
				int top = x < rows ? (x + 1) * colsPlusOne + z : -1;
				int bottom = x > 0 ? (x - 1) * colsPlusOne + z : -1;
				int left = z > 0 ? x * colsPlusOne + (z - 1) : -1;
				int right = z < cols ? x * colsPlusOne + (z + 1) : -1;
	
				float valueTop = top == -1 ? 0 : vertices[top].Y;
				float valueBottom = bottom == -1 ? 0 : vertices[bottom].Y;
				float valueLeft = left == -1 ? 0 : vertices[left].Y;
				float valueRight = right == -1 ? 0 : vertices[right].Y;
	
				if ((valueTop == 0 && valueBottom == 0) || (valueLeft == 0 && valueRight == 0))
					vertices[currentVertex].Y = 0;
			}
		
		return vertices;
	}

	private float GenerateVertexHeight(float height) =>
		height switch
		{
			< 0.86f => 0,
			< 1.3f => _cellSize,
			_ => 2 * _cellSize
		};
}