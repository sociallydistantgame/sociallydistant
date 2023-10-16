#nullable enable
using System;
using System.Linq;
using System.Xml;

namespace UI.Themes.Serialization
{
	public class XmlElementSerializer : IElementSerializer
	{
		private readonly XmlDocument document;
		private readonly bool isReading;
		private readonly XmlElement element;

		public XmlElementSerializer(XmlDocument doc, XmlElement element, bool isReading)
		{
			this.document = doc;
			this.isReading = isReading;
			this.element = element;
		}
		
		/// <inheritdoc />
		public void Serialize(ref int value, string name, int defaultValue)
		{
			if (isReading)
			{
				if (!TryGetElementText(name, out string textValue) && !int.TryParse(textValue, out value))
					value = defaultValue;
			}
			else
			{
				WriteTextElement(name, value.ToString());
			}
		}

		/// <inheritdoc />
		public void Serialize(ref float value, string name, float defaultValue)
		{
			if (isReading)
			{
				if (!TryGetElementText(name, out string textValue) && !float.TryParse(textValue, out value))
					value = defaultValue;
			}
			else
			{
				WriteTextElement(name, value.ToString());
			}
		}

		/// <inheritdoc />
		public void Serialize(ref bool value, string name, bool defaultValue)
		{
			if (isReading)
			{
				if (!TryGetElementText(name, out string textValue) && !bool.TryParse(textValue, out value))
					value = defaultValue;
			}
			else
			{
				WriteTextElement(name, value.ToString());
			}
		}

		/// <inheritdoc />
		public void Serialize(ref string value, string name, string defaultValue)
		{
			if (isReading)
			{
				if (!TryGetElementText(name, out value))
					value = defaultValue;
			}
			else
			{
				WriteTextElement(name, value);
			}
		}

		/// <inheritdoc />
		public bool IsReading => isReading;

		/// <inheritdoc />
		public string ElementName => element.Name;

		/// <inheritdoc />
		public IElementSerializer? GetChildElement(string elementName)
		{
			XmlElement? child = element.ChildNodes
				.OfType<XmlElement>()
				.FirstOrDefault(x => x.Name == elementName);

			if (isReading && child == null)
				return null;

			if (child == null)
			{
				child = document.CreateElement(elementName);
				element.AppendChild(child);
			}

			return new XmlElementSerializer(document, child, isReading);
		}

		/// <inheritdoc />
		public IElementIterator GetChildCollection(string elementName, string itemName)
		{
			if (!isReading)
				throw new InvalidOperationException("You cannot iterate through an XML element collection when the serializer is in write mode.");

			throw new NotImplementedException();
		}

		private bool TryGetElementText(string name, out string text)
		{
			XmlElement? child = element.ChildNodes
				.OfType<XmlElement>()
				.FirstOrDefault(x => x.Name == name);

			if (child == null)
			{
				text = string.Empty;
				return false;
			}

			text = child.InnerText.Trim();
			return true;
		}
		
		private void WriteTextElement(string name, string value)
		{
			XmlElement child = document.CreateElement(name);
			XmlText text = document.CreateTextNode(value);

			child.AppendChild(text);
			this.element.AppendChild(child);
		}
	}

	public interface IThemeData
	{
		void Serialize(IElementSerializer serializer, ThemeAssets assets);
	}

	public static class ThemeDataUtility
	{
		public static void Serialize(this IElementSerializer serializer, IThemeData themeData, ThemeAssets themeAssets, string elementName)
		{
			IElementSerializer? childSerializer = serializer.GetChildElement(elementName);

			if (childSerializer == null)
				return;
			
			themeData.Serialize(childSerializer, themeAssets);
		}
		
		public static void Serialize<T>(this IElementSerializer serializer, ref T themeData, ThemeAssets themeAssets, string elementName)
			where T : struct, IThemeData
		{
			IElementSerializer? childSerializer = serializer.GetChildElement(elementName);

			if (childSerializer == null)
				return;
			
			themeData.Serialize(childSerializer, themeAssets);
		}
	}
}