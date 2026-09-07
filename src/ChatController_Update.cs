using HarmonyLib;
using UnityEngine;
using UnityEngine.Audio;

namespace ZenithX;

[HarmonyPatch(typeof(ChatController), "Update")]
public static class ChatController_Update
{
	public static int CurrentHistorySelection = -1;

	private static string InProgressMessage = "";

	private static bool IsNavigatingHistory;

	public static void Postfix(ChatController __instance)
	{
		if ((Object)(object)__instance == (Object)null || (Object)(object)__instance.freeChatField == (Object)null || (Object)(object)__instance.freeChatField.textArea == (Object)null)
		{
			return;
		}
		if (CheatToggles.chatJailbreak)
		{
			if (__instance.timeSinceLastMessage < 0.9f)
			{
				__instance.timeSinceLastMessage = 0.9f;
			}
			__instance.freeChatField.textArea.characterLimit = 120;
			__instance.freeChatField.textArea.AllowSymbols = true;
			__instance.freeChatField.textArea.AllowEmail = true;
			__instance.freeChatField.textArea.allowAllCharacters = true;
		}
		if (ChatHistory_ChatController_SendChat_Prefix.ChatHistory.Count == 0)
		{
			return;
		}
		if (Input.GetKeyDown((KeyCode)273))
		{
			if (!IsNavigatingHistory)
			{
				InProgressMessage = __instance.freeChatField.textArea.text;
				IsNavigatingHistory = true;
				CurrentHistorySelection = ChatHistory_ChatController_SendChat_Prefix.ChatHistory.Count;
			}
			if (CurrentHistorySelection > 0)
			{
				CurrentHistorySelection--;
				__instance.freeChatField.textArea.SetText(ChatHistory_ChatController_SendChat_Prefix.ChatHistory[CurrentHistorySelection], "");
			}
			else
			{
				SoundManager.Instance.PlaySound(__instance.warningSound, false, 1f, (AudioMixerGroup)null);
			}
		}
		if (Input.GetKeyDown((KeyCode)274) && IsNavigatingHistory)
		{
			if (CurrentHistorySelection < ChatHistory_ChatController_SendChat_Prefix.ChatHistory.Count - 1)
			{
				CurrentHistorySelection++;
				__instance.freeChatField.textArea.SetText(ChatHistory_ChatController_SendChat_Prefix.ChatHistory[CurrentHistorySelection], "");
			}
			else
			{
				CurrentHistorySelection = ChatHistory_ChatController_SendChat_Prefix.ChatHistory.Count;
				__instance.freeChatField.textArea.SetText(InProgressMessage, "");
				IsNavigatingHistory = false;
			}
		}
	}
}
