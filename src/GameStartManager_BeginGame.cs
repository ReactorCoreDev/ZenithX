using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(GameStartManager), "BeginGame")]
public static class GameStartManager_BeginGame
{
	public static bool Prefix()
	{
		if (CheatToggles.attemptToCrashLobby && Utils.IsInGame() && !Utils.GameLoaded)
		{
			PlayerControl.LocalPlayer.CmdReportDeadBody((NetworkedPlayerInfo)null);
			return true;
		}
		return true;
	}
}
