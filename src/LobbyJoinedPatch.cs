using System;
using HarmonyLib;
using InnerNet;
using UnityEngine;

namespace ZenithX;

[HarmonyPatch(typeof(AmongUsClient), "OnGameJoined")]
public static class LobbyJoinedPatch
{
	[HarmonyPostfix]
	public static void Postfix(string gameIdString)
	{
		try
		{
			if (!((Object)(object)AmongUsClient.Instance == (Object)null))
			{
				LobbyAgeData.WaitingForLobby = true;
				LobbyAgeData.CurrentGameId = ((InnerNetClient)AmongUsClient.Instance).GameId;
				LobbyAgeData.LobbyAge = -1;
				LobbyAgeData.LobbyTime = 600;
				ZenithX.Log($"Joined GameId: {LobbyAgeData.CurrentGameId}");
				ZenithX.Log("GameId String: " + gameIdString);
				FindLobbyListing();
			}
		}
		catch (Exception value)
		{
			ZenithX.Log($"LobbyJoined error: {value}");
		}
	}

	private static void FindLobbyListing()
	{
		for (int i = 0; i < LobbyAgeData.Listings.Count; i++)
		{
			GameListing val = LobbyAgeData.Listings[i];
			if (val.GameId == LobbyAgeData.CurrentGameId)
			{
				SetLobbyTimer(val.Age);
				return;
			}
		}
		ZenithX.Log($"Current lobby {LobbyAgeData.CurrentGameId} was not found. Defaulting to 600 seconds.");
		LobbyAgeData.WaitingForLobby = false;
		LobbyAgeData.LobbyAge = -1;
		LobbyAgeData.LobbyTime = 600;
		ShowLobbyTimerWhenReady();
	}

	public static void SetLobbyTimer(int Age)
	{
		Age = Mathf.Max(0, Age);
		int num = Age % 600;
		int num2 = Mathf.Clamp(600 - num, 0, 600);
		LobbyAgeData.LobbyAge = Age;
		LobbyAgeData.LobbyTime = num2;
		LobbyAgeData.WaitingForLobby = false;
		ZenithX.Log($"Lobby Age: {Age}");
		ZenithX.Log($"Lobby Age Within 600: {num}");
		ZenithX.Log($"Lobby Time Remaining: {num2}");
		ShowLobbyTimerWhenReady();
	}

	private static void ShowLobbyTimerWhenReady()
	{
		if (!((Object)(object)DestroyableSingleton<HudManager>.Instance == (Object)null))
		{
			DestroyableSingleton<HudManager>.Instance.ShowLobbyTimer(LobbyAgeData.LobbyTime);
		}
	}
}
