using System;

namespace ZenithX;

public struct KeybindInfo : CreateList
{
	private readonly string translationKey;

	public Func<string> getValue;

	public Action<string> setValue;

	public string label => Localization.Translate(translationKey);

	public KeybindInfo(string label, Func<string> getValue, Action<string> setValue = null)
	{
		translationKey = label?.Trim() ?? "";
		this.getValue = getValue;
		this.setValue = setValue;
	}
}
