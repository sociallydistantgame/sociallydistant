#nullable enable
using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace UI.Themes.Serialization
{
	public class XmlThemeDataSerializer : 
		IThemeDataSerializer
	{
		private readonly XmlDocument document;
		private readonly bool isReading;
		private readonly string rootElementName;
		private readonly XmlElementSerializer rootElementSerializer;
		

		private XmlThemeDataSerializer(string rootElement, bool isReading, XmlDocument doc)
		{
			this.document = doc;
			this.isReading = isReading;
			this.rootElementName = rootElement;

			if (isReading)
			{
				if (document.DocumentElement == null)
					throw new InvalidOperationException("Cannot read theme data XML: No root element!");

				if (document.DocumentElement.Name != rootElementName)
					throw new InvalidOperationException("Cannot read theme XML data, unexpected root element name.");

				rootElementSerializer = new XmlElementSerializer(document, document.DocumentElement, true);
			}
			else
			{
				XmlElement root = document.CreateElement(rootElementName);

				document.AppendChild(root);

				rootElementSerializer = new XmlElementSerializer(document, root, false);
			}
		}
		
		public async Task SaveTo(Stream stream)
		{
			if (!stream.CanWrite)
				throw new InvalidOperationException("Cannot save theme data XML to the specified stream. The stream is not writeable.");

			using var writer = XmlWriter.Create(stream, new XmlWriterSettings()
			{
				Indent = true
			});

			await Task.Run(() =>
			{
				document.WriteTo(writer);
			});
		}
        
		/// <inheritdoc />
		public void Serialize(ref int value, string name, int defaultValue)
		{
			rootElementSerializer.Serialize(ref value, name, defaultValue);
		}

		/// <inheritdoc />
		public void Serialize(ref float value, string name, float defaultValue)
		{
			rootElementSerializer.Serialize(ref value, name, defaultValue);
		}

		/// <inheritdoc />
		public void Serialize(ref bool value, string name, bool defaultValue)
		{
			rootElementSerializer.Serialize(ref value, name, defaultValue);
		}

		/// <inheritdoc />
		public void Serialize(ref string value, string name, string defaultValue)
		{
			rootElementSerializer.Serialize(ref value, name, defaultValue);
		}

		/// <inheritdoc />
		public bool IsReading => isReading;

		/// <inheritdoc />
		public string ElementName => rootElementName;

		/// <inheritdoc />
		public IElementSerializer? GetChildElement(string elementName)
		{
			return rootElementSerializer.GetChildElement(elementName);
		}

		/// <inheritdoc />
		public IElementIterator GetChildCollection(string elementName, string itemName)
		{
			return rootElementSerializer.GetChildCollection(elementName, itemName);
		}

		public static XmlThemeDataSerializer Create(string rootElementName)
		{
			var doc = new XmlDocument();
			
			return new XmlThemeDataSerializer(rootElementName, false, doc);
		}

		public static XmlThemeDataSerializer CreateFromStream(Stream stream, string expectedRoot)
		{
			var doc = new XmlDocument();

			doc.Load(stream);

			return new XmlThemeDataSerializer(expectedRoot, true, doc);
		}
	}
}