namespace FT.Terrain;

public class TerrainIndices
{
	public int[] GenerateConnections(ushort rows, ushort cols)
	{
		int[] indices = new int[rows * cols * 6];

		uint index = 0;
		for (ushort x = 0; x < rows; x++)
			for (ushort z = 0; z < cols; z++)
			{
				int tl = x * (cols + 1) + z,
					tr = tl + 1,
					bl = tl + cols + 1,
					br = tl + cols + 2;
								
				indices[index] = bl;
				indices[index + 1] = tr;
				indices[index + 2] = tl;
				indices[index + 3] = bl;
				indices[index + 4] = br;
				indices[index + 5] = tr;
				index += 6;
			}

		return indices;
	}
}
