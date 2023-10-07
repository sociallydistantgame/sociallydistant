#nullable enable
using System;
using Social;
using UI.Popovers;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;

namespace UI.Applications.Chat
{
	public class GuildIconView : MonoBehaviour
	{
		[Header("UI")]
		[SerializeField]
		private Toggle toggle = null!;

		[SerializeField]
		private RawImage iconImage = null!;

		private Popover popover;
		private GuildItemModel model;
		
		public Action<IGuild>? Callback { get; set; }
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(GuildIconView));
			this.MustGetComponent(out popover);
		}

		private void Start()
		{
			toggle.onValueChanged.AddListener(OnValueChanged);
		}

		public void UpdateModel(GuildItemModel model)
		{
			this.model = model;
			
			toggle.group = model.ToggleGroup;
			toggle.SetIsOnWithoutNotify(model.IsSelected);

			this.iconImage.texture = model.GuildIcon;

			popover.Text = model.Guild?.Name ?? string.Empty;
		}

		private void OnValueChanged(bool newValue)
		{
			model.IsSelected = newValue;

			if (model.IsSelected && model.Guild != null)
				Callback?.Invoke(model.Guild);
		}
	}
}