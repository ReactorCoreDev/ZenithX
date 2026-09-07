using HarmonyLib;
using InnerNet;

namespace ZenithX;

[HarmonyPatch(typeof(EngineerRole), "FixedUpdate")]
public static class EngineerRole_FixedUpdate
{
	public static void Postfix(EngineerRole __instance)
	{
		if (((InnerNetObject)((RoleBehaviour)__instance).Player).AmOwner)
		{
			ZenithXCheats.engineerCheats(__instance);
		}
	}
}
