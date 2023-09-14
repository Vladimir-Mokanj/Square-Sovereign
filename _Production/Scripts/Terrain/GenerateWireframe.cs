using Godot;

namespace FT.Terrain;

public class GenerateWireframe
{
	public Mesh GenerateAndConnectWireframeMesh(ushort rows, ushort cols, Vector3[] vertices)
	{
		SurfaceTool st = new();
		st.Begin(Mesh.PrimitiveType.Lines);
		
		for (ushort x = 0; x < rows; x++)
			for (ushort z = 0; z < cols; z++)
			{
				int tl = x * (cols + 1) + z;
				int tr = tl + 1,
					bl = tl + cols + 1,
					br = tl + cols + 2;
									
				st.AddVertex(vertices[bl]);
				st.AddVertex(vertices[tl]);
	
				st.AddVertex(vertices[tl]);
				st.AddVertex(vertices[tr]);
	
				st.AddVertex(vertices[tr]);
				st.AddVertex(vertices[bl]);
				
				st.AddVertex(vertices[tr]);
				st.AddVertex(vertices[br]);
				
				st.AddVertex(vertices[br]);
				st.AddVertex(vertices[bl]);
			}

		return st.Commit();
	}
}
