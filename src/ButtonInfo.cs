using System;

namespace ZenithX;

public class ButtonInfo : CreateList
{
	private readonly string translationKey;

	public Action action;

	public string label => Localization.Translate(translationKey);

	public ButtonInfo(string label, Action action = null)
	{
		translationKey = label?.Trim() ?? "";
		this.action = action;
	}
}
