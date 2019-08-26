using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class MapDataParser
{
	public static MapData ParseMapData(string mapFolderName)
	{
		var folder = Path.Combine(Application.streamingAssetsPath, mapFolderName);

		var height = Path.Combine(folder, "height.png");
		var surface = Path.Combine(folder, "surface.png");

		MapData data = new MapData();

		var heightFileData = File.ReadAllBytes(height);
		var h = new Texture2D(2, 2);
		h.LoadImage(heightFileData);
		data.HeightMap = new float[h.width, h.height];
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

		data.TerrainLookup = ParseTerrainLookup();

		return data;
	}

	private static Dictionary<Color, Texture2D> ParseTerrainLookup()
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
