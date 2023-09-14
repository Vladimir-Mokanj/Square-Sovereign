using Godot;

namespace FT.Terrain;

public class TerrainNormals
{
	public Vector3[] GenerateVertexNormals(int vertexCount)
	{
		Vector3[] normals = new Vector3[vertexCount];
		for (int i = 0; i < normals.Length; i++) {
			normals[i] = new Vector3(0, 1, 0);
		}

		return normals;
	}
}
