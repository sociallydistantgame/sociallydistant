﻿#nullable enable

using AcidicGui.Widgets;
using UI.Applications.Chat;
using UI.Widgets.Settings;
using UnityEngine;

namespace UI.Widgets
{
	[CreateAssetMenu(menuName = "System Widgets")]
	public class SystemWidgets :
		ScriptableObject,
		IWidgetAssembler
	{
		private WidgetRecycleBin? recycleBin;

		[SerializeField]
		private SectionWidgetController sectionPrefab = null!;
		
		[SerializeField]
		private LabelWidgetController labelPrefab = null!;

		[SerializeField]
		private ImageWidgetController imagePrefab = null!;

		[SerializeField]
		private RawImageWidgetController rawImagePrefab = null!;
		
		[Header("Settings widgets")]
		[SerializeField]
		private SettingsSliderWidgetController sliderPrefab = null!;

		[SerializeField]
		private SettingsToggleWidgetController settingsTogglePrefab = null!;

		[SerializeField]
		private SettingsInputFieldController settingsInputFieldPrefab = null!;

		[SerializeField]
		private SettingsDropdownWidgetController settingsDropdownPrefab = null!;

		[SerializeField]
		private ChatBubbleWidgetController chatBubblePrefab = null!;
		
		/// <inheritdoc />
		public WidgetRecycleBin RecycleBin
		{
			get
			{
				if (recycleBin == null)
					recycleBin = new WidgetRecycleBin();

				return recycleBin;
			}
		}

		/// <inheritdoc />
		public SectionWidgetController GetSectionWidget(RectTransform destination)
		{
			return RecycleOrInstantiate(this.sectionPrefab, destination);
		}
		
		/// <inheritdoc />
		public LabelWidgetController GetLabel(RectTransform destination)
		{
			return RecycleOrInstantiate(this.labelPrefab, destination);
		}

		/// <inheritdoc />
		public ImageWidgetController GetImage(RectTransform destination)
		{
			return RecycleOrInstantiate(imagePrefab, destination);
		}

		/// <inheritdoc />
		public RawImageWidgetController GetRawImage(RectTransform destination)
		{
			return RecycleOrInstantiate(rawImagePrefab, destination);
		}

		public SettingsSliderWidgetController GetSettingsSlider(RectTransform destination)
		{
			return RecycleOrInstantiate(this.sliderPrefab, destination);
		}
		
		public SettingsToggleWidgetController GetSettingsToggle(RectTransform destination)
		{
			return RecycleOrInstantiate(this.settingsTogglePrefab, destination);
		}

		public SettingsInputFieldController GetSettingsInputField(RectTransform destination)
		{
			return RecycleOrInstantiate(this.settingsInputFieldPrefab, destination);
		}
		
		public SettingsDropdownWidgetController GetSettingsDropdown(RectTransform destination)
		{
			return RecycleOrInstantiate(this.settingsDropdownPrefab, destination);
		}
		
		public ChatBubbleWidgetController GetChatBubble(RectTransform destination)
		{
			return RecycleOrInstantiate(this.chatBubblePrefab, destination);
		}
		
		
		
		private T RecycleOrInstantiate<T>(T prefabToInstantiate, RectTransform rectTransform)
			where T : WidgetController
		{
			T? recycled = this.RecycleBin.TryGetRecycled<T>(rectTransform);

			if (recycled != null)
				return recycled;

			return Instantiate(prefabToInstantiate, rectTransform);
		}
	}
}