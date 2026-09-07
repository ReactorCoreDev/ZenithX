using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(ShipStatus), "FixedUpdate")]
public static class FungleShipStatus_FixedUpdate
{
	public static void Postfix(FungleShipStatus __instance)
	{
		ZenithXCheats.fungleSabotageCheat(__instance);
	}
}
