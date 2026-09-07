using HarmonyLib;
using InnerNet;

namespace ZenithX;

[HarmonyPatch(typeof(ScientistRole), "Update")]
public static class ScientistRole_Update
{
	public static void Postfix(ScientistRole __instance)
	{
		if (((InnerNetObject)((RoleBehaviour)__instance).Player).AmOwner)
		{
			ZenithXCheats.scientistCheats(__instance);
		}
	}
}
