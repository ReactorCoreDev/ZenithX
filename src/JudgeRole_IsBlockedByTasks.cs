using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(JudgeRole), "IsBlockedByTasks")]
public static class JudgeRole_IsBlockedByTasks
{
	public static bool Prefix(ref bool __result)
	{
		__result = false;
		return false;
	}
}
