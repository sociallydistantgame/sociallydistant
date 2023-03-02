#nullable enable

using System;
using GameplaySystems.GameManagement;
using Player;
using UnityEngine;
using Utility;

namespace UI.Login
{
	public abstract class LoginManager : MonoBehaviour
	{
		[Header("Dependencies")]
		[SerializeField]
		private GameManagerHolder gameManager = null!;

		[SerializeField]
		private PlayerInstanceHolder player = null!;

		protected GameManager GameManager => gameManager.Value;
		protected PlayerInstanceHolder Player => player;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(LoginManager));
			OnAwake();
		}

		protected virtual void OnAwake()
		{
			
		}
	}
}