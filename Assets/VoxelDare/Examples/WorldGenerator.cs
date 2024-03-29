using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class WorldGenerator : MonoBehaviour
{
	public int worldSize = 64;
	public int worldHeight = 16;

	const int WATER_HEIGHT = 4;
	const float XY_SCALE = 2.0f;
	
	Perlin perlin;

	VoxelDare.Voxel vAir, vWater, vStone, vBedRock;
		
	public WorldGenerator()
	{
		perlin = new Perlin();
		vAir = VoxelDare.Voxel.Empty;
		vStone = new VoxelDare.Voxel(VoxelDare.Voxel.Solidness.Normal, Color.white/*new Color(0.92f,0.92f,0.92f)*/, 1);
		vBedRock = new VoxelDare.Voxel(VoxelDare.Voxel.Solidness.Ultra, Color.white/*new Color(0.21f,0.22f,0.22f)*/, 3);
		vWater = new VoxelDare.Voxel(VoxelDare.Voxel.Solidness.Soft, Color.white/*new Color(0.35f,0.50f,0.75f)*/, 2);
	}

	VoxelDare.Voxel FMinecraft(int x, int y, int z)
	{
		const float XY_SCALE = 1.3f;
		float q = Mathf.Max(0, 5.0f*(1.0f+perlin.Compute(XY_SCALE*x,XY_SCALE*y,0)));
		if(z > q) {
			if(z < WATER_HEIGHT) {
				return vWater;
			}
			else {
				return vAir;
			}
		}
		else {
			return vStone;
		}
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

	public VoxelDare.World CreateRollingHills(int size, int height)
	{
		int radius = size/2;
		return Create(
			new VoxelDare.Int3(-radius,-radius,0), new VoxelDare.Int3(radius,radius,height),
			Vector3.one,
			(x,y,z) => FMinecraft(x,y,z));
	}

	public void ClearWorld()
	{
		GetComponent<VoxelDare.VoxelRenderer>().Voxels = new VoxelDare.World(Vector3.one);
	}

	public void GenerateWorld()
	{
		GetComponent<VoxelDare.VoxelRenderer>().Voxels = CreateRollingHills(worldSize, worldHeight);
	}

	void Start()
	{
	}

	void Update()
	{
	}

}
