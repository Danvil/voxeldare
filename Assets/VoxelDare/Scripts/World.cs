using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace VoxelDare
{
	[Serializable]
	public class World
		: ISerializationCallbackReceiver
	{
		Dictionary<Int3,Chunk> chunks;

		[SerializeField]
		List<Chunk> _tmp_chunks;

		[SerializeField]
		Vector3 scale = Vector3.one;

		[SerializeField]
		Int3 min = Int3.Zero;
		[SerializeField]
		Int3 max = Int3.Zero;

		public World(Vector3 scale)
		{
			chunks = new Dictionary<Int3,Chunk>();
			this.scale = scale;
		}

		void Split(int w, out int c, out int l)
		{
			if(w >= 0) {
				c = w/Chunk.S;
			}
			else {
				c = (w + 1)/Chunk.S - 1;
			}
			l = w - Chunk.S*c;
			//Debug.Log(string.Format("w={0} c={1} l={2}", w, c, l));
		}

		void Split(Int3 w, out Int3 c, out Int3 l)
		{
			int cx, cy, cz, lx, ly, lz;
			Split(w.x, out cx, out lx);
			Split(w.y, out cy, out ly);
			Split(w.z, out cz, out lz);
			c = new Int3(cx,cy,cz);
			l = new Int3(lx,ly,lz);
		}

		public int Combine(int c, int l)
		{
			return Chunk.S*c + l;
		}

		public Int3 Combine(Int3 c, Int3 l)
		{
			return new Int3(
				Combine(c.x, l.x),
				Combine(c.y, l.y),
				Combine(c.z, l.z)
			);
		}

		public bool Dirty
		{
			get {
				return chunks.Values.Any(c => c.Dirty);
			}
			set {
				foreach(var c in chunks.Values) {
					c.Dirty = value;
				}
			}
		}

		public void Set(Int3 p, Voxel b)
		{			
			min = min.Min(p);
			max = max.Max(p);
			Int3 c, l;
			Split(p, out c, out l);
			if(!chunks.ContainsKey(c)) {
				chunks[c] = new Chunk(this, c);
			}
			chunks[c].Set(l,b);
			// mark neighbours as dirty
			Chunk chunk = null;
			if(l.x == Chunk.S-1 && chunks.TryGetValue(c+Int3.X, out chunk)) {
				chunk.Dirty = true;
			}
			if(l.x == 0 && chunks.TryGetValue(c-Int3.X, out chunk)) {
				chunk.Dirty = true;
			}
			if(l.y == Chunk.S-1 && chunks.TryGetValue(c+Int3.Y, out chunk)) {
				chunk.Dirty = true;
			}
			if(l.y == 0 && chunks.TryGetValue(c-Int3.Y, out chunk)) {
				chunk.Dirty = true;
			}
			if(l.z == Chunk.S-1 && chunks.TryGetValue(c+Int3.Z, out chunk)) {
				chunk.Dirty = true;
			}
			if(l.z == 0 && chunks.TryGetValue(c-Int3.Z, out chunk)) {
				chunk.Dirty = true;
			}
		}

		public Voxel Get(Int3 p)
		{
			Int3 c, l;
			Split(p, out c, out l);
			if(!chunks.ContainsKey(c)) {
				return Voxel.Empty;
			}
			else {
				return chunks[c].Get(l);
			}
		}

		public bool IsSolid(Int3 p)
		{
			Int3 c, l;
			Split(p, out c, out l);
			if(!chunks.ContainsKey(c)) {
				return false;
			}
			else {
				return chunks[c].Get(l).IsSolid;
			}
		}

		public Dictionary<Int3,Mesh> RecreateDirty()
		{
			Dictionary<Int3,Mesh> result = new Dictionary<Int3,Mesh>();
			if(!Dirty) {
				return result;
			}
			foreach(var p in chunks) {
				if(p.Value.Dirty) {
					result[p.Key] = p.Value.CreateMesh(scale);
					p.Value.Dirty = false;
				}
			}
			Debug.Log(string.Format("Voxels: Recreated {0}/{1} chunks", result.Count, chunks.Count));
			return result;
		}

		public Dictionary<Int3,Mesh> CreateAll()
		{
			Dirty = true;
			return RecreateDirty();
		}

		public IEnumerable<Int3> GetSolidVoxels()
		{
			return chunks.SelectMany(x => x.Value.GetSolidVoxels().Select(l => Combine(x.Key, l)));
		}


		#region ISerializationCallbackReceiver implementation

		public void OnBeforeSerialize()
		{
			_tmp_chunks = chunks.Values.ToList();
		}

		public void OnAfterDeserialize()
		{
			chunks = new Dictionary<Int3, Chunk>();
			foreach(Chunk c in _tmp_chunks) {
				chunks[c.Position] = c;
				c.World = this;
			}
		}

		#endregion


		public IEnumerable<Int3> GetBottomVoxels()
		{
			Dictionary<Int2,Int3> bottom = new Dictionary<Int2,Int3>();
			foreach(Int3 p in GetSolidVoxels()) {
				Int2 key = p.xy;
				Int3 current;
				if(bottom.TryGetValue(key, out current)) {
					if(current.z > p.z) {
						bottom[key] = p;
					}
				}
				else {
					bottom[key] = p;
				}
			}
			return bottom.Values.AsEnumerable();
		}

		public IEnumerable<Int3> GetTopVoxels()
		{
			Dictionary<Int2,Int3> topVoxels = new Dictionary<Int2,Int3>();
			foreach(Int3 p in GetSolidVoxels()) {
				Int2 key = p.xy;
				Int3 current;
				if(topVoxels.TryGetValue(key, out current)) {
					if(current.z < p.z) {
						topVoxels[key] = p;
					}
				}
				else {
					topVoxels[key] = p;
				}
			}
			return topVoxels.Values.AsEnumerable();
		}

		public bool TryGetTopVoxel(Int2 p2, out Int3 top)
		{
			for(int z=max.z; z>=min.z; z--) {
				Int3 p = new Int3(p2.x, p2.y, z);
				if(IsSolid(p)) {
					top = p;
					return true;
				}
			}
			top = new Int3(p2.x, p2.y, int.MinValue);
			return false;
		}

		public bool HasTopVoxel(Int3 p)
		{
			Int3 top;
			return TryGetTopVoxel(p, out top);
		}

		public bool HasTopVoxel(Vector3 p)
		{
			Int3 top;
			return TryGetTopVoxel(p.ToInt3(), out top);
		}
		
		public bool TryGetTopVoxel(Int3 p, out Int3 top)
		{
			return TryGetTopVoxel(p.xy, out top);
		}

		public bool IsTopVoxel(Int3 p)
		{
			Int3 top;
			TryGetTopVoxel(p.xy, out top);
			return top == p;
		}

		public bool IsTopVoxelOrHigher(Int3 p)
		{
			Int3 top;
			TryGetTopVoxel(p.xy, out top);
			return p.z >= top.z;
		}

		public int GetTopVoxelHeight(Int3 p)
		{
			Int3 top;
			TryGetTopVoxel(p.xy, out top);
			return top.z;
		}

		public int GetTopVoxelHeight(Vector3 p)
		{
			Int3 pi = p.ToInt3();
			Int3 top;
			TryGetTopVoxel(pi.xy, out top);
			return top.z;
		}
	}
}
