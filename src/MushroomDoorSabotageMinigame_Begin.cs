using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(MushroomDoorSabotageMinigame), "Begin")]
public static class MushroomDoorSabotageMinigame_Begin
{
	public static bool Prefix(MushroomDoorSabotageMinigame __instance)
	{
		if (!CheatToggles.autoOpenDoorsOnUse)
		{
			return true;
		}
		__instance.FixDoorAndCloseMinigame();
		return false;
	}
}
