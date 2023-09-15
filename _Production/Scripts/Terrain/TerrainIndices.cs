namespace FT.Terrain;

public class TerrainIndices
{
	public int[] GenerateConnections(ushort rows, ushort cols, float[] yHeight)
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


				if (yHeight[br] > yHeight[tl] && yHeight[br] > yHeight[tr] && yHeight[br] > yHeight[bl] || 
				    yHeight[tl] > yHeight[tr] && yHeight[tl] > yHeight[br] && yHeight[tl] > yHeight[bl] || 
				    yHeight[br] < yHeight[tr] && yHeight[br] < yHeight[tl] && yHeight[br] < yHeight[bl]) 
				    //yHeight[br] > yHeight[tl] && yHeight[br] > yHeight[tr] && yHeight[br] > yHeight[bl] || 
				    //yHeight[br] > yHeight[tl] && yHeight[br] > yHeight[tr] && yHeight[br] > yHeight[bl] || 
				    //yHeight[br] > yHeight[tl] && yHeight[br] > yHeight[tr] && yHeight[br] > yHeight[bl])
				{
					indices[index] = br;
					indices[index + 1] = tr;
					indices[index + 2] = tl;
					indices[index + 3] = br;
					indices[index + 4] = tl;
					indices[index + 5] = bl;
				}
				else
				{
					indices[index] = bl;
					indices[index + 1] = tr;
					indices[index + 2] = tl;
					indices[index + 3] = bl;
					indices[index + 4] = br;
					indices[index + 5] = tr;
				}

				index += 6;
			}

		return indices;
	}
}