using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityStandardAssets.Water;

public class SurfaceTextureRenderer
{
	public void RenderMap(MapData data, MapParent mapParent, Material surfaceMatPrefab, Material waterMatPrefab)
	{
		Dictionary<Color, Texture2D> terrainLookup = ParseTerrainLookup();
		
		List<Material> mats = new List<Material>();

		foreach (KeyValuePair<Color, Texture2D> terrainOption in terrainLookup)
		{
			var mat = new Material(surfaceMatPrefab);
			mat.SetTexture("_LookupTex", data.TerrainMap);
			mat.SetColor("_LookupColor", terrainOption.Key);
			mat.SetTexture("_GroundTex", terrainOption.Value);
			mats.Add(mat);
		}

		var water = new Material(waterMatPrefab);
		water.SetTexture("_MaskTex", data.TerrainMap);
		mats.Add(water);

		mapParent.GroundParent.GetComponent<MeshRenderer>().materials = mats.ToArray();
		
		var waterScript = mapParent.GroundParent.AddComponent<WaterBasic>();
		waterScript.matNumber = mats.Count - 1;
	}

	private Dictionary<Color, Texture2D> ParseTerrainLookup()
	{
		Dictionary<Color, Texture2D> terrainLookup = new Dictionary<Color, Texture2D>();

		var file = Path.Combine(Path.Combine(Application.streamingAssetsPath, "TerrainTextures"), "TerrainTextureLookup.txt");
		var terrainLookupTxt = File.ReadAllText(file);
		List<string> terrainOptions = terrainLookupTxt.Split('\n').ToList();
		foreach (string option in terrainOptions)
		{
			var optionParts = option.Split(':');
			string color = optionParts[0].Trim();
			string texturePath = Path.Combine(Path.Combine(Application.streamingAssetsPath, "TerrainTextures"), optionParts[1].Trim() + ".png");

			Color resColor;
			ColorUtility.TryParseHtmlString(color, out resColor);

			var texFileData = File.ReadAllBytes(texturePath);
			var resTex = new Texture2D(2, 2);
			resTex.LoadImage(texFileData);

			terrainLookup[resColor] = resTex;
		}

		return terrainLookup;
	}
}