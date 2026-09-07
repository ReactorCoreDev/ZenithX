using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(PlayerControl), "OnGameStart")]
public static class PlayerControl_OnGameStart
{
	public static void Prefix()
	{
		Utils.GameLoaded = true;
	}
}
