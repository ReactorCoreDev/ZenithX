using HarmonyLib;
using UnityEngine;

namespace ZenithX;

[HarmonyPatch(typeof(Minigame), "Begin")]
public static class Minigame_Begin_Patch
{
	public static void Postfix(Minigame __instance)
	{
		if (!((Object)(object)__instance == (Object)null))
		{
			MinigamePatches.currentMinigame = __instance;
			MinigamePatches.currentTask = __instance.MyTask;
		}
	}
}
