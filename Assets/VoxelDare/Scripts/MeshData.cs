using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VoxelDare
{
	public class MeshData
	{
		public List<Vector3> vertices = new List<Vector3>();
		public List<Vector3> normals = new List<Vector3>();
		public List<Vector2> uv = new List<Vector2>();
		public List<Color> colors = new List<Color>();
		public List<int> indices = new List<int>();

		public void Clear()
		{
			vertices.Clear();
			normals.Clear();
			uv.Clear();
			colors.Clear();
			indices.Clear();
		}

		public static Vector3 CwiseMult(Vector3 a, Vector3 b)
		{
			return new Vector3(a.x*b.x, a.y*b.y, a.z*b.z);
		}

		public Mesh CreateMesh(Vector3 scale)
		{
			Mesh mesh = new Mesh();
			for(int i=0; i<vertices.Count; i++) {
				vertices[i] = CwiseMult(vertices[i], scale);
			}
			mesh.vertices = vertices.ToArray();
			mesh.normals = normals.ToArray();
			mesh.uv = uv.ToArray();
			mesh.colors = colors.ToArray();
			mesh.triangles = indices.ToArray();
			return mesh;
		}

		public Mesh CreateMesh()
		{ return CreateMesh(Vector3.one); }

	}
}
