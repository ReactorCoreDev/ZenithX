using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(ChatController), "Awake")]
public static class ChatHistoryLimit_ChatController_Awake_Postfix
{
	public static void Postfix(ChatController __instance)
	{
		__instance.chatBubblePool.poolSize = 20;
		__instance.chatBubblePool.ReclaimOldest();
	}
}
