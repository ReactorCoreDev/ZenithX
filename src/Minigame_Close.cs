using System;
using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(Minigame), "Close")]
public static class Minigame_Close
{
	[HarmonyPatch(new Type[] { })]
	public static void Postfix()
	{
		MinigamePatches.currentMinigame = null;
		MinigamePatches.currentTask = null;
	}
}
