using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(MatchInfoHudButton), "Update")]
public static class MatchInfoHudButton_Update
{
	public static bool Prefix(MatchInfoHudButton __instance)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (CheatToggles.chatJailbreak)
		{
			__instance.aspectPosition.DistanceFromEdge = MatchInfoHudButton.adjustedDistanceFromEdge;
			return false;
		}
		return true;
	}
}
