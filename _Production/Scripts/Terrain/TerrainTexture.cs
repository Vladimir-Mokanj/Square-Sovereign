using Godot;

namespace FT.Terrain;

public class TerrainTexture
{
	public Vector2[] GenerateUVs(ushort rows, ushort cols)
	{
		Vector2[] uvs = new Vector2[(rows + 1) * (cols + 1)];
		uint index = 0;
		for (ushort x = 0; x <= rows; x++)
			for (ushort z = 0; z <= cols; z++)
			{
				uvs[index] = new Vector2(x, z);
				index++;
			}

		return uvs;
	}
}