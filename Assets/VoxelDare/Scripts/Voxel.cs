using UnityEngine;
using System.Collections.Generic;
using System;

namespace VoxelDare
{
	[Serializable]
	public class Voxel
	{
		public const int TEX_ATLAS_SIZE = 4;

		public enum Solidness { Air, Soft, Normal, Ultra };

		public Solidness solid;
		public Color32 color;
		public int texId = 0;

		public bool IsAir { get { return solid == Solidness.Air; } }
		public bool IsSolid { get { return !IsAir; } }

		public static Voxel Empty = new Voxel(Solidness.Air, Color.white, 0);

		public Voxel()
		{}

		public Voxel(Solidness solid, Color color, int texId)
		{
			this.solid = solid;
			this.color = color;
			this.texId = texId;
		}
	}
}
