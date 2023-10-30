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
			Camera cam = Camera.main;

			RenderTexture? renderTexture = RenderTexture.GetTemporary(Screen.width, Screen.height, 1);

			if (renderTexture == null)
			{
				Debug.LogWarning("Could not get pooled RenderTexture for screenshot.");
				return;
			}

			RenderTexture? camTarget = cam.targetTexture;

			cam.targetTexture = renderTexture;

			cam.Render();

			cam.targetTexture = camTarget;

			var imageTexture = new Texture2D(renderTexture.width, renderTexture.height);
			
			// Copy the render target to the image texture
			RenderTexture previousActiveTexture = RenderTexture.active;
			RenderTexture.active = renderTexture;
			imageTexture.ReadPixels(new Rect(0, 0, imageTexture.width, imageTexture.height), 0, 0);
			RenderTexture.active = previousActiveTexture;
			
			renderTexture.Release();

			// This is the biggest mistake I've made with the Restitched level format, saving the preview image
			// as a PNG. So we'll not re-make that mistake, and instead save SD screenshots as a jpeg.
			byte[] jpegBytes = imageTexture.EncodeToJPG();

			string screenshotsPath = Path.Combine(Application.persistentDataPath, "screenshots");

			if (!Directory.Exists(screenshotsPath))
				Directory.CreateDirectory(screenshotsPath);
	
			var fileTime = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss");
			
			string filePath = Path.Combine(screenshotsPath, fileTime + ".jpeg");

			await using FileStream destination = File.OpenWrite(filePath);

			using var memory = new MemoryStream(jpegBytes);

			await memory.CopyToAsync(destination);
			
			Object.Destroy(imageTexture);
		}
	}
}