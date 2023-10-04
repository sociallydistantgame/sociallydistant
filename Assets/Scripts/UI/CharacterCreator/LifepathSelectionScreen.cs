using System;
using System.Linq;
using System.Threading.Tasks;
using Architecture;
using UI.UiHelpers;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;
using UniRx;

namespace UI.CharacterCreator
{
	public class LifepathSelectionScreen : CharacterCreatorView
	{
		[SerializeField]
		private LifepathAsset[] lifepaths = Array.Empty<LifepathAsset>();

		[Header("UI")]
		[SerializeField]
		private LifepathListView lifepathListView = null!;

		private ToggleGroup toggleGroup;
		private CharacterCreatorState state;
		private DialogHelper dialogHelper;

		/// <inheritdoc />
		public override bool CanGoForward => state.Lifepath != null;

		/// <inheritdoc />
		protected override void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(LifepathSelectionScreen));
			this.lifepathListView.MustGetComponentInChildren(out toggleGroup);
			this.MustGetComponent(out dialogHelper);
			
			base.Awake();
		}

		/// <inheritdoc />
		protected override void Start()
		{
			lifepathListView.LifepathObservable.Subscribe(OnLifepathSelected);
			base.Start();
		}

		/// <inheritdoc />
		public override void SetData(CharacterCreatorState data)
		{
			this.state = data;
			lifepathListView.SetItems(this.lifepaths.ToList());
		}

		private void OnLifepathSelected(string lifepathId)
		{
			state.Lifepath = lifepaths.FirstOrDefault(x => x.Name == lifepathId);
		}

		/// <inheritdoc />
		public override Task<bool> ConfirmNextPage()
		{
			var source = new TaskCompletionSource<bool>();

			dialogHelper.AskQuestion(
				"Confirm Lifepath Choice",
				"Are you sure you want to continue with this Lifepath? You will not be able to change it later during this playthrough.",
				null,
				(res) => source.SetResult(res)
			);
            
			return source.Task;
		}
	}
}