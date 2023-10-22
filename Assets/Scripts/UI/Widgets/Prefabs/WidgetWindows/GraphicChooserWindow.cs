#nullable enable

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AcidicGui.Widgets;
using Cysharp.Threading.Tasks;
using Shell.Windowing;
using UI.UiHelpers;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityExtensions;

namespace UI.Widgets.Prefabs.WidgetWindows
{
	public class GraphicChooserWindow : 
		MonoBehaviour,
		IWindowCloseBlocker
	{
		[SerializeField]
		private WidgetList graphicList = null!;

		[SerializeField]
		private RawImage graphic = null!;

		[SerializeField]
		private Button browse = null!;

		[SerializeField]
		private Button select = null!;

		[SerializeField]
		private Button cancel = null!;

		private DialogHelper dialogHelper;
		private AspectRatioFitter aspectRatioFitter;
		private Action? cancelCallback;
		private Action<string?>? callback;
		private string? graphicName = string.Empty;
		private IGraphicPickerSource? source;

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(GraphicChooserWindow));
			this.graphic.MustGetComponent(out aspectRatioFitter);
			this.MustGetComponent(out dialogHelper);
		}

		private void Start()
		{
			cancel.onClick.AddListener(Cancel);
			select.onClick.AddListener(Select);
			browse.onClick.AddListener(Import);
		}

		private void Cancel()
		{
			cancelCallback?.Invoke();
		}

		private void Select()
		{
			callback?.Invoke(graphicName);
		}
		
		public Task<string?> GetNewGraphic(string? currentGraphic, IGraphicPickerSource source)
		{
			this.graphicName = currentGraphic;
			this.source = source;

			if (this.source == null)
				return Task.FromResult(currentGraphic);

			var completionSource = new TaskCompletionSource<string?>();

			cancelCallback = () => completionSource.SetResult(currentGraphic);
			callback = completionSource.SetResult;

			SetupUI();
			
			return completionSource.Task;
		}

		private void SetGraphic(string newGraphic)
		{
			this.graphicName = newGraphic;
			this.RefreshCurrentGraphic();
		}
		
		private void SetupUI()
		{
			RefreshGraphicList();
			RefreshCurrentGraphic();
		}

		private void RefreshGraphicList()
		{
			var builder = new WidgetBuilder();

			builder.Begin();

			var list = new ListWidget
			{
				AllowSelectNone = false
			};

			builder.AddSection("Select graphic", out SectionWidget section)
				.AddWidget(list, section);

			builder.AddWidget(new ListItemWidget<string>()
			{
				Data = string.Empty,
				Callback = SetGraphic,
				Selected = graphic.texture == null,
				List = list,
				Title = "No graphic"
			}, section);

			if (source != null)
			{
				foreach (string gname in source.GetGraphicNames())
				{
					builder.AddWidget(new ListItemWidget<string>()
					{
						Data = gname,
						Callback = SetGraphic,
						Selected = this.graphicName == gname,
						List = list,
						Title = gname
					}, section);
				}
			}

			graphicList.SetItems(builder.Build());
		}
		
		private void RefreshCurrentGraphic()
		{
			Texture2D? texture = source?.GetGraphic(graphicName ?? string.Empty);

			graphic.texture = texture;
			graphic.color = texture != null ? Color.white : default;

			if (texture != null)
				aspectRatioFitter.aspectRatio = (float) texture.width / (float) texture.height;
		}

		/// <inheritdoc />
		public bool CheckCanClose()
		{
			cancelCallback?.Invoke();
			return true;
		}

		private async void Import()
		{
			string importPath = await dialogHelper.OpenFile("Import texture", Environment.CurrentDirectory, "JPEG Image|*.jpg,.jpeg|PNG image|*.png");
			if (string.IsNullOrWhiteSpace(importPath))
				return;

			string filename = Path.GetFileNameWithoutExtension(importPath);
			string extension = Path.GetExtension(importPath);

			if (string.IsNullOrWhiteSpace(filename))
				return;
            
			var graphicFileName = string.Empty;
			await Task.Run(() =>
			{
				var sb = new StringBuilder();
				var identifier = 0;
				
				do
				{
					sb.Append(filename);

					if (identifier > 0)
					{
						sb.Append(" (");
						sb.Append(identifier);
						sb.Append(")");
					}
                    
					sb.Append(extension);
					graphicFileName = sb.ToString();
					identifier++;
				} while (source != null && source.GetGraphic(graphicFileName) != null);
			});

			using UnityWebRequest? textureRequest = UnityWebRequestTexture.GetTexture(importPath);

			await textureRequest.SendWebRequest();

			Texture2D? texture = DownloadHandlerTexture.GetContent(textureRequest);
			if (texture == null)
				return;
			
			source?.SetGraphic(graphicFileName, texture);

			this.graphicName = graphicFileName;
			SetupUI();
		}
	}
}