using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(/*Could not decode attribute arguments.*/)]
public static class PlayerBanData_IsBanned
{
	public static bool Prefix(ref bool __result)
	{
		if (CheatToggles.avoidBans)
		{
			__result = false;
			return false;
		}
		return true;
	}
}
