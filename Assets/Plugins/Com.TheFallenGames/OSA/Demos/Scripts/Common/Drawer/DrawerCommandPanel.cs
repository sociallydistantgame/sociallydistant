using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using frame8.Logic.Misc.Visual.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.Demos.Common.CommandPanels;

namespace Com.TheFallenGames.OSA.Demos.Common.Drawer
{
	/// <summary>
	/// Versatile context menu drawer that is programmatically configured for each demo scene to contain specific UI controls
	/// </summary>
	public class DrawerCommandPanel : MonoBehaviour
	{
		public event Action<int> ItemCountChangeRequested;
		public event Action<int> AddItemRequested;
		public event Action<int> RemoveItemRequested;


		public ButtonWithInputPanel buttonWithInputPrefab;
		public LabelWithInputPanel labelWithInputPrefab;
		public LabelWithToggle labelWithTogglePrefab;
		public LabelWithToggles labelWithTogglesPrefab;
		public LabelWithSliderPanel labelWithSliderPrefab;
		public ButtonsWithOptionalInputPanel buttonsPrefab;
		public Button openCloseButton;
		public Text itemsLimitNoteText;
		public bool skipAutoDraw;


		[NonSerialized] public Text titleText;
		[NonSerialized] public LoadSceneOnClick backButtonBehavior, nextButtonBehavior;
		[NonSerialized] public LabelWithToggles contentGravityPanel;
		[NonSerialized] public ButtonsWithOptionalInputPanel addRemoveOnePanel;
		[NonSerialized] public ButtonsWithOptionalInputPanel addRemoveOneAtIndexPanel;
		[NonSerialized] public ButtonWithInputPanel setCountPanel;
		[NonSerialized] public ScrollToPanel scrollToPanel;
		[NonSerialized] public RectTransform settingsPanel;
		[NonSerialized] public Toggle freezeItemEndEdgeToggle, freezeContentEndEdgeToggle;
		[NonSerialized] public LabelWithInputPanel serverDelaySetting;
		[NonSerialized] public LabelWithToggle forceLowFPSSetting;
		[NonSerialized] public LabelWithSliderPanel galleryEffectSetting;
		
		public int AdaptersCount { get { return _Adapters == null ? 0 : _Adapters.Length; } }

		const int TARGET_FRAMERATE_FOR_SIMULATING_SLOW_DEVICES = 20;

		ScrollRect _ScrollRect;
		IOSA[] _Adapters;
		float _LastScreenWidth, _LastScreenHeight;


		void Awake()
		{
			Application.targetFrameRate = 60;

			transform.GetComponentAtPath("SettingsScrollView/SettingsPanel", out settingsPanel);
			transform.GetComponentAtPath("TitlePanel/LabelText", out titleText);
			transform.GetComponentAtPath("NavPanelParent/BackPanel/Button", out backButtonBehavior);
			transform.GetComponentAtPath("NavPanelParent/NextPanel/Button", out nextButtonBehavior);
			settingsPanel.GetComponentAtPath("ScrollToPanel", out scrollToPanel);
			settingsPanel.GetComponentAtPath("SetCountPanel", out setCountPanel);

			if (openCloseButton)
			{
				openCloseButton.onClick.AddListener(() => {
					if (!_ScrollRect)
						return;

					if (_ScrollRect.horizontalNormalizedPosition > .5f)
						Draw(90f);
					else
						Draw(-90f);
				});
			}

			SetSceneName();

			var p = transform;
			while (!_ScrollRect && (p = p.parent))
				_ScrollRect = p.GetComponent<ScrollRect>();
		}

		void Start()
		{
			//if (itemsLimitNoteText)
			//	itemsLimitNoteText.text = "Some functionalities like item resizing are disabled due to count >" + OSAConst.MAX_ITEMS_TO_SUPPORT_ITEM_RESIZING;

			if (!skipAutoDraw)
				Draw(90f); // draw the scrollrect from the left
		}

