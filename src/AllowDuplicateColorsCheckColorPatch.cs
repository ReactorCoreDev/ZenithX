using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(PlayerControl), "CheckColor")]
public static class AllowDuplicateColorsCheckColorPatch
{
	public static bool Prefix(PlayerControl __instance, byte bodyColor)
	{
		return ZenithXCheats.AllowDuplicateColor(__instance, bodyColor);
	}
}
