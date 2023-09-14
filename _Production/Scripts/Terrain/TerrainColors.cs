using Godot;

namespace FT.Terrain;

public class TerrainColors
{
	public Color[] SetVertexColor(uint vertexCount, bool isRandomColor)
	{
		Color[] vertices = new Color[vertexCount];
		for (uint x = 0; x < vertexCount; x++)
			vertices[x] = isRandomColor ? new Color(GD.Randf(), GD.Randf(), GD.Randf()) : Colors.Gray;
		
		return vertices;
	}
}