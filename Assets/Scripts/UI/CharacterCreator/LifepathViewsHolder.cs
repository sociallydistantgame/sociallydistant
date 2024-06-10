using Architecture;
using UI.Shell.InfoPanel;
using UnityExtensions;
using System;
using UnityEngine.UI;

namespace UI.CharacterCreator
{
	public class LifepathViewsHolder : AutoSizedItemsViewsHolder
	{
		private LifepathView lifepathView;
		private string lifepathId = string.Empty;
		private Toggle toggle;

		public Action<string>? Callback { get; set; }
		
		/// <inheritdoc />
		protected override bool Horizontal => true;

		/// <inheritdoc />
		public override void CollectViews()
		{
			root.MustGetComponentInChildren(out lifepathView);
			lifepathView.MustGetComponentInChildren(out toggle);
			
			toggle.onValueChanged.AddListener(OnToggleValueChanged);
			
			base.CollectViews();
		}

		public void SetData(LifepathAsset data)
		{
			lifepathId = data.Name;
			lifepathView.SetData(data);
		}

		private void OnToggleValueChanged(bool value)
		{
			if (value)
				Callback?.Invoke(lifepathId);
		}

		/// <inheritdoc />
		public LifepathViewsHolder(int itemIndex) : base(itemIndex)
		{ }
	}
}