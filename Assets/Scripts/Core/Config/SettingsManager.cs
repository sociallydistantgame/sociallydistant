#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml;
using UniRx;

namespace Core.Config
{
	public class SettingsManager : 
		ISettingsManager,
		IDisposable
	{
		private readonly Dictionary<string, SettingsCategory> categories = new Dictionary<string, SettingsCategory>();
		private readonly Dictionary<string, SettingsValue> allSettings = new Dictionary<string, SettingsValue>();
		private readonly Subject<ISettingsManager> changesSubject = new Subject<ISettingsManager>();
		private bool isInitialized;

		public static string GameDataPath => UnityEngine.Application.persistentDataPath.Replace('/', Path.DirectorySeparatorChar);
		public static string RegistryFilePath => Path.Combine(GameDataPath, "settings.json");
		
		public SettingsManager()
		{
			isInitialized = true;
			Load();
		}

		/// <inheritdoc />
		public IEnumerable<string> Keys => this.allSettings.Keys;
		
		/// <inheritdoc />
		public void Dispose()
		{
			SaveToFile();
			allSettings.Clear();
			isInitialized = false;
			changesSubject.Dispose();
		}
		
		/// <inheritdoc />
		string ISettingsManager.GameDataPath => SettingsManager.GameDataPath;

		/// <inheritdoc />
		public bool IsInitialized => isInitialized;
		
		/// <inheritdoc />
		public SettingsCategory? FindSettingsByKey(string key)
		{
			return !categories.TryGetValue(key, out SettingsCategory? category) ? null : category;
		}

		/// <inheritdoc />
		public T? FindSettings<T>() where T : SettingsCategory
		{
			return categories.Values.OfType<T>().FirstOrDefault();
		}

		/// <inheritdoc />
		public bool GetBool(string key, bool defaultValue = false)
		{
			if (!IsBool(key))
				return defaultValue;

			return allSettings[key].BoolValue;
		}

		/// <inheritdoc />
		public string? GetString(string key, string? defaultValue = null)
		{
			if (!IsString(key))
				return defaultValue;

			return allSettings[key].StringValue;
		}

		/// <inheritdoc />
		public float GetFloat(string key, float defaultValue = 0)
		{
			if (!IsFloat(key))
				return defaultValue;

			return allSettings[key].FloatValue;
		}

		/// <inheritdoc />
		public int GetInt(string key, int defaultValue = 0)
		{
			if (!IsInt(key))
				return defaultValue;

			return allSettings[key].IntValue;
		}

		/// <inheritdoc />
		public bool IsBool(string key)
		{
			return HasKey(key) && allSettings[key].Type == SettingsType.Boolean;
		}

		/// <inheritdoc />
		public bool IsString(string key)
		{
			return HasKey(key) && allSettings[key].Type == SettingsType.String;
		}

		/// <inheritdoc />
		public bool IsFloat(string key)
		{
			return HasKey(key) && allSettings[key].Type == SettingsType.Float;
		}

		/// <inheritdoc />
		public bool IsInt(string key)
		{
			return HasKey(key) && allSettings[key].Type == SettingsType.Int;
		}

		/// <inheritdoc />
		public void SetFloat(string key, float value)
		{
			ResetKey(key);
			allSettings.Add(key, new SettingsValue
			{
				Type = SettingsType.Float,
				FloatValue = value
			});
			
			changesSubject.OnNext(this);
		}

		/// <inheritdoc />
		public void SetString(string key, string value)
		{
			ResetKey(key);
			allSettings.Add(key, new SettingsValue
			{
				Type = SettingsType.String,
				StringValue = value
			});
			
			changesSubject.OnNext(this);
		}

		/// <inheritdoc />
		public void SetInt(string key, int value)
		{
			ResetKey(key);
			allSettings.Add(key, new SettingsValue
			{
				Type = SettingsType.Int,
				IntValue = value
			});
			
			changesSubject.OnNext(this);
		}

		/// <inheritdoc />
		public void SetBool(string key, bool value)
		{
			ResetKey(key);
			allSettings.Add(key, new SettingsValue
			{
				Type = SettingsType.Boolean,
				BoolValue = value
			});
			
			changesSubject.OnNext(this);
		}

		/// <inheritdoc />
		public void Save()
		{
			SaveToFile();
		}

		/// <inheritdoc />
		public void Load()
		{
			LoadSettings();
		}

		/// <inheritdoc />
		public void ResetKey(string key)
		{
			if (HasKey(key))
			{
				allSettings.Remove(key);
				changesSubject.OnNext(this);
			}
		}

		/// <inheritdoc />
		public bool HasKey(string key)
		{
			return allSettings.ContainsKey(key);
		}

		/// <inheritdoc />
		public IDisposable ObserveChanges(Action<ISettingsManager> onUpdate)
		{
			return Observable.Create<ISettingsManager>((observer) =>
			{
				observer.OnNext(this);

				return changesSubject.Subscribe(onUpdate);
			}).Subscribe(onUpdate);
		}

		private void LoadSettings()
		{
			allSettings.Clear();

			if (!File.Exists(RegistryFilePath))
			{
				changesSubject.OnNext(this);
				return;
			}

			using Stream stream = File.OpenRead(RegistryFilePath);
			using var reader = new StreamReader(stream);

			var xmlDoc = new XmlDocument();
            xmlDoc.Load(reader);

            XmlElement? allSettingsElement = xmlDoc.ChildNodes
	            .OfType<XmlElement>()
	            .FirstOrDefault(x => x.Name == nameof(allSettings));

            if (allSettingsElement == null)
	            return;

            foreach (XmlElement settingElement in allSettingsElement.ChildNodes.OfType<XmlElement>().Where(x => x.Name == "setting"))
            {
	            if (!settingElement.HasAttribute("name"))
		            continue;
	            
	            if (!settingElement.HasAttribute("type"))
		            continue;
	            
	            if (!settingElement.HasAttribute("value"))
		            continue;

	            string name = settingElement.GetAttribute("name");
	            string type = settingElement.GetAttribute("type");
	            string value = settingElement.GetAttribute("value");

	            if (!Enum.TryParse(type, out SettingsType actualType))
		            continue;
	            
	            SettingsValue actualValue = default;

	            switch (actualType)
	            {
		            case SettingsType.String:
			            actualValue.StringValue = value;
			            break;
		            case SettingsType.Int:
			            if (!int.TryParse(value, out actualValue.IntValue))
				            continue;
			            break;
                    case SettingsType.Float:
	                    if (!float.TryParse(value, out actualValue.FloatValue))
		                    continue;
	                    break;
                    case SettingsType.Boolean:
	                    if (!bool.TryParse(value, out actualValue.BoolValue))
		                    continue;
	                    break;
                    default:
	                    continue;
	                    break;
	            }

	            actualValue.Type = actualType;

	            if (allSettings.ContainsKey(name))
		            allSettings[name] = actualValue;
	            else allSettings.Add(name, actualValue);
            }
            
            changesSubject.OnNext(this);
		}

		private void SaveToFile()
		{
			var xmlDoc = new XmlDocument();

			XmlElement allSettingsElement = xmlDoc.CreateElement(nameof(allSettings));

			foreach (string name in allSettings.Keys)
			{
				XmlElement settingElement = xmlDoc.CreateElement("setting");

				SettingsValue actualValue = allSettings[name];

				XmlAttribute nameAttribute = xmlDoc.CreateAttribute("name");
				XmlAttribute typeAttribute = xmlDoc.CreateAttribute("type");
				XmlAttribute valueAttribute = xmlDoc.CreateAttribute("value");

				nameAttribute.Value = name;
				typeAttribute.Value = actualValue.Type.ToString();

				switch (actualValue.Type)
				{
					case SettingsType.Boolean:
						valueAttribute.Value = actualValue.BoolValue.ToString();
						break;
					case SettingsType.String:
						valueAttribute.Value = actualValue.StringValue;
						break;
					case SettingsType.Int:
						valueAttribute.Value = actualValue.IntValue.ToString();
						break;
					case SettingsType.Float:
						valueAttribute.Value = actualValue.FloatValue.ToString();
						break;
					default:
						continue;
						break;
				}

				settingElement.Attributes.Append(nameAttribute);
				settingElement.Attributes.Append(typeAttribute);
				settingElement.Attributes.Append(valueAttribute);
                
				allSettingsElement.AppendChild(settingElement);
			}
			
			xmlDoc.AppendChild(allSettingsElement);
            
			using var writer = new XmlTextWriter(RegistryFilePath, Encoding.UTF8);
			
			xmlDoc.WriteTo(writer);
		}
	}
}