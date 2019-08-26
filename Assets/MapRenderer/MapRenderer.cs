using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class MapRenderer : MonoBehaviour
{
	public string MapFolderName;

	public MapParent MapPrefab;

	public Material BasingMatPrefab;
	public Material TerrainTextureMatPrefab;
	public Material WaterMatPrefab;


	private readonly HeightRenderer heightRenderer = new HeightRenderer();
	private readonly SurfaceTextureRenderer surfaceRenderer = new SurfaceTextureRenderer();
	private readonly DoodadRenderer doodadRenderer = new DoodadRenderer();

	void Start()
	{
		MapData data = MapDataParser.ParseMapData(MapFolderName);
		var mapParent = GameObject.Instantiate(MapPrefab);

		heightRenderer.RenderMap(data, mapParent);
		surfaceRenderer.RenderMap(data, mapParent, BasingMatPrefab, TerrainTextureMatPrefab, WaterMatPrefab);
		doodadRenderer.RenderMap(data, mapParent);
	}
}
