using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityStandardAssets.Water;

public class SurfaceTextureRenderer
{
	public void RenderMap(MapData data, MapParent mapParent, Material basingMatPrefab, Material surfaceMatPrefab, Material waterMatPrefab)
	{
		List<Material> mats = new List<Material>();

		foreach (KeyValuePair<Color, Texture2D> terrainOption in data.TerrainLookup)
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

		mats.Add(new Material(basingMatPrefab));

		mapParent.GroundParent.GetComponent<MeshRenderer>().materials = mats.ToArray();
		
		var waterScript = mapParent.GroundParent.AddComponent<WaterBasic>();
		waterScript.matNumber = mats.Count - 2;
	}
}