using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(AccountManager), "CanPlayOnline")]
public static class AccountManager_OnlinePatch
{
	public static void Postfix(ref bool __result)
	{
		if (CheatToggles.unlockFeatures)
		{
			__result = true;
		}
	}
}
