﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateScriptableObjects : MonoBehaviour {

	[MenuItem("Assets/Create/Game State")]
	public static GameState createGamewState() {
		return createAsset<GameState>();
	}

	private static T createAsset<T> () where T : ScriptableObject {
		T asset = ScriptableObject.CreateInstance<T> ();
		string path = createPath(asset);
		AssetDatabase.CreateAsset (asset, path);
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh();
		Selection.activeObject = asset;
		return asset;
	}

	private static string createPath(Object obj) {
		string path = AssetDatabase.GetAssetPath(Selection.activeObject);
		if (path == "") {
			path = "Assets";
		} else if (Path.GetExtension(path) != "") {
			path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
		}
		return AssetDatabase.GenerateUniqueAssetPath(path + "/New " + obj.GetType().ToString() + ".asset");
	}
}
