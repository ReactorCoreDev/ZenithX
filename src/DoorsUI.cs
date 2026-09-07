using System;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace ZenithX;

public class DoorsUI : MonoBehaviour
{
	public static Rect WindowRect = new Rect(320f, 10f, 530f, 280f);

	private GUIStyle _separatorStyle;

	private static UILibrary.UIWindowData UI;

	private List<SystemTypes> doorsToSpamOpen = new List<SystemTypes>();

	private List<SystemTypes> doorsToSpamClose = new List<SystemTypes>();

	private static readonly Color gradientTop = new Color(0.678f, 0.847f, 0.902f);

	private static readonly Color gradientBottom = Color.blue;

	public static Color GradientColor => Color.Lerp(gradientTop, gradientBottom, 0.5f);

	public static Color GetGradientColor(float t)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return Color.Lerp(gradientTop, gradientBottom, Mathf.Clamp01(t));
	}

	private unsafe void OnGUI()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Expected O, but got Unknown
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Expected O, but got Unknown
		//IL_0091: Expected O, but got Unknown
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		if (!CheatToggles.showDoorsMenu)
		{
			return;
		}
		if (UI == null)
		{
			UI = UILibrary.UI.CreateUI("Doors", draggable: true, "", 0, "Room control", "Door automation");
		}
		if (_separatorStyle == null)
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
			_separatorStyle = val;
		}
		if (Utils.TryParseHtmlString(ZenithX.menuHtmlColor.Value, out var uiColor))
		{
			GUI.backgroundColor = uiColor;
		}
		GUI.backgroundColor = GradientColor;
		WindowRect = GUI.Window(2, WindowRect, WindowFunction.op_Implicit((Action<int>)delegate
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Invalid comparison between Unknown and I4
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Invalid comparison between Unknown and I4
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Invalid comparison between Unknown and I4
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Invalid comparison between Unknown and I4
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Invalid comparison between Unknown and I4
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Invalid comparison between Unknown and I4
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Invalid comparison between Unknown and I4
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Invalid comparison between Unknown and I4
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Invalid comparison between Unknown and I4
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			if (!Utils.isShip)
			{
				GUI.DragWindow();
			}
			else
			{
				MapNames val2 = (MapNames)Utils.getCurrentMapID();
				if ((int)val2 == 1)
				{
					GUI.DragWindow();
				}
				else
				{
					GUILayout.BeginVertical((Il2CppReferenceArray<GUILayoutOption>)null);
					bool flag;
					foreach (SystemTypes doorRoom in DoorsHandler.GetDoorRooms())
					{
						GUILayout.BeginHorizontal((Il2CppReferenceArray<GUILayoutOption>)null);
						GUILayout.Label(((object)(*(SystemTypes*)(&doorRoom))/*cast due to .constrained prefix*/).ToString() ?? "", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(120f) });
						GUILayout.BeginHorizontal((Il2CppReferenceArray<GUILayoutOption>)null);
						GUILayout.Label("Status: " + DoorsHandler.GetStatusOfDoorsInRoom(doorRoom, colorize: true), (Il2CppReferenceArray<GUILayoutOption>)null);
						GUILayout.FlexibleSpace();
						if (GUILayout.Button("Close", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(50f) }))
						{
							DoorsHandler.CloseDoorsOfRoom(doorRoom);
						}
						flag = (((int)val2 == 2 || val2 - 4 <= 1) ? true : false);
						if (flag && GUILayout.Button("Open", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(50f) }))
						{
							DoorsHandler.OpenDoorsOfRoom(doorRoom);
						}
						if (Utils.isHost)
						{
							bool flag2 = doorsToSpamClose.Contains(doorRoom);
							flag2 = GUILayout.Toggle(flag2, "Spam Close", (Il2CppReferenceArray<GUILayoutOption>)null);
							if (flag2 && !doorsToSpamClose.Contains(doorRoom))
							{
								doorsToSpamClose.Add(doorRoom);
							}
							else if (!flag2 && doorsToSpamClose.Contains(doorRoom))
							{
								doorsToSpamClose.Remove(doorRoom);
							}
							if (((int)val2 == 2 || val2 - 4 <= 1) ? true : false)
							{
								bool flag3 = doorsToSpamOpen.Contains(doorRoom);
								flag3 = GUILayout.Toggle(flag3, "Spam Open", (Il2CppReferenceArray<GUILayoutOption>)null);
								if (flag3 && !doorsToSpamOpen.Contains(doorRoom))
								{
									doorsToSpamOpen.Add(doorRoom);
								}
								else if (!flag3 && doorsToSpamOpen.Contains(doorRoom))
								{
									doorsToSpamOpen.Remove(doorRoom);
								}
							}
						}
						else if (doorsToSpamClose.Count != 0 || doorsToSpamOpen.Count != 0)
						{
							doorsToSpamClose.Clear();
							doorsToSpamOpen.Clear();
						}
						GUILayout.EndHorizontal();
						GUILayout.EndHorizontal();
					}
					GUILayout.FlexibleSpace();
					GUILayout.Box("", _separatorStyle, (GUILayoutOption[])(object)new GUILayoutOption[2]
					{
						GUILayout.Height(1f),
						GUILayout.ExpandWidth(true)
					});
					GUILayout.Box("", GUIStyle.none, (GUILayoutOption[])(object)new GUILayoutOption[2]
					{
						GUILayout.Height(1f),
						GUILayout.ExpandWidth(true)
					});
					GUILayout.BeginHorizontal((Il2CppReferenceArray<GUILayoutOption>)null);
					if (GUILayout.Button("Close All", (Il2CppReferenceArray<GUILayoutOption>)null))
					{
						CheatToggles.closeAllDoors = true;
					}
					flag = (((int)val2 == 2 || val2 - 4 <= 1) ? true : false);
					if (flag && GUILayout.Button("Open All", (Il2CppReferenceArray<GUILayoutOption>)null))
					{
						CheatToggles.openAllDoors = true;
					}
					GUILayout.FlexibleSpace();
					if (Utils.isHost)
					{
						CheatToggles.spamCloseAllDoors = GUILayout.Toggle(CheatToggles.spamCloseAllDoors, "Spam Close All", (Il2CppReferenceArray<GUILayoutOption>)null);
						if (((int)val2 == 2 || val2 - 4 <= 1) ? true : false)
						{
							CheatToggles.spamOpenAllDoors = GUILayout.Toggle(CheatToggles.spamOpenAllDoors, "Spam Open All", (Il2CppReferenceArray<GUILayoutOption>)null);
						}
					}
					else
					{
						CheatToggles.spamCloseAllDoors = (CheatToggles.spamOpenAllDoors = false);
					}
					GUILayout.EndHorizontal();
					GUILayout.EndVertical();
					GUI.DragWindow();
				}
			}
		}), UI.Title);
	}

	public void Update()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Invalid comparison between Unknown and I4
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Invalid comparison between Unknown and I4
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (!Utils.isShip)
		{
			return;
		}
		Enumerator<SystemTypes> enumerator = doorsToSpamClose.GetEnumerator();
		while (enumerator.MoveNext())
		{
			DoorsHandler.CloseDoorsOfRoom(enumerator.Current);
		}
		MapNames val = (MapNames)Utils.getCurrentMapID();
		if (((int)val == 2 || val - 4 <= 1) ? true : false)
		{
			enumerator = doorsToSpamOpen.GetEnumerator();
			while (enumerator.MoveNext())
			{
				DoorsHandler.OpenDoorsOfRoom(enumerator.Current);
			}
		}
	}
}
