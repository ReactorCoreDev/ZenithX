using System.Collections.Generic;

namespace ZenithX;

public struct GroupInfo
{
	private readonly string translationKey;

	public string icon;

	public bool isExpanded;

	public List<CreateList> items;

	public List<ToggleInfo> toggles;

	public List<ButtonInfo> buttons;

	public List<SubmenuInfo> submenus;

	public List<SliderInfo> sliders;

	public List<InputInfo> inputs;

	public List<KeybindInfo> keybinds;

	public List<ColorPickerInfo> colorPickers;

	public string name => Localization.Translate(translationKey);

	public GroupInfo(string name, string icon = null, bool isExpanded = false, List<CreateList> items = null, List<SubmenuInfo> submenus = null)
	{
		translationKey = name?.Trim() ?? "";
		this.icon = icon;
		this.isExpanded = isExpanded;
		this.items = items;
		this.submenus = submenus;
		toggles = new List<ToggleInfo>();
		buttons = new List<ButtonInfo>();
		sliders = new List<SliderInfo>();
		inputs = new List<InputInfo>();
		keybinds = new List<KeybindInfo>();
		colorPickers = new List<ColorPickerInfo>();
		if (items == null)
		{
			return;
		}
		foreach (CreateList item7 in items)
		{
			if (item7 is ToggleInfo item)
			{
				toggles.Add(item);
			}
			else if (item7 is ButtonInfo item2)
			{
				buttons.Add(item2);
			}
			else if (item7 is SliderInfo item3)
			{
				sliders.Add(item3);
			}
			else if (item7 is InputInfo item4)
			{
				inputs.Add(item4);
			}
			else if (item7 is KeybindInfo item5)
			{
				keybinds.Add(item5);
			}
			else if (item7 is ColorPickerInfo item6)
			{
				colorPickers.Add(item6);
			}
		}
	}
}
