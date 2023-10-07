#nullable enable
using System;
using System.Linq;
using Core;
using Core.WorldData.Data;
using Social;

namespace GameplaySystems.Social
{
	public class UserMessage : IUserMessage
	{
		private readonly SocialService socialService;
		private DocumentElement[] documentData = Array.Empty<DocumentElement>();
		private ObjectId authorId;

		/// <inheritdoc />
		public ObjectId ChannelId { get; private set; }

		/// <inheritdoc />
		public IProfile Author => socialService.GetProfileById(authorId);

		/// <inheritdoc />
		public DateTime Date { get; private set; }

		internal UserMessage(SocialService socialService)
		{
			this.socialService = socialService;
		}
		
		/// <inheritdoc />
		public DocumentElement[] GetDocumentData()
		{
			return documentData;
		}

		internal void SetData(WorldMessageData data)
		{
			ChannelId = data.ChannelId;
			Date = data.Date;

			authorId = data.Author;
			documentData = data.DocumentElements.ToArray();
		}
	}
}