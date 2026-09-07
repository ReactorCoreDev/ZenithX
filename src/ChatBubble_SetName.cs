using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(ChatBubble), "SetName")]
public static class ChatBubble_SetName
{
	public static void Postfix(ChatBubble __instance)
	{
		ZenithXESP.ChatNametags(__instance);
	}
}
