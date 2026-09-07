using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(LogicGameFlowNormal), "IsGameOverDueToDeath")]
public static class LogicGameFlowNormal_IsGameOverDueToDeath
{
	public static void Postfix(ref bool __result)
	{
		if (CheatToggles.noGameEnd)
		{
			__result = false;
		}
	}
}
