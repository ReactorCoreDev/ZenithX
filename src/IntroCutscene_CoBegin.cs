using System;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace ZenithX;

[HarmonyPatch(typeof(IntroCutscene), "CoBegin")]
public static class IntroCutscene_CoBegin
{
	public static void Prefix(IntroCutscene __instance)
	{
		if (!CheatToggles.memeify)
		{
			return;
		}
		try
		{
			TextMeshPro teamTitle = __instance.TeamTitle;
			if ((Object)(object)teamTitle == (Object)null)
			{
				if (CheatToggles.debugMode)
				{
					ZenithX.Error("TeamTitle is NULL");
				}
			}
			else
			{
				if (CheatToggles.debugMode)
				{
					ZenithX.Error("Old TeamTitle: " + ((TMP_Text)teamTitle).text);
				}
				((TMP_Text)teamTitle).SetText("That's Crazy", true);
				if (CheatToggles.debugMode)
				{
					ZenithX.Error("New TeamTitle: " + ((TMP_Text)teamTitle).text);
				}
			}
		}
		catch (Exception value)
		{
			ZenithX.Error($"TeamTitle exception: {value}");
		}
		try
		{
			TextMeshPro impostorText = __instance.ImpostorText;
			if ((Object)(object)impostorText == (Object)null)
			{
				if (CheatToggles.debugMode)
				{
					ZenithX.Error("ImpostorText is NULL");
				}
				return;
			}
			if (CheatToggles.debugMode)
			{
				ZenithX.Error("Old ImpostorText: " + ((TMP_Text)impostorText).text);
			}
			((TMP_Text)impostorText).SetText("There are <color=#FF1919FF>0 People</color> who asked", true);
			if (CheatToggles.debugMode)
			{
				ZenithX.Error("New ImpostorText: " + ((TMP_Text)impostorText).text);
			}
		}
		catch (Exception value2)
		{
			ZenithX.Error($"ImpostorText exception: {value2}");
		}
	}
}