		void Update()
		{
			// Debug
			//if (Input.touchCount == 3)
			//	gameObject.SetActive(false);

			//Debug.Log("RRR: " + QualitySettings.vSyncCount + ", " + Application.targetFrameRate);

			if (Screen.width != _LastScreenWidth && Screen.height != _LastScreenHeight)
			{
				if (Screen.width < Screen.height)
					Draw(-90f);

				_LastScreenWidth = Screen.width;
				_LastScreenHeight = Screen.height;
			}

			if (contentGravityPanel)
			{
				bool contentGravityEnabled = false;
				foreach (var adapter in _Adapters)
					if (adapter.GetContentSizeToViewportRatio() < 1d)
					{
						contentGravityEnabled = true;
						break;
					}

				contentGravityPanel.Interactable = contentGravityEnabled;
			}

			if (_ScrollRect)
			{
				bool enable = _ScrollRect.horizontalNormalizedPosition < .8f;
				if (settingsPanel && settingsPanel.gameObject.activeSelf != enable)
					settingsPanel.gameObject.SetActive(enable);
				if (_ScrollRect.verticalScrollbar && _ScrollRect.verticalScrollbar.gameObject.activeSelf != enable)
					_ScrollRect.verticalScrollbar.gameObject.SetActive(enable);
			}

			//bool enableNote = false;
			//if (_Adapters != null)
			//{
			//	foreach (var adapter in _Adapters)
			//	{
			//		if ((UnityEngine.Object)adapter == null)
			//			continue;

			//		if (!adapter.Initialized)
			//			continue;

			//		if (adapter.GetItemsCount() > OSAConst.MAX_ITEMS_TO_SUPPORT_ITEM_RESIZING)
			//			enableNote = true;
			//	}
			//}
			//if (itemsLimitNoteText && itemsLimitNoteText.transform.parent.gameObject.activeSelf != enableNote)
			//	itemsLimitNoteText.transform.parent.gameObject.SetActive(enableNote);
		}

		void OnDestroy()
		{
			if (forceLowFPSSetting)
				forceLowFPSSetting.toggle.isOn = false;
		}

		public void Init(
			IOSA adapter,
			bool addGravityCommand = true,
			bool addItemEdgeFreezeCommand = true,
			bool addContentEdgeFreezeCommand = true,
			bool addServerDelaySetting = true,
			bool addOneItemAddRemovePanel = true,
			bool addInsertRemoveAtIndexPanel = true
		) { Init(new IOSA[] { adapter }, addGravityCommand, addItemEdgeFreezeCommand, addContentEdgeFreezeCommand, addServerDelaySetting, addOneItemAddRemovePanel, addInsertRemoveAtIndexPanel); }

