using Il2CppSystem.Collections.Generic;
using InnerNet;
using UnityEngine;

namespace ZenithX;

public static class Game
{
	public enum PlayerId : byte
	{
		Player0,
		Player1,
		Player2,
		Player3,
		Player4,
		Player5,
		Player6,
		Player7,
		Player8,
		Player9,
		Player10,
		Player11,
		Player12,
		Player13,
		Player14
	}

	public static PlayerControl pLocalPlayer
	{
		get
		{
			if ((Object)(object)PlayerControl.LocalPlayer == (Object)null)
			{
				return null;
			}
			return PlayerControl.LocalPlayer;
		}
	}

	public static string ToString(PlayerId playerId)
	{
		NetworkedPlayerInfo playerDataById = GetPlayerDataById((byte)playerId);
		if (!((Object)(object)playerDataById != (Object)null))
		{
			return $"Player{playerId}";
		}
		return playerDataById.PlayerName;
	}

	public static bool IsHost()
	{
		if ((Object)(object)AmongUsClient.Instance != (Object)null)
		{
			return ((InnerNetClient)AmongUsClient.Instance).AmHost;
		}
		return false;
	}

	public static bool IsInGame()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Invalid comparison between Unknown and I4
		if ((Object)(object)GameData.Instance != (Object)null && (Object)(object)AmongUsClient.Instance != (Object)null)
		{
			return (int)((InnerNetClient)AmongUsClient.Instance).GameState == 2;
		}
		return false;
	}

	public static bool IsInLobby()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Invalid comparison between Unknown and I4
		if ((Object)(object)GameData.Instance != (Object)null && (Object)(object)AmongUsClient.Instance != (Object)null)
		{
			return (int)((InnerNetClient)AmongUsClient.Instance).GameState == 1;
		}
		return false;
	}

	public static NetworkedPlayerInfo GetPlayerData(PlayerControl player)
	{
		if ((Object)(object)player == (Object)null)
		{
			return null;
		}
		return player.Data;
	}

	public static PlayerOutfit GetPlayerOutfit(NetworkedPlayerInfo playerData)
	{
		if ((Object)(object)playerData == (Object)null)
		{
			return null;
		}
		return playerData.DefaultOutfit;
	}

	public static bool PlayerIsImpostor(NetworkedPlayerInfo playerData)
	{
		if ((Object)(object)playerData == (Object)null || (Object)(object)playerData.Role == (Object)null)
		{
			return false;
		}
		return playerData.Role.IsImpostor;
	}

	public static NetworkedPlayerInfo GetPlayerDataById(byte playerId)
	{
		if ((Object)(object)GameData.Instance == (Object)null)
		{
			return null;
		}
		Enumerator<NetworkedPlayerInfo> enumerator = GameData.Instance.AllPlayers.GetEnumerator();
		while (enumerator.MoveNext())
		{
			NetworkedPlayerInfo current = enumerator.Current;
			if ((Object)(object)current != (Object)null && current.PlayerId == playerId)
			{
				return current;
			}
		}
		return null;
	}
}
