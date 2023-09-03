using System;
using UnityEngine;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;

namespace Com.TheFallenGames.OSA.CustomAdapters.TableView.Tuple.Basic
{
	/// <summary>
	/// A views holder for a value inside a row from a database, which casts the value passed to it to a 
	/// string or Texture and binds it to a Text or RawImage component, respectively
	/// </summary>
	public class BasicTupleValueViewsHolder : TupleValueViewsHolder
	{
		RectTransform _ImagePanel;
		RawImage _Image;
		RectTransform _TextPanel;
		RectTransform _TogglePanel;
		Toggle _Toggle;
		bool _ForwardValueChanges = true;
		RectTransform _InputAvailableDot;
		//bool _IsCurrentlyReadonly;

		//LayoutElement _ImagePanelLE;
		//LayoutElement _TextPanelLE;
		//LayoutElement _TogglePanelLE;

		public override void CollectViews()
		{
			base.CollectViews();

			root.GetComponentAtPath("ImagePanel", out _ImagePanel);
			_ImagePanel.GetComponentAtPath("Image", out _Image);

			root.GetComponentAtPath("TextPanel", out _TextPanel);

			//root.GetComponentAtPath("TogglePanel", out _TogglePanel);
			//_TogglePanel.GetComponentAtPath("Toggle", out _Toggle);

			root.GetComponentAtPath("Toggle", out _TogglePanel);
			_Toggle = _TogglePanel.GetComponent<Toggle>();

			root.GetComponentAtPath("InputAvailableDot", out _InputAvailableDot);

			_Toggle.onValueChanged.AddListener(OnToggleValueChanged);


			//_ImagePanelLE = _ImagePanel.GetComponent<LayoutElement>();
			//if (!_ImagePanelLE)
			//	_ImagePanelLE = _ImagePanel.gameObject.AddComponent<LayoutElement>();
			//_ImagePanelLE.ignoreLayout = true;

			//_TextPanelLE = _TextPanel.GetComponent<LayoutElement>();
			//if (!_TextPanelLE)
			//	_TextPanelLE = _TextPanel.gameObject.AddComponent<LayoutElement>();
			//_TextPanelLE.ignoreLayout = true;

			//_TogglePanelLE = _TogglePanel.GetComponent<LayoutElement>();
			//if (!_TogglePanelLE)
			//	_TogglePanelLE = _TogglePanel.gameObject.AddComponent<LayoutElement>();
			//_TogglePanelLE.ignoreLayout = true;
		}

		public override void UpdateViews(object value, ITableColumns columnsProvider)
		{
			bool isNull = value == null;
			var column = columnsProvider.GetColumnState(ItemIndex);
			if (isNull)
			{
				UpdateAsNullText(column);
				return;
			}

			UpdateViews(value, column);
		}

		void UpdateAsNullText(IColumnState column)
		{
			UpdateAsText("<color=#88444499>NULL</color>", false);
			SetInputAvailable(!column.CurrentlyReadOnly && IsStringInputType(column.Info.ValueType));
		}

		void UpdateIntOrLong(string asStr, bool canChangeValue)
		{
			bool updatedSuccessfully = UpdateAsText(asStr, canChangeValue);
			SetInputAvailable(canChangeValue && updatedSuccessfully);
		}

		void UpdateFloatOrDouble(string asStr, bool canChangeValue)
		{
			bool updatedSuccessfully = UpdateAsText(asStr, canChangeValue);
			SetInputAvailable(canChangeValue && updatedSuccessfully);
		}

