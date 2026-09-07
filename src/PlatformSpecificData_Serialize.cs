using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(PlatformSpecificData), "Serialize")]
public static class PlatformSpecificData_Serialize
{
	public static void Prefix(PlatformSpecificData __instance)
	{
		ZenithXSpoof.spoofPlatform(__instance);
	}
}
