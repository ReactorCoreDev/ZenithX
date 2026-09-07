using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(LogicGameFlowNormal), "CheckEndCriteria")]
public static class LogicGameFlowNormal_CheckEndCriteria
{
	public static bool Prefix()
	{
		return !CheatToggles.noGameEnd;
	}
}
