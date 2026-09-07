using System;
using System.Collections.Generic;
using System.Linq;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using InnerNet;
using UnityEngine;

namespace ZenithX;

public class OverloadUI : MonoBehaviour
{
	public static int numSuccesses;

	public static int maxPossibleTargets;

	public static int killSwitchThreshold;

	public static HashSet<NetworkedPlayerInfo> currentTargets = new HashSet<NetworkedPlayerInfo>(new NetPlayerInfoCidComparer());

	private HashSet<NetworkedPlayerInfo> _tmpTargets = new HashSet<NetworkedPlayerInfo>(new NetPlayerInfoCidComparer());

	private bool _hasAutoStarted;

	public static Rect WindowRect = new Rect(320f, 10f, 595f, 500f);

	private GUIStyle _targetButtonStyle;

	private GUIStyle _normalButtonStyle;

	private GUIStyle _logStyle;

	private static Vector2 _scrollPosition = Vector2.zero;

	private static List<string> _logEntries = new List<string>();

	private const int MaxLogEntries = 300;

	private static UILibrary.UIWindowData UI;

	private bool _areTargetsUnlocked
	{
		get
		{
			if (CheatToggles.runOverload)
			{
				return !CheatToggles.olLockTargets;
			}
			return true;
		}
	}

	private void Start()
	{
		killSwitchThreshold = 500;
		if (UI == null)
		{
			UI = UILibrary.UI.CreateUI("Overload", draggable: true, "", 0, "Target control", "Live status");
		}
	}

	private void Update()
	{
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		if (!CheatToggles.runOverload && !CheatToggles.olAutoStart && !CheatToggles.olAutoAdapt && !CheatToggles.showProtectMenu && !CheatToggles.olAutoStop && !CheatToggles.olKillSwitch && !CheatToggles.olLockTargets)
		{
			if (!CheatToggles.runOverload)
			{
				currentTargets.Clear();
				OverloadHandler.ClearCustomTargets();
			}
			_tmpTargets.Clear();
			_hasAutoStarted = false;
			return;
		}
		PlayerControl[] array = ((IEnumerable<PlayerControl>)PlayerControl.AllPlayerControls.ToArray()).Where((PlayerControl player) => (Object)(object)((player != null) ? player.Data : null) != (Object)null && !((InnerNetObject)player).AmOwner).ToArray();
		maxPossibleTargets = array.Length;
		if (!Utils.isFreePlay)
		{
			for (int num = 0; num < maxPossibleTargets; num++)
			{
				NetworkedPlayerInfo data = array[num].Data;
				bool item = OverloadHandler.GetTarget(data).isTarget;
				if (_areTargetsUnlocked)
				{
					if (item)
					{
						_tmpTargets.Add(data);
						if (CheatToggles.runOverload && CheatToggles.olLogAddRemove && !currentTargets.Contains(data))
						{
							string value = ColorUtility.ToHtmlStringRGB(Color.blue);
							LogConsole($"> <b><color=#{value}>ADD : {data.DefaultOutfit.PlayerName} (ID : {data.ClientId})</color></b>");
						}
					}
					else if (CheatToggles.runOverload && CheatToggles.olLogAddRemove && currentTargets.Contains(data))
					{
						string value2 = ColorUtility.ToHtmlStringRGB(Color.blue);
						LogConsole($"> <b><color=#{value2}>REMOVE : {data.DefaultOutfit.PlayerName} (ID : {data.ClientId})</color></b>");
					}
				}
				else if (currentTargets.Contains(data))
				{
					_tmpTargets.Add(data);
				}
			}
		}
		if (Utils.isPlayer)
		{
			HashSet<NetworkedPlayerInfo> tmpTargets = currentTargets;
			currentTargets = _tmpTargets;
			_tmpTargets = tmpTargets;
		}
		else
		{
			if (!CheatToggles.runOverload)
			{
				currentTargets.Clear();
				OverloadHandler.ClearCustomTargets();
			}
			_hasAutoStarted = false;
		}
		_tmpTargets.Clear();
		if (CheatToggles.olAutoAdapt)
		{
			(OverloadHandler.strength, OverloadHandler.cooldown) = OverloadHandler.CalculateAdaptedValues();
		}
		int count = currentTargets.Count;
		if (CheatToggles.runOverload)
		{
			bool num2 = CheatToggles.olAutoStop && count <= 0;
			bool flag = Utils.GetPing() > killSwitchThreshold;
			bool flag2 = CheatToggles.olKillSwitch && flag;
			if (num2 || flag2)
			{
				StopOverload(flag2 ? " : ! Kill Switch !" : "");
			}
		}
		else if (Utils.isPlayer && CheatToggles.olAutoStart && !_hasAutoStarted && count > 0)
		{
			_hasAutoStarted = true;
			StartOverload();
		}
	}

