using System;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using UnityEngine;
using UnityEngine.UI;

namespace Com.TheFallenGames.OSA.CustomAdapters.TableView.Tuple
{
	[Serializable]
	public class TupleParams : BaseParamsWithPrefab
	{
		[SerializeField]
		RectTransform _EdgeDragger = null;
		public RectTransform EdgeDragger { get { return _EdgeDragger; } set { _EdgeDragger = value; } }

		[SerializeField]
		TableResizingMode _ResizingMode = TableResizingMode.NONE;
		public TableResizingMode ResizingMode { get { return _ResizingMode; } set { _ResizingMode = value; } }


		public override void InitIfNeeded(IOSA iAdapter)
		{
			if (ItemPrefab == null || !ItemPrefab.gameObject.activeInHierarchy)
				throw new OSAException(
					"A Tuple's value prefab should be non-null, and a scene object (i.e. not directly assigned from project view) active in hierarchy." +
					"This error may be caused by the Tuple prefab itself not being an object present in scene");

			base.InitIfNeeded(iAdapter);

			var tupleAdapter = iAdapter as ITupleAdapter;
			if (tupleAdapter == null)
				throw new OSAException(typeof(TupleParams).Name + ": No script implementing " + typeof(ITupleAdapter).Name + " found on TuplePrefab");
			bool prefabRebuildNeeded = InitResizingNeeded(tupleAdapter);
			if (prefabRebuildNeeded)
				InitItemPrefab();
			else
				AssertValidWidthHeight(ItemPrefab);

			if (ContentPadding.left == -1 || ContentPadding.right == -1 || ContentPadding.top == -1 || ContentPadding.bottom == -1)
				throw new OSAException(typeof(TupleParams).Name + ": A Tuple's ContentPadding isn't allowed to have negative components in any direction. Set them to zero or positive values");
			
			if (Navigation.Enabled)
			{
				Debug.Log(typeof(TupleParams).Name + ": Navigation.Enabled is true, but this is not yet supported with a TupleAdapter. Setting back to false...");
				Navigation.Enabled = false;
			}
		}


