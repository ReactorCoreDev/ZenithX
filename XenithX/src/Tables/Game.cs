using InnerNet;
using UnityEngine;
using static NetworkedPlayerInfo;

namespace ZenithX;

public static class Game
{
    public static PlayerControl pLocalPlayer
    {
        get
        {
            if (PlayerControl.LocalPlayer == null)
            {
                Debug.LogWarning("Game.pLocalPlayer: LocalPlayer is null");
                return null;
            }
            return PlayerControl.LocalPlayer;
        }
    }

    public static string ToString(PlayerId playerId)
    {
        var player = GetPlayerDataById((byte)playerId);
        return player != null ? player.PlayerName : $"Player{(byte)playerId}";
    }

    public static bool IsHost()
    {
        return AmongUsClient.Instance != null && AmongUsClient.Instance.AmHost;
    }

    public static bool IsInGame()
    {
        return GameData.Instance != null && AmongUsClient.Instance != null && 
               AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started;
    }

    public static bool IsInLobby()
    {
        return GameData.Instance != null && AmongUsClient.Instance != null && 
               AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Joined;
    }

    public static NetworkedPlayerInfo GetPlayerData(PlayerControl player)
    {
        if (player == null)
        {
            Debug.LogWarning("Game.GetPlayerData: Player is null");
            return null;
        }
        return player.Data;
    }

    public static PlayerOutfit GetPlayerOutfit(NetworkedPlayerInfo playerData)
    {
        if (playerData == null)
        {
            Debug.LogWarning("Game.GetPlayerOutfit: PlayerData is null");
            return null;
        }
        return playerData.DefaultOutfit;
    }

    public static bool PlayerIsImpostor(NetworkedPlayerInfo playerData)
    {
        if (playerData == null || playerData.Role == null)
        {
            Debug.LogWarning("Game.PlayerIsImpostor: PlayerData or Role is null");
            return false;
        }
        return playerData.Role.IsImpostor;
    }
    
    public static NetworkedPlayerInfo GetPlayerDataById(byte playerId)
    {
        if (GameData.Instance == null)
        {
            Debug.LogWarning("Game.GetPlayerDataById: GameData.Instance is null");
            return null;
        }
        foreach (var player in GameData.Instance.AllPlayers)
        {
            if (player != null && player.PlayerId == playerId)
            {
                return player;
            }
        }
        Debug.LogWarning($"Game.GetPlayerDataById: No player found with ID {playerId}");
        return null;
    }

    public enum PlayerId : byte
    {
        Player0 = 0,
        Player1 = 1,
        Player2 = 2,
        Player3 = 3,
        Player4 = 4,
        Player5 = 5,
        Player6 = 6,
        Player7 = 7,
        Player8 = 8,
        Player9 = 9,
        Player10 = 10,
        Player11 = 11,
        Player12 = 12,
        Player13 = 13,
        Player14 = 14
    }
}