		public void Init(
			IOSA[] adapters, 
			bool addGravityCommand = true, 
			bool addItemEdgeFreezeCommand = true, 
			bool addContentEdgeFreezeCommand = true,
			bool addServerDelaySetting = true,
			bool addOneItemAddRemovePanel = true,
			bool addInsertRemoveAtIndexPanel = true
		) {
			_Adapters = adapters;

			scrollToPanel.mainSubPanel.button.onClick.AddListener(RequestSmoothScrollToSpecified);
			setCountPanel.button.onClick.AddListener(RequestChangeItemCountToSpecifiedIgnoringServerDelay);

			if (addGravityCommand)
			{
				contentGravityPanel = AddLabelWithTogglesPanel("Gravity when content smaller than viewport", "Start", "Center", "End");
				contentGravityPanel.ToggleChanged += (toggleIdx, isOn) =>
				{
					if (!isOn)
						return;

					DoForAllAdapters((adapter) =>
					{
						if (adapter.IsInitialized)
						{
							adapter.BaseParameters.Gravity = (BaseParams.ContentGravity)(toggleIdx + 1);
							adapter.BaseParameters.UpdateContentPivotFromGravityType();
							//adapter.SetVirtualAbstractNormalizedScrollPosition(1d, true); // scrollto start
						}
					});
				};
			}

			if (addItemEdgeFreezeCommand)
				freezeItemEndEdgeToggle = AddLabelWithTogglePanel("Freeze item end edge when expanding/collapsing").toggle;
			if (addContentEdgeFreezeCommand)
				freezeContentEndEdgeToggle = AddLabelWithTogglePanel("Freeze content end edge on add/remove/resize").toggle;

			if (addServerDelaySetting)
			{
				serverDelaySetting = AddLabelWithInputPanel("Server simulated delay:");
				serverDelaySetting.inputField.text = 1 + ""; // 1 second, initially
				serverDelaySetting.inputField.keyboardType = TouchScreenKeyboardType.NumberPad;
				serverDelaySetting.inputField.characterLimit = 1;
			}

			if (addOneItemAddRemovePanel)
			{
				addRemoveOnePanel = AddButtonsWithOptionalInputPanel("+1 tail", "+1 head", "-1 tail", "-1 head");
				addRemoveOnePanel.transform.SetSiblingIndex(1);
				addRemoveOnePanel.button1.onClick.AddListener(() => { AddItemWithChecks(true); });
				addRemoveOnePanel.button2.onClick.AddListener(() => { AddItemWithChecks(false); });
				addRemoveOnePanel.button3.onClick.AddListener(() => { RemoveItemWithChecks(true); });
				addRemoveOnePanel.button4.onClick.AddListener(() => { RemoveItemWithChecks(false); });
			}

			if (addInsertRemoveAtIndexPanel)
			{
				addRemoveOneAtIndexPanel = AddButtonsWithOptionalInputPanel("+1 at", "-1 at", "", "", "index");
				addRemoveOneAtIndexPanel.transform.SetSiblingIndex(1);
				addRemoveOneAtIndexPanel.button1.onClick.AddListener(() => AddItemWithChecks(addRemoveOneAtIndexPanel.InputFieldValueAsInt));
				addRemoveOneAtIndexPanel.button2.onClick.AddListener(() => RemoveItemWithChecks(addRemoveOneAtIndexPanel.InputFieldValueAsInt));
			}

			galleryEffectSetting = AddLabelWithSliderPanel("Gallery effect", "None", "Max");
			galleryEffectSetting.slider.onValueChanged.AddListener((v) =>
				DoForAllAdapters((adapter) =>
				{
					var gal = adapter.BaseParameters.effects.Gallery;
					//gal.OverallAmount = gal.Scale.Amount = gal.Rotation.Amount = v;
					gal.OverallAmount = v;
				})
			);
			galleryEffectSetting.Set(0f, 1f, .1f);

			// Simulate low end device toggle
			int vSyncCountBefore = QualitySettings.vSyncCount;
			int targetFrameRateBefore = Application.targetFrameRate;
			forceLowFPSSetting = AddLabelWithTogglePanel("Force low FPS");
			forceLowFPSSetting.transform.SetAsLastSibling();
			forceLowFPSSetting.toggle.onValueChanged.AddListener(isOn =>
			{
				if (isOn)
				{
					vSyncCountBefore = QualitySettings.vSyncCount;
					targetFrameRateBefore = Application.targetFrameRate;

					QualitySettings.vSyncCount = 0;  // VSync must be disabled for Application.targetFrameRate to work
					Application.targetFrameRate = TARGET_FRAMERATE_FOR_SIMULATING_SLOW_DEVICES;
				}
				else
				{
					if (QualitySettings.vSyncCount == 0) // if it wasn't modified since the last time we modified it (i.e. if it was modified externally, don't override that value)
						QualitySettings.vSyncCount = vSyncCountBefore;

					if (Application.targetFrameRate == TARGET_FRAMERATE_FOR_SIMULATING_SLOW_DEVICES) // item comment as above
						Application.targetFrameRate = targetFrameRateBefore;
				}
			});
		}

		public ButtonWithInputPanel AddButtonWithInputPanel(string label = "")
		{
			var go = Instantiate(buttonWithInputPrefab.gameObject) as GameObject;
			go.transform.SetParent(settingsPanel, false);
			var comp = go.GetComponent<ButtonWithInputPanel>();
			//buttonWithInput_Panels.Add(comp);
			comp.button.transform.GetComponentAtPath<Text>("Text").text = label;

			return comp;
		}

