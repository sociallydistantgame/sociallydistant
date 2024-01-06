#nullable enable

using GamePlatform;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityExtensions;
using UnityEngine.UI;

namespace UI.Controllers
{
	public class StatusBarController : UIBehaviour
	{
		[Header("Dependencies")]
		[SerializeField]
		private GameManagerHolder gameManagerHolder = null!;

		[SerializeField]
		private PlayerInstanceHolder player = null!;
		
		[Header("UI")]
		[SerializeField]
		private RectTransform userArea = null!;

		[SerializeField]
		private TextMeshProUGUI userText = null!;

		[SerializeField]
		private Button systemSettingsButton = null!;

		public string UserInfo
		{
			get => userText.text;
			set => userText.SetText(value);
		}
		
		/// <inheritdoc />
		protected override void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(StatusBarController));
			base.Awake();
		}

		/// <inheritdoc />
		protected override void Start()
		{
			base.Start();
			
			this.systemSettingsButton.onClick.AddListener(OpenSettings);
		}

		private void OpenSettings()
		{
			if (player.Value.UiManager == null)
				return;

			player.Value.UiManager.OpenSettings();
		}
	}
}