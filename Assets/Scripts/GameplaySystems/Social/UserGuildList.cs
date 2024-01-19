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
	public class UserGuildList : IGuildList
	{
		private readonly IGuildList parentList;
		private readonly IProfile[] members;
		private readonly Subject<IGuild> joinSubject = new Subject<IGuild>();
		private readonly Subject<IGuild> leaveSubject = new Subject<IGuild>();
		private readonly IDisposable createObserver;
		private readonly IDisposable deleteObserver;
		private readonly WorldManager world;
		

		public UserGuildList(WorldManager world, IGuildList parent, params IProfile[] profiles)
		{
			this.world = world;
			
			this.parentList = parent;
			this.members = profiles;

			deleteObserver = parent.ObserveGuildRemoved().Subscribe(OnDelete);

			world.Callbacks.AddCreateCallback<WorldMemberData>(OnCreateMember);
			world.Callbacks.AddDeleteCallback<WorldMemberData>(OnDeleteMember);
		}

		/// <inheritdoc />
		public void Dispose()
		{
			createObserver?.Dispose();
			deleteObserver?.Dispose();
			joinSubject.Dispose();
			leaveSubject.Dispose();
		}


		/// <inheritdoc />
		public IEnumerator<IGuild> GetEnumerator()
		{
			return Query().GetEnumerator();
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <inheritdoc />
		public int Count => Query().Count();

		/// <inheritdoc />
		public IGuild this[int index] => Query().Skip(index).First();

		/// <inheritdoc />
		public IGuildList ThatHaveMember(IProfile profile)
		{
			if (members.Contains(profile))
				return this;

			return new UserGuildList(world, parentList, members.Concat(new[] { profile }).ToArray());
		}

		/// <inheritdoc />
		public IObservable<IGuild> ObserveGuildAdded()
		{
			return joinSubject;
		}

		/// <inheritdoc />
		public IObservable<IGuild> ObserveGuildRemoved()
		{
			return leaveSubject;
		}

		private IEnumerable<IGuild> Query()
		{
			return parentList.Where(Filter);
		}

		private bool Filter(IGuild guild)
		{
			IEnumerable<IProfile> profiles = guild.Members.Select(x => x.Profile);

			return members.All(profiles.Contains);
		}

		private void OnDelete(IGuild guild)
		{
			if (Filter(guild))
				leaveSubject.OnNext(guild);
		}
		
		private void OnDeleteMember(WorldMemberData subject)
		{
			// Only listen to guild member events
			if (subject.GroupType != MemberGroupType.Guild)
				return;

			// Check if the member is anyone we're listening for.
			if (members.All(x => x.ProfileId != subject.ProfileId))
				return;
			
			// Find an associated guild
			IGuild? guild = parentList.FirstOrDefault(x => x.Id == subject.GroupId);
			if (guild == null)
				return;
			
			leaveSubject.OnNext(guild);
		}
		
		private void OnCreateMember(WorldMemberData subject)
		{
			// Only listen to guild member events
			if (subject.GroupType != MemberGroupType.Guild)
				return;

			// Check if the member is anyone we're listening for.
			if (members.All(x => x.ProfileId != subject.ProfileId))
				return;
			
			// Find an associated guild
			IGuild? guild = parentList.FirstOrDefault(x => x.Id == subject.GroupId);
			if (guild == null)
				return;
			
			joinSubject.OnNext(guild);
		}
	}
}