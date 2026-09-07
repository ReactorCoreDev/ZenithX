using System;
using System.Collections.Generic;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;

namespace ZenithX;

public class ConsoleUI : MonoBehaviour
{
	private Vector2 scrollPosition = Vector2.zero;

	public static List<string> logEntries = new List<string>();

	public static Rect WindowRect = new Rect(320f, 10f, 500f, 300f);

	private GUIStyle logStyle;

	private static UILibrary.UIWindowData UI;

	private void Start()
	{
		if (UI == null)
		{
			UI = UILibrary.UI.CreateUI("ZenithX Console", draggable: true, "", 0, "Logs", "Live output");
		}
	}

	public void Log(string message)
	{
		if (logEntries.Count >= 100)
		{
			logEntries.RemoveAt(0);
		}
		logEntries.Add(message);
		scrollPosition.y = float.MaxValue;
	}

	private void OnGUI()
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Expected O, but got Unknown
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		if (!CheatToggles.showConsoleMenu)
		{
			return;
		}
		if (UI == null)
		{
			UI = UILibrary.UI.CreateUI("ZenithX Console", draggable: true, "", 0, "Logs", "Live output");
		}
		if (logStyle == null)
		{
			logStyle = new GUIStyle(GUI.skin.label)
			{
				fontSize = 20,
				richText = true
			};
		}
		GUI.backgroundColor = GUIStyles.GradientColor;
		if (Utils.TryParseHtmlString(ZenithX.menuHtmlColor.Value, out var uiColor))
		{
			GUI.backgroundColor = uiColor;
		}
		WindowRect = GUI.Window(1, WindowRect, WindowFunction.op_Implicit((Action<int>)delegate
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			GUILayout.BeginVertical((Il2CppReferenceArray<GUILayoutOption>)null);
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, Array.Empty<GUILayoutOption>());
			foreach (string logEntry in logEntries)
			{
				GUILayout.Label(logEntry, logStyle, (Il2CppReferenceArray<GUILayoutOption>)null);
			}
			if (GUILayout.Button("Clear Log", GUIStyles.buttonLabelStyle, Array.Empty<GUILayoutOption>()))
			{
				logEntries.Clear();
			}
			GUILayout.EndScrollView();
			GUILayout.EndVertical();
			GUI.DragWindow();
		}), UI.Title);
	}
}
