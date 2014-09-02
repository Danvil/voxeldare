using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VoxelDare
{
	public class Chunk
	{
		public const int S = 8;
		
		World world;

		Int3 pos;

		List<Voxel> voxels;

		HashSet<Int3> solidVoxels = new HashSet<Int3>();

		public bool Dirty { get; set; }

		public Chunk(World world, Int3 pos)
		{
			this.world = world;
			this.pos = S*pos;
			this.voxels = Enumerable.Repeat(Voxel.Empty, S*S*S).ToList();
			solidVoxels.Clear();
		}
		
		int ToIndex(Int3 p)
		{
			return p.x + S*(p.y + S*p.z);
		}

		Int3 FromIndex(int i)
		{
			int z = i / (S*S);
			int y = (i - S*z) / S;
			int x = i - S*(y + S*z);
			return new Int3(x,y,z);
		}

		public void Set(Int3 l, Voxel b)
		{
			int idx = ToIndex(l);
			voxels[idx] = b;
			if(b.IsSolid) {
				solidVoxels.Add(l);
			}
			else {
				solidVoxels.Remove(l);
			}
			Dirty = true;
		}

		public Voxel Get(Int3 l)
		{
			return voxels[ToIndex(l)];
		}

		public bool IsAir(Int3 l)
		{
			return voxels[ToIndex(l)].IsAir;
		}

		public bool IsSolid(Int3 l)
		{
			return !IsAir(l);
		}

		public Mesh CreateMesh(Vector3 scale)
		{
			MeshData md = new MeshData();
			int i = 0;
			for(int z=0; z<S; z++) {
				for(int y=0; y<S; y++) {
					for(int x=0; x<S; x++,i++) {
						Voxel b = voxels[i];
						if(b.IsSolid) {
							Color color = b.color;
							int texId = b.texId;
							Vector2 texCoord = new Vector2(texId%Voxel.TEX_ATLAS_SIZE, texId/Voxel.TEX_ATLAS_SIZE);
							Int3 w = pos + new Int3(x,y,z);
							if(!world.IsSolid(w + Int3.X)) {
								md.AddFace(w, 0, color, texCoord);
							}
							if(!world.IsSolid(w - Int3.X)) {
								md.AddFace(w, 1, color, texCoord);
							}
							if(!world.IsSolid(w + Int3.Y)) {
								md.AddFace(w, 4, color, texCoord);
							}
							if(!world.IsSolid(w - Int3.Y)) {
								md.AddFace(w, 5, color, texCoord);
							}
							if(!world.IsSolid(w + Int3.Z)) {
								md.AddFace(w, 2, color, texCoord);
							}
							if(!world.IsSolid(w - Int3.Z)) {
								md.AddFace(w, 3, color, texCoord);
							}
						}
					}
				}
			}
			return md.CreateMesh(scale);
		}

		public Mesh CreateMesh()
		{
			return CreateMesh(Vector3.one);
		}

		public IEnumerable<Int3> GetSolidVoxels()
		{
			return solidVoxels.AsEnumerable();
		}
	}
}
