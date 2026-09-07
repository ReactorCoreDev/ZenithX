using System.Collections.Generic;
using InnerNet;

namespace ZenithX;

public static class LobbyAgeData
{
	public static bool WaitingForLobby = false;

	public static int CurrentGameId = -1;

	public static int LobbyAge = -1;

	public static int LobbyTime = 600;

	public static readonly List<GameListing> Listings = new List<GameListing>();
}
