using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(EOSManager), "IsAllowedOnline")]
public static class EOSManager_IsAllowedOnline
{
	public static void Prefix(ref bool canOnline)
	{
		if (CheatToggles.unlockFeatures)
		{
			canOnline = true;
		}
	}
}
