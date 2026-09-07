using System;

namespace ZenithX;

public struct SliderInfo : CreateList
{
	private readonly string translationKey;

	public Func<float> getValue;

	public Action<float> setValue;

	public float minValue;

	public float maxValue;

	public string label => Localization.Translate(translationKey);

	public SliderInfo(string label, Func<float> getValue, Action<float> setValue = null, float minValue = 0f, float maxValue = 100f)
	{
		translationKey = label?.Trim() ?? "";
		this.getValue = getValue;
		this.setValue = setValue;
		this.minValue = minValue;
		this.maxValue = maxValue;
	}
}
