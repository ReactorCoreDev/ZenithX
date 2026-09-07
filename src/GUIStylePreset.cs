using UnityEngine;

namespace ZenithX;

public static class GUIStylePreset
{
	private static GUIStyle _separator;

	private static GUIStyle _darkSeparator;

	private static GUIStyle _normalButton;

	private static GUIStyle _normalToggle;

	private static GUIStyle _tabButton;

	private static GUIStyle _tabTitle;

	private static GUIStyle _tabSubtitle;

	public static GUIStyle Separator
	{
		get
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Expected O, but got Unknown
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Expected O, but got Unknown
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Expected O, but got Unknown
			//IL_005a: Expected O, but got Unknown
			if (_separator == null)
			{
				GUIStyle val = new GUIStyle(GUI.skin.box);
				val.normal.background = Texture2D.whiteTexture;
				val.margin = new RectOffset
				{
					top = 4,
					bottom = 4
				};
				val.padding = new RectOffset();
				val.border = new RectOffset();
				_separator = val;
			}
			return _separator;
		}
	}

	public static GUIStyle DarkSeparator
	{
		get
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Expected O, but got Unknown
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Expected O, but got Unknown
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Expected O, but got Unknown
			//IL_005a: Expected O, but got Unknown
			if (_darkSeparator == null)
			{
				GUIStyle val = new GUIStyle(GUI.skin.box);
				val.normal.background = Texture2D.grayTexture;
				val.margin = new RectOffset
				{
					top = 4,
					bottom = 4
				};
				val.padding = new RectOffset();
				val.border = new RectOffset();
				_darkSeparator = val;
			}
			return _darkSeparator;
		}
	}

	public static GUIStyle NormalButton
	{
		get
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Expected O, but got Unknown
			if (_normalButton == null)
			{
				_normalButton = new GUIStyle(GUI.skin.button)
				{
					fontSize = 13
				};
			}
			return _normalButton;
		}
	}

	public static GUIStyle NormalToggle
	{
		get
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Expected O, but got Unknown
			if (_normalToggle == null)
			{
				_normalToggle = new GUIStyle(GUI.skin.toggle)
				{
					fontSize = 13
				};
			}
			return _normalToggle;
		}
	}

	public static GUIStyle TabButton
	{
		get
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Expected O, but got Unknown
			if (_tabButton == null)
			{
				_tabButton = new GUIStyle(GUI.skin.button)
				{
					fontSize = 17,
					fontStyle = (FontStyle)1
				};
			}
			return _tabButton;
		}
	}

	public static GUIStyle TabTitle
	{
		get
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Expected O, but got Unknown
			if (_tabTitle == null)
			{
				_tabTitle = new GUIStyle(GUI.skin.label)
				{
					fontSize = 20,
					fontStyle = (FontStyle)1,
					alignment = (TextAnchor)3
				};
			}
			return _tabTitle;
		}
	}

	public static GUIStyle TabSubtitle
	{
		get
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Expected O, but got Unknown
			if (_tabSubtitle == null)
			{
				_tabSubtitle = new GUIStyle(GUI.skin.label)
				{
					fontSize = 16,
					fontStyle = (FontStyle)1,
					alignment = (TextAnchor)3
				};
			}
			return _tabSubtitle;
		}
	}
}
