using System;
using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(HudManager), "Start")]
public static class LobbyTimerHudPatch
{
	[HarmonyPostfix]
	public static void Postfix()
	{
		try
		{
			if (LobbyAgeData.WaitingForLobby)
			{
				LobbyJoinedPatch.SetLobbyTimer(LobbyAgeData.LobbyAge);
			}
		}
		catch (Exception value)
		{
			ZenithX.Log($"LobbyTimerHudPatch error: {value}");
		}
	}
}
