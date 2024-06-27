#nullable enable
using System;
using System.Linq;
using Core;
using Core.WorldData.Data;
using Social;

namespace GameplaySystems.Social
{
	public class SocialPost : IUserMessage
	{
		private readonly SocialService socialService;
		private WorldPostData data;

		/// <inheritdoc />
		public ObjectId ChannelId => ObjectId.Invalid;

		/// <inheritdoc />
		public ObjectId Id => data.InstanceId;

		/// <inheritdoc />
		public IProfile Author => socialService.GetProfileById(data.Author);

		/// <inheritdoc />
		public DateTime Date => data.Date;

		public SocialPost(SocialService service)
		{
			this.socialService = service;
		}
		
		/// <inheritdoc />
		public DocumentElement[] GetDocumentData()
		{
			return data.DocumentElements.ToArray();
		}

		public void SetData(WorldPostData data)
		{
			this.data = data;
		}
		
	}
}