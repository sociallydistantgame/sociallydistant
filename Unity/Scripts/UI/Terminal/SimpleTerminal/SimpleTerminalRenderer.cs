using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using OS.Devices;
using TMPro;
using UI.Terminal.SimpleTerminal.Pty;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityExtensions;
using Utility;
using TrixelCreative.TrixelAudio;
using TrixelCreative.TrixelAudio.Data;
using OS.Network.MessageTransport;
using UI.CustomGraphics;
using UI.PlayerUI;
using System.Threading.Tasks;
using TrixelCreative.TrixelAudio.Core;

namespace UI.Terminal.SimpleTerminal
{
	public class SimpleTerminalRenderer :
		MonoBehaviour,
		ITerminalSounds,
		IUpdateSelectedHandler,
		IPointerDownHandler,
		IPointerUpHandler,
		ISelectHandler,
		IDeselectHandler,
		IScrollHandler,
		IClipboard,
		IEndDragHandler,
		IDragHandler
	{
		[Header("Color Plotters")]
		
		private RectanglePlotter backgroundColorPlotter = null!;

		[Header("Text")]
		
		private TMP_FontAsset font;
		
		
		private int fontSize;

		
		private TextMeshProUGUI textMeshPro = null!;

		[Header("Sound Effects")]
		
		private bool enableTypingSounds = true;

		
		private bool audibleBell = true;
		
		
		private SoundEffectAsset asciiBeep = null!;

		
		private SoundEffectAsset typingSound = null!;
		
		[Header("Appearance")]
		
		private int minimumColumnCount = 132;
		
		
		private int minimumRowCount = 43;
		
		[Header("Settings")]
		
		private bool allowAltScreen = true;

		
		private string vtiden = "st";
		
		[Header("Timing")]
		
		private float minLatency;

		
		private float doubleClickTime = 0;

		
		private float trippleClickTime = 0;

		
		private float maxLatency;

		
		private float blinkTimeout;

		
		private int mouseScrollLineCount = 3;

		[Header("Layout")]
		
		private ThreadSafeTerminalRenderer terminalRenderer;
		private LayoutElement layoutElement;
		private float ww;
		private float wh;
		private RectTransform parentRectTransform;
		private RectTransform rectTransform;
		
		public RectTransform TextAreaTransform => textAreaGroup.transform as RectTransform;
		public int DefaultRowCount => this.rowCount;
		public int DefaultColumnCount => this.columnCount;
		public int DefaultBackgroundId => SimpleTerminal.defaultbg;
		public int DefaultForegroundId => SimpleTerminal.defaultfg;
		public float LineHeight => lineHeight;
		public float CharacterWidth => characterWidth;
		public float UnscaledLineHeight { get; private set; }
		public float UnscaledCharacterWidth { get; private set; }
		public int Columns => this.simpleTerminal.Columns;
		public int Rows => this.simpleTerminal.Rows;

		public bool EnableAudibleBell
		{
			get => audibleBell;
			set => audibleBell = value;
		}
		
		public bool EnableTypingSounds
		{
			get => enableTypingSounds;
			set => enableTypingSounds = value;
		}
		
		public string WindowTitle => this.simpleTerminal.WindowTitle;
		

		private void Awake()
		{
			this.MustGetComponent(out rectTransform);
			this.MustGetComponent(out layoutElement);
			this.MustGetComponent(out layoutElement);

			this.terminalRenderer = new ThreadSafeTerminalRenderer(
				this,
				workQueue,
				this.layoutElement,
				this.textMeshPro,
				backgroundColorPlotter
			);
			
			textMeshPro.font = this.font;
			textMeshPro.fontSize = this.fontSize;

			this.CalculateTextSize();
			
			this.transform.parent.MustGetComponent(out parentRectTransform);
			
			this.simpleTerminal = new SimpleTerminal(this, terminalRenderer, this, minLatency, maxLatency, this.minimumColumnCount, this.minimumRowCount);
			this.simpleTerminal.BlinkTimeout = blinkTimeout;
			this.simpleTerminal.DoubleClickTime = doubleClickTime;
			this.simpleTerminal.TripleClickTime = trippleClickTime;
			this.simpleTerminal.MouseScrollLinesCount = mouseScrollLineCount;
			this.simpleTerminal.TerminalIdentifier = vtiden;
			this.simpleTerminal.AllowAltScreen = allowAltScreen;
			
			this.TtyInit();
		}

		private void OnEnable()
		{
			emulatorEnabled = true;
			emulatorShutdownCompleted.Reset();

			emulatorThread = new Thread(EmulatorUpdate);
			emulatorThread.Start();
		}

		private void OnDisable()
		{
			emulatorEnabled = false;
			emulatorShutdownCompleted.WaitOne();
			emulatorThread = null;
		}
		
		private void OnDestroy()
		{
			this.master.Close();
			this.slave.Close();

			this.master = null;
			this.slave = null;
		}

		private void Update()
		{
			if (resizeTask != null)
			{
				if (resizeTask.IsCanceled)
					resizeTask = null;
				
				return;
			}
			
			if (this.master is null)
				return;

			if (font == null)
				return;
			
			Rect parentArea = this.parentRectTransform.rect;
			float nww = parentArea.width;
			float nwh = parentArea.height;
			const float tolerance = 0.001f;
			if (Math.Abs(this.ww - nww) > tolerance || Math.Abs(this.wh - nwh) > tolerance)
			{
				this.ww = nww;
				this.wh = nwh;

				float cw = UnscaledCharacterWidth;
				float ch = UnscaledLineHeight;

				int r = (int)Math.Ceiling(this.wh / ch) - 1;
				int c = (int)Math.Ceiling(this.ww / cw) - 1;

				r = Math.Max(r, this.DefaultRowCount);
				c = Math.Max(c, this.DefaultColumnCount);

				this.layoutElement.minWidth = DefaultColumnCount * cw;
				this.layoutElement.minHeight = this.DefaultRowCount * ch;
				if (r != simpleTerminal.Rows || c != simpleTerminal.Columns)
				{
					// Must be executed on the emulator thread since the emulator itself is not threadsafe
					resizeTask = emulatorWorkQueue.EnqueueAsync(() => { simpleTerminal.Resize(c, r); });
				}
			}
		}

		private void EmulatorUpdate()
		{
			var stopwatch = new Stopwatch();

			var lastTime = 0f;
			while (emulatorEnabled)
			{
				if (this.simpleTerminal == null)
					break;
				
				stopwatch.Start();

				emulatorWorkQueue.RunPendingWork();
				
				this.simpleTerminal.Update(lastTime);

				stopwatch.Stop();

				lastTime = (float) stopwatch.Elapsed.TotalSeconds;
				
				stopwatch.Reset();
			}

			emulatorShutdownCompleted.Set();
		}

		private void LateUpdate()
		{
			workQueue.RunPendingWork();
			terminalRenderer.Present();
		}

		public void Bell()
		{
			if (this.asciiBeep != null && audibleBell)
				AudioManager.PlaySound(this.asciiBeep);
		}
		
		private void TtyInit()
		{
			// Enforce CRLF
			this.ptyOptions.LFlag = 0;

			// Control codes
			this.ptyOptions.C_cc[PtyConstants.VERASE] = (byte)'\b';

			PseudoTerminal.CreatePair(out this.master, out this.slave, this.ptyOptions);

			this.simpleTerminal.SetTty(new SociallyDistantTty(this.master));
			this.console = new SimpleTerminalSession(this.simpleTerminal, this.slave, new RepeatableCancellationToken(tokenSource));
		}

		public ITextConsole StartSession()
		{
			return console;
		}




		#region Unity UI Event Handlers

		public void OnScroll(PointerEventData eventData)
		{
			float delta = eventData.scrollDelta.y;

			emulatorWorkQueue.Enqueue(() =>
			{
				simpleTerminal.MouseScroll(delta);
			});
		}

		public void PlayTypingSound()
		{
			if (!enableTypingSounds)
				return;

			if (this.typingSound == null)
				return;
			
			AudioManager.PlaySound(typingSound);
		}
		
		public void OnUpdateSelected(BaseEventData eventData)
		{
			if (!simpleTerminal.IsFocused)
				return;

			var ev = new Event();
			while (Event.PopEvent(ev))
				if (ev.rawType == EventType.KeyDown)
				{
					// Force redraw on keyboard events, just like st does
					simpleTerminal.Redraw();

					bool control = (ev.modifiers & (EventModifiers.Control | EventModifiers.Command)) != 0;
					bool alt = (ev.modifiers & EventModifiers.Alt) != 0;
					bool shift = (ev.modifiers & EventModifiers.Shift) != 0;
					bool modifiers = control
					                 || alt
					                 || shift;

					bool bullshit = Application.platform == RuntimePlatform.WindowsEditor
					                || Application.platform == RuntimePlatform.WindowsPlayer;

					// KeyCode.None means we're getting text, send a character instead.
					if (ev.keyCode == KeyCode.None && bullshit)
					{
						simpleTerminal.Input.Char(ev.character);
						continue;
					}

					bool isFunctionKey = ev.functionKey;
					bool isSpecial = control || alt;
					bool isPrintable = !char.IsControl((char) ev.keyCode);

					if (isPrintable && !isFunctionKey && !isSpecial)
					{
						if (!bullshit)
							simpleTerminal.Input.Char(ev.character);

						continue;
					}

					if (control && !alt && !shift && ev.keyCode == KeyCode.C)
					{
						this.console.WriteText("^C");
						tokenSource.CancelAll();
						return;
					}
					
					simpleTerminal.Input.Raw(ev.keyCode, control, alt, shift);
				}

			eventData.Use();
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, UiManager.UiCamera, out Vector2 localPosition);

			if (localPosition.x < 0 || localPosition.y < 0)
				return;

			float x = Math.Abs(localPosition.x);
			float y = Math.Abs(localPosition.y);

			if (eventData.button == PointerEventData.InputButton.Left)
			{
				emulatorWorkQueue.Enqueue(() => 
				{
					this.simpleTerminal.MouseUp(MouseButton.Left, x, y);
				});
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (EventSystem.current == null)
				return;

			EventSystem.current.SetSelectedGameObject(this.gameObject);

			var clickMode = ClickMode.Single;
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				if (clickDoubleTime > 0)
				{
					if (Time.time - clickDoubleTime <= trippleClickTime)
						clickMode = ClickMode.Triple;

					clickTime = 0;
					clickDoubleTime = 0;
				}
				else if (clickTime > 0)
				{
					if (Time.time - clickTime <= trippleClickTime)
					{
						clickDoubleTime = Time.time;
						clickMode = ClickMode.Double;
					}

					clickTime = 0;
				}
				else
				{
					clickDoubleTime = 0;
					clickTime = Time.time;
					clickMode = ClickMode.Single;
				}
			}
			else
			{
				this.clickCount = 0;
			}
			
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this.rectTransform, eventData.position, UiManager.UiCamera, out Vector2 localPosition);

