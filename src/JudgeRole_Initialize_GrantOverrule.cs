using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(JudgeRole), "Initialize")]
public static class JudgeRole_Initialize_GrantOverrule
{
	public static void Postfix(JudgeRole __instance)
	{
		if (CheatToggles.judgeOverrule)
		{
			__instance.HasAnOverruleUse = true;
		}
	}
}
