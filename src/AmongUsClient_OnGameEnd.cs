using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(AmongUsClient), "OnGameEnd")]
public static class AmongUsClient_OnGameEnd
{
	public static void Postfix(EndGameResult endGameResult)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (CheatToggles.logGameState)
		{
			AlertUI.Notification($"Game ended with reason {endGameResult.GameOverReason}", 5f);
		}
	}
}
