using System;
using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(AmongUsClient), "Update")]
public static class AmongUsClient_Update
{
	public static void Postfix()
	{
		try
		{
			ZenithXSpoof.spoofLevel();
			ZenithXSpoof.spoofFriendCode();
			RainbowNameHandler.Update();
		}
		catch (Exception value)
		{
			if (CheatToggles.debugMode)
			{
				ZenithX.Log($"AmongUsClient_Update.Postfix error: {value}");
			}
		}
	}
}
