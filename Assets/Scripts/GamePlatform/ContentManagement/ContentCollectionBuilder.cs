#nullable enable
using System.Collections.Generic;

namespace GamePlatform.ContentManagement
{
	public class ContentCollectionBuilder
	{
		private readonly IList<IGameContent> contentList;

		public ContentCollectionBuilder(IList<IGameContent> destination)
		{
			this.contentList = destination;

			this.contentList.Clear();
		}

		public void AddContent(IGameContent content)
		{
			this.contentList.Add(content);
		}
	}
}