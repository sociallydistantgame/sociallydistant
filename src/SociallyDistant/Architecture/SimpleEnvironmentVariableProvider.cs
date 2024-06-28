#nullable enable
using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Architecture
{
	public class SimpleEnvironmentVariableProvider : IEnvironmentVariableProvider
	{
		private readonly Dictionary<string, string> envVariables = new Dictionary<string, string>();

		public IEnumerable<string> Keys => envVariables.Keys;

		/// <inheritdoc />
		public string this[string key]
		{
			get
			{
				if (envVariables.TryGetValue(key, out string value))
					return value;
				return string.Empty;
			}
			set
			{
				if (envVariables.ContainsKey(key))
					envVariables[key] = value;
				else envVariables.Add(key, value);
			}
		}

		/// <inheritdoc />
		public bool IsSet(string variable)
		{
			return this.envVariables.ContainsKey(variable);
		}

		/// <inheritdoc />
		public IEnvironmentVariableProvider DeepClone()
		{
			var clone = new SimpleEnvironmentVariableProvider();

			foreach (string key in envVariables.Keys)
			{
				clone.envVariables.Add(key, envVariables[key]);
			}
			
			return clone;
		}
	}
}