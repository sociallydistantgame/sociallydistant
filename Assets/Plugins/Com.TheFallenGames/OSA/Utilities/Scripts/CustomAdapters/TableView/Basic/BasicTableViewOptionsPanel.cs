using System;
using UnityEngine;
using UnityEngine.UI;

namespace Com.TheFallenGames.OSA.CustomAdapters.TableView.Basic
{
	public class BasicTableViewOptionsPanel : MonoBehaviour, ITableViewOptionsPanel
	{
		[SerializeField]
		Button _EnterClearingStateButton = null;

		[SerializeField]
		Button _ExitClearingStateButton = null;

		[SerializeField]
		GameObject _ClearingStateShownObject = null;

		[SerializeField]
		GameObject _NoneStateShownObject = null;

		[SerializeField]
		GameObject _LoadingGameObject = null;

		[SerializeField]
		CanvasGroup _CanvasGroupToDisableOnLoad = null;

		public bool IsClearing { get { return _IsClearing; } set { SetIsClearing(value); } }
		public bool IsLoading { get { return _IsLoading; } set { SetIsLoading(value); } }

		bool _IsClearing;
		bool _IsLoading;


		void Start()
		{
			if (_EnterClearingStateButton)
				_EnterClearingStateButton.onClick.AddListener(SetClearing);
			if (_ExitClearingStateButton)
				_ExitClearingStateButton.onClick.AddListener(SetNoClearing);

			IsClearing = false;
			IsLoading = false;
		}

		void Update()
		{
			if (_IsLoading)
			{
				if (_LoadingGameObject)
				{
					_LoadingGameObject.transform.Rotate(Vector3.forward, -270f * Time.deltaTime, Space.Self);
				}
			}
		}

		void SetNoClearing()
		{
			IsClearing = false;
		}

		void SetClearing()
		{
			IsClearing = true;
		}

		void SetIsClearing(bool isClearing)
		{
			if (_EnterClearingStateButton)
				_EnterClearingStateButton.gameObject.SetActive(!isClearing);
			if (_ExitClearingStateButton)
				_ExitClearingStateButton.gameObject.SetActive(isClearing);
			if (_ClearingStateShownObject)
				_ClearingStateShownObject.gameObject.SetActive(isClearing);
			if (_NoneStateShownObject)
				_NoneStateShownObject.gameObject.SetActive(!isClearing);

			_IsClearing = isClearing;
		}

		void SetIsLoading(bool isLoading)
		{
			_IsLoading = isLoading;
			if (_LoadingGameObject)
				_LoadingGameObject.SetActive(_IsLoading);

			if (_CanvasGroupToDisableOnLoad)
				_CanvasGroupToDisableOnLoad.blocksRaycasts = !_IsLoading;
		}
	}
}