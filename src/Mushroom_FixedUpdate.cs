using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(Mushroom), "FixedUpdate")]
public static class Mushroom_FixedUpdate
{
	public static void Postfix(Mushroom __instance)
	{
		ZenithXESP.sporeCloudVision(__instance);
	}
}
