#nullable enable

using SociallyDistant.Core.Core;
using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Core.OS
{
	public interface IKernel
	{
		IInitProcess InitProcess { get; }
		
		IComputer Computer { get; }
        
		ISkillTree SkillTree { get; }
	}
}