using TrixelCreative.TrixelAudio.Core;
using TrixelCreative.TrixelAudio.Data;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TrixelCreative.TrixelAudio.Players
{
	[RequireComponent(typeof(RectTransform))]
	public sealed class UserInterfaceNavigationSounds : 
		AudioPlayerBase,
		ISelectHandler,
		IPointerEnterHandler,
		IPointerClickHandler,
		ISubmitHandler,
		ICancelHandler
	{
		/// <inheritdoc />
		public void OnSelect(BaseEventData eventData)
		{
			if (!TryGetUiSounds(out UserInterfaceSoundSchemeAsset uiSounds))
				return;
			
			uiSounds.PlayNavigateSound();
		}

		/// <inheritdoc />
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (!TryGetUiSounds(out UserInterfaceSoundSchemeAsset uiSounds))
				return;
			
			uiSounds.PlayNavigateSound();
		}

		/// <inheritdoc />
		public void OnPointerClick(PointerEventData eventData)
		{
			if (!TryGetUiSounds(out UserInterfaceSoundSchemeAsset uiSounds))
				return;
			
			uiSounds.PlaySelectSound();
		}

		/// <inheritdoc />
		public void OnSubmit(BaseEventData eventData)
		{
			if (!TryGetUiSounds(out UserInterfaceSoundSchemeAsset uiSounds))
				return;
			
			uiSounds.PlaySelectSound();
		}

		private bool TryGetUiSounds(out UserInterfaceSoundSchemeAsset soundScheme)
		{
			soundScheme = AudioManager.GuiSounds!;
			return soundScheme != null;
		}

		/// <inheritdoc />
		public void OnCancel(BaseEventData eventData)
		{
			if (!TryGetUiSounds(out UserInterfaceSoundSchemeAsset uiSounds))
				return;
			
			uiSounds.PlayCancelSound();
		}
	}
}