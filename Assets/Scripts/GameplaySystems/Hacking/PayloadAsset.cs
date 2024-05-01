#nullable enable
using System;
using Architecture;
using Core;
using Hacking;
using OS.Devices;
using Player;
using UI.Terminal;
using UnityEngine;

namespace GameplaySystems.Hacking
{
	[CreateAssetMenu(menuName = "ScriptableObject/Assets/Payload")]
	public class PayloadAsset : 
		ScriptableObject,
		IPayload
	{
		[SerializeField]
		private string payloadName;
		
		/// <inheritdoc />
		public string Name => payloadName;

		/// <inheritdoc />
		public bool IsUnlocked(ISkillTree skills)
		{
			return true;
		}

		/// <inheritdoc />
		public bool CanUnlock(ISkillTree skills)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public bool Unlock(ISkillTree skills)
		{
			throw new NotImplementedException();
		}

		public void Run(ISystemProcess process, ConsoleWrapper console)
		{
			System.Diagnostics.Process.Start("https://youtu.be/K7Hn1rPQouU");
		}
	}
}