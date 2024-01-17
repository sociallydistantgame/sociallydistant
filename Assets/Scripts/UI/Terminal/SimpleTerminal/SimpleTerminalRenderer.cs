using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using OS.Devices;
using TMPro;
using UI.Terminal.SimpleTerminal.Data;
using UI.Terminal.SimpleTerminal.Pty;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityExtensions;
using Utility;
using static UI.Terminal.SimpleTerminal.Data.EmulatorConstants;
using BinaryExpression = System.Linq.Expressions.BinaryExpression;
using Expression = System.Linq.Expressions.Expression;
using Image = UnityEngine.UI.Image;

namespace UI.Terminal.SimpleTerminal
{
    public class SimpleTerminalRenderer :
        MonoBehaviour,
        IUpdateSelectedHandler,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerMoveHandler,
        ISelectHandler,
        IDeselectHandler,
        IScrollHandler,
        IClipboard
    {
        private UguiTerminalScreen screen;
        private SimpleTerminal simpleTerminal;
        
        private RectTransform rectTransform;
        private Camera myCamera;
        private LayoutElement layoutElement;
        private RectTransform parentRectTransform;
        private int tickInterval;
        private RectTransform parentRect;
        private RectTransform rect;
        private TerminalOptions ptyOptions = new TerminalOptions();
        private PseudoTerminal master;
        private PseudoTerminal slave;
        private bool isFocused;
        private int oldRows;
        private int oldColumns;
        private int rowCount = 43;
        private int columnCount = 132;
        


        public int DefaultBackgroundId => SimpleTerminal.defaultbg;
        public int DefaultForegroundId => SimpleTerminal.defaultfg;

        private float ww;
        private float wh;

        [Header("Settings")]
        [SerializeField]
        private TMP_FontAsset font = null!;

        [SerializeField]
        private int fontSize = 12;
        
        [SerializeField]
        private bool allowAltScreen = true;

        [SerializeField]
        private string vtiden = "st";
        
        [SerializeField]
        private int tabSpaces = 8;

        [Header("Timing")]
        [SerializeField]
        private float minLatency;

        [SerializeField]
        private float doubleClickTime = 0;

        [SerializeField]
        private float trippleClickTime = 0;

        [SerializeField]
        private float maxLatency;

        [SerializeField]
        private float blinkTimeout;

        [SerializeField]
        private int mouseScrollLineCount = 3;

        [Header("Layout")]
        [SerializeField]
        private VerticalLayoutGroup textAreaGroup = null!;
        
        public RectTransform TextAreaTransform => textAreaGroup.transform as RectTransform;
        public int DefaultRowCount => this.rowCount;
        public int DefaultColumnCount => this.columnCount;
        
        private void Awake()
        {
            this.MustGetComponentInChildren(out screen);
            this.MustGetComponent(out rectTransform);
            this.MustGetComponent(out layoutElement);

            this.myCamera = Camera.main;
            
            this.transform.parent.MustGetComponent(out parentRectTransform);
        }

        private void Start()
        {
            this.simpleTerminal = new SimpleTerminal(this, screen, minLatency, maxLatency, this.columnCount, this.rowCount);
            this.simpleTerminal.BlinkTimeout = blinkTimeout;
            this.simpleTerminal.DoubleClickTime = doubleClickTime;
            this.simpleTerminal.TripleClickTime = trippleClickTime;
            this.simpleTerminal.MouseScrollLinesCount = mouseScrollLineCount;
            this.simpleTerminal.TerminalIdentifier = vtiden;
            this.simpleTerminal.AllowAltScreen = allowAltScreen;
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
            if (this.master is null)
                return;

            if (font == null)
                return;
            
            float nww = this.parentRectTransform.rect.width;
            float nwh = this.parentRectTransform.rect.height;
            if (this.ww != nww || this.wh != nwh)
            {
                this.ww = nww;
                this.wh = nwh;

                float cw = screen.UnscaledCharacterWidth;
                float ch = screen.UnscaledLineHeight;

                int r = (int)Math.Ceiling(this.wh / ch) - 1;
                int c = (int)Math.Ceiling(this.ww / cw) - 1;

                r = Math.Max(r, this.DefaultRowCount);
                c = Math.Max(c, this.DefaultColumnCount);

                this.layoutElement.minWidth = DefaultColumnCount * cw;
                this.layoutElement.minHeight = this.DefaultRowCount * ch;
                if (r != simpleTerminal.Rows || c != simpleTerminal.Columns)
                {
                    simpleTerminal.Resize(c, r);
                }
            }

            this.simpleTerminal.Update(Time.deltaTime);
        }
        
        public bool Selected(int x, int y)
        {
            return simpleTerminal.Selected(x, y);
        }
        
        private void TtyInit()
        {
            // Enforce CRLF
            this.ptyOptions.LFlag = 0;

            // Control codes
            this.ptyOptions.C_cc[PtyConstants.VERASE] = (byte)'\b';

            PseudoTerminal.CreatePair(out this.master, out this.slave, this.ptyOptions);

            this.simpleTerminal.SetTty(new SociallyDistantTty(this.master));
        }

        public ITextConsole StartSession()
        {
            this.TtyInit();

            return new SimpleTerminalSession(this.simpleTerminal, this.slave);
        }

        
        

        #region Unity UI Event Handlers
        
        public void OnPointerMove(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, null, out Vector2 localPosition);

            if (localPosition.x < 0 || localPosition.y < 0)
                return;

            float x = Math.Abs(localPosition.x);
            float y = Math.Abs(localPosition.y);

            simpleTerminal.MouseMove(x, y);
        }

        public void OnScroll(PointerEventData eventData)
        {
            float delta = eventData.scrollDelta.y;

            simpleTerminal.MouseScroll(delta);
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



                    // KeyCode.None means we're getting text, send a character instead.
                    if (ev.keyCode == KeyCode.None)
                    {
                        simpleTerminal.Input.Char(ev.character);
                        continue;
                    }

                    bool isFunctionKey = ev.functionKey;
                    bool isSpecial = control || alt;
                    bool isPrintable = !char.IsControl((char) ev.keyCode);

                    if (isPrintable && !isFunctionKey && !isSpecial)
                        continue;
                    
                    simpleTerminal.Input.Raw(ev.keyCode, control, alt, shift);
                }

            eventData.Use();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, null, out Vector2 localPosition);

            if (localPosition.x < 0 || localPosition.y < 0)
                return;

            float x = Math.Abs(localPosition.x);
            float y = Math.Abs(localPosition.y);
            
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                this.simpleTerminal.MouseUp(MouseButton.Left, x, y);
            }
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (EventSystem.current == null)
                return;
            
            EventSystem.current.SetSelectedGameObject(this.gameObject);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(this.rectTransform, eventData.position, null, out Vector2 localPosition);

            if (localPosition.x < 0 || localPosition.y < 0)
                return;

            float x = Math.Abs(localPosition.x);
            float y = Math.Abs(localPosition.y);

            if (eventData.button == PointerEventData.InputButton.Left)
            {
                simpleTerminal.MouseDown(MouseButton.Left, x, y);
            }

            if (eventData.button == PointerEventData.InputButton.Right)
            {
                simpleTerminal.MouseDown(MouseButton.Right, x, y);
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
            return PlatformHelper.GetClipboardText();
        }

        /// <inheritdoc />
        public void SetText(string text)
        {
            PlatformHelper.SetClipboardText(text);
        }
    }
}