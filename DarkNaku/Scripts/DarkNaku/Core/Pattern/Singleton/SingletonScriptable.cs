using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DarkNaku {
	public abstract class SingletonScriptable<T> : ScriptableObject where T : ScriptableObject {
		private const string RESOURCES_PATH = "Resources";

		private static string _assetName = null;
		private static string AssetName {
			get {
				if (string.IsNullOrEmpty(_assetName)) {
					_assetName = string.Format("{0}Setting", typeof(T).ToString());
				}

				return _assetName;
			}
		}

		private static T _instance = null;
		public static T Instance { 
			get {
				if (_instance == null) {
					_instance = Resources.Load(AssetName) as T;

					if (_instance == null) {
						_instance = CreateInstance<T>();
#if UNITY_EDITOR
						string resourcePath = System.IO.Path.Combine(Application.dataPath, RESOURCES_PATH);

						if (System.IO.Directory.Exists(resourcePath) == false) {
							AssetDatabase.CreateFolder("Assets", RESOURCES_PATH);
						}

						string fullPath = System.IO.Path.Combine(System.IO.Path.Combine("Assets", RESOURCES_PATH), AssetName + ".asset");
						AssetDatabase.CreateAsset(_instance, fullPath);
#endif
					}
				}

				return _instance;
			}
		}
	}
}