	private void OnGUI()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		if (!CheatToggles.showProtectMenu || !MenuUI.isGUIActive)
		{
			return;
		}
		if (UI == null)
		{
			UI = UILibrary.UI.CreateUI("Overload", draggable: true, "", 0, "Target control", "Live status");
		}
		InitStyles();
		UIHelpers.ApplyUIColor();
		WindowRect = GUI.Window(1, WindowRect, WindowFunction.op_Implicit((Action<int>)delegate
		{
			GUILayout.BeginHorizontal((Il2CppReferenceArray<GUILayoutOption>)null);
			GUILayout.Space(15f);
			GUILayout.BeginVertical((Il2CppReferenceArray<GUILayoutOption>)null);
			GUILayout.Space(5f);
			PlayerControl[] array = ((IEnumerable<PlayerControl>)PlayerControl.AllPlayerControls.ToArray()).Where((PlayerControl player) => (Object)(object)((player != null) ? player.Data : null) != (Object)null && !((InnerNetObject)player).AmOwner).ToArray();
			int num = array.Length;
			GUILayout.BeginHorizontal((Il2CppReferenceArray<GUILayoutOption>)null);
			if (num > 0 && !Utils.isFreePlay)
			{
				GUILayout.BeginVertical((GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.ExpandWidth(false) });
				GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.ExpandWidth(false) });
				DrawPlayers(array, num);
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
				GUILayout.Space(10f);
			}
			GUILayout.BeginVertical((Il2CppReferenceArray<GUILayoutOption>)null);
			DrawSelectionToggles();
			if (CheatToggles.overloadReset)
			{
				CheatToggles.overloadAll = false;
				CheatToggles.overloadHost = false;
				CheatToggles.overloadCrew = false;
				CheatToggles.overloadImps = false;
				OverloadHandler.ClearCustomTargets();
				CheatToggles.overloadReset = false;
			}
			GUILayout.EndVertical();
			GUILayout.Space(40f);
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
			GUILayout.Box("", GUIStylePreset.DarkSeparator, (GUILayoutOption[])(object)new GUILayoutOption[2]
			{
				GUILayout.Height(1f),
				GUILayout.Width(420f)
			});
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal((Il2CppReferenceArray<GUILayoutOption>)null);
			DrawStateButtons();
			GUILayout.Space(3f);
			DrawStateLabel();
			GUILayout.EndHorizontal();
			GUILayout.Space(20f);
			GUILayout.BeginVertical(GUI.skin.box, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(539f) });
			DrawConsole();
			GUILayout.EndVertical();
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			GUI.DragWindow();
		}), UI.Title);
	}

	private void InitStyles()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		if (_targetButtonStyle == null)
		{
			_targetButtonStyle = new GUIStyle(GUI.skin.button)
			{
				fontStyle = (FontStyle)2
			};
		}
		if (_normalButtonStyle == null)
		{
			_normalButtonStyle = new GUIStyle(GUI.skin.button)
			{
				fontStyle = (FontStyle)1
			};
		}
		if (_logStyle == null)
		{
			_logStyle = new GUIStyle(GUI.skin.label)
			{
				fontSize = 15
			};
		}
	}

	public static void LogConsole(string message)
	{
		if (_logEntries.Count >= 300)
		{
			_logEntries.RemoveAt(0);
		}
		_logEntries.Add(message);
		_scrollPosition.y = float.MaxValue;
	}

	public static void StartOverload()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		CheatToggles.runOverload = true;
		if (CheatToggles.olAutoClear)
		{
			_logEntries.Clear();
		}
		if (CheatToggles.olLogStartStop)
		{
			string value = ColorUtility.ToHtmlStringRGB(Color.red);
			string value2 = ((currentTargets.Count != 1) ? "s" : "");
			string value3 = ((maxPossibleTargets > 0 && currentTargets.Count == maxPossibleTargets) ? " - ALL" : "");
			LogConsole($"> <b><color=#{value}>START : [{currentTargets.Count}] Target{value2}{value3}</color></b>");
		}
		numSuccesses = 0;
	}

	public static void StopOverload(string extraStr = "")
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (CheatToggles.olLogStartStop)
		{
			int value = currentTargets.Count + numSuccesses;
			string value2 = ColorUtility.ToHtmlStringRGB(Color.red);
			LogConsole($"> <b><color=#{value2}>STOP : [{numSuccesses} / {value}] Kicked{extraStr}</color></b>");
		}
		CheatToggles.runOverload = false;
		numSuccesses = 0;
	}

	[HideFromIl2Cpp]
	private void DrawPlayers(PlayerControl[] players, int playerCount)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < playerCount; i++)
		{
			int num = i + 1;
			NetworkedPlayerInfo data = players[i].Data;
			(HashSet<OverloadHandler.TargetType>, bool) target = OverloadHandler.GetTarget(data);
			bool item = target.Item2;
			Color color = data.Color;
			Color contentColor = Color.Lerp(color, Color.white, 0.5f);
			Color backgroundColor = GUI.backgroundColor;
			Color contentColor2 = GUI.contentColor;
			GUI.backgroundColor = (item ? Color.black : color);
			GUI.contentColor = contentColor;
			GUIStyle val = (item ? _targetButtonStyle : _normalButtonStyle);
			if (GUILayout.Button(data.DefaultOutfit.PlayerName, val, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(140f) }) && _areTargetsUnlocked)
			{
				if (item)
				{
					var (hashSet, _) = target;
					if (hashSet.Contains(OverloadHandler.TargetType.All))
					{
						OverloadHandler.PopulateCustomTargets(players, OverloadHandler.TargetType.All);
						CheatToggles.overloadAll = false;
					}
					if (hashSet.Contains(OverloadHandler.TargetType.Host))
					{
						OverloadHandler.PopulateCustomTargets(players, OverloadHandler.TargetType.Host);
						CheatToggles.overloadHost = false;
					}
					if (hashSet.Contains(OverloadHandler.TargetType.Crewmate))
					{
						OverloadHandler.PopulateCustomTargets(players, OverloadHandler.TargetType.Crewmate);
						CheatToggles.overloadCrew = false;
					}
					else if (hashSet.Contains(OverloadHandler.TargetType.Impostor))
					{
						OverloadHandler.PopulateCustomTargets(players, OverloadHandler.TargetType.Impostor);
						CheatToggles.overloadImps = false;
					}
					OverloadHandler.RemoveCustomTarget(data);
				}
				else
				{
					OverloadHandler.AddCustomTarget(data);
				}
			}
			GUI.backgroundColor = backgroundColor;
			GUI.contentColor = contentColor2;
			if (num % 3 == 0 && num < playerCount)
			{
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.ExpandWidth(false) });
			}
		}
	}

	private void DrawSelectionToggles()
	{
		bool flag = GUILayout.Toggle(CheatToggles.overloadAll, " All", (Il2CppReferenceArray<GUILayoutOption>)null);
		CheatToggles.overloadAll = _areTargetsUnlocked && flag;
		bool flag2 = GUILayout.Toggle(CheatToggles.overloadHost, " Host", (Il2CppReferenceArray<GUILayoutOption>)null);
		CheatToggles.overloadHost = _areTargetsUnlocked && flag2;
		bool flag3 = GUILayout.Toggle(CheatToggles.overloadCrew, " Crewmates", (Il2CppReferenceArray<GUILayoutOption>)null);
		CheatToggles.overloadCrew = _areTargetsUnlocked && flag3;
		bool flag4 = GUILayout.Toggle(CheatToggles.overloadImps, " Impostors", (Il2CppReferenceArray<GUILayoutOption>)null);
		CheatToggles.overloadImps = _areTargetsUnlocked && flag4;
		bool flag5 = GUILayout.Toggle(CheatToggles.overloadReset, " Reset", (Il2CppReferenceArray<GUILayoutOption>)null);
		CheatToggles.overloadReset = _areTargetsUnlocked && flag5;
	}

	private void DrawStateButtons()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		Color backgroundColor = GUI.backgroundColor;
		bool flag = !CheatToggles.runOverload && Utils.isPlayer;
		Color green = Color.green;
		GUI.backgroundColor = (flag ? green : Color.black);
		if (GUILayout.Button("START", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(140f) }) && flag)
		{
			StartOverload();
		}
		GUI.backgroundColor = backgroundColor;
		bool runOverload = CheatToggles.runOverload;
		Color red = Color.red;
		GUI.backgroundColor = (runOverload ? red : Color.black);
		if (GUILayout.Button("STOP", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(140f) }) && runOverload)
		{
			StopOverload();
		}
		GUI.backgroundColor = backgroundColor;
	}

	private void DrawStateLabel()
	{
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (CheatToggles.runOverload)
		{
			string text = ColorUtility.ToHtmlStringRGB(Color.Lerp(MenuUI.UIAcceptedGreen, Color.white, 0.5f));
			string text2 = "<b><color=#" + text + "> On : ";
			string text3 = "</color></b>";
			string text4;
			if (currentTargets.Count > 0)
			{
				string value = ((currentTargets.Count != 1) ? "s" : "");
				text4 = $"Attacking {currentTargets.Count} target{value}";
			}
			else
			{
				text4 = "Idle";
			}
			GUILayout.Label(text2 + text4 + text3, (Il2CppReferenceArray<GUILayoutOption>)null);
			return;
		}
		string value2 = ColorUtility.ToHtmlStringRGB(Color.Lerp(MenuUI.UIDisabledGrey, Color.white, 0.6f));
		string value3 = "";
		if (currentTargets.Count > 0)
		{
			string value4 = ((currentTargets.Count != 1) ? "s" : "");
			value3 = $" : {currentTargets.Count} target{value4} selected";
		}
		GUILayout.Label($"<b><color=#{value2}> Off{value3}</color></b>", (Il2CppReferenceArray<GUILayoutOption>)null);
	}

	private void DrawConsole()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		_scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, false, Array.Empty<GUILayoutOption>());
		foreach (string logEntry in _logEntries)
		{
			GUILayout.Label(logEntry, _logStyle, (Il2CppReferenceArray<GUILayoutOption>)null);
		}
		GUILayout.EndScrollView();
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.ExpandWidth(false) });
		if (GUILayout.Button("Clear Log", (Il2CppReferenceArray<GUILayoutOption>)null))
		{
			_logEntries.Clear();
		}
		GUILayout.EndHorizontal();
	}
}
