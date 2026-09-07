using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(Constants), "IsVersionModded")]
public static class Constants_IsVersionModded_Patch
{
	private static bool Prefix(ref bool __result)
	{
		__result = false;
		return false;
	}
}
