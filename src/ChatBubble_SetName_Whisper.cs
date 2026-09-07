using HarmonyLib;
using TMPro;
using UnityEngine;

namespace ZenithX;

[HarmonyPatch(typeof(ChatBubble), "SetName")]
public static class ChatBubble_SetName_Whisper
{
	public static void Postfix(ChatBubble __instance)
	{
		if ((Object)(object)ChatMimic_RpcSendChat_Patch.pendingWhisperDisplayTarget != (Object)null && (Object)(object)__instance.playerInfo != (Object)null && (Object)(object)__instance.playerInfo.Object == (Object)(object)PlayerControl.LocalPlayer)
		{
			PlayerControl pendingWhisperDisplayTarget = ChatMimic_RpcSendChat_Patch.pendingWhisperDisplayTarget;
			ChatMimic_RpcSendChat_Patch.pendingWhisperDisplayTarget = null;
			object obj;
			if (pendingWhisperDisplayTarget == null)
			{
				obj = null;
			}
			else
			{
				NetworkedPlayerInfo data = pendingWhisperDisplayTarget.Data;
				obj = ((data != null) ? data.PlayerName : null);
			}
			if (obj == null)
			{
				obj = "???";
			}
			string text = (string)obj;
			((TMP_Text)__instance.NameText).text = "[You] -> [" + text + "]";
			((TMP_Text)__instance.NameText).ForceMeshUpdate(true, true);
		}
	}
}
