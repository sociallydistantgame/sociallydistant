#nullable enable
using System;
using Architecture;
using Player;
using UnityEngine;

namespace GameplaySystems.Hacking
{
	[CreateAssetMenu(menuName = "ScriptableObject/Assets/Payload")]
	public class PayloadAsset : 
		ScriptableObject,
		IUnlockableAsset
	{
		[SerializeField]
		private string payloadName;


		/// <inheritdoc />
		public string Name => payloadName;

		/// <inheritdoc />
		public bool IsUnlocked(PlayerInstanceHolder player)
		{
			return true;
		}

		/// <inheritdoc />
		public bool CanUnlock(PlayerInstanceHolder player)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public bool Unlock(PlayerInstanceHolder player)
		{
			throw new NotImplementedException();
		}
	}
}