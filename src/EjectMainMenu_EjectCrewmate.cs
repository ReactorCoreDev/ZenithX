using System.Reflection;
using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(EjectMainMenu), "EjectCrewmate")]
public static class EjectMainMenu_EjectCrewmate
{
	public static void Prefix(EjectMainMenu __instance)
	{
		if (CheatToggles.zeroEjectCrewmateButtonCD)
		{
			typeof(EjectMainMenu).GetField("onCooldown", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(__instance, false);
		}
	}
}
