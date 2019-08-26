using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MapData
{
	public Vector2 Size;

	public float[,] HeightMap;
	public Texture2D TerrainMap;
	public Dictionary<Color, Texture2D> TerrainLookup;
}