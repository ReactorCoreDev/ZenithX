using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(MatchInfoGuide), "Open")]
public static class MatchInfoGuide_Open
{
	public static void Prefix(MatchInfoGuide __instance)
	{
		if (__instance.NormalModeSettings.Count > 0 || __instance.HnSModeSettings.Count > 0)
		{
			__instance.ControllerSelectable.Clear();
			__instance.CreatePlayerEntries();
		}
	}
}