		public LabelWithInputPanel AddLabelWithInputPanel(string label = "")
		{
			var go = Instantiate(labelWithInputPrefab.gameObject) as GameObject;
			go.transform.SetParent(settingsPanel, false);
			var comp = go.GetComponent<LabelWithInputPanel>();
			//labelWithInput_Panels.Add(comp);
			comp.labelText.text = label;

			return comp;
		}

		public ButtonsWithOptionalInputPanel AddButtonsWithOptionalInputPanel(string label1 = "", string label2 = "", string label3 = "", string label4 = "", string inputFieldLabel = "")
		{
			var go = Instantiate(buttonsPrefab.gameObject) as GameObject;
			go.transform.SetParent(settingsPanel, false);
			var comp = go.GetComponent<ButtonsWithOptionalInputPanel>();
			//buttons_Panels.Add(comp);
			comp.Init(label1, label2, label3, label4, inputFieldLabel);

			return comp;
		}

		public LabelWithToggle AddLabelWithTogglePanel(string label = "")
		{
			var go = Instantiate(labelWithTogglePrefab.gameObject) as GameObject;
			go.transform.SetParent(settingsPanel, false);
			var comp = go.GetComponent<LabelWithToggle>();
			//labelWithToggle_Panels.Add(comp);
			comp.Init(label);

			return comp;
		}

		public LabelWithToggles AddLabelWithTogglesPanel(string mainLabel, params string[] subItemLabels)
		{
			var go = Instantiate(labelWithTogglesPrefab.gameObject) as GameObject;
			go.transform.SetParent(settingsPanel, false);
			var comp = go.GetComponent<LabelWithToggles>();
			//labelWithToggles_Panels.Add(comp);
			comp.Init(mainLabel, subItemLabels);

			return comp;
		}

		public LabelWithSliderPanel AddLabelWithSliderPanel(string label = "", string minLabel = "", string maxLabel = "")
		{
			var go = Instantiate(labelWithSliderPrefab.gameObject) as GameObject;
			go.transform.SetParent(settingsPanel, false);
			var comp = go.GetComponent<LabelWithSliderPanel>();
			//buttons_Panels.Add(comp);
			comp.Init(label, minLabel, maxLabel);

			return comp;
		}

		public void AddLoadCustomSceneButton(string title, string sceneName)
		{
			var buttons = AddButtonsWithOptionalInputPanel(title);
			var script = buttons.button1.gameObject.AddComponent<LoadSceneOnClick>();
			script.sceneName = sceneName;
			script.loadMode = LoadSceneOnClick.Mode.Specified;
			buttons.button1.image.color = backButtonBehavior.GetComponent<Image>().color;
			var backButtonText = backButtonBehavior.GetComponentInChildren<Text>();
			var loadNonOptimizedButtonText = buttons.button1.GetComponentInChildren<Text>();
			loadNonOptimizedButtonText.font = backButtonText.font;
			loadNonOptimizedButtonText.resizeTextForBestFit = backButtonText.resizeTextForBestFit;
			loadNonOptimizedButtonText.fontStyle = backButtonText.fontStyle;
			loadNonOptimizedButtonText.fontSize = backButtonText.fontSize;
			buttons.transform.SetAsFirstSibling();
		}

		#region UI events
		/// <summary>Callback from UI Button. Parses the text in the count input field as an int and sets it as the new item count, refreshing all the views. It mocks a basic server communication where you request x items and you receive them after a few seconds</summary>
		public void RequestChangeItemCountToSpecified()
		{
			int newCount = setCountPanel.InputFieldValueAsInt;
			if (ItemCountChangeRequested != null)
				ItemCountChangeRequested(newCount);
		}
		public void RequestChangeItemCountToSpecifiedIgnoringServerDelay()
		{
			var ignoreServerDelaySetting = serverDelaySetting != null;
			string textBefore = ignoreServerDelaySetting ? serverDelaySetting.inputField.text : null;
			if (ignoreServerDelaySetting)
				serverDelaySetting.inputField.text = "0";

			RequestChangeItemCountToSpecified();

			if (ignoreServerDelaySetting)
				serverDelaySetting.inputField.text = textBefore;
		}

