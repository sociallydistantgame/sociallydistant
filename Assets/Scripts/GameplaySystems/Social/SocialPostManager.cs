#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.WorldData.Data;
using Social;
using UniRx;

namespace GameplaySystems.Social
{
	public class SocialPostManager : IDisposable
	{
		private readonly SocialService socialService;
		private readonly WorldManager worldManager;
		private readonly Dictionary<ObjectId, SocialPost> posts = new Dictionary<ObjectId, SocialPost>();
		private readonly Subject<IUserMessage> createSubject = new Subject<IUserMessage>();
		private readonly Subject<IUserMessage> modifySubject = new Subject<IUserMessage>();
		private readonly Subject<IUserMessage> deleteSubject = new Subject<IUserMessage>();


		public IEnumerable<IUserMessage> Posts => posts.Values.OrderByDescending(x => x.Date);
		
		public SocialPostManager(SocialService socialService, WorldManager worldManager)
		{
			this.socialService = socialService;
			this.worldManager = worldManager;
			
			this.worldManager.Callbacks.AddCreateCallback<WorldPostData>(OnCreateSocialPost);
			this.worldManager.Callbacks.AddModifyCallback<WorldPostData>(OnModifySocialPost);
			this.worldManager.Callbacks.AddDeleteCallback<WorldPostData>(OnDeleteSocialPost);
			
		}

		private void OnModifySocialPost(WorldPostData subjectprevious, WorldPostData subjectnew)
		{
			if (!posts.TryGetValue(subjectnew.InstanceId, out SocialPost post))
				return;

			post.SetData(subjectnew);
			modifySubject.OnNext(post);
		}

		private void OnDeleteSocialPost(WorldPostData subject)
		{
			if (!posts.TryGetValue(subject.InstanceId, out SocialPost post))
				return;

			deleteSubject.OnNext(post);
			posts.Remove(subject.InstanceId);
		}

		private void OnCreateSocialPost(WorldPostData subject)
		{
			var notify = false;

			if (!posts.TryGetValue(subject.InstanceId, out SocialPost post))
			{
				post = new SocialPost(socialService);
				posts.Add(subject.InstanceId, post);
				notify = true;
			}

			post.SetData(subject);
			
			if (notify)
				createSubject.OnNext(post);
		}

		/// <inheritdoc />
		public void Dispose()
		{
			createSubject.Dispose();
			modifySubject.Dispose();
			deleteSubject.Dispose();
		}

		public IEnumerable<IUserMessage> GetPostsByUser(IProfile profile)
		{
			return this.Posts.Where(x => x.Author == profile);
		}
	}
}