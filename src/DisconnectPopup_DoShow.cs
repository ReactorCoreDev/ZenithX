using HarmonyLib;
using TMPro;
using UnityEngine;

namespace ZenithX;

[HarmonyPatch(typeof(DisconnectPopup), "DoShow")]
public static class DisconnectPopup_DoShow
{
	public static void Postfix(DisconnectPopup __instance)
	{
		if ((Object)(object)__instance == (Object)null || !CheatToggles.copyLobbyCodeOnDisconnect)
		{
			return;
		}
		string lastGameIdString = AmongUsClient_OnGameJoined.LastGameIdString;
		if (string.IsNullOrEmpty(lastGameIdString) || !Utils.setClipboard(lastGameIdString))
		{
			return;
		}
		TextMeshPro textArea = __instance._textArea;
		if ((Object)(object)textArea == (Object)null)
		{
			return;
		}
		string text = ((TMP_Text)textArea).text;
		if (!string.IsNullOrEmpty(text))
		{
			string text2 = "Lobby code copied.";
			if (!text.Contains(text2))
			{
				((TMP_Text)textArea).text = text + "\n\n<color=#FFD700>" + text2 + "</color>";
			}
		}
	}
}