			if (localPosition.x < 0 || localPosition.y < 0)
				return;

			float x = Math.Abs(localPosition.x);
			float y = Math.Abs(localPosition.y);

			if (eventData.button == PointerEventData.InputButton.Left)
			{
				emulatorWorkQueue.Enqueue(() => 
				{
					simpleTerminal.MouseDown(MouseButton.Left, x, y, clickMode);
				});
			}

			if (eventData.button == PointerEventData.InputButton.Right)
			{
				emulatorWorkQueue.Enqueue(() => 
				{
					simpleTerminal.MouseDown(MouseButton.Right, x, y, ClickMode.Single);
				});
			}
		}

		public void OnSelect(BaseEventData eventData)
		{
			simpleTerminal.SetFocus(true);
		}

		public void OnDeselect(BaseEventData eventData)
		{
			simpleTerminal.SetFocus(false);
		}

		#endregion

		/// <inheritdoc />
		public string GetText()
		{
			// marijuana may be needed to comprehend this cursed shit
			if (Thread.CurrentThread.ManagedThreadId != mainThreadId)
			{
				var threadedResult = string.Empty;
				workQueue.EnqueueAsync(() =>
				{
					threadedResult = GetText();
				}).Wait();
				return threadedResult;
			}
			
			return PlatformHelper.GetClipboardText();
		}

		/// <inheritdoc />
		public void SetText(string text)
		{
			if (Thread.CurrentThread.ManagedThreadId != mainThreadId)
			{
				workQueue.Enqueue(() =>
				{
					SetText(text);
				});
				return;
			}
			
			PlatformHelper.SetClipboardText(text);
		}

		private void CalculateTextSize()
		{
			char ch = '#';
			var style = FontStyles.Normal;
			float fs = fontSize;

			// Compute scale of the target point size relative to the sampling point size of the font asset.
			float pointSizeScale = fs / (font.faceInfo.pointSize * font.faceInfo.scale);
			float emScale = fs * 0.01f;



			float styleSpacingAdjustment = (style & FontStyles.Bold) == FontStyles.Bold ? font.boldSpacing : 0;
			float normalSpacingAdjustment = font.normalSpacingOffset;

			// Make sure the given unicode exists in the font asset.
			font.TryAddCharacters(ch.ToString());
			if (!font.characterLookupTable.TryGetValue(ch, out TMP_Character character))
				character = font.characterLookupTable['?'];
			float width = (character.glyph.metrics.horizontalAdvance * pointSizeScale +
			               (styleSpacingAdjustment + normalSpacingAdjustment) * emScale);

			Vector3 scale = transform.lossyScale;

			float height = font.faceInfo.lineHeight
			               / font.faceInfo.pointSize
			               * fs;

			this.characterWidth = width / scale.x;
			this.lineHeight = height / scale.y;
			UnscaledLineHeight = height;
			UnscaledCharacterWidth = width;
		}
		
		/// <inheritdoc />
		public void OnEndDrag(PointerEventData eventData)
		{
			OnPointerUp(eventData);
		}

		/// <inheritdoc />
		public void OnDrag(PointerEventData eventData)
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, UiManager.UiCamera, out Vector2 localPosition);

			if (localPosition.x < 0 || localPosition.y < 0)
				return;

			float x = Math.Abs(localPosition.x);
			float y = Math.Abs(localPosition.y);

			this.emulatorWorkQueue.Enqueue(() =>
			{
				simpleTerminal.MouseMove(x, y);
			});
		}
	}
}