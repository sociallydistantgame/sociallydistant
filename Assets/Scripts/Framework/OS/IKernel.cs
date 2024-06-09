#nullable enable

using Core;
using OS.Devices;

namespace OS
{
	public interface IKernel
	{
		IInitProcess InitProcess { get; }
		
		IComputer Computer { get; }
        
		ISkillTree SkillTree { get; }
	}
}