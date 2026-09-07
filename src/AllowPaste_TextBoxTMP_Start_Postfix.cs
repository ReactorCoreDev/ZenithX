using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(TextBoxTMP), "Start")]
public static class AllowPaste_TextBoxTMP_Start_Postfix
{
	public static void Postfix(TextBoxTMP __instance)
	{
		if (CheatToggles.chatJailbreak && (Utils.isLobby || Utils.isInGame))
		{
			__instance.allowAllCharacters = CheatToggles.chatJailbreak;
			__instance.AllowEmail = CheatToggles.chatJailbreak;
			__instance.AllowSymbols = CheatToggles.chatJailbreak;
			__instance.characterLimit = (CheatToggles.chatJailbreak ? 120 : 100);
		}
	}
}
