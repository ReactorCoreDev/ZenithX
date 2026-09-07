using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(/*Could not decode attribute arguments.*/)]
public static class Passive_SpoofedPUIDPostfix
{
	public static void Postfix(ref string __result)
	{
		if (ZenithX.spoofPuid.Value != "")
		{
			__result = ZenithX.spoofPuid.Value;
		}
	}
}
