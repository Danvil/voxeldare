using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VoxelDare
{

	[AddComponentMenu("VoxelDare/VoxelRenderer")]
	public class VoxelRenderer : MonoBehaviour
	{
		public GameObject pfVoxelChunk;

		public Material pfVoxelMaterial;

		VoxelDare.World voxels;
		public VoxelDare.World Voxels
		{
			get { return voxels; }
			set
			{
				this.voxels = value;
				Create();
			}
		}

		Dictionary<Int3,GameObject> chunks = new Dictionary<VoxelDare.Int3,GameObject>();

		void Create()
		{
			// clear old
			foreach(var p in chunks) {
				Destroy(p.Value);
			}
			chunks.Clear();
			// get all meshes
			Dictionary<Int3,Mesh> meshes = voxels.CreateAll();
			// create gameobjects
			foreach(var p in meshes) {
				GameObject go = (GameObject)Instantiate(pfVoxelChunk);
				go.GetComponent<Renderer>().material = pfVoxelMaterial;
				SetMesh(go, p.Value);
				go.transform.parent = this.transform;
				go.transform.localPosition = new Vector3(0,0,0);
				chunks[p.Key] = go;
			}
			// combine
			StaticBatchingUtility.Combine(chunks.Values.ToArray(), this.gameObject);
		}

		static void SetMesh(GameObject go, Mesh mesh)
		{
			var meshFilter = go.GetComponent<MeshFilter>();
			if(meshFilter) {
				meshFilter.mesh = mesh;
			}
			var meshCollider = go.GetComponent<MeshCollider>();
			if(meshCollider) {
				meshCollider.sharedMesh = mesh;
			}
		}

		void Start()
		{
		}			

		void Update()
		{
			foreach(var p in voxels.RecreateDirty()) {
				SetMesh(chunks[p.Key], p.Value);
			}
		}
	}

}
