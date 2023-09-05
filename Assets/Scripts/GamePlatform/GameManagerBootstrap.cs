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
		private GameManagerHolder holder = null!;

		[SerializeField]
		private GameManager prefab = null!;

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(GameManagerBootstrap));

			this.holder.Value = Instantiate(this.prefab);
		}

		private void OnDestroy()
		{
			if (this.holder.Value != null)
			{
				Destroy(this.holder.Value.gameObject);
				this.holder.Value = null;
			}
		}
	}
}