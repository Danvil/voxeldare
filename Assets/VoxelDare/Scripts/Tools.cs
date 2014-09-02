using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VoxelDare
{
	public static class Tools
	{
		public static Transform Search(this Transform target, string name)
		{
			if (target.name == name) return target;
			
			for (int i = 0; i < target.childCount; ++i)
			{
				var result = Search(target.GetChild(i), name);
				
				if (result != null) return result;
			}
			
			return null;
		}

		public static T FindBest<T>(this IEnumerable<T> items, Vector3 pos, System.Func<T,float> fval) where T : Component
		{
			T best = null;
			float score = 0.0f;
			foreach(T t in items) {
				float currentValue = fval(t);
				float currentDist = 1.0f + (t.transform.position.xz() - pos.xz()).magnitude;
				float currentScore = currentValue / currentDist;
				if(currentScore > score) {
					score = currentScore;
					best = t;
				}
			}
			return best;
		}

		public static Vector2 xz(this Vector3 a)
		{
			return new Vector2(a.x, a.z);
		}

		public static Vector3 CoeffMult(this Vector3 a, Vector3 b)
		{
			return new Vector3(a.x*b.x, a.y*b.y, a.z*b.z);
		}

		static System.Random random = new System.Random();

		public static int RandomIndex(int num)
		{
			return random.Next(num);
		}

		public static int Random(int a, int b)
		{
			return random.Next(a,b);
		}

		public static float Random01()
		{
			return UnityEngine.Random.value;
		}

		public static float Random(float a, float b)
		{
			return a + (b - a)*UnityEngine.Random.value;
		}

		public static Vector3 RandomInRing(float rmin, float rmax)
		{
			float a = Random(0, Mathf.PI);
			float r = Random(rmin, rmax);
			return r * new Vector3(Mathf.Cos(a), 0, Mathf.Sin(a));
		}

		public static bool ProbabilityTest(float p)
		{
			return Random01() <= p;
		}

		public static bool PoissonTest(float occurence)
		{
			float p = 1.0f - Mathf.Exp(-occurence);
			return ProbabilityTest(p);
		}

		public static bool PoissonTest(float dt, float rate)
		{
			return PoissonTest(dt * rate);
		}

		public static void ShuffleInplace<T>(this List<T> array)
		{
			int n = array.Count;
			while (n > 1)
			{
				n--;
				int i = random.Next(n + 1);
				T temp = array[i];
				array[i] = array[n];
				array[n] = temp;
			}
		}

		public static IEnumerable<T> Shuffle<T>(this List<T> array)
		{
			List<int> indices = Enumerable.Range(0, array.Count).ToList();
			indices.ShuffleInplace();
			for(int i=0; i<array.Count; i++) {
				yield return array[indices[i]];
			}
		}

		public static T RandomSample<T>(this List<T> v)
		{
			int n = v.Count;
			if(n == 0) {
				// error
				Debug.Log("ERROR in RandomSample");
				return default(T);
			}
			return v[RandomIndex(n)];
		}

		public static T RandomSample<T>(this IEnumerable<T> v)
		{
			return RandomSample(v.ToList());
		}

		static IEnumerable<T> RandomSampleImpl<T>(this List<T> v, int n)
		{
			int max = v.Count;
			if(n > max) {
				// error
				Debug.Log("ERROR in RandomSample");
				return new T[] {};
			}
			return v.Shuffle().Take(n);
		}

		public static IEnumerable<T> RandomSample<T>(this List<T> v, int n)
		{
			if(n == 1) {
				return new T[] { RandomSample(v) };
			}
			else {
				return RandomSampleImpl(v,n);
			}
		}

		public static IEnumerable<T> RandomSample<T>(this IEnumerable<T> v, int n)
		{
			return RandomSample(v.ToList(), n);
		}
	}
}
