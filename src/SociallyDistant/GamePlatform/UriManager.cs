#nullable enable
using SociallyDistant.Core.Modules;

namespace SociallyDistant.GamePlatform
{
	
	
	public sealed class UriManager : IUriManager
	{
		private readonly SociallyDistantGame gameManager;
		private readonly Dictionary<string, IUriSchemeHandler> registeredSchemas = new Dictionary<string, IUriSchemeHandler>();

		/// <inheritdoc />
		public IGameContext GameContext => gameManager;

		internal UriManager(SociallyDistantGame game)
		{
			this.gameManager = game;
		}
		
		/// <inheritdoc />
		public bool IsSchemeRegistered(string name)
		{
			return registeredSchemas.ContainsKey(name);
		}

		/// <inheritdoc />
		public void RegisterSchema(string schemaName, IUriSchemeHandler handler)
		{
			if (IsSchemeRegistered(schemaName))
				throw new InvalidOperationException($"A schema handler for the {schemaName} schema has already been registered.");

			registeredSchemas.Add(schemaName, handler);
		}

		/// <inheritdoc />
		public void UnregisterSchema(string schemaName)
		{
			if (!IsSchemeRegistered(schemaName))
				throw new InvalidOperationException($"A schema handler for the {schemaName} schema has not been registered.");

			registeredSchemas.Remove(schemaName);
		}

		/// <inheritdoc />
		public void ExecuteNavigationUri(Uri uri)
		{
			string schema = uri.Scheme;
			if (!IsSchemeRegistered(schema))
				throw new InvalidOperationException($"Invalid schema: {schema}");
			
			registeredSchemas[schema].HandleUri(uri);
		}
	}
}