using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(AmongUsClient), "CoStartGame")]
public static class AmongUsClient_CoStartGame
{
	public static void Postfix()
	{
		if (CheatToggles.logGameState)
		{
			AlertUI.Notification("Game started", 5f);
		}
	}
}
