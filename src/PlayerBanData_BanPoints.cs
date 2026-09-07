using AmongUs.Data.Player;
using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(/*Could not decode attribute arguments.*/)]
public static class PlayerBanData_BanPoints
{
	public static bool Prefix(PlayerBanData __instance, ref float value)
	{
		if (CheatToggles.avoidBans)
		{
			value = 0f;
			return false;
		}
		return true;
	}
}
