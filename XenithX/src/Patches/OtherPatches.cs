using HarmonyLib;
using AmongUs.Data;
using AmongUs.Data.Player;
using UnityEngine;
using System;
using System.Security.Cryptography;
using InnerNet;
using System.IO;
using System.Collections.Generic;
using AmongUs.GameOptions;

namespace ZenithX;

[HarmonyPatch(typeof(PlatformSpecificData), nameof(PlatformSpecificData.Serialize))]
public static class PlatformSpecificData_Serialize
{
    // Prefix patch of Constants.GetPlatformType to spoof the user's platform type
    public static void Prefix(PlatformSpecificData __instance)
    {

        ZenithXSpoof.spoofPlatform(__instance);

    }
}

[HarmonyPatch(typeof(SystemInfo), nameof(SystemInfo.deviceUniqueIdentifier), MethodType.Getter)]
public static class SystemInfo_deviceUniqueIdentifier_Getter
{
    // Postfix patch of SystemInfo.deviceUniqueIdentifier Getter method 
    // Made to hide the user's real unique deviceId by generating a random fake one
    public static void Postfix(ref string __result)
    {
        if (ZenithX.spoofDeviceId.Value)
        {

            var bytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            __result = BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.Update))]
public static class AmongUsClient_Update
{
    public static void Postfix()
    {
        try
        {
            // Safely spoof level first
            ZenithXSpoof.spoofLevel();

            // Validate EOSManager and guest mode
            var eos = EOSManager.Instance;
            if (eos == null)
                return;

            if (!eos.HasFinishedLoginFlow() || !ZenithX.guestMode.Value)
                return;

            // Safely ensure account objects exist
            if (DataManager.Player?.Account == null)
            {
                ZenithX.Log("[ZenithX] DataManager.Player.Account is null, skipping Postfix.");
                return;
            }

            DataManager.Player.Account.LoginStatus = EOSManager.AccountLoginStatus.LoggedIn;

            // Only spoof friend code if its empty
            if (string.IsNullOrWhiteSpace(eos.FriendCode))
            {
                string friendCode = ZenithXSpoof.spoofFriendCode();

                if (eos.editAccountUsername != null && eos.editAccountUsername.UsernameText != null)
                {
                    eos.editAccountUsername.UsernameText.SetText(friendCode);
                    eos.editAccountUsername.SaveUsername();
                    eos.FriendCode = friendCode;

                    ZenithX.Log($"[ZenithX] Generated spoof friend code: {friendCode}");
                }
                else
                {
                    ZenithX.Log("[ZenithX] editAccountUsername or UsernameText was null, skipping username save.");
                }
            }
        }
        catch (Exception ex)
        {
            ZenithX.Log($"[ZenithX] AmongUsClient_Update.Postfix error: {ex}");
        }
    }
}

[HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
public static class PingTracker_Update
{
    // Postfix patch of PingTracker.Update to show mod name & ping
    public static void Postfix(PingTracker __instance)
    {
        __instance.text.alignment = TMPro.TextAlignmentOptions.Center;

        if (AmongUsClient.Instance.IsGameStarted)
        {

            __instance.aspectPosition.DistanceFromEdge = new Vector3(-0.21f, 0.50f, 0f);

            __instance.text.text = $"Zenith X by ReactorCoreDev - {Utils.getColoredPingText(AmongUsClient.Instance.Ping)}";

            return;
        }

        __instance.text.text = $"Zenith X by ReactorCoreDev\n{Utils.getColoredPingText(AmongUsClient.Instance.Ping)}";

    }
}

[HarmonyPatch(typeof(Mushroom), nameof(Mushroom.FixedUpdate))]
public static class Mushroom_FixedUpdate
{
    public static void Postfix(Mushroom __instance)
    {
        ZenithXESP.sporeCloudVision(__instance);
    }
}

[HarmonyPatch(typeof(Vent), nameof(Vent.CanUse))]
public static class Vent_CanUse
{
    // Prefix patch of Vent.CanUse to allow venting for cheaters
    // Basically does what the original method did with the required modifications
    public static void Postfix(Vent __instance, NetworkedPlayerInfo pc, ref bool canUse, ref bool couldUse, ref float __result)
    {
        if (!PlayerControl.LocalPlayer.Data.Role.CanVent && !PlayerControl.LocalPlayer.Data.IsDead)
        {
            if (CheatToggles.useVents)
            {
                float num = float.MaxValue;
                PlayerControl @object = pc.Object;

                Vector3 center = @object.Collider.bounds.center;
                Vector3 position = __instance.transform.position;
                num = Vector2.Distance(center, position);

                // Allow usage of vents unless the vent is too far or there are objects blocking the player's path
                canUse = num <= __instance.UsableDistance && !PhysicsHelpers.AnythingBetween(@object.Collider, center, position, Constants.ShipOnlyMask, false);
                couldUse = true;
                __result = num;
            }
        }
    }
}

[HarmonyPatch(typeof(PlayerPurchasesData), nameof(PlayerPurchasesData.GetPurchase))]
public class GetPurchasePatch
{
    public static void Postfix(ref bool __result)
    {
        if (CheatToggles.unlockFeatures)
        {
            __result = true;
        }
    }
}

// Stat disabler
//[HarmonyPatch(typeof(PlayerStatsData), nameof(PlayerStatsData.IncrementStat))]
//public static class IncreaseStatPatch
//{
//    public static bool Prefix(PlayerStatsData __instance, StatID stat)
//    {
//        if (CheatToggles.endlessVentTime && stat == StatID.HideAndSeek_TimesVented)
//        {
//           return false; // cancel the method entirely, prevents the vent stat from incrementing
//        }
//
//       return true; // allow IncrementStat to run for other stats
//    }
//}

// https://github.com/g0aty/SickoMenu/blob/main/hooks/LobbyBehaviour.cpp
[HarmonyPatch(typeof(GameContainer), nameof(GameContainer.SetupGameInfo))]
public static class MoreLobbyInfo_GameContainer_SetupGameInfo_Postfix
{
    public static void Postfix(GameContainer __instance)
    {
        if (!CheatToggles.moreLobbyInfo) return;

        var trueHostName = __instance.gameListing.TrueHostName;

        // The Crewmate icon gets aligned properly with this
        const string separator = "<#0000>000000000000000</color>";

        int gameId = __instance.gameListing.GameId;
        var age = __instance.gameListing.Age;
        var lobbyTime = $"Age: {age / 60}:{(age % 60 < 10 ? "0" : "")}{age % 60}";
        ZenithX.Log($"Lobby age: {age}");
        var platformId = __instance.gameListing.Platform switch
        {
            Platforms.StandaloneEpicPC => "Epic",
            Platforms.StandaloneSteamPC => "Steam",
            Platforms.StandaloneMac => "Mac",
            Platforms.StandaloneWin10 => "Microsoft Store",
            Platforms.StandaloneItch => "Itch.io",
            Platforms.IPhone => "iPhone / iPad",
            Platforms.Android => "Android",
            Platforms.Switch => "Nintendo Switch",
            Platforms.Xbox => "Xbox",
            Platforms.Playstation => "PlayStation",
            _ => "Unknown"
        };
        
        // Set the text of the capacity field to include the new information
        __instance.capacity.text = $"<size=40%>{separator}\n{trueHostName}\n{__instance.capacity.text}\n" +
                                   $"<#fb0>{GameCode.IntToGameName(__instance.gameListing.GameId)}</color>\n" +
                                   $"<#b0f>{platformId}</color>\n{lobbyTime}\n{separator}</size>";
    }
}

[HarmonyPatch(typeof(FindAGameManager), nameof(FindAGameManager.HandleList))]
public static class MoreLobbyInfo_FindAGameManager_HandleList_Postfix
{
    public static void Postfix(InnerNetClient.TotalGameData totalGames, HttpMatchmakerManager.FindGamesListFilteredResponse response, FindAGameManager __instance)
    {
        if (!CheatToggles.unlockFeatures) return;

        __instance.TotalText.text = response.Metadata.AllGamesCount.ToString();
    }
}

[HarmonyPatch(typeof(HudManager))]
[HarmonyPatch("SetHudActive", typeof(PlayerControl), typeof(RoleBehaviour), typeof(bool))]
public static class ShowTaskPanelInMeetings_HudManager_SetHudActive_Postfix
{
    public static void Postfix(HudManager __instance, RoleBehaviour role, bool isActive)
    {
        if (!CheatToggles.unlockFeatures) return;
        if (!MeetingHud.Instance) return;

        // Modify openPosition so the task panel appears on top of the meeting screen
        var openPosition = __instance.TaskPanel.openPosition;
        openPosition.z = -20f;
        __instance.TaskPanel.openPosition = openPosition;

        __instance.TaskPanel.gameObject.SetActive(true);
    }
}

public static class LobbyAgeTracker
{
    // Stores room code -> lobby start time in seconds (for dynamic timer)
    public static Dictionary<string, float> LobbyStartTimes = new Dictionary<string, float>();
}

[HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Start))]
public static class ShowLobbyTimer_GameStartManager_Start_Postfix
{
    public static void Postfix(GameStartManager __instance)
    {
        if (!CheatToggles.unlockFeatures) return;
        if (__instance == null) return;
        if (!GameData.Instance || !AmongUsClient.Instance || AmongUsClient.Instance.NetworkMode == NetworkModes.LocalGame) return;

        int maxLobbyTime = 600;
        int age = 0;
        
        if (LobbyAgeTracker.LobbyStartTimes.TryGetValue(Utils.RoomCode, out float startTime))
        {
           age = (int)(Time.time - startTime);
        }

        int remainingTime = Mathf.Max(maxLobbyTime - age, 0);
        HudManager.Instance.ShowLobbyTimer(remainingTime);
    }
}

[HarmonyPatch(typeof(GameData), nameof(GameData.RemovePlayer))]
public static class GameData_RemovePlayer_Patch
{
    private static readonly HashSet<byte> notifiedDisconnects = new();

    public static void ClearNotifiedDisconnects() => notifiedDisconnects.Clear();

    // Use a Prefix patch to capture the PlayerInfo *before* it gets removed from the game's data lists.
    public static void Prefix(GameData __instance, byte playerId)
    {
        // Only notify during an active game, not in lobby or post-game.
        if (CheatToggles.notifyOnDisconnect && Utils.isInGame)
        {
            // If we've already notified for this player, don't do it again.
            if (notifiedDisconnects.Contains(playerId))
            {
                return;
            }

            var player = __instance.GetPlayerById(playerId);
            // The check for `!player.Disconnected` was preventing this from ever firing. It's removed.
            if (player != null)
            {
                NotificationHandler.HandlePlayerDisconnect(player);
                // Add the player to the set so we don't notify again for this game session.
                notifiedDisconnects.Add(playerId);
            }
        }
    }
}

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
public static class AmongUsClient_OnGameEnd_Patch
{
    public static void Postfix()
    {
        // Clear the set of notified disconnected players when a game ends.
        GameData_RemovePlayer_Patch.ClearNotifiedDisconnects();

        // Clear the set of notified killed victims when a game ends.
        PlayerControl_MurderPlayer_Patch.ClearNotifiedKilledVictims();
    }
}