using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(/*Could not decode attribute arguments.*/)]
public static class EOSManager_HasServerTimestamp_Getter
{
	public static void Postfix(ref bool __result)
	{
		if (CheatToggles.spoofAprilFoolsDate)
		{
			__result = true;
		}
	}
}
