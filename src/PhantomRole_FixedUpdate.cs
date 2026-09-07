using HarmonyLib;
using InnerNet;

namespace ZenithX;

[HarmonyPatch(typeof(PhantomRole), "FixedUpdate")]
public static class PhantomRole_FixedUpdate
{
	public static void Postfix(PhantomRole __instance)
	{
		if (((InnerNetObject)((RoleBehaviour)__instance).Player).AmOwner)
		{
			ZenithXCheats.phantomCheats(__instance);
		}
	}
}
