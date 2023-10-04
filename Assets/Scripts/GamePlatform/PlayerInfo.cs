#nullable enable
using System;
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

			XmlText name = document.CreateTextNode(nameof(Name));
			name.Value = Name;
			
			XmlText userName = document.CreateTextNode(nameof(UserName));
			userName.Value = UserName;
			
			XmlText hostName = document.CreateTextNode(nameof(HostName));
			hostName.Value = HostName;
			
			XmlText comment = document.CreateTextNode(nameof(Comment));
			comment.Value = Comment;
			
			XmlText gender = document.CreateTextNode(nameof(Gender));
			gender.Value = PlayerGender.ToString();
			
			XmlText lastPlayed = document.CreateTextNode(nameof(LastPlayed));
			lastPlayed.Value = LastPlayed.ToString();

			root.AppendChild(name);
			root.AppendChild(userName);
			root.AppendChild(hostName);
			root.AppendChild(comment);
			root.AppendChild(gender);
			root.AppendChild(lastPlayed);
			document.AppendChild(root);

			var sb = new StringBuilder();
			using var writer = XmlWriter.Create(sb, new XmlWriterSettings()
			{
				Indent = true
			});

			document.WriteTo(writer);
			
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
				field = child.Value;
			else
				field = string.Empty;
		}
		
		private static void GetElement(XmlElement parent, string elemName, out Gender field)
		{
			XmlElement? child = parent.ChildNodes
				.OfType<XmlElement>()
				.FirstOrDefault(x => x.Name == elemName);

			if (child != null && Enum.TryParse<Gender>(child.Value, out Gender gender))
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
				field = DateTime.Parse(child.Value);
			else
				field = DateTime.UtcNow;
		}
	}
}