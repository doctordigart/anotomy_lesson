using UnityEngine;
using UnityEditor;

public class AssetBundleBuilder : EditorWindow {

	public string filePath; 
	public  BuildTarget targetPlatform = BuildTarget.Android;

	[MenuItem ("Window/AssetBudleBuilder")]
	static void Init () {
		AssetBundleBuilder window = (AssetBundleBuilder)EditorWindow.GetWindow (typeof (AssetBundleBuilder));
		window.Show();
	}

	void OnGUI(){

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Asset Bundle Output Path");
		filePath = EditorGUILayout.TextField (filePath);
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal ();
		targetPlatform = (BuildTarget)EditorGUILayout.EnumPopup (targetPlatform);
		if (GUILayout.Button ("BuildAssetBundles")) {
			BuildPipeline.BuildAssetBundles(filePath,BuildAssetBundleOptions.None,targetPlatform);
		}
		EditorGUILayout.EndHorizontal ();

		if (GUILayout.Button ("Clear Asset Budles From Cahe")) {
			Caching.CleanCache ();
		}

		EndWindows ();
	}



}




