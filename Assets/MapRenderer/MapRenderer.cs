using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapRenderer : MonoBehaviour
{
	public string MapFolderName;

	public MapParent MapPrefab;

	public Material WaterMatPrefab;
	public Material TerrainTextureMatPrefab;

	private readonly HeightRenderer heightRenderer = new HeightRenderer();
	private readonly SurfaceTextureRenderer surfaceRenderer = new SurfaceTextureRenderer();
	private readonly DoodadRenderer doodadRenderer = new DoodadRenderer();

	void Start()
	{
		MapData data = LoadStoredMapData();
		var mapParent = GameObject.Instantiate(MapPrefab);

		heightRenderer.RenderMap(data, mapParent);
		surfaceRenderer.RenderMap(data, mapParent, TerrainTextureMatPrefab, WaterMatPrefab);
		doodadRenderer.RenderMap(data, mapParent);
	}

	private MapData LoadStoredMapData()
	{
		var folder = Path.Combine(Application.streamingAssetsPath, MapFolderName);

		var height = Path.Combine(folder, "height.png");
		var surface = Path.Combine(folder, "surface.png");

		MapData data = new MapData();

		var heightFileData = File.ReadAllBytes(height);
		var h = new Texture2D(2, 2);
		h.LoadImage(heightFileData);
		data.HeightMap = new float[h.width,h.height];
		for (int i = 0; i < h.width; i++)
		{
			for (int j = 0; j < h.height; j++)
			{
				data.HeightMap[i, j] = h.GetPixel(i, j).r;
			}
		}

		var surfFileData = File.ReadAllBytes(surface);
		var s = new Texture2D(2, 2);
		s.LoadImage(surfFileData);
		data.TerrainMap = s;

		data.Size = new Vector2(h.width, h.height);

		return data;
	}
}
