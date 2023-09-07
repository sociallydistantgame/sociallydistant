#nullable enable
using Architecture;
using Core;
using OS;
using OS.Devices;
using UnityEngine;

namespace Player
{
	[CreateAssetMenu(menuName = "ScriptableObject/Architecture/PlayerInstance Holder")]
	public class PlayerInstanceHolder : 
		Holder<PlayerInstance>,
		IKernel
	{
		/// <inheritdoc />
		public IInitProcess InitProcess => Value.OsInitProcess;

		/// <inheritdoc />
		public IComputer Computer => Value.Computer;

		/// <inheritdoc />
		public ISkillTree SkillTree => Value.SkillTree;
	}
}