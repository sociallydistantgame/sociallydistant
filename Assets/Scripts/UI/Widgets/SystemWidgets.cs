#nullable enable

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
		private ButtonWidgetController buttonPrefab = null!;
        
		[SerializeField]
		private LabelWidgetController labelPrefab = null!;

		[SerializeField]
		private ImageWidgetController imagePrefab = null!;

		[SerializeField]
		private RawImageWidgetController rawImagePrefab = null!;

		[SerializeField]
		private ListWidgetController listPrefab = null!;

		[SerializeField]
		private ListItemWidgetController listItemPrefab = null!;
		
		[Header("Settings widgets")]
		[SerializeField]
		private SettingsSliderWidgetController sliderPrefab = null!;

		[SerializeField]
		private SettingsToggleWidgetController settingsTogglePrefab = null!;

		[SerializeField]
		private SettingsInputFieldController settingsInputFieldPrefab = null!;

		[SerializeField]
		private SettingsDropdownWidgetController settingsDropdownPrefab = null!;

		[Header("Chat")]
		[SerializeField]
		private ChatBubbleWidgetController chatBubblePrefab = null!;

		[SerializeField]
		private GuildHeaderWidgetController guildHeaderPrefab = null!;

		[Header("Steam Workshop Editors")]
		[SerializeField]
		private NamedColorEntryController namedColorEntryPrefab = null!;

		[SerializeField]
		private ThemeColorSelectWidgetController themeColorSelectPrefab = null!;
		
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
		public ButtonWidgetController GetButtonWidget(RectTransform destination)
		{
			return RecycleOrInstantiate(this.buttonPrefab, destination);
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

		/// <inheritdoc />
		public ListWidgetController GetList(RectTransform destination)
		{
			return RecycleOrInstantiate(listPrefab, destination);
		}

		/// <inheritdoc />
		public ListItemWidgetController GetListItem(RectTransform destination)
		{
			return RecycleOrInstantiate(listItemPrefab, destination);
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
		
		public GuildHeaderWidgetController GetGuildHeader(RectTransform destination)
		{
			return RecycleOrInstantiate(this.guildHeaderPrefab, destination);
		}

		public NamedColorEntryController GetNamedColorEntry(RectTransform destination)
		{
			return RecycleOrInstantiate(this.namedColorEntryPrefab, destination);
		}

		public ThemeColorSelectWidgetController GetThemeColorSelect(RectTransform destination)
		{
			return RecycleOrInstantiate(this.themeColorSelectPrefab, destination);
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