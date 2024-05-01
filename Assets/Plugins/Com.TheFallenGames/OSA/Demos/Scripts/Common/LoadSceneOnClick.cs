using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Com.TheFallenGames.OSA.Demos.Common
{
	/// <summary>Added to a GameObject that has a Button component, it loads <see cref="sceneName"/> scene when the button is clicked</summary>
	public class LoadSceneOnClick : MonoBehaviour
	{
		const string SCENES_FOLDER_NAME = "Scenes";

		/// <summary>The scene to load on click</summary>
		public Mode loadMode;
		public string sceneName;
		public List<string> allSceneNamesInOrder;


		void Start()
		{
			GetComponent<Button>().onClick.AddListener(LoadScene);
		}

		void LoadScene()
		{
			string sceneNameToLoad;
			if (loadMode == Mode.Specified)
			{
				sceneNameToLoad = sceneName;
			}
			else
			{
				if (allSceneNamesInOrder == null || allSceneNamesInOrder.Count == 0)
					return;

				int incr = loadMode == Mode.Next ? 1 : -1;
				string curSceneName;
				curSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
				int idx = allSceneNamesInOrder.IndexOf(curSceneName);
				if (idx == -1)
					idx = 0;
				else
					idx = (idx + incr + allSceneNamesInOrder.Count) % allSceneNamesInOrder.Count;
				sceneNameToLoad = allSceneNamesInOrder[idx];
			}

			try
			{
				if (!IsDemoSceneInBuild(sceneNameToLoad))
				{
					ShowSceneCouldntBeLoadedNotice(sceneNameToLoad);
					return;
				}

				UnityEngine.SceneManagement.SceneManager.LoadScene(sceneNameToLoad);
			}
			catch (System.Exception e)
			{
				Debug.Log("Couldn't load scene " + sceneNameToLoad + ": " + e);
			}
		}

		bool IsDemoSceneInBuild(string sceneName)
		{
			bool found = false;
			//string scenePathToFind = DemosUtil.OSA_SCENES_LOCAL_PATH + "/" + sceneName + ".unity";
			// Looking for suffix instead, as the user might've moved OSA to a sub-sub-folder in Assets
			string scenePathSuffixToFind = SCENES_FOLDER_NAME + "/" + sceneName + ".unity";
#if UNITY_5_5_OR_NEWER
			for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; i++)
			{
				var scenePath = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
				//if (scenePath == scenePathToFind)
				if (scenePath != null && scenePath.EndsWith(scenePathSuffixToFind))
				{
					found = true;
					break;
				}
			}
#elif UNITY_EDITOR
			for (int i = 0; i < UnityEditor.EditorBuildSettings.scenes.Length; i++)
			{
				var scene = UnityEditor.EditorBuildSettings.scenes[i];
				if (!scene.enabled)
					continue;

				var scenePath = scene.path;

				//if (scenePath == scenePathToFind)
				if (scenePath != null && scenePath.EndsWith(scenePathSuffixToFind))
				{
					found = true;
					break;
				}
			}

#else
			// Before Unity 5.5 and not in editor => a classic error will show if the scene is not in build. 
			// We can't do anything about it, as we can't know which scenes were added to the build
			found = true;
#endif

			return found;
		}

		void ShowSceneCouldntBeLoadedNotice(string sceneNameToLoad)
		{
			string error = "Couldn't load scene " + sceneNameToLoad + " because it wasn't added to build settings. \n" +
				"Open it via File->Build Settings. Select all scenes that you want to include in build and drag and drop them into that window. " +
				"Scenes aren't automatically added to keep your original build settings intact.";

			string errorObject = "OSALoadSceneError";
			var c = FindObjectOfType<Canvas>();
			if (c)
			{
				c = c.rootCanvas;
				var existing = c.transform.Find(errorObject);
				Text text;
				if (existing)
					text = existing.GetComponent<Text>();
				else
				{
					var go = new GameObject(errorObject, typeof(RectTransform));
					go.transform.SetParent(c.transform, false);
					var rt = go.transform as RectTransform;
					rt.anchorMin = Vector2.zero;
					rt.anchorMax = Vector2.one;
					rt.sizeDelta = Vector2.zero;
					var img = go.AddComponent<Image>();
					img.color = Color.black * .3f;
					img.raycastTarget = false;

					var goCh = new GameObject("Text", typeof(RectTransform));
					goCh.transform.SetParent(go.transform, false);
					rt = goCh.transform as RectTransform;
					rt.anchorMin = Vector2.zero;
					rt.anchorMax = Vector2.one;
					rt.sizeDelta = Vector2.zero;

					text = goCh.AddComponent<Text>();
					string fontName;
#if UNITY_2022_2_OR_NEWER
					fontName = "LegacyRuntime.ttf";
#else
					fontName = "Arial.ttf";
#endif
					text.font = Resources.GetBuiltinResource<Font>(fontName);
					text.fontSize = 20;
					text.fontStyle = FontStyle.Bold;
					text.color = Color.white;
					text.alignment = TextAnchor.MiddleCenter;
					text.raycastTarget = false;
					var shad = goCh.AddComponent<Shadow>();
					shad.effectDistance = new Vector2(1, -1);
					shad.effectColor = Color.black;
				}

				if (text)
					text.text = error;
			}

			Debug.Log(error);
		}


		public enum Mode
		{
			Previous,
			Next,
			Specified
		}
	}
}