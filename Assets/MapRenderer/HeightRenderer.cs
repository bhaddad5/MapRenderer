using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class HeightRenderer : IMapRenderer
{
	public void RenderMap(MapData data, MapParent mapParent)
	{
		List<Vector3> vertices = SetVerticesFromHeights(data.HeightMap);
		List<int> indices = SetTriangles(data.HeightMap.GetLength(0), data.HeightMap.GetLength(1));
		List<Vector2> uvCoords = SetUVs(data.HeightMap.GetLength(0), data.HeightMap.GetLength(1));

		Mesh mesh = new Mesh();
		mesh.indexFormat = IndexFormat.UInt32;
		mesh.vertices = vertices.ToArray();
		mesh.triangles = indices.ToArray();
		mesh.uv = uvCoords.ToArray();

		mesh.RecalculateNormals();

		mapParent.GroundParent.GetComponent<MeshFilter>().mesh = mesh;
	}

	private static List<Vector3> SetVerticesFromHeights(float[,] heights)
	{
		List<Vector3> vertices = new List<Vector3>();
		
		for (int x = 0; x < heights.GetLength(0); x += 1)
		{
			for (int y = 0; y < heights.GetLength(1); y += 1)
			{
				vertices.Add(new Vector3(x, heights[x, y] * 25f, y));
			}
		}

		return vertices;
	}

	//FROM: http://answers.unity3d.com/questions/667029/convert-an-array-of-points-into-a-mesh-generate-tr.html
	private static List<int> SetTriangles(int width, int height)
	{
		List<int> indices = new int[width * height * 6].ToList();
		int index = 0;
		for (int w = 0; w < width - 1; w++)
		{
			for (int h = 0; h < height - 1; h++)
			{
				indices[index + 0] = h + w * height;
				indices[index + 1] = h + 1 + w * height + height;
				indices[index + 2] = h + w * height + height;

				indices[index + 3] = h + w * height;
				indices[index + 4] = h + 1 + w * height;
				indices[index + 5] = h + 1 + w * height + height;

				index += 6;
			}
		}

		return indices;
	}

	private static List<Vector2> SetUVs(int width, int height)
	{
		List<Vector2> uvCoords = new List<Vector2>();

		for (int w = 0; w < width; w++)
		{
			for (int h = 0; h < height; h++)
			{
				uvCoords.Add(new Vector2(w/(float)width, h/(float)height));
			}
		}

		return uvCoords;
	}
}