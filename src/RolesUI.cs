using System;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace ZenithX;

public class RolesUI : MonoBehaviour
{
	private Vector2 _scrollPosition = Vector2.zero;

	public static Rect WindowRect = new Rect(320f, 10f, 450f, 100f);

	private static UILibrary.UIWindowData UI;

	private void OnGUI()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		if (!CheatToggles.showRolesMenu)
		{
			return;
		}
		if (UI == null)
		{
			UI = UILibrary.UI.CreateUI("Assign Roles", draggable: true, "", 0, "Role assignment", "Next game start");
		}
		GUI.backgroundColor = GUIStyles.GradientColor;
		if (Utils.TryParseHtmlString(ZenithX.menuHtmlColor.Value, out var uiColor))
		{
			GUI.backgroundColor = uiColor;
		}
		WindowRect = GUI.Window(4, WindowRect, WindowFunction.op_Implicit((Action<int>)delegate
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			GUILayout.BeginVertical((Il2CppReferenceArray<GUILayoutOption>)null);
			_scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true, Array.Empty<GUILayoutOption>());
			Enumerator<PlayerControl> enumerator = PlayerControl.AllPlayerControls.GetEnumerator();
			while (enumerator.MoveNext())
			{
				PlayerControl current = enumerator.Current;
				if (Object.op_Implicit((Object)(object)current.Data) && Object.op_Implicit((Object)(object)current.Data.Role) && !string.IsNullOrEmpty(current.Data.PlayerName) && !((Object)(object)current != (Object)(object)PlayerControl.LocalPlayer))
				{
					GUILayout.BeginHorizontal((Il2CppReferenceArray<GUILayoutOption>)null);
					GUILayout.Label($"<color=#{ColorUtility.ToHtmlStringRGB(current.Data.Color)}>{current.Data.PlayerName}</color>", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(140f) });
					GUILayout.BeginHorizontal((Il2CppReferenceArray<GUILayoutOption>)null);
					GUILayout.Label($"{CheatToggles.forcedRole}", (Il2CppReferenceArray<GUILayoutOption>)null);
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("Reset", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(80f) }))
					{
						CheatToggles.forcedRole = null;
					}
					if (GUILayout.Button("Assign", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(80f) }))
					{
						CheatToggles.forceRole = true;
					}
					GUILayout.EndHorizontal();
					GUILayout.EndHorizontal();
				}
			}
			GUILayout.EndScrollView();
			GUILayout.EndVertical();
			GUILayout.Label("Roles will be assigned on next game start.", (Il2CppReferenceArray<GUILayoutOption>)null);
			GUI.DragWindow();
		}), UI.Title);
	}
}
