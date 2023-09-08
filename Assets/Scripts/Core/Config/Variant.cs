#nullable enable

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using System.Text;
using System.Xml;
using UnityEngine;

namespace Core.Config
{
	public interface IVariantValue
	{
		object Value { get; }
		bool Is<T>();
	}
	
	public sealed class VariantValue<TValue> :
		IVariantValue
	{
		private TValue value;

		public bool Is<T>() => value is T;
		public object Value => value;

		public VariantValue(TValue value)
		{
			this.value = value;
		}
	}
	
	public class Variant
	{
		private IVariantValue? value;

		public bool Is<T>()
			=> value == null ? false : value.Is<T>();

		public T GetValue<T>()
			=> (T) value?.Value!;

		public object Value => value?.Value;

		private Variant()
		{
			// intentional stub.
		}
		
		public static Variant FromObject<T>(T value)
		{
			var variant = new Variant();
			variant.value = new VariantValue<T>(value);
			return variant;
		}
	}

	public class VariantDatabase :
		IDictionary<string, Variant>
	{
		private readonly Dictionary<string, Variant> database = new Dictionary<string, Variant>();

		/// <inheritdoc />
		public IEnumerator<KeyValuePair<string, Variant>> GetEnumerator()
		{
			return database.GetEnumerator();
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <inheritdoc />
		public void Add(KeyValuePair<string, Variant> item)
		{
			Add(item.Key, item.Value);
		}

		/// <inheritdoc />
		public void Clear()
		{
			database.Clear();
		}

		/// <inheritdoc />
		public bool Contains(KeyValuePair<string, Variant> item)
		{
			return database.Contains(item);
		}

		/// <inheritdoc />
		public void CopyTo(KeyValuePair<string, Variant>[] array, int arrayIndex)
		{
			database.ToArray().CopyTo(array, arrayIndex);
		}

		/// <inheritdoc />
		public bool Remove(KeyValuePair<string, Variant> item)
		{
			return Remove(item.Key);
		}

		/// <inheritdoc />
		public int Count => database.Count;

		/// <inheritdoc />
		public bool IsReadOnly => false;

		/// <inheritdoc />
		public void Add(string key, Variant value)
		{
			database.Add(key, value);
		}

		/// <inheritdoc />
		public bool ContainsKey(string key)
		{
			return database.ContainsKey(key);
		}

		/// <inheritdoc />
		public bool Remove(string key)
		{
			return database.Remove(key);
		}

		/// <inheritdoc />
		public bool TryGetValue(string key, out Variant value)
		{
			return database.TryGetValue(key, out value);
		}

		/// <inheritdoc />
		public Variant this[string key]
		{
			get => database[key];
			set => database[key] = value;
		}

		/// <inheritdoc />
		public ICollection<string> Keys => database.Keys;

		/// <inheritdoc />
		public ICollection<Variant> Values => database.Values;

		/// <inheritdoc />
		public override string ToString()
		{
			var xml = new XmlDocument();

			XmlComment warning = xml.CreateComment("This is the Socially Distant settings registry file. Unless you really know what you're doing, do not edit it. I don't feel like debugging problems that exist between a keyboard and chair that is not my own.");
			XmlElement settingsElement = xml.CreateElement("settings");

			settingsElement.AppendChild(warning);
			
			foreach (string key in Keys)
			{
				XmlAttribute keyAttribute = xml.CreateAttribute("key");
				XmlAttribute typeAttribute = xml.CreateAttribute("type");
				XmlAttribute valueAttribute = xml.CreateAttribute("value");
				
				keyAttribute.Value = key;
				valueAttribute.Value = database[key].Value.ToString();
				typeAttribute.Value = database[key].Value.GetType().FullName;

				XmlElement settingField = xml.CreateElement("setting");
				settingField.SetAttributeNode(keyAttribute);
				settingField.SetAttributeNode(typeAttribute);
				settingField.SetAttributeNode(valueAttribute);

				settingsElement.AppendChild(settingField);
			}

			xml.AppendChild(settingsElement);

			using var ms = new MemoryStream();
			using var writer = new StreamWriter(ms);
			
			xml.Save(writer);

			return Encoding.UTF8.GetString(ms.ToArray());
		}

		public static VariantDatabase Parse(string text)
		{
			var db = new VariantDatabase();

			var xml = new XmlDocument();
			xml.LoadXml(text);

			XmlElement? settingsElement = xml.ChildNodes.OfType<XmlElement>().FirstOrDefault(x => x.Name == "settings");

			if (settingsElement != null)
			{
				foreach (XmlElement setting in settingsElement.ChildNodes.OfType<XmlElement>().Where(x => x.Name == "setting"))
				{
					XmlAttribute? keyAttribute = setting.Attributes["key"];
					XmlAttribute? typeAttribute = setting.Attributes["type"];
					XmlAttribute? valueAttribute = setting.Attributes["value"];

					if (keyAttribute == null)
					{
						Debug.LogWarning("setting with missing key attribute will be skipped.");
						continue;
					}

					string key = keyAttribute.Value;

					if (typeAttribute == null)
					{
						Debug.LogWarning($"Setting with key {key} has no type attribute. It will be skipped.");
						continue;
					}

					if (valueAttribute == null)
					{
						Debug.LogWarning($"Setting with key {key} has no value attribute. It will be skipped.");
						continue;
					}

					Type? type = Type.GetType(typeAttribute.Value);

					if (type == null)
					{
						Debug.LogWarning($"Setting with key {key} has a type attribute, but the given type name isn't supported by the runtime! This is a problem. Skipping lading the setting since we can't parse the value.");
						continue;
					}

					object value = Convert.ChangeType(valueAttribute.Value, type);

					if (db.ContainsKey(key))
						db[key] = Variant.FromObject(value);
					else
						db.Add(key, Variant.FromObject(value));
				}
			}
			else
			{
				Debug.LogWarning("Registry file has valid XML, but no valid <settings> root element. Skipping loading of settings.");
			}
			
			return db;
		}
	}
}