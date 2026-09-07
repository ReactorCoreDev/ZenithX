using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(IGameOptionsExtensions), "GetAdjustedNumImpostors")]
public static class IGameOptionsExtensions_GetAdjustedNumImpostors
{
	public static bool Prefix(ref int __result)
	{
		if (!CheatToggles.noOptionsLimits)
		{
			return true;
		}
		__result = GameOptionsManager.Instance.CurrentGameOptions.NumImpostors;
		return false;
	}
}
