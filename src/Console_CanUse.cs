using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(Console), "CanUse")]
public static class Console_CanUse
{
	public static void Prefix(Console __instance, ref bool __state)
	{
		if (CheatToggles.impostorTasks)
		{
			__state = __instance.AllowImpostor;
			__instance.AllowImpostor = true;
		}
	}

	public static void Postfix(Console __instance, ref bool __state)
	{
		if (CheatToggles.impostorTasks)
		{
			__instance.AllowImpostor = __state;
		}
	}
}
