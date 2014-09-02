using UnityEngine;

namespace VoxelDare
{
	[System.Serializable]
	public struct Int2
	{
		readonly public int x, y;

		public Int2(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public static Int2 Zero = new Int2(0,0);
		public static Int2 X = new Int2(1,0);
		public static Int2 Y = new Int2(0,1);

		public Int2 Min(Int2 v)
		{ return new Int2(Mathf.Min(x,v.x), Mathf.Min(y,v.y)); }

		public Int2 Max(Int2 v)
		{ return new Int2(Mathf.Max(x,v.x), Mathf.Max(y,v.y)); }

		public override string ToString()
		{ return string.Format("[Int2 ({0},{1})]", x, y); }

		public static bool operator==(Int2 a, Int2 b)
		{ return a.x == b.x && a.y == b.y; }
		
		public static bool operator!=(Int2 a, Int2 b)
		{ return a.x != b.x && a.y != b.y; }
		
		public override bool Equals(object obj)
		{ return obj != null && obj is Int2 && this == (Int2)obj; }

		public override int GetHashCode()
		{ return x ^ y; }

		public static Int2 operator-(Int2 a) 
		{ return new Int2(-a.x, -a.y); }

		public static Int2 operator*(int s, Int2 a) 
		{ return new Int2(s*a.x, s*a.y); }

		public static Int2 operator*(Int2 a, int s) 
		{ return new Int2(s*a.x, s*a.y); }

		public static Int2 operator+(Int2 a, Int2 b) 
		{ return new Int2(a.x + b.x, a.y + b.y); }

		public static Int2 operator-(Int2 a, Int2 b) 
		{ return new Int2(a.x - b.x, a.y - b.y); }

		public static Vector3 operator+(Vector3 a, Int2 b) 
		{ return new Vector3(a.x + b.x, a.y + b.y, a.z); }
		
		public static Vector3 operator-(Vector3 a, Int2 b) 
		{ return new Vector3(a.x - b.x, a.y - b.y, a.z); }
		
		public static Vector3 operator+(Int2 a, Vector3 b) 
		{ return new Vector3(a.x + b.x, a.y + b.y, b.z); }
		
		public static Vector3 operator-(Int2 a, Vector3 b) 
		{ return new Vector3(a.x - b.x, a.y - b.y, -b.z); }
		
	};
}
