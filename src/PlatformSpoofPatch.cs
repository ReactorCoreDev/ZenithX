using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(PlayerControl), "Start")]
internal class PlatformSpoofPatch
{
	private static void Postfix(PlayerControl __instance)
	{
		if (AntiCheatConfig.Enabled && AntiCheatConfig.CheckSpoofedPlatforms)
		{
			AntiCheat.CheckPlatformSpoof(__instance);
		}
	}
}
