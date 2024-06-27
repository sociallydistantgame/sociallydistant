#nullable enable
using System;
using UnityEngine;
using UnityExtensions;
using Utility;

namespace GamePlatform
{
	public class GameManagerBootstrap : MonoBehaviour
	{
		[SerializeField]
		private GameObject mandatoryScriptsPrefab = null!;
        
		[SerializeField]
		private GameManager prefab = null!;

		private GameManager gameManager = null!;
		private GameObject mandatoryScripts;
		
		private void Awake()
		{
			Debug.Log("Starting initial game boot...");
			DontDestroyOnLoad(this.gameObject);
			
			this.AssertAllFieldsAreSerialized(typeof(GameManagerBootstrap));

			gameManager = Instantiate(this.prefab);
			Debug.Log("Brought the game manager up. Now booting the rest of the game.");
		}

		private void Start()
		{
			mandatoryScripts = Instantiate(mandatoryScriptsPrefab);
		}
	}
}