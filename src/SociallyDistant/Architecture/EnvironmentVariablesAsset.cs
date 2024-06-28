#nullable enable

using System.Collections;
using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Architecture
{
	public class EnvironmentVariablesAsset : IIterableEnvironmentVariableProvider
	{
		
		private SerializableKeyValuePair<string, string>[] environmentVariables = Array.Empty<SerializableKeyValuePair<string, string>>();

		public IEnumerable<string> Keys => environmentVariables.Select(x => x.Key);

		/// <inheritdoc />
		public string this[string key]
		{
			get => this.environmentVariables.FirstOrDefault(x => x.Key == key)?.Value ?? string.Empty;
			set
			{
				// this is a really shitty, horribly inefficient way of doing this because it may cause an array realloc and a copy, but...
				// this is a Unity asset that shouldn't be modified at runtime.
				SerializableKeyValuePair<string, string>? existing = this.environmentVariables.FirstOrDefault(x => x.Key == key);
				if (existing == null)
				{
					existing = new SerializableKeyValuePair<string, string>
					{
						Key = key
					};

					Array.Resize(ref environmentVariables, environmentVariables.Length + 1);
					environmentVariables[^1] = existing;
				}

				existing.Value = value;
			}
		}

		/// <inheritdoc />
		public bool IsSet(string variable)
		{
			return this.environmentVariables.Any(x => x.Key == variable);
		}

		/// <inheritdoc />
		public IEnvironmentVariableProvider DeepClone()
		{
			// Clone into a SimpleEnvironmentVariableProvider because we may be doing this in a player build
			var clone = new SimpleEnvironmentVariableProvider();

			foreach (SerializableKeyValuePair<string, string> pair in environmentVariables)
			{
				clone[pair.Key] = pair.Value;
			}
			
			return clone;
		}

		/// <inheritdoc />
		public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
		{
			// Ow. My heap.
			return environmentVariables.ToDictionary(x => x.Key, x => x.Value)
				.GetEnumerator();
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}