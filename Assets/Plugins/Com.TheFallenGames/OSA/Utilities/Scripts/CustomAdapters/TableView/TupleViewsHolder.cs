using System;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomAdapters.TableView.Tuple;
using frame8.Logic.Misc.Other.Extensions;
using UnityEngine.UI;

#if OSA_TV_TMPRO
using TText = TMPro.TextMeshProUGUI;
#else
using TText = UnityEngine.UI.Text;
#endif

namespace Com.TheFallenGames.OSA.CustomAdapters.TableView
{
	public class TupleViewsHolder : BaseItemViewsHolder
	{
		public ITupleAdapter Adapter { get; private set; }

		protected TText _IndexText;


		public override void CollectViews()
		{
			base.CollectViews();

			Adapter = root.GetComponent(typeof(ITupleAdapter)) as ITupleAdapter;
			root.GetComponentAtPath("IndexText", out _IndexText);
		}

		public virtual void UpdateViews(ITuple tuple, ITableColumns columns)
		{
			if (_IndexText)
				_IndexText.text = ItemIndex.ToString();

			Adapter.ResetWithTuple(tuple, columns);
		}
	}
}
