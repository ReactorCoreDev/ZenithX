using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(EjectMainMenu), "CoCooldown")]
public static class EjectMainMenu_CoCooldown
{
	public static bool Prefix()
	{
		return !CheatToggles.zeroEjectCrewmateButtonCD;
	}
}
