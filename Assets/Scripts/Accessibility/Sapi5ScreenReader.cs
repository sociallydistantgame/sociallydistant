#nullable enable
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
using System;
using System.Runtime.InteropServices;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using SpeechLib;

namespace Accessibility
{
	public class Sapi5ScreenReader : 
		IScreenREader,
		IDisposable
	{
		public Sapi5ScreenReader()
		{
		}

		/// <inheritdoc />
		public Task Speak(string text)
		{
			return Task.CompletedTask;
			
			using var synth = new SpeechSynthesizer();

			Prompt test =  synth.SpeakAsync(text);

			return Task.CompletedTask;
		}

		/// <inheritdoc />
		public void StopSpeaking()
		{
			
		}

		/// <inheritdoc />
		public bool IsSpeaking { get; }

		/// <inheritdoc />
		public float VoicePitch
		{
			get;
			set;
		}

		/// <inheritdoc />
		public float VoiceSpeed { get; set; }

		/// <inheritdoc />
		public void Dispose()
		{
		}

		private class Speaker
		{
			private readonly TaskCompletionSource<bool> completionSource;
			private readonly SpVoice voice = new SpVoice();

			public Speaker(TaskCompletionSource<bool> completionSource)
			{
				this.completionSource = completionSource;
			}

			public unsafe void Speak(string text)
			{
				var SpFlags = SpeechVoiceSpeakFlags.SVSFlagsAsync;
				
				voice.EndStream += VoiceOnEndStream;
				voice.Speak(text, SpFlags);
			}

			private unsafe void VoiceOnEndStream(int streamnumber, object streamposition)
			{
				voice.EndStream -= VoiceOnEndStream;
				completionSource.SetResult(true);
			}
		}
	}
}
#endif