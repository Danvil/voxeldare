using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VoxelDare
{

	[AddComponentMenu("VoxelDare/VoxelRenderer"), ExecuteInEditMode]
	public class VoxelRenderer : MonoBehaviour, ISerializationCallbackReceiver
	{
		public GameObject pfVoxelChunk;

		public Material pfVoxelMaterial;

		bool needsChunkCreate;

		[SerializeField]
		VoxelDare.World voxels;
		public VoxelDare.World Voxels
		{
			get { return voxels; }
			set {
				this.voxels = value;
				needsChunkCreate = true;
				this.transform.localPosition = Vector3.zero;
			}
		}

		Dictionary<Int3,GameObject> chunks;

		[SerializeField]
		List<Int3> tmp_chunks_keys;
		[SerializeField]
		List<GameObject> tmp_chunks_values;

		VoxelRenderer()
		{
		}

		#region ISerializationCallbackReceiver implementation

		public void OnBeforeSerialize()
		{
			tmp_chunks_keys = chunks.Keys.ToList();
			tmp_chunks_values = chunks.Values.ToList();
			Debug.Log("OnBeforeSerialize"); 
		}

		public void OnAfterDeserialize()
		{
			chunks = new Dictionary<VoxelDare.Int3,GameObject>();
			for(int i=0; i<tmp_chunks_keys.Count; i++) {
				chunks[tmp_chunks_keys[i]] = tmp_chunks_values[i];
			}
			needsChunkCreate = false;
			Debug.Log("OnAfterDeserialize");
		}

		#endregion

		void CreateChunks()
		{
			// clear old
			if(chunks != null) {
				foreach(var p in chunks) {
					if(Application.isPlaying)
						Destroy(p.Value);
					if(Application.isEditor)
						DestroyImmediate(p.Value);
				}
			}
			chunks = new Dictionary<VoxelDare.Int3,GameObject>();
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

		void RefreshChunks()
		{
			foreach(var p in voxels.RecreateDirty()) {
				SetMesh(chunks[p.Key], p.Value);
			}
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
			needsChunkCreate = true;
		}			

		void Update()
		{
			if(needsChunkCreate) {
				CreateChunks();
			}
			else {
				RefreshChunks();
			}
		}
	}

}
