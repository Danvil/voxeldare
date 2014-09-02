using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class WorldGenerator : MonoBehaviour
{
	public int worldSize = 32;
	public int worldHeight = 6;

	public enum WorldType { MINI_MINECRAFT, DISCWORLD };
	public WorldType worldType;

	Perlin perlin;

	VoxelEngine.Voxel vAir, vWater, vLand, vBedRock;
		
	public WorldGenerator()
	{
		perlin = new Perlin();
		vAir = VoxelEngine.Voxel.Empty;
		vWater = new VoxelEngine.Voxel(VoxelEngine.Voxel.Solidness.Soft, new Color(0.35f,0.50f,0.75f));
		vLand = new VoxelEngine.Voxel(VoxelEngine.Voxel.Solidness.Normal, new Color(0.92f,0.92f,0.92f));
		vBedRock = new VoxelEngine.Voxel(VoxelEngine.Voxel.Solidness.Ultra, new Color(0.21f,0.22f,0.22f));
	}

	public VoxelEngine.World Create(Int3 min, Int3 max, Vector3 scale, Func<int,int,int,VoxelEngine.Voxel> f)
	{
		VoxelEngine.World w = new VoxelEngine.World(scale);
		Int3 p = Int3.Zero;
		for(p.z=min.z; p.z<max.z; p.z++) {
			for(p.y=min.y; p.y<max.y; p.y++) {
				for(p.x=min.x; p.x<max.x; p.x++) {
					w.Set(p, f(p.x,p.y,p.z));
				}
			}
		}
		return w;
	}

	const int WATER_HEIGHT = 0; // 4
	const float XY_SCALE = 2.0f;

	VoxelEngine.Voxel FMiniMinecraft(int x, int y, int z)
	{
		float q = Mathf.Max(0, 5*(1+perlin.Compute(XY_SCALE*x,XY_SCALE*y,0)));
		if(z > q) {
			if(z < WATER_HEIGHT) {
				return vWater;
			}
			else {
				return vAir;
			}
		}
		else {
			return vLand;
		}
	}

	public VoxelEngine.World CreateMiniMinecraft(int size, int height)
	{
		return Create(
			new Int3(0,0,0), new Int3(size,size,height),
			Vector3.one, FMiniMinecraft);
	}

	VoxelEngine.Voxel FDiscworld(int x, int y, int z, int radius)
	{
		float r = Mathf.Sqrt(x*x + y*y);
		if(r > radius) {
			return vAir;
		}
		return FMiniMinecraft(x,y,z);
	}

	public VoxelEngine.World CreateDiscworld(int radius, int height)
	{
		Vector3 scale = Vector3.one;// new Vector3(4,4,4);
		// pass 1: solid
		VoxelEngine.World vw = Create(
			new Int3(-radius,-radius,0), new Int3(radius,radius,height),
			scale,
			(x,y,z) => FDiscworld(x,y,z,radius));
		// pass 2: bottom is bedrock
		foreach(Int3 i in vw.GetBottomVoxels()) {
			vw.Set(i, vBedRock);
		}
		return vw;
	}

	void Start()
	{
		VoxelEngine.World voxels = null;
		switch(worldType) {
			case WorldType.MINI_MINECRAFT: voxels = CreateMiniMinecraft(worldSize, worldHeight); break;
			case WorldType.DISCWORLD: voxels = CreateDiscworld(worldSize/2, worldHeight); break;
		};
		GetComponent<VoxelRenderer>().SetWorld(voxels);
	}

	void Update()
	{
	}

}
