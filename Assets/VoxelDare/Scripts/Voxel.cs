using UnityEngine;
using System.Collections.Generic;

namespace VoxelDare
{
	public class Voxel
		: ISerialize
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


		#region IVoxelSerialize implementation

		public void Read(List<byte> data)
		{
			data.Add((byte)solid); // TODO
			data.Add(color.r);
			data.Add(color.g);
			data.Add(color.b);
			data.Add(color.a);
			data.Add((byte)texId); // TODO
		}

		public void Write(List<byte> data)
		{
			solid = (Solidness)data[0];
			color = new Color32(data[1], data[2], data[3], data[4]);
			texId = data[5];
		}

		#endregion

	}
}
