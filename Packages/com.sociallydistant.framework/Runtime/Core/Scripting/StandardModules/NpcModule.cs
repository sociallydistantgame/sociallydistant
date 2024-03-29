#nullable enable

using Social;

namespace Core.Scripting.StandardModules
{
	public class NpcModule : ScriptModule
	{
		private readonly ISocialService socialService;

		public NpcModule(ISocialService socialService)
		{
			this.socialService = socialService;
		}

		[Function("fullname")]
		public string GetFullName(string narrativeIdentifier)
		{
			return socialService.GetNarrativeProfile(narrativeIdentifier).ChatName;
		}

		[Function("username")]
		public string GetUsername(string narrativeIdentifier)
		{
			return socialService.GetNarrativeProfile(narrativeIdentifier).ChatUsername;
		}
	}
}