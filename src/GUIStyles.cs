using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;

namespace ZenithX;

public static class GUIStyles
{
	private static Dictionary<string, float> buttonClickTimes = new Dictionary<string, float>();

	public static Texture2D whiteTex = MakeSolidTexture(2, 2, Color.white);

	public static readonly Color gradientTop = new Color(0.678f, 0.847f, 0.902f);

	public static readonly Color gradientBottom = Color.blue;

	[CompilerGenerated]
	private static GUIStyle _003CbuttonLabelStyle_003Ek__BackingField;

	[CompilerGenerated]
	private static GUIStyle _003CsubmenuLabelStyle_003Ek__BackingField;

	[CompilerGenerated]
	private static GUIStyle _003CtoggleLabelStyle_003Ek__BackingField;

	public static Color GradientColor => Color.Lerp(gradientTop, gradientBottom, 0.5f);

	public static GUIStyle buttonLabelStyle
	{
		get
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Expected O, but got Unknown
			if (_003CbuttonLabelStyle_003Ek__BackingField == null)
			{
				GUIStyle val = new GUIStyle();
				val.normal.textColor = Color.white;
				val.fontSize = 17;
				val.alignment = (TextAnchor)4;
				_003CbuttonLabelStyle_003Ek__BackingField = val;
			}
			return _003CbuttonLabelStyle_003Ek__BackingField;
		}
	}

	public static GUIStyle submenuLabelStyle
	{
		get
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected O, but got Unknown
			if (_003CsubmenuLabelStyle_003Ek__BackingField == null)
			{
				_003CsubmenuLabelStyle_003Ek__BackingField = new GUIStyle(buttonLabelStyle)
				{
					fontSize = 15
				};
			}
			return _003CsubmenuLabelStyle_003Ek__BackingField;
		}
	}

	public static GUIStyle toggleLabelStyle
	{
		get
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Expected O, but got Unknown
			if (_003CtoggleLabelStyle_003Ek__BackingField == null)
			{
				GUIStyle val = new GUIStyle();
				val.normal.textColor = Color.white;
				val.fontSize = 17;
				val.alignment = (TextAnchor)3;
				_003CtoggleLabelStyle_003Ek__BackingField = val;
			}
			return _003CtoggleLabelStyle_003Ek__BackingField;
		}
	}

	public static Texture2D MakeSolidTexture(int width, int height, Color color)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Expected O, but got Unknown
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		Texture2D val = new Texture2D(width, height);
		Color[] array = (Color[])(object)new Color[width * height];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = color;
		}
		val.SetPixels(Il2CppStructArray<Color>.op_Implicit(array));
		val.Apply();
		return val;
	}

	public static Color GetGradientColor(float t)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return Color.Lerp(gradientTop, gradientBottom, Mathf.Clamp01(t));
	}

	public static bool ManualButton(Rect rect, string label, GUIStyle lblStyle, Action onClick = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Expected O, but got Unknown
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		Color val = gradientTop;
		Color val2 = default(Color);
		((Color)(ref val2))._002Ector(0.85f, 0.92f, 1f, 1f);
		Color val3 = default(Color);
		((Color)(ref val3))._002Ector(0.72f, 0.86f, 0.98f, 1f);
		Event current = Event.current;
		bool flag = ((Rect)(ref rect)).Contains(current.mousePosition);
		bool flag2 = false;
		bool num = flag2;
		flag2 = (int)current.type == 0 && current.button == 0 && flag;
		bool num2 = !num && flag2;
		if (num2)
		{
			current.Use();
			buttonClickTimes[label] = Time.time;
			onClick?.Invoke();
		}
		bool flag3 = false;
		if (buttonClickTimes.ContainsKey(label))
		{
			float num3 = Time.time - buttonClickTimes[label];
			flag3 = num3 < 0.15f;
			if (num3 >= 0.15f)
			{
				buttonClickTimes.Remove(label);
			}
		}
		Color backgroundColor = (flag2 ? val3 : (flag ? val2 : val));
		Color backgroundColor2 = GUI.backgroundColor;
		Color color = GUI.color;
		if (flag3)
		{
			DrawLightBlueOutline(rect);
		}
		GUI.backgroundColor = backgroundColor;
		GUI.Box(rect, "");
		GetContrastingTextColor(backgroundColor);
		GUIStyle val4 = new GUIStyle(lblStyle);
		val4.normal.textColor = Color.white;
		Rect val5 = rect;
		((Rect)(ref val5)).x = ((Rect)(ref val5)).x + 15f;
		GUI.Label(val5, label, val4);
		GUI.backgroundColor = backgroundColor2;
		GUI.color = color;
		return num2;
	}

	public static bool ManualToggle(Rect rect, bool currentState, string label, GUIStyle lblStyle, Action<bool> onValueChanged = null)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Expected O, but got Unknown
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Expected O, but got Unknown
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		Color val = (Color)(currentState ? Color.Lerp(gradientTop, gradientBottom, 0.2f) : new Color(0.1f, 0.1f, 0.1f, 0.8f));
		Color val2 = Color.Lerp(val, Color.white, 0.2f);
		Color val3 = Color.Lerp(val, gradientBottom, 0.3f);
		Event current = Event.current;
		bool flag = ((Rect)(ref rect)).Contains(current.mousePosition);
		bool flag2 = false;
		if (flag && (int)current.type == 0 && current.button == 0)
		{
			flag2 = true;
			current.Use();
			bool obj = !currentState;
			onValueChanged?.Invoke(obj);
		}
		Color backgroundColor = (flag2 ? val3 : (flag ? val2 : val));
		Color backgroundColor2 = GUI.backgroundColor;
		Color color = GUI.color;
		GUI.backgroundColor = backgroundColor;
		GUI.Box(rect, "");
		GUIStyle val4 = new GUIStyle();
		val4.fontSize = 18;
		val4.fontStyle = (FontStyle)1;
		val4.alignment = (TextAnchor)3;
		Rect val5 = default(Rect);
		((Rect)(ref val5))._002Ector(((Rect)(ref rect)).x + 5f, ((Rect)(ref rect)).y, 30f, ((Rect)(ref rect)).height);
		if (currentState)
		{
			val4.normal.textColor = Color.green;
			GUI.Label(val5, "✓", val4);
		}
		else
		{
			val4.normal.textColor = Color.red;
			GUI.Label(val5, "✗", val4);
		}
		GetContrastingTextColor(backgroundColor);
		GUIStyle val6 = new GUIStyle(lblStyle);
		val6.normal.textColor = Color.white;
		Rect val7 = rect;
		((Rect)(ref val7)).x = ((Rect)(ref val7)).x + 35f;
		GUI.Label(val7, label, val6);
		GUI.backgroundColor = backgroundColor2;
		GUI.color = color;
		if (!flag2)
		{
			return currentState;
		}
		return !currentState;
	}

	public static void DrawLightBlueOutline(Rect rect, float thickness = 2f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		Color color = GUI.color;
		Color backgroundColor = GUI.backgroundColor;
		Color backgroundColor2 = gradientTop;
		Rect val = new Rect(((Rect)(ref rect)).x - thickness, ((Rect)(ref rect)).y - thickness, ((Rect)(ref rect)).width + thickness * 2f, ((Rect)(ref rect)).height + thickness * 2f);
		GUI.backgroundColor = Color.black;
		GUI.Box(val, "");
		GUI.backgroundColor = backgroundColor2;
		GUI.Box(rect, "");
		GUI.backgroundColor = backgroundColor;
		GUI.color = color;
	}

	public static Color GetContrastingTextColor(Color backgroundColor)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return Color.white;
	}
}
