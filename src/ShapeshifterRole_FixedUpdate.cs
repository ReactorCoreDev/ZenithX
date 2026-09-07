using HarmonyLib;
using InnerNet;

namespace ZenithX;

[HarmonyPatch(typeof(ShapeshifterRole), "FixedUpdate")]
public static class ShapeshifterRole_FixedUpdate
{
	public static void Postfix(ShapeshifterRole __instance)
	{
		try
		{
			if (((InnerNetObject)((RoleBehaviour)__instance).Player).AmOwner)
			{
				ZenithXCheats.shapeshifterCheats(__instance);
			}
		}
		catch
		{
		}
	}
}
