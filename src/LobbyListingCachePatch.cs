using System;
using HarmonyLib;
using InnerNet;

namespace ZenithX;

[HarmonyPatch(typeof(GameContainer), "SetupGameInfo")]
public static class LobbyListingCachePatch
{
	[HarmonyPostfix]
	public static void Postfix(GameContainer __instance)
	{
		try
		{
			if (__instance.gameListing == null)
			{
				return;
			}
			GameListing gameListing = __instance.gameListing;
			for (int i = 0; i < LobbyAgeData.Listings.Count; i++)
			{
				if (LobbyAgeData.Listings[i].GameId == gameListing.GameId)
				{
					LobbyAgeData.Listings[i] = gameListing;
					if (LobbyAgeData.WaitingForLobby && gameListing.GameId == LobbyAgeData.CurrentGameId)
					{
						LobbyJoinedPatch.SetLobbyTimer(gameListing.Age);
					}
					return;
				}
			}
			LobbyAgeData.Listings.Add(gameListing);
			if (LobbyAgeData.WaitingForLobby && gameListing.GameId == LobbyAgeData.CurrentGameId)
			{
				LobbyJoinedPatch.SetLobbyTimer(gameListing.Age);
			}
		}
		catch (Exception value)
		{
			ZenithX.Log($"LobbyListingCache error: {value}");
		}
	}
}