		/// <summary>
		/// Expecting value to be non-null
		/// </summary>
		void UpdateViews(object value, IColumnState column)
		{
			bool? textInputAvailable = null;
			try
			{
				bool canChangeValue = !column.CurrentlyReadOnly;
				bool updatedSuccessfully;
				switch (column.Info.ValueType)
				{
					case TableValueType.RAW:
						UpdateAsText("<color=#22552266>" + value.GetType().Name + "</color> " + value.ToString(), false);
						textInputAvailable = false;
						break;

					case TableValueType.STRING:
						updatedSuccessfully = UpdateAsText((string)value, canChangeValue);
						textInputAvailable = canChangeValue && updatedSuccessfully;
						break;

					case TableValueType.INT:
						UpdateIntOrLong(((int)value).ToString(), canChangeValue);
						break;

					case TableValueType.LONG_INT:
						UpdateIntOrLong(((long)value).ToString(), canChangeValue);
						break;

					case TableValueType.FLOAT:
						float fl = (float)value;
						UpdateFloatOrDouble(fl.ToString(OSAConst.FLOAT_TO_STRING_CONVERSION_SPECIFIER_PRESERVE_PRECISION), canChangeValue);
						break;

					case TableValueType.DOUBLE:
						double db = (double)value;
						//string text = val.ToString();
						// Spent like 2 hours to find out C# double doesn't always convert successfully to string by default without losing precision, smh.
						// https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings#RFormatString
						// Furthermore, they have a useless R specifier that is more annoying because it doesn't always work. Probably there for historical reasons.
						// The odd "G17" should be used for making sure a string that doesn't lose precision is output
						UpdateFloatOrDouble(db.ToString(OSAConst.DOUBLE_TO_STRING_CONVERSION_SPECIFIER_PRESERVE_PRECISION), canChangeValue);
						break;

					case TableValueType.ENUMERATION:
						string textToSet = null;
						if (column.Info.EnumValueType != null && column.Info.EnumValueType.IsEnum)
							try { textToSet = Enum.GetName(column.Info.EnumValueType, value); } catch { }
						bool validEnum = !string.IsNullOrEmpty(textToSet);
						if (!validEnum)
							textToSet = value.ToString();

						updatedSuccessfully = UpdateAsText(textToSet, false /*enum text is changed by other means, not direct text editing*/);
						textInputAvailable = false;
						break;

					case TableValueType.BOOL:
						updatedSuccessfully = UpdateAsCheckbox((bool)value, canChangeValue);
						break;

					case TableValueType.TEXTURE:
						UpdateAsImage((Texture)value);
						break;
				}

				if (textInputAvailable != null)
					SetInputAvailable(textInputAvailable.Value);
			}
			catch (Exception e)
			{
				Debug.LogError("Exception pre-details: " + column.Info.ValueType + ", value " + (value == null ? "NULL" : value));
				throw e;
			}
		}

		protected void UpdateAsImage(Texture texture)
		{
			if (ActivatePanelOnlyFor(_Image))
			{
				_Image.texture = texture;
			}
			SetInputAvailable(false);
		}

		protected bool UpdateAsText(string text, bool editable)
		{
			if (TextComponent)
				TextComponent.supportRichText = !editable;

			if (ActivatePanelOnlyFor(TextComponent))
			{
				// Don't forward changes that are done from the model
				_ForwardValueChanges = false;
				HasPendingTransversalSizeChanges = TextComponent.text != text;
				if (HasPendingTransversalSizeChanges)
				{
					TextComponent.text = text;
				}
				_ForwardValueChanges = true;
				return true;
			}

			return false;
		}

		protected bool UpdateAsCheckbox(bool value, bool editable)
		{
			if (_Toggle)
				_Toggle.interactable = editable;

			SetInputAvailable(false);

			if (ActivatePanelOnlyFor(_Toggle))
			{
				// Don't forward changes that are done from the model
				_ForwardValueChanges = false;
				_Toggle.isOn = value;
				_ForwardValueChanges = true;

				return true;
			}

			return false;
		}

		bool ActivatePanelOnlyFor(UnityEngine.MonoBehaviour uiElement)
		{
			bool result = false;
			if (_Image)
			{
				bool act = _Image == uiElement;
				_ImagePanel.gameObject.SetActive(act);

				result = result || act;
			}
			if (TextComponent)
			{
				bool act = TextComponent == uiElement;
				_TextPanel.gameObject.SetActive(act);

				result = result || act;
			}
			if (_Toggle)
			{
				bool act = _Toggle == uiElement;
				_TogglePanel.gameObject.SetActive(act);

				result = result || act;
			}

			return result;
		}

		void SetInputAvailable(bool available)
		{
			//_InputAvailableDot.localScale = available ? Vector3.one : Vector3.zero;
			_InputAvailableDot.gameObject.SetActive(available);
		}

		void OnToggleValueChanged(bool newValue)
		{
			if (_ForwardValueChanges && _TogglePanel.gameObject.activeSelf) // just a sanity check
				NotifyValueChangedFromInput(newValue);
		}

		bool IsStringInputType(TableValueType type)
		{
			return type == TableValueType.STRING
					|| type == TableValueType.INT
					|| type == TableValueType.LONG_INT
					|| type == TableValueType.FLOAT
					|| type == TableValueType.DOUBLE;
		}
	}
}
