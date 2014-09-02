using UnityEngine;

namespace VoxelDare
{
	public static class Int3Helper
	{
		public static Int3 ToInt3(this Vector3 v)
		{
			return new Int3(
				Mathf.FloorToInt(v.x),
				Mathf.FloorToInt(v.z),
				Mathf.FloorToInt(v.y));
		}
	}

	[System.Serializable]
	public struct Int3
	{
		readonly public int x, y, z;

		public Int3(int x, int y, int z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public static Int3 Zero = new Int3(0,0,0);
		public static Int3 X = new Int3(1,0,0);
		public static Int3 Y = new Int3(0,1,0);
		public static Int3 Z = new Int3(0,0,1);

		public Int2 xy { get{ return new Int2(x,y); } }
		public Int2 xz { get{ return new Int2(x,z); } }
		public Int2 yz { get{ return new Int2(y,z); } }

		public Vector3 ToVector3()
		{ return new Vector3(x,z,y); }

		public Int3 Min(Int3 v)
		{ return new Int3(Mathf.Min(x,v.x), Mathf.Min(y,v.y), Mathf.Min(z,v.z)); }

		public Int3 Max(Int3 v)
		{ return new Int3(Mathf.Max(x,v.x), Mathf.Max(y,v.y), Mathf.Max(z,v.z)); }

		public override string ToString()
		{ return string.Format("[Int3 ({0},{1},{2})]", x, y, z); }

		public static bool operator==(Int3 a, Int3 b)
		{ return a.x == b.x && a.y == b.y && a.z == b.z; }
		
		public static bool operator!=(Int3 a, Int3 b)
		{ return a.x != b.x && a.y != b.y && a.z != b.z; }
		
		public override bool Equals(object obj)
		{ return obj != null && obj is Int3 && this == (Int3)obj; }

		public override int GetHashCode()
		{ return x ^ y ^ z; }

		public static Int3 operator-(Int3 a) 
		{ return new Int3(-a.x, -a.y, -a.z); }

		public static Int3 operator*(int s, Int3 a) 
		{ return new Int3(s*a.x, s*a.y, s*a.z); }

		public static Int3 operator*(Int3 a, int s) 
		{ return new Int3(s*a.x, s*a.y, s*a.z); }

		public static Int3 operator+(Int3 a, Int3 b) 
		{ return new Int3(a.x + b.x, a.y + b.y, a.z + b.z); }

		public static Int3 operator-(Int3 a, Int3 b) 
		{ return new Int3(a.x - b.x, a.y - b.y, a.z - b.z); }

		public static Vector3 operator+(Vector3 a, Int3 b) 
		{ return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z); }
		
		public static Vector3 operator-(Vector3 a, Int3 b) 
		{ return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z); }
		
		public static Vector3 operator+(Int3 a, Vector3 b) 
		{ return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z); }
		
		public static Vector3 operator-(Int3 a, Vector3 b) 
		{ return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z); }

	};
}
