using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VoxelDare
{
	static public class CubeGenerator
	{
		static Vector3[] CUBE_NORMALS = new Vector3[] {
		    new Vector3(1,0,0),
		    new Vector3(-1,0,0),
		    new Vector3(0,1,0),
		    new Vector3(0,-1,0),
		    new Vector3(0,0,1),
		    new Vector3(0,0,-1),
		};

		static Vector3[,] CUBE_VERTICES = new Vector3[,] {
		    {
		        new Vector3(0.5f,-0.5f,-0.5f),
		        new Vector3(0.5f,0.5f,-0.5f),
		        new Vector3(0.5f,0.5f,0.5f),
		        new Vector3(0.5f,-0.5f,0.5f),
		    },
		    {
		        new Vector3(-0.5f,0.5f,-0.5f),
		        new Vector3(-0.5f,-0.5f,-0.5f),
		        new Vector3(-0.5f,-0.5f,0.5f),
		        new Vector3(-0.5f,0.5f,0.5f),
		    },
		    {
		        new Vector3(0.5f,0.5f,-0.5f),
		        new Vector3(-0.5f,0.5f,-0.5f),
		        new Vector3(-0.5f,0.5f,0.5f),
		        new Vector3(0.5f,0.5f,0.5f),
		    },
		    {
		        new Vector3(-0.5f,-0.5f,-0.5f),
		        new Vector3(0.5f,-0.5f,-0.5f),
		        new Vector3(0.5f,-0.5f,0.5f),
		        new Vector3(-0.5f,-0.5f,0.5f),
		    },
		    {
		        new Vector3(0.5f,-0.5f,0.5f),
		        new Vector3(0.5f,0.5f,0.5f),
		        new Vector3(-0.5f,0.5f,0.5f),
		        new Vector3(-0.5f,-0.5f,0.5f),
		    },
		    {
		        new Vector3(-0.5f,-0.5f,-0.5f),
		        new Vector3(-0.5f,0.5f,-0.5f),
		        new Vector3(0.5f,0.5f,-0.5f),
		        new Vector3(0.5f,-0.5f,-0.5f),
		    },
		};

		static Vector2[] CUBE_UV = new Vector2[] {
			new Vector2( 0.0f,  0.0f),
			new Vector2( 1.0f,  0.0f),
			new Vector2( 1.0f,  1.0f),
			new Vector2( 0.0f,  1.0f),
		};

		public static void AddFace(this MeshData md, Int3 posi, int a, Color color, Vector2 texCoord)
		{
			int n = md.vertices.Count;
			var pos = posi.ToVector3();
			for(int i=0; i<4; i++) {
				md.vertices.Add(CUBE_VERTICES[a,i] + pos);
				md.normals.Add(CUBE_NORMALS[a]);
				md.uv.Add((CUBE_UV[i] + texCoord)/(float)Voxel.TEX_ATLAS_SIZE);
				md.colors.Add(color);
			}	
			md.indices.Add(n  );
			md.indices.Add(n+1);
			md.indices.Add(n+2);
			md.indices.Add(n  );
			md.indices.Add(n+2);
			md.indices.Add(n+3);
		}

		public static void AddFace(this MeshData md, Int3 posi, int a)
		{
			md.AddFace(posi, a, Color.white, Vector2.zero);
		}

		public static void AddCube(this MeshData md, Int3 posi, Color color, Vector2 texCoord)
		{
			for(int a=0; a<6; a++) {
				md.AddFace(posi, a, color, texCoord);
			}
		}

		public static void AddCube(this MeshData md, Int3 posi)
		{
			md.AddCube(posi, Color.white, Vector2.zero);
		}
	}
}
