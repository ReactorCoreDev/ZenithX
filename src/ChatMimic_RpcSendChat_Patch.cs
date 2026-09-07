using HarmonyLib;
using Hazel;
using InnerNet;
using UnityEngine;

namespace ZenithX;

[HarmonyPatch(typeof(PlayerControl), "RpcSendChat")]
public static class ChatMimic_RpcSendChat_Patch
{
	public static PlayerControl whisperTarget;

	public static PlayerControl pendingWhisperDisplayTarget;

	public static bool Prefix(ref string chatText, PlayerControl __instance)
	{
		if ((Object)(object)whisperTarget != (Object)null && ((InnerNetObject)__instance).AmOwner)
		{
			if (string.IsNullOrWhiteSpace(chatText))
			{
				return false;
			}
			int num = -1;
			try
			{
				num = ((InnerNetClient)AmongUsClient.Instance).GetClientIdFromCharacter(whisperTarget);
			}
			catch
			{
			}
			if (num >= 0)
			{
				if (((InnerNetClient)AmongUsClient.Instance).AmClient && (Object)(object)DestroyableSingleton<HudManager>.Instance != (Object)null)
				{
					pendingWhisperDisplayTarget = whisperTarget;
					DestroyableSingleton<HudManager>.Instance.Chat.AddChat(__instance, chatText, false);
				}
				MessageWriter val = ((InnerNetClient)AmongUsClient.Instance).StartRpcImmediately(((InnerNetObject)__instance).NetId, (byte)13, (SendOption)1, num);
				val.Write(chatText);
				((InnerNetClient)AmongUsClient.Instance).FinishRpcImmediately(val);
				whisperTarget = null;
				return false;
			}
		}
		if (CheatToggles.chatJailbreak && (Object)(object)AmongUsClient.Instance != (Object)null && !((InnerNetClient)AmongUsClient.Instance).AmHost)
		{
			chatText = chatText.Replace("<", string.Empty).Replace(">", string.Empty);
		}
		return !string.IsNullOrWhiteSpace(chatText);
	}
}
