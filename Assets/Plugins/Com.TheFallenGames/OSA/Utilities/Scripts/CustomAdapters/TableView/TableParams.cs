using System;
using UnityEngine;
using UnityEngine.UI;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomAdapters.TableView.Input;
using UnityEngine.Serialization;

namespace Com.TheFallenGames.OSA.CustomAdapters.TableView
{
	/// <summary>Base class for params to be used with a <see cref="GridAdapter{TParams, TCellVH}"/></summary>
	[Serializable] // serializable, so it can be shown in inspector
	public class TableParams : BaseParams
	{
		#region Configuration
		[SerializeField]
		TableConfig _Table = new TableConfig();
		public TableConfig Table { get { return _Table; } set { _Table = value; } }
		#endregion


		/// <inheritdoc/>
		public override void InitIfNeeded(IOSA iAdapter)
		{
			base.InitIfNeeded(iAdapter);

			if (optimization.ScaleToZeroInsteadOfDisable)
			{
				Debug.Log(typeof(TableParams).Name + ": optimization.ScaleToZeroInsteadOfDisable is true, but this is not supported with a TableView. Setting back to false...");
				optimization.ScaleToZeroInsteadOfDisable = false;
			}

			if (Navigation.Enabled)
			{
				Debug.Log(typeof(TableParams).Name + ": Navigation.Enabled is true, but this is not yet supported with a TableView. Setting back to false...");
				Navigation.Enabled = false;
			}

			Table.InitIfNeeded(iAdapter);
			DefaultItemSize = Table.TuplePrefabSize;
		}


		[Serializable]
		public class TableConfig
		{
			[SerializeField]
			RectTransform _TuplePrefab = null;
			/// <summary>The prefab to use for each tuple (aka row in database)</summary>
			public RectTransform TuplePrefab { get { return _TuplePrefab; } set { _TuplePrefab = value; } }

			[SerializeField]
			[Tooltip("The prefab to use for the columns header. Can be the same as TuplePrefab")]
			RectTransform _ColumnsTuplePrefab = null;
			/// <summary>The prefab to use for the columns header. Can be the same as <see cref="TuplePrefab"/></summary>
			public RectTransform ColumnsTuplePrefab { get { return _ColumnsTuplePrefab; } set { _ColumnsTuplePrefab = value; } }

			[SerializeField]
			[FormerlySerializedAs("_ColumnsHeaderSize")]
			[Tooltip("Size (height for vertical ScrollViews, width otherwise) of the header containing the columns. Leave to -1 to use the prefab's one")]
			float _ColumnsTupleSize = -1f;
			/// <summary>Size (height for vertical ScrollViews, width otherwise) of the header containing the columns. Leave to -1 to use the prefab's one</summary>
			public float ColumnsTupleSize { get { return _ColumnsTupleSize; } set { _ColumnsTupleSize = value; } }

			[SerializeField]
			[FormerlySerializedAs("_ColumnsHeaderSpacing")]
			[Tooltip("Additional space between the header and the actual content")]
			float _ColumnsTupleSpacing = 0f;
			/// <summary>Additional space between the header and the actual content</summary>
			public float ColumnsTupleSpacing { get { return _ColumnsTupleSpacing; } set { _ColumnsTupleSpacing = value; } }

			[SerializeField]
			Scrollbar _ColumnsScrollbar = null;
			public Scrollbar ColumnsScrollbar { get { return _ColumnsScrollbar; } set { _ColumnsScrollbar = value; } }

			[Tooltip("A GameObject having a component that implements ITableViewFloatingDropdown")]
			[SerializeField]
			RectTransform _FloatingDropdownPrefab = null;
			/// <summary>A GameObject having a component that implements <see cref="Input.ITableViewFloatingDropdown"/></summary>
			public RectTransform FloatingDropdownPrefab { get { return _FloatingDropdownPrefab; } set { _FloatingDropdownPrefab = value; } }

			[Tooltip("Used for text input")]
			[SerializeField]
			TableViewTextInputController _TextInputControllerPrefab = null;
			/// <summary>Used for text input. See <see cref="Input.TableViewTextInputController"/></summary>
			public TableViewTextInputController TextInputControllerPrefab { get { return _TextInputControllerPrefab; } set { _TextInputControllerPrefab = value; } }

			[Tooltip("A GameObject having a component that implements ITableViewOptionsPanel")]
			[SerializeField]
			RectTransform _OptionsPanel = null;
			/// <summary>A GameObject having a component that implements <see cref="ITableViewOptionsPanel"/></summary>
			public RectTransform OptionsPanel { get { return _OptionsPanel; } set { _OptionsPanel = value; } }

			public float TuplePrefabSize
			{
				get
				{
					if (!TuplePrefab)
						throw new OSAException(typeof(TableParams).Name + ": the TuplePrefab was not set. Please set it through inspector or in code");

					if (_TuplePrefabSize == -1f)
						_TuplePrefabSize = _IsHorizontal ? TuplePrefab.rect.width : TuplePrefab.rect.height;

					return _TuplePrefabSize;
				}
			}

			float _TuplePrefabSize = -1f;
			bool _IsHorizontal;


			public void InitIfNeeded(IOSA iAdapter)
			{
				string sceneObjectErrSuffix = " should be non-null, and a scene object that's active in hierarchy (i.e. not directly assigned from project view)";
				if (TuplePrefab == null || !TuplePrefab.gameObject.activeInHierarchy)
					throw new OSAException("TuplePrefab" + sceneObjectErrSuffix);
				if (ColumnsTuplePrefab == null || !ColumnsTuplePrefab.gameObject.activeInHierarchy)
					throw new OSAException("ColumnsTuplePrefab" + sceneObjectErrSuffix);

				_IsHorizontal = iAdapter.IsHorizontal;
				if (_ColumnsTupleSize == -1f)
					_ColumnsTupleSize = _TuplePrefab.rect.size[_IsHorizontal ? 0 : 1];

				var adapterParams = iAdapter.BaseParameters;
				if (TuplePrefab.parent != adapterParams.ScrollViewRT)
					LayoutRebuilder.ForceRebuildLayoutImmediate(TuplePrefab.parent as RectTransform);
				else
					LayoutRebuilder.ForceRebuildLayoutImmediate(TuplePrefab);

				adapterParams.AssertValidWidthHeight(TuplePrefab);

				_TuplePrefabSize = -1f; // so the prefab's size will be recalculated next time is accessed
			}
		}
	}
}