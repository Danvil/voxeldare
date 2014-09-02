using UnityEngine;

namespace VoxelDare
{
	public class Voxel
	{
		public enum Solidness { Air, Soft, Normal, Ultra };

		public Solidness solid;
		public Color color;

		public bool IsAir { get { return solid == Solidness.Air; } }
		public bool IsSolid { get { return !IsAir; } }

		public static Voxel Empty = new Voxel(Solidness.Air, Color.white);

		public Voxel(Solidness solid, Color color)
		{
			this.solid = solid;
			this.color = color;
		}
	}
}