		// Returns if prefab needs to be rebuilt
		bool InitResizingNeeded(ITupleAdapter tupleAdapter)
		{
			var tupleParams = tupleAdapter.TupleParameters;
			var valuePrefab = tupleParams.ItemPrefab;

			var csf = valuePrefab.GetComponent<ContentSizeFitter>();
			var layoutGroup = valuePrefab.GetComponent<LayoutGroup>();
			if (ResizingMode == TableResizingMode.AUTO_FIT_TUPLE_CONTENT)
			{
				if (EdgeDragger && EdgeDragger.gameObject.activeSelf)
					EdgeDragger.gameObject.SetActive(false);

				if (tupleParams.ItemTransversalSize != -1f)
				{
					Debug.Log(
						typeof(TupleParams).Name + ": ItemTransversalSize needs to be -1, " +
						"because ResizingMode is " + ResizingMode + 
						". To avoid this message, manually set it in inspector"
					);
					tupleParams.ItemTransversalSize = -1f;
				}

				if (!csf)
				{
					//Debug.Log(
					//	typeof(TupleParams).Name + ": ResizingMode is " + TableResizingMode.AUTO_FIT_TUPLE_CONTENT + 
					//	", but no ContentSizeFitter found on tuple's value prefab. Adding one..."
					//);
					csf = valuePrefab.gameObject.AddComponent<ContentSizeFitter>();
				}
				var prefr = ContentSizeFitter.FitMode.PreferredSize;
				var unconstr = ContentSizeFitter.FitMode.Unconstrained;
				csf.horizontalFit = tupleAdapter.IsHorizontal ? unconstr : prefr;
				csf.verticalFit = tupleAdapter.IsHorizontal ? prefr : unconstr;

				// Update: will be manually enabled when needed;
				//csf.enabled = true;
				csf.enabled = false;

				//var valPrefabLayG = valuePrefab.GetComponent<LayoutGroup>();
				//HorizontalLayoutGroup valPrefabLayGHor;
				//if (valPrefabLayG)
				//{
				//	valPrefabLayGHor = valPrefabLayG as HorizontalLayoutGroup;
				//	if (!valPrefabLayGHor)
				//		throw new OSAException(
				//			typeof(TupleParams).Name + ": Only " + typeof(HorizontalLayoutGroup).Name + 
				//			" is allowed on value prefab ATM when using ResizingMode " + TableResizingMode.AUTO_FIT_TUPLE_CONTENT
				//		);
				//}
				//else
				//	valPrefabLayGHor = valuePrefab.gameObject.AddComponent<HorizontalLayoutGroup>();

				//if (IsHorizontal)
				//	_PrefabStandardPaddingTransvEnd = valPrefabLayGHor.padding.bottom;
				//else
				//	_PrefabStandardPaddingTransvEnd = valPrefabLayGHor.padding.right;

				//valPrefabLayG.childAlignment = TextAnchor.UpperLeft;
				//valPrefabLayGHor.childControlHeight = valPrefabLayGHor.childControlWidth = true;
				//valPrefabLayGHor.childForceExpandHeight = valPrefabLayGHor.childForceExpandWidth = false;

				//// Make sure the value prefab has the same size as the tuple
				//SetPaddingTransvEndToAchieveTansvSizeFor(valuePrefab, valPrefabLayG, ScrollViewRT.rect.size[IsHorizontal ? 1 : 0]);

				//return true;

			}
			else
			{
				if (EdgeDragger && !EdgeDragger.gameObject.activeSelf)
					EdgeDragger.gameObject.SetActive(true);

				if (tupleParams.ItemTransversalSize == -1f)
				{
					Debug.Log(
						typeof(TupleParams).Name + ": ItemTransversalSize set to -1 is not supported, " +
						"because ResizingMode is not " + TableResizingMode.AUTO_FIT_TUPLE_CONTENT + 
						". To avoid this message, manually set it to something else in inspector"
					);
					tupleParams.ItemTransversalSize = 0f;
				}

				if (csf && csf.enabled)
				{
					Debug.Log(typeof(TupleParams).Name + ": Found enabled ContentSizeFitter on tuple's value prefab, but ResizingMode is not "
						+ TableResizingMode.AUTO_FIT_TUPLE_CONTENT + ". Disabling ContentSizeFitter...");
					csf.enabled = false;
				}

				if (ResizingMode == TableResizingMode.NONE)
				{

				}
				else
				{

				}

			}

			// Layout components are disabled when no resizing is available. You 
			// should rely on anchoring to properly size the views in this case
			if (ResizingMode == TableResizingMode.NONE)
			{
				if (layoutGroup && layoutGroup.enabled)
				{
					layoutGroup.enabled = false;
					Debug.Log(typeof(TupleParams).Name + ": Found enabled LayoutGroup on tuple's value prefab, but ResizingMode is " + ResizingMode +
						". Disabling LayoutGroup...");
				}

				foreach (RectTransform rt in valuePrefab)
				{
					var l = rt.GetComponent<LayoutGroup>();
					if (l)
						l.enabled = false;

					var le = rt.GetComponent<LayoutElement>();
					if (le)
						le.enabled = false;
				}
			}
			else
			{
				if (layoutGroup && !layoutGroup.enabled)
					layoutGroup.enabled = true;

				foreach (RectTransform rt in valuePrefab)
				{
					var l = rt.GetComponent<LayoutGroup>();
					if (l)
						l.enabled = true;

					var le = rt.GetComponent<LayoutElement>();
					if (le)
						le.enabled = true;
				}
			}

			return false;
		}


		//public void SetPaddingTransvEndToAchieveTansvSizeFor(RectTransform rt, LayoutGroup layoutGroup, double targetTransvSize)
		//{
		//	double vhTransvSize = rt.rect.size[IsHorizontal ? 1 : 0];
		//	double vhTransvSizeMinusCurrentPaddingBottom = vhTransvSize - layoutGroup.padding.bottom;
		//	double padTransvEnd = targetTransvSize - vhTransvSizeMinusCurrentPaddingBottom;
		//	//if (padTransvEnd < 0d)
		//	//	throw new Exception(targetTransvSize + ", " + vhTransvSizeMinusCurrentPaddingBottom);
		//	if (padTransvEnd < _PrefabStandardPaddingTransvEnd)
		//		padTransvEnd = _PrefabStandardPaddingTransvEnd;

		//	SetPaddingTransvEndFor(rt, layoutGroup, padTransvEnd);
		//}

		//public void SetStandardTransvPaddingFor(RectTransform rt, LayoutGroup layoutGroup)
		//{
		//	SetPaddingTransvEndFor(rt, layoutGroup, _PrefabStandardPaddingTransvEnd);
		//}

		//void SetPaddingTransvEndFor(RectTransform rt, LayoutGroup layGroup, double paddingTransvEnd)
		//{
		//	int padEnd = (int)paddingTransvEnd;
		//	if (IsHorizontal)
		//		layGroup.padding.bottom = padEnd;
		//	else
		//		layGroup.padding.right = padEnd;
		//	LayoutRebuilder.MarkLayoutForRebuild(rt);
		//}
	}
}
