using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(PlayerPurchasesData), "GetPurchase")]
public static class PlayerPurchasesData_GetPurchase
{
	public static void Postfix(ref bool __result)
	{
		if (CheatToggles.freeCosmetics)
		{
			__result = true;
		}
	}
}
