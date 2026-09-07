using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(PlayerControl), "CmdCheckVanish")]
public static class PlayerControl_CmdCheckVanish
{
	public static void Prefix(ref float maxDuration)
	{
		if (CheatToggles.endlessVanishDuration)
		{
			maxDuration = float.MaxValue;
		}
	}
}
