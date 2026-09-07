using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(PlayerControl), "CmdCheckMurder")]
public static class PlayerControl_CmdCheckMurder
{
	public static bool Prefix(PlayerControl __instance, PlayerControl target)
	{
		if (Utils.isLobby)
		{
			AlertUI.Warning("Killing in lobby disabled for being too buggy");
			return false;
		}
		if (Utils.isHost && CheatToggles.killAnyone)
		{
			PlayerControl.LocalPlayer.RpcMurderPlayer(target, true);
			return false;
		}
		return true;
	}
}
