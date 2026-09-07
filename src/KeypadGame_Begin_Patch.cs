using HarmonyLib;
using TMPro;
using UnityEngine;

namespace ZenithX;

[HarmonyPatch(typeof(KeypadGame), "Begin")]
public static class KeypadGame_Begin_Patch
{
	public static void Postfix(KeypadGame __instance)
	{
		if (!CheatToggles.autoKeypadCode || (Object)(object)__instance == (Object)null)
		{
			return;
		}
		if (CheatToggles.debugMode)
		{
			ZenithX.Log($"Target text: {__instance.TargetText}");
		}
		if (int.TryParse(((TMP_Text)__instance.NumberText).text, out var result))
		{
			__instance.number = result;
			__instance.Enter();
			if (CheatToggles.debugMode)
			{
				ZenithX.Log($"Code set to {result}");
			}
		}
		else if (CheatToggles.debugMode)
		{
			ZenithX.Log("Failed to parse keypad code");
		}
	}
}
