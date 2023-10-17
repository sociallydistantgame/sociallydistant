#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace UI.Themes.Serialization
{
	public class XmlElementIterator : IElementIterator
	{
		private readonly XmlDocument doc;
		private readonly XmlElement? element;
		private readonly string itemName;

		public XmlElementIterator(XmlDocument doc, XmlElement? element, string itemName)
		{
			this.doc = doc;
			this.element = element;
			this.itemName = itemName;
		}

		/// <inheritdoc />
		public string ElementName => element?.Name ?? string.Empty;

		/// <inheritdoc />
		public IEnumerable<IElementSerializer> ChildElements
			=> element?.ChildNodes.OfType<XmlElement>()
				   .Where(x => x.Name == itemName)
				   .Select(x => new XmlElementSerializer(this.doc, x, true))
			   ?? Enumerable.Empty<IElementSerializer>();
	}
}