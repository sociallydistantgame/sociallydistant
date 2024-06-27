#nullable enable
using System.Collections.Generic;
using UnityEngine;

namespace UI.Widgets
{
	public interface IGraphicPickerSource
	{
		IEnumerable<string> GetGraphicNames();

		Texture2D? GetGraphic(string graphicName);
		void SetGraphic(string name, Texture2D? texture);
	}
}