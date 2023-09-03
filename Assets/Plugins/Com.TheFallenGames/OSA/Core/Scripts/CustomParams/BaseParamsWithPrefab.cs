using UnityEngine;
using Com.TheFallenGames.OSA.Core;
using UnityEngine.UI;
using System;
using UnityEngine.Serialization;

namespace Com.TheFallenGames.OSA.CustomParams
{
	/// <summary>
	/// Custom params containing a single prefab. <see cref="ItemPrefabSize"/> is calculated on first accessing and invalidated each time <see cref="InitIfNeeded(IOSA)"/> is called.
	/// <para>Note: If you change items sizes via <see cref="OSA{TParams, TItemViewsHolder}.CollectItemsSizes(ItemCountChangeMode, int, int, ItemsDescriptor)"/> or a <see cref="ContentSizeFitter"/>,
	/// Consider either setting <see cref="PrefabControlsDefaultItemSize"/> to false, or making sure the prefab's initial size is smaller than any of the actual items</para>
	/// </summary>
	[System.Serializable]
	public class BaseParamsWithPrefab : BaseParams
	{
		[FormerlySerializedAs("itemPrefab")]
		[SerializeField]
		RectTransform _ItemPrefab = null;
		[Obsolete("Use ItemPrefab instead", true)]
		public RectTransform itemPrefab { get { return ItemPrefab; } set { ItemPrefab = value; } }
		public RectTransform ItemPrefab { get { return _ItemPrefab; } set { _ItemPrefab = value; } }

		[Tooltip("Whether to set the BaseParam's DefaultItemSize to the size of the prefab.\n" +
				"Setting it to false, leaves DefaultItemSize unchanged.\n")]
		[SerializeField]
		bool _PrefabControlsDefaultItemSize = true;
		/// <summary>
		/// Whether to set the <see cref="BaseParams.DefaultItemSize"/> to the size of the prefab.
		/// <para>Setting it to false, leaves <see cref="BaseParams.DefaultItemSize"/> unchanged</para>
		/// </summary>
		public bool PrefabControlsDefaultItemSize { get { return _PrefabControlsDefaultItemSize; } set { _PrefabControlsDefaultItemSize = value; } }

		[Tooltip("Whether to set the BaseParam's ItemTransversalSize to the transversal size of the prefab, like it's done with DefaultItemSize.\n" +
			"Setting this to true naturally overrides any value you set to ItemTransversalSize.\n" +
			"Setting it to false, leaves ItemTransversalSize unchanged.\n")]
		[SerializeField]
		bool _AlsoControlItemTransversalSize = false;
		/// <summary>
		/// Whether to set the <see cref="BaseParams.ItemTransversalSize"/> to the transversal size of the prefab, like it's done with <see cref="BaseParams.DefaultItemSize"/>
		/// <para>Setting this to true naturally overrides any value you set to ItemTransversalSize</para>
		/// <para>Setting it to false, leaves <see cref="BaseParams.ItemTransversalSize"/> unchanged</para>
		/// </summary>
		public bool AlsoControlItemTransversalSize { get { return _AlsoControlItemTransversalSize; } set { _AlsoControlItemTransversalSize = value; } }

		public float ItemPrefabSize
		{
			get
			{
				if (!ItemPrefab)
					throw new OSAException(typeof(BaseParamsWithPrefab).Name + ": the prefab was not set. Please set it through inspector or in code");

				if (_ItemPrefabSize == -1f)
				{
					var rect = ItemPrefab.rect;
					if (IsHorizontal)
					{
						_ItemPrefabSize = rect.width;
						if (_AlsoControlItemTransversalSize)
						{
							ItemTransversalSize = rect.height;
							// Center it transversally
							ContentPadding.top = ContentPadding.bottom = -1;
						}
					}
					else
					{
						_ItemPrefabSize = ItemPrefab.rect.height;
						if (_AlsoControlItemTransversalSize)
						{
							ItemTransversalSize = rect.width;
							// Center it transversally
							ContentPadding.left = ContentPadding.right = -1;
						}
					}
				}

				return _ItemPrefabSize;
			}
		}

		float _ItemPrefabSize = -1f;


		/// <inheritdoc/>
		public override void InitIfNeeded(IOSA iAdapter)
		{
			base.InitIfNeeded(iAdapter);
			InitItemPrefab();
		}

		protected void InitItemPrefab()
		{
			if (ItemPrefab.parent == null)
			{
				// On Unity 2017.1 and up, the RectTransform.rect.width and height report the "correct" values (values the 
				// prefab had at the moment it was created in the assets section), but in older versions the reported 
				// width and height were 0 or negative.
				// For the older versions, we show a warning when the prefab is supposed to control the item size.
#if !UNITY_2017_1_OR_NEWER
				if (PrefabControlsDefaultItemSize)
				{
					string error = 
						typeof(BaseParamsWithPrefab).Name + ": PrefabControlsDefaultItemSize is true, but the prefab " +
						"is not in scene or has no parent, and thus it'll report an invalid size." +
						" Either bring your prefab to the scene (preferably, as a child of the Scroll View), or disable this property";

					// TODO promote this to an exception in latter versions
					//throw new OSAException(error);
					Debug.Log(error);
					return;
				}
#endif
			}
			else
			{
				if (ItemPrefab.parent != ScrollViewRT)
				{
					var asRT = ItemPrefab.parent as RectTransform;
					if (!asRT)
						throw new OSAException("ItemPrefab " + ItemPrefab.name + " should only be parented to an object that has a RectTransform");
					LayoutRebuilder.ForceRebuildLayoutImmediate(asRT);
				}
				else
					LayoutRebuilder.ForceRebuildLayoutImmediate(ItemPrefab);
			}

			if (PrefabControlsDefaultItemSize)
			{
				AssertValidWidthHeight(ItemPrefab);
				_ItemPrefabSize = -1f; // so the prefab's size will be recalculated next time is accessed
				DefaultItemSize = ItemPrefabSize;
			}
		}
	}
}