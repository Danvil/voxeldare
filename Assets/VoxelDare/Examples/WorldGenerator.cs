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

	VoxelDare.Voxel vAir, vWater, vLand, vBedRock;
		
	public WorldGenerator()
	{
		perlin = new Perlin();
		vAir = VoxelDare.Voxel.Empty;
		vWater = new VoxelDare.Voxel(VoxelDare.Voxel.Solidness.Soft, new Color(0.35f,0.50f,0.75f));
		vLand = new VoxelDare.Voxel(VoxelDare.Voxel.Solidness.Normal, new Color(0.92f,0.92f,0.92f));
		vBedRock = new VoxelDare.Voxel(VoxelDare.Voxel.Solidness.Ultra, new Color(0.21f,0.22f,0.22f));
	}

	public VoxelDare.World Create(VoxelDare.Int3 min, VoxelDare.Int3 max, Vector3 scale, Func<int,int,int,VoxelDare.Voxel> f)
	{
		VoxelDare.World w = new VoxelDare.World(scale);
		for(int z=min.z; z<max.z; z++) {
			for(int y=min.y; y<max.y; y++) {
				for(int x=min.x; x<max.x; x++) {
					w.Set(new VoxelDare.Int3(x,y,z), f(x,y,z));
				}
			}
		}
		return w;
	}

	const int WATER_HEIGHT = 0; // 4
	const float XY_SCALE = 2.0f;

	VoxelDare.Voxel FMiniMinecraft(int x, int y, int z)
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

	public VoxelDare.World CreateMiniMinecraft(int size, int height)
	{
		return Create(
			new VoxelDare.Int3(0,0,0), new VoxelDare.Int3(size,size,height),
			Vector3.one, FMiniMinecraft);
	}

	VoxelDare.Voxel FDiscworld(int x, int y, int z, int radius)
	{
		float r = Mathf.Sqrt(x*x + y*y);
		if(r > radius) {
			return vAir;
		}
		return FMiniMinecraft(x,y,z);
	}

	public VoxelDare.World CreateDiscworld(int radius, int height)
	{
		Vector3 scale = Vector3.one;// new Vector3(4,4,4);
		// pass 1: solid
		VoxelDare.World vw = Create(
			new VoxelDare.Int3(-radius,-radius,0), new VoxelDare.Int3(radius,radius,height),
			scale,
			(x,y,z) => FDiscworld(x,y,z,radius));
		// pass 2: bottom is bedrock
		foreach(VoxelDare.Int3 i in vw.GetBottomVoxels()) {
			vw.Set(i, vBedRock);
		}
		return vw;
	}

	void Start()
	{
		VoxelDare.World voxels = null;
		switch(worldType) {
			case WorldType.MINI_MINECRAFT: voxels = CreateMiniMinecraft(worldSize, worldHeight); break;
			case WorldType.DISCWORLD: voxels = CreateDiscworld(worldSize/2, worldHeight); break;
		};
		GetComponent<VoxelDare.VoxelRenderer>().Voxels = voxels;
	}

	void Update()
	{
	}

}
