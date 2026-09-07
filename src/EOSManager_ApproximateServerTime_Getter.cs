using System;
using HarmonyLib;
using Il2CppSystem;

namespace ZenithX;

[HarmonyPatch(/*Could not decode attribute arguments.*/)]
public static class EOSManager_ApproximateServerTime_Getter
{
	public static void Postfix(ref DateTime __result)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (CheatToggles.spoofAprilFoolsDate)
		{
			DateTime dateTime = new DateTime(DateTime.UtcNow.Year, 4, 1, 7, 1, 0, DateTimeKind.Utc);
			__result = new DateTime(dateTime.Ticks);
		}
	}
}
