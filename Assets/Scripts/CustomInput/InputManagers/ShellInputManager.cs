#nullable enable
using System;
using System.IO;
using System.Threading.Tasks;
using Player;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using Object = UnityEngine.Object;

namespace CustomInput.InputManagers
{
	public class ShellInputManager : IInputManager
	{
		private Task? screenshotTask;
		
		/// <inheritdoc />
		public bool HandleInputs(PlayerInstance playerInstance, GameControls gameControls, bool consumedByOtherSystem)
		{
			if (screenshotTask != null)
			{
				if (screenshotTask.IsCompleted || screenshotTask.IsCanceled)
				{
					if (screenshotTask.Exception != null)
						Debug.LogException(screenshotTask.Exception);

					screenshotTask = null;
				}
			}

			if (gameControls.Shell.TakeScreenshot.triggered)
			{
				TryTakeScreenshot();
				return true;
			}

			return false;
		}

		private void TryTakeScreenshot()
		{
			if (screenshotTask != null)
				return;

			screenshotTask = TakeScreenshotAsync();
		}

		private async Task TakeScreenshotAsync()
		{
			await Task.Yield();
            
			string screenshotsPath = Path.Combine(Application.persistentDataPath, "screenshots");

			if (!Directory.Exists(screenshotsPath))
				Directory.CreateDirectory(screenshotsPath);
	
			var fileTime = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss");
			
			string filePath = Path.Combine(screenshotsPath, fileTime + ".jpeg");
            
			ScreenCapture.CaptureScreenshot(filePath, 1);
		}
	}
}