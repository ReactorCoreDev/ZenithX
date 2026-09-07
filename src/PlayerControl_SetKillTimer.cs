using HarmonyLib;
using InnerNet;

namespace ZenithX;

[HarmonyPatch(typeof(PlayerControl), "SetKillTimer")]
public static class PlayerControl_SetKillTimer
{
	public static void Prefix(PlayerControl __instance, ref float time)
	{
		if (((InnerNetObject)__instance).AmOwner && Utils.isHost && CheatToggles.zeroKillCd)
		{
			time = 0f;
		}
	}
}
