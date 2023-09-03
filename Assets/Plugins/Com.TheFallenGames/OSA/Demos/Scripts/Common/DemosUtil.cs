using Com.TheFallenGames.OSA.Core;
using UnityEngine;

namespace Com.TheFallenGames.OSA.Demos.Common
{
    /// <summary> Constants static class </summary>
    public static class DemosUtil
	{
		public static readonly string[] SMALL_IMAGES_URLS = new string[]
        {
            "https://68.media.tumblr.com/avatar_a6abe54fa29b_128.png",
            "https://www.lebanoninapicture.com/Prv/Images/Pages/Page_92120/meditation-nature-peace-lebanesemountains-livel-2-22-2017-12-19-17-am-t.jpg",
            "https://www.lebanoninapicture.com/Prv/Images/Pages/Page_96905/purple-flower-green-plants-nature-naturecolors--2-28-2017-4-06-40-pm-t.jpg",
            "https://68.media.tumblr.com/avatar_4b20b991f1fa_128.png",
            "https://s-media-cache-ak0.pinimg.com/236x/cd/de/a2/cddea289ff95c409ce414983f02847b6.jpg",
            "https://images-na.ssl-images-amazon.com/images/I/71L8cVOFuAL._CR204,0,1224,1224_UX128.jpg",
            //"https://d2ujflorbtfzji.cloudfront.net/key-image/691c98df-8e89-491f-b7ac-c7ff5a0c441e.png",
            "https://images-na.ssl-images-amazon.com/images/I/71QdMdCSz5L._CR204,0,1224,1224_UX128.jpg",
            "https://a.wattpad.com/useravatar/CorpseDragneel0.128.192903.jpg",
            "http://wiki.teamliquid.net/commons/images/1/18/Crystal_maiden_freezing_field.png",
            "http://icons.iconarchive.com/icons/emoopo/darktheme-folder/128/Folder-Nature-Leave-icon.png",
            "http://icons.iconarchive.com/icons/majid-ksa/nature-folder/128/flower-folder-icon.png"
        };
		public const string LOREM_IPSUM = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. " +
			"Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. " +
			"It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. " +
			"It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.";

		public static string GetRandomTextBody(int minLen = 0, int maxLen = int.MaxValue)
		{
			int maxStartIndex = LOREM_IPSUM.Length - minLen;
			int startIndex = Random.Range(0, maxStartIndex);
			int remainedLength = LOREM_IPSUM.Length - startIndex;

			return LOREM_IPSUM.Substring(startIndex, Random.Range(minLen, Mathf.Min(maxLen, remainedLength)));
		}

		public static string GetRandomSmallImageURL() { return SMALL_IMAGES_URLS[UnityEngine.Random.Range(0, SMALL_IMAGES_URLS.Length)]; }

		public static Color GetRandomColor(bool fullAlpha = false)
		{ return new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), fullAlpha ? 1f : UnityEngine.Random.Range(0f, 1f)); }
	}
}
