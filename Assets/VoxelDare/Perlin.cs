using System;

/// <summary>
/// Improved perlin noise after Ken Perlin's reference implementation.
/// See http://mrl.nyu.edu/~perlin/noise/
/// Implemented 2014 by David Weikersdorfer
/// </summary>
public class Perlin
{
	private int[] permutation;
	private int[] p;
	
	public float Frequency { get; set; }
	
	public float Amplitude { get; set; }
	
	public float Persistence {
		get { return persistence.Length == 0 ? 0.0f : persistence[0]; }
		set { recomputePersistence(value, Octaves); }
	}
	
	public int Octaves {
		get { return persistence.Length; }
		set { recomputePersistence(Persistence, value); }
	}
	
	private float[] persistence;
	
	private void recomputePersistence(float a, int n) {
		persistence = new float[n];
		persistence[0] = 1.0f;
		for(int i=1; i<persistence.Length; i++) {
			persistence[i] = a;
		}
	}
	
	
	public Perlin()
	{
		Initialize();
		
		Frequency = 0.023f;
		Amplitude = 2.2f;
		recomputePersistence(0.9f, 2);
	}
	
	private void Initialize()
	{
		// random permutation
		permutation = new int[256];
		for(int i=0; i<permutation.Length; i++) {
			permutation[i] = i;
		}
		permutation.ShuffleInplace();
		
		// double it
		p = new int[permutation.Length*2];
		for(int i=0; i<permutation.Length; i++) {
			p[i] = permutation[i];
			p[permutation.Length + i] = permutation[i];
		}
	}
	
	public float Compute(UnityEngine.Vector3 v)
	{
		return Compute(v.x, v.y, v.z);
	}
	
	public float Compute(float x, float y, float z)
	{
		return Compute(x, y, z, this.persistence);
	}
	
	public float Compute(UnityEngine.Vector3 v, float[] persistence)
	{
		return Compute(v.x, v.y, v.z, persistence);
	}
	
	public float Compute(float x, float y, float z, float[] persistence)
	{
		float noise = 0;
		float amp = this.Amplitude;
		float freq = this.Frequency;
		for(int i=0; i<persistence.Length; i++) {
			amp *= persistence[i];
			noise += amp*Noise(freq*x, freq*y, freq*z);
			freq *= 2.0f;
		}
		return noise;
	}
	
	private float Noise(float x, float y, float z)
	{
		// find unit cube that contains point
		int ifx = UnityEngine.Mathf.FloorToInt(x);
		int ify = UnityEngine.Mathf.FloorToInt(y);
		int ifz = UnityEngine.Mathf.FloorToInt(z);
		
		// wrap to fit into entropy vector
		int iX = ifx & 255;
		int iY = ify & 255;
		int iZ = ifz & 255;
		
		// find relative x,y,z of point in cube
		x -= (float)ifx;
		y -= (float)ify;
		z -= (float)ifz;
		
		// compute fade curves for each of x,y,z
		float u = Fade(x);
		float v = Fade(y);
		float w = Fade(z);
		
		// hash coordinates of the 8 cube corners
		int A = p[iX  ] + iY;
		int AA = p[A  ] + iZ;
		int AB = p[A+1] + iZ;
		int B = p[iX+1] + iY;
		int BA = p[B  ] + iZ;
		int BB = p[B+1] + iZ;
		
		// add blended results from 8 corners of cube
		return
			Lerp(w, Lerp(v, Lerp(u, Grad(p[AA  ], x  , y  , z  ),
									Grad(p[BA  ], x-1, y  , z  )),
							Lerp(u, Grad(p[AB  ], x  , y-1, z  ),
									Grad(p[BB  ], x-1, y-1, z  ))),
					Lerp(v, Lerp(u, Grad(p[AA+1], x  , y  , z-1),
									Grad(p[BA+1], x-1, y  , z-1)),
							Lerp(u, Grad(p[AB+1], x  , y-1, z-1),
									Grad(p[BB+1], x-1, y-1, z-1))));
	}

	// smooth and differentiable fade function 0 -> 1
	private static float Fade(float t)
	{
		return t*t*t*(t*(t*6.0f - 15.0f) + 10.0f);
	}

	// linear interpolation
	private static float Lerp(float p, float a, float b)
	{
		return a + p*(b - a);
	}

	// convert lower 4 bits of hash code into 12 gradient directions
	private static float Grad(int hash, float x, float y, float z)
	{
		int h = hash & 15;
		float u = h < 8 ? x : y;
		float v = h < 4 ? y : (h == 12 || h == 14 ? x : z);
		return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
	}
}
