using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VoxelDare
{

	public class EditorTools : EditorWindow
	{
		[MenuItem("Voxel Dare/Voxels")]
		static void OpenWindow()
		{
			EditorWindow.GetWindow(typeof(EditorTools));
		}
		
		Mesh mesh;
		Material mat;
		
		void OnGUI()
		{
			EditorGUIUtility.LookLikeInspector();
			if (GUILayout.Button("Generate World"))
			{
				ExecuteWorldGenerate();
			}
			if (GUILayout.Button("Clear World"))
			{
				ExecuteWorldClear();
			}
			mat = EditorGUILayout.ObjectField("Material", mat, typeof(Material)) as Material;
			if (GUILayout.Button("SetMaterial"))
			{
				ExecuteFoo();
			}
			mesh = EditorGUILayout.ObjectField("Mesh", mesh, typeof(Mesh)) as Mesh;
			if (GUILayout.Button("Set Mesh"))
			{
				ExecuteFoo();
			}
		}

		VoxelDare.VoxelRenderer FindVoxelRenderer()
		{
			object[] obj = GameObject.FindObjectsOfType(typeof(VoxelDare.VoxelRenderer));
			if(obj.Length != 1) {
				Debug.LogError("ExecuteWorldGenerate obj.Length != 1");
			}
			return obj[0] as VoxelDare.VoxelRenderer;
		}
		
		WorldGenerator FindWorldGenerator()
		{
			object[] obj = GameObject.FindObjectsOfType(typeof(WorldGenerator));
			if(obj.Length != 1) {
				Debug.LogError("ExecuteWorldGenerate obj.Length != 1");
			}
			return obj[0] as WorldGenerator;
		}
		
		void ExecuteWorldGenerate()
		{
			var wg = FindWorldGenerator();
			wg.GenerateWorld();
		}
		
		void ExecuteWorldClear()
		{
			var wg = FindWorldGenerator();
			wg.ClearWorld();
		}
		
		void ExecuteFoo()
		{
		}
		
	}

}