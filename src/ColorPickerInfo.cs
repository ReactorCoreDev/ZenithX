using System;
using UnityEngine;

namespace ZenithX;

public struct ColorPickerInfo : CreateList
{
	private readonly string translationKey;

	public Func<Color> getValue;

	public Action<Color> setValue;

	public string label => Localization.Translate(translationKey);

	public ColorPickerInfo(string label, Func<Color> getValue, Action<Color> setValue = null)
	{
		translationKey = label?.Trim() ?? "";
		this.getValue = getValue;
		this.setValue = setValue;
	}
}
