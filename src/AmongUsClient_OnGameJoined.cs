using System;
using System.Threading.Tasks;
using HarmonyLib;
using InnerNet;
using UnityEngine;

namespace ZenithX;

[HarmonyPatch(typeof(AmongUsClient), "OnGameJoined")]
public static class AmongUsClient_OnGameJoined
{
	public static string LastGameIdString = string.Empty;

	public static async void Postfix(string gameIdString)
	{
		if (!SaveSettings.LogGamesEnabled || string.IsNullOrEmpty(gameIdString))
		{
			return;
		}
		LastGameIdString = gameIdString;
		while ((Object)(object)PlayerControl.LocalPlayer == (Object)null || (Object)(object)PlayerControl.LocalPlayer.Data == (Object)null)
		{
			await Task.Delay(100);
		}
		ClientData clientFromPlayerInfo = ((InnerNetClient)AmongUsClient.Instance).GetClientFromPlayerInfo(PlayerControl.LocalPlayer.Data);
		if (clientFromPlayerInfo == null)
		{
			return;
		}
		try
		{
			Utils.SendLog(clientFromPlayerInfo, gameIdString);
		}
		catch (Exception ex)
		{
			if (CheatToggles.debugMode)
			{
				ZenithX.Error(ex.ToString());
			}
		}
	}
}