		public void RequestSmoothScrollToSpecified() { RequestSmoothScrollTo(scrollToPanel.mainSubPanel.InputFieldValueAsInt, null); }
		#endregion

		public void RequestSmoothScrollTo(int index, Action onDone)
		{
			if (index < 0 || index + 1 > _Adapters[0].GetItemsCount() /*|| _Adapters[0].GetItemsCount() > OSAConst.MAX_ITEMS_TO_SUPPORT_SMOOTH_SCROLL_AND_ITEM_RESIZING*/)
				return;

			int numDone = 0;

			float dur = scrollToPanel.durationAdvPanel.InputFieldValueAsFloat;
			dur = Mathf.Clamp(dur, .01f, 100f);
			//if (_AdaptersParamsArray[0].loopItems && dur < .1f)
			//{
			//	dur = .1f;
			//	scrollToPanel.durationAdvPanel.inputField.text = dur + "";
			//}

			//scrollToPanel.mainSubPanel.button.interactable = false;
			DoForAllAdapters((adapter) =>
			{
				//if (adapterParams.data == null || adapterParams.data.Count > 10)
				//bool started = 
				adapter.SmoothScrollTo(
					index,
					dur,
					Mathf.Clamp01(scrollToPanel.gravityAdvPanel.InputFieldValueAsFloat),
					Mathf.Clamp01(scrollToPanel.itemPivotAdvPanel.InputFieldValueAsFloat),
					progress => 
					{
						scrollToPanel.durationAdvPanel.inputField.text = (dur * progress).ToString("#0.##"); // show the scroll progress

						return true;
					},
					() =>
					{
						if (++numDone == _Adapters.Length) // only do it when the last one has finished
						{
							//scrollToPanel.mainSubPanel.button.interactable = true;
							if (onDone != null)
								onDone();
						}
					},
					true
				);

				//if (!started)
				//	scrollToPanel.mainSubPanel.button.interactable = true;
			});
		}

		public void DoForAllAdapters(Action<IOSA> action)
		{
			for (int i = 0; i < _Adapters.Length; i++)
				action(_Adapters[i]);
		}

		void AddItemWithChecks(bool atTail) { AddItemWithChecks(atTail ? _Adapters[0].GetItemsCount() : 0); }
		void AddItemWithChecks(int index)
		{
			if (AddItemRequested != null)
			{
				int c;
				if (index >= 0 && index <= (c=_Adapters[0].GetItemsCount()) && c < OSAConst.MAX_ITEMS)
					AddItemRequested(index);
			}
		}

		void RemoveItemWithChecks(bool fromTail) { RemoveItemWithChecks(fromTail ? _Adapters[0].GetItemsCount() - 1 : 0); }
		void RemoveItemWithChecks(int index)
		{
			if (RemoveItemRequested != null)
			{
				if (index >= 0 && index < _Adapters[0].GetItemsCount())
					RemoveItemRequested(index);
			}
		}

		public void Draw(float horSpeed)
		{
			PointerEventData ped = new PointerEventData(EventSystem.current);
			ped.scrollDelta = new Vector2(horSpeed, 0f);
			_ScrollRect.OnScroll(ped);
			_ScrollRect.velocity = ped.scrollDelta * 7;
		}

		void SetSceneName()
		{
			string sceneFriendlyName;
#if UNITY_5_3_OR_NEWER
			sceneFriendlyName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
#else
			sceneFriendlyName = Application.loadedLevelName;
#endif
			sceneFriendlyName = sceneFriendlyName.Replace("_", " ");

			// Hardcoding the main scene's name, because "example" is not that descriptive, adn changing the scene would break the scene indices for those that update from a previous package
			if (sceneFriendlyName == "example")
				sceneFriendlyName = "main demo";
			else
				sceneFriendlyName = sceneFriendlyName.Replace("example", "");

			sceneFriendlyName = char.ToUpper(sceneFriendlyName[0]) + sceneFriendlyName.Substring(1);
			titleText.text = sceneFriendlyName;
		}
	}
}
