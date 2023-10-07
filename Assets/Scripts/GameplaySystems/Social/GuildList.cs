#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.WorldData.Data;
using Social;
using UniRx;

namespace GameplaySystems.Social
{
	public class GuildList : IGuildList
	{
		private readonly WorldManager world;
		private readonly Subject<IGuild> createObservable = new Subject<IGuild>();
		private readonly Subject<IGuild> deleteObservable = new Subject<IGuild>();
		private readonly Dictionary<ObjectId, Guild> guilds = new Dictionary<ObjectId, Guild>();
		private readonly ChatMemberManager memberManager;
		

		public GuildList(ChatMemberManager memberManager, WorldManager world)
		{
			this.world = world;
			this.memberManager = memberManager;
			
			world.Callbacks.AddCreateCallback<WorldGuildData>(OnCreateGuild);
			world.Callbacks.AddDeleteCallback<WorldGuildData>(OnDeleteGuild);
			world.Callbacks.AddModifyCallback<WorldGuildData>(OnGuildModify);
		}

		/// <inheritdoc />
		public void Dispose()
		{
			createObservable.Dispose();
			deleteObservable.Dispose();
			
			world.Callbacks.RemoveCreateCallback<WorldGuildData>(OnCreateGuild);
			world.Callbacks.RemoveDeleteCallback<WorldGuildData>(OnDeleteGuild);
			world.Callbacks.RemoveModifyCallback<WorldGuildData>(OnGuildModify);
		}

		/// <inheritdoc />
		public IEnumerator<IGuild> GetEnumerator()
		{
			foreach (IGuild guild in guilds.Values)
				yield return guild;
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <inheritdoc />
		public int Count => guilds.Count;

		/// <inheritdoc />
		public IGuild this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
					throw new ArgumentOutOfRangeException(nameof(index));

				return guilds.Values.Skip(index).First();
			}
		}

		/// <inheritdoc />
		public IGuildList ThatHaveMember(IProfile profile)
		{
			return new UserGuildList(world, this, profile);
		}

		/// <inheritdoc />
		public IObservable<IGuild> ObserveGuildAdded()
		{
			return createObservable;
		}

		/// <inheritdoc />
		public IObservable<IGuild> ObserveGuildRemoved()
		{
			return deleteObservable;
		}
		
		private void OnGuildModify(WorldGuildData subjectprevious, WorldGuildData subjectnew)
		{
			if (!guilds.TryGetValue(subjectnew.InstanceId, out Guild guild))
				return;

			guild.SetData(subjectnew);
		}

		private void OnDeleteGuild(WorldGuildData subject)
		{
			if (!guilds.TryGetValue(subject.InstanceId, out Guild guild))
				return;

			guilds.Remove(subject.InstanceId);
            deleteObservable.OnNext(guild);
		}

		private void OnCreateGuild(WorldGuildData subject)
		{
			var notify = false;
			if (!guilds.TryGetValue(subject.InstanceId, out Guild guild))
			{
				guild = new Guild(memberManager);
				guilds.Add(subject.InstanceId, guild);
				notify = true;
			}

			guild.SetData(subject);

			if (notify)
				createObservable.OnNext(guild);
		}
	}
}