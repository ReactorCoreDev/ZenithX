using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(AmongUsClient), "OnGameEnd")]
public static class AmongUsClient_OnGameEnd_Patch
{
	public static void Postfix()
	{
		GameData_RemovePlayer_Patch.notifiedDisconnects.Clear();
		PlayerControl_MurderPlayer_Patch.notifiedKilledVictims.Clear();
	}
}
