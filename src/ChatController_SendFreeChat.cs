using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(ChatController), "SendFreeChat")]
public static class ChatController_SendFreeChat
{
	public static bool Prefix(ChatController __instance)
	{
		if (!CheatToggles.chatJailbreak | !Utils.isLobby | !Utils.isInGame)
		{
			return true;
		}
		string text = Utils.CensorUrlsAndEmails(__instance.freeChatField.Text);
		PlayerControl.LocalPlayer.RpcSendChat(text);
		return false;
	}
}
