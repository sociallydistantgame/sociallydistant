using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Com.TheFallenGames.OSA.Editor
{
    /// <summary>
    /// Used to keep track of the plugin's path, wherever you move it inside the project. This way, we don't need to hardcode paths.
    /// Do not move this file from its default location
    /// </summary>
    [CreateAssetMenu(fileName= CLASS_NAME + ".asset", menuName= "OSA/PluginPathTracker")]
	public class PluginPathTracker : ScriptableObject
    {
        const string CLASS_NAME = "PluginPathTracker";

        public static string PathInAssets
        {
            get
            {
                var path = AssetDatabase.GetAssetPath(Instance).Replace("\\", "/");
                return path;
            }
        }

        public static string DirectoryPathInAssets
        {
            get
            {
                return Path.GetDirectoryName(PathInAssets).Replace("\\", "/");
            }
        }

        static PluginPathTracker Instance 
        {
            get
            {
                if (_Instance != null)
                    return _Instance;

                var assets = AssetDatabase.FindAssets("t:" + CLASS_NAME);
                if (assets.Length == 0)
                    throw new InvalidOperationException("No PluginPathTracker asset found! Did you delete it?");

                if (assets.Length > 1)
                    throw new InvalidOperationException("Found more than 1 asset with type PluginPathTracker");

                var path = AssetDatabase.GUIDToAssetPath(assets[0]);
                _Instance = AssetDatabase.LoadAssetAtPath<PluginPathTracker>(path);

                return _Instance;
            }
        }

        static PluginPathTracker _Instance;


        void OnEnable()
        {
            if (_Instance)
                throw new InvalidOperationException("Only one instance of PluginPath.asset is allowed");

            _Instance = this;
        }


        void OnDestroy()
        {
            if (_Instance == this)
                _Instance = null;
        }
    }
}
