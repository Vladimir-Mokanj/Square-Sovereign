using FT.Managers;

namespace FT.Terrain;

public class TerrainIndices
{
	private static void AssignIndices(int[] indices, ref ushort idx, int a, int b, int c, int d)
	{
		indices[idx++] = a;
		indices[idx++] = b;
		indices[idx++] = c;
		indices[idx++] = a;
		indices[idx++] = d;
		indices[idx++] = b;
	}

	public int[] GenerateConnections(byte rows, byte cols/*, ref CellManager cellManager*/, float[] yHeight)
	{
		int[] indices = new int[rows * cols * 6];
		
		ushort index = 0;
		for (byte x = 0; x < rows; x++)
			for (byte z = 0; z < cols; z++)
			{
				int tl = x * (cols + 1) + z,
					tr = tl + 1,
					bl = tl + cols + 1,
					br = tl + cols + 2;

				
				//cellManager.InitializeCell(x, z, new[] { yHeight[tl], yHeight[tr], yHeight[bl], yHeight[br] });
				if (IsGreater(yHeight[br], yHeight[tl], yHeight[tr], yHeight[bl]) ||
					IsGreater(yHeight[tl], yHeight[tr], yHeight[br], yHeight[bl]) ||
					IsSmaller(yHeight[br], yHeight[tr], yHeight[tl], yHeight[bl]) ||
					IsSmaller(yHeight[tl], yHeight[tr], yHeight[bl], yHeight[br]))
				{
					AssignIndices(indices, ref index, tl, br, tr, bl);
					continue;
				}

				AssignIndices(indices, ref index, bl, tr, tl, br);
			}

		return indices;
	}
	
	private static bool IsGreater(float a, float b, float c, float d) => a > b && a > c && a > d;
	private static bool IsSmaller(float a, float b, float c, float d) => a < b && a < c && a < d;
}
