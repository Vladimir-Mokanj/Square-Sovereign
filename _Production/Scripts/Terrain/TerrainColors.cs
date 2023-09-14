using Godot;

namespace FT.Terrain;

public class TerrainColors
{
	public Color[] SetVertexColor(uint vertexCount, float[] vertexHeight)
	{
		Color[] vertices = new Color[vertexCount];
		for (uint i = 0; i < vertexCount; i++)
			vertices[i] =  SetVertexColor(vertexHeight[i]);
		
		return vertices;
	}

	private Color SetVertexColor(float vertexHeight) =>
		vertexHeight switch
		{
			< 0 => Colors.Aqua,
			> 0 => Colors.Brown,
			_ => Colors.Green
		};
}