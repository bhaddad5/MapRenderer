using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IMapRenderer
{
	void RenderMap(MapData data, MapParent mapParent);
}