#nullable enable
using System.Threading.Tasks;

namespace Accessibility
{
	public interface IScreenREader
	{
		Task Speak(string text);
		void StopSpeaking();
		
		bool IsSpeaking { get; }
		float VoicePitch { get; set; }
		float VoiceSpeed { get; set; }
	}
}