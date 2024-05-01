#nullable enable
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;
using UnityEngine.Analytics;

namespace GamePlatform
{
	public struct PlayerInfo
	{
		public string Name;
		public string UserName;
		public string HostName;
		public Gender PlayerGender;
		public string Comment;
		public DateTime LastPlayed;

		public string ToXML()
		{
			var document = new XmlDocument();

			XmlElement root = document.CreateElement(nameof(PlayerInfo));

			XmlText nameText = document.CreateTextNode(Name);
			XmlText userNameText = document.CreateTextNode(UserName);
			XmlText hostNameText = document.CreateTextNode(HostName);
			XmlText commentText = document.CreateTextNode(Comment);
			XmlText genderText = document.CreateTextNode(PlayerGender.ToString());
			XmlText lastPlayedText = document.CreateTextNode(LastPlayed.ToString(CultureInfo.InvariantCulture));
                
			XmlElement name = document.CreateElement(nameof(Name));
			XmlElement userName = document.CreateElement(nameof(UserName));
			XmlElement hostName = document.CreateElement(nameof(HostName));
			XmlElement comment = document.CreateElement(nameof(Comment));
			XmlElement gender = document.CreateElement(nameof(PlayerGender));
			XmlElement lastPlayed = document.CreateElement(nameof(LastPlayed));
			
			name.AppendChild(nameText);
			userName.AppendChild(userNameText);
			hostName.AppendChild(hostNameText);
			comment.AppendChild(commentText);
			gender.AppendChild(genderText);
			lastPlayed.AppendChild(lastPlayedText);
			
			document.AppendChild(root);
			
			root.AppendChild(name);
			root.AppendChild(userName);
			root.AppendChild(hostName);
			root.AppendChild(comment);
			root.AppendChild(gender);
			root.AppendChild(lastPlayed);

			var sb = new StringBuilder();
			using var writer = XmlWriter.Create(sb, new XmlWriterSettings()
			{
				Indent = true
			});

			document.WriteTo(writer);

			writer.Flush();
			
			return sb.ToString();
		}
		
		public static bool TryGetFromXML(string xml, out PlayerInfo info)
		{
			info = default;
			
			var xmlDocument = new XmlDocument();

			try
			{
				xmlDocument.LoadXml(xml);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				return false;
			}

			XmlElement? root = xmlDocument.DocumentElement;
			if (root == null)
				return false;

			if (root.Name != nameof(PlayerInfo))
				return false;

			GetElement(root, nameof(Name), out info.Name);
			GetElement(root, nameof(UserName), out info.UserName);
			GetElement(root, nameof(HostName), out info.HostName);
			GetElement(root, nameof(Comment), out info.Comment);
			
			GetElement(root, nameof(PlayerGender), out info.PlayerGender);
			
			GetElement(root, nameof(LastPlayed), out info.LastPlayed);

			return true;
		}

		private static void GetElement(XmlElement parent, string elemName, out string field)
		{
			XmlElement? child = parent.ChildNodes
				.OfType<XmlElement>()
				.FirstOrDefault(x => x.Name == elemName);

			if (child != null)
				field = child.InnerText;
			else
				field = string.Empty;
		}
		
		private static void GetElement(XmlElement parent, string elemName, out Gender field)
		{
			XmlElement? child = parent.ChildNodes
				.OfType<XmlElement>()
				.FirstOrDefault(x => x.Name == elemName);

			if (child != null && Enum.TryParse<Gender>(child.InnerText, out Gender gender))
				field = gender;
			else
				field = Gender.Unknown;
		}
		
		private static void GetElement(XmlElement parent, string elemName, out DateTime field)
		{
			XmlElement? child = parent.ChildNodes
				.OfType<XmlElement>()
				.FirstOrDefault(x => x.Name == elemName);

			if (child != null)
				field = DateTime.Parse(child.InnerText);
			else
				field = DateTime.UtcNow;
		}
	}
}