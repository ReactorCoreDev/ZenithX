using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(PlayerControl), "OnGameEnd")]
public static class PlayerControl_OnGameEnd
{
	public static void Prefix()
	{
		Utils.GameLoaded = false;
	}
}
