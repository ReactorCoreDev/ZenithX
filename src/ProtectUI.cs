using System;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace ZenithX;

public class ProtectUI : MonoBehaviour
{
	private Vector2 _scrollPosition = Vector2.zero;

	public static Rect WindowRect = new Rect(320f, 10f, 500f, 300f);

	public static List<PlayerControl> playersToProtect = new List<PlayerControl>();

	private bool _keepEveryoneProtected;

	private static UILibrary.UIWindowData _windowData;

	private void OnGUI()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		if (CheatToggles.showProtectMenu)
		{
			if (_windowData == null)
			{
				_windowData = UILibrary.UI.CreateUI("Protect Players", draggable: true, "", 0, "Guardian status", "Keep protected");
			}
			GUI.backgroundColor = GUIStyles.GradientColor;
			if (Utils.TryParseHtmlString(ZenithX.menuHtmlColor.Value, out var uiColor))
			{
				GUI.backgroundColor = uiColor;
			}
			WindowRect = GUI.Window(5, WindowRect, WindowFunction.op_Implicit((Action<int>)ProtectWindow), "Protect Players");
		}
	}

	private void ProtectWindow(int windowID)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		GUILayout.BeginVertical((Il2CppReferenceArray<GUILayoutOption>)null);
		_scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true, Array.Empty<GUILayoutOption>());
		Enumerator<PlayerControl> enumerator = playersToProtect.GetEnumerator();
		while (enumerator.MoveNext())
		{
			PlayerControl current = enumerator.Current;
			if (!Object.op_Implicit((Object)(object)current.Data) || !Object.op_Implicit((Object)(object)current.Data.Role) || string.IsNullOrEmpty(current.Data.PlayerName))
			{
				playersToProtect.Remove(current);
				break;
			}
		}
		enumerator = PlayerControl.AllPlayerControls.GetEnumerator();
		while (enumerator.MoveNext())
		{
			PlayerControl current2 = enumerator.Current;
			if (!Object.op_Implicit((Object)(object)current2.Data) || !Object.op_Implicit((Object)(object)current2.Data.Role) || string.IsNullOrEmpty(current2.Data.PlayerName))
			{
				if (playersToProtect.Contains(current2))
				{
					playersToProtect.Remove(current2);
				}
				continue;
			}
			GUILayout.BeginHorizontal((Il2CppReferenceArray<GUILayoutOption>)null);
			GUILayout.Label($"<color=#{ColorUtility.ToHtmlStringRGB(current2.Data.Color)}>{current2.Data.PlayerName}</color>", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(140f) });
			GUILayout.Label(((current2.protectedByGuardianId != -1) ? $"<color=#00FF00>Protected</color> by <color=#{ColorUtility.ToHtmlStringRGB(GameData.Instance.GetPlayerById((byte)current2.protectedByGuardianId).Color)}>{GameData.Instance.GetPlayerById((byte)current2.protectedByGuardianId)._object.Data.PlayerName}</color>" : "<color=#FF0000>Unprotected</color>") ?? "", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(135f) });
			if (GUILayout.Button("Protect", (Il2CppReferenceArray<GUILayoutOption>)null) && Utils.isHost && !Utils.isLobby)
			{
				PlayerControl.LocalPlayer.RpcProtectPlayer(current2, current2.cosmetics.ColorId);
			}
			bool flag = playersToProtect.Contains(current2);
			flag = GUILayout.Toggle(flag, "Keep protected", (Il2CppReferenceArray<GUILayoutOption>)null);
			if (flag && !playersToProtect.Contains(current2))
			{
				playersToProtect.Add(current2);
			}
			else if (!flag && playersToProtect.Contains(current2))
			{
				playersToProtect.Remove(current2);
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.EndScrollView();
		GUILayout.BeginHorizontal((Il2CppReferenceArray<GUILayoutOption>)null);
		if (GUILayout.Button("Protect Everyone", (Il2CppReferenceArray<GUILayoutOption>)null) && Utils.isHost && !Utils.isLobby)
		{
			enumerator = PlayerControl.AllPlayerControls.GetEnumerator();
			while (enumerator.MoveNext())
			{
				PlayerControl current3 = enumerator.Current;
				PlayerControl.LocalPlayer.RpcProtectPlayer(current3, current3.cosmetics.ColorId);
			}
		}
		GUILayout.FlexibleSpace();
		_keepEveryoneProtected = GUILayout.Toggle(_keepEveryoneProtected, "Keep Everyone Protected", (Il2CppReferenceArray<GUILayoutOption>)null);
		if (_keepEveryoneProtected)
		{
			enumerator = PlayerControl.AllPlayerControls.GetEnumerator();
			while (enumerator.MoveNext())
			{
				PlayerControl current4 = enumerator.Current;
				if (!playersToProtect.Contains(current4))
				{
					playersToProtect.Add(current4);
				}
			}
		}
		else if (PlayerControl.AllPlayerControls.Count == playersToProtect.Count)
		{
			playersToProtect.Clear();
		}
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		GUI.DragWindow();
	}
}
