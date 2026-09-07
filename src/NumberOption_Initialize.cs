using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(NumberOption), "Initialize")]
public static class NumberOption_Initialize
{
	public static void Postfix(NumberOption __instance)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		if (CheatToggles.noOptionsLimits)
		{
			__instance.ValidRange = new FloatRange(-999f, 999f);
		}
	}
}
