using System.Diagnostics;
using BepInEx;
using UnityEngine;
using InnerNet;
using System.Linq;
using System.IO;
using Hazel;
using System.Reflection;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime;
using TMPro;
using System.Collections.Generic;
using Il2CppSystem.Collections.Generic;
using System;
using Sentry.Internal.Extensions;

namespace ZenithX;

public static class Utils
{
    //Useful for getting full lists of all the Among Us cosmetics IDs

    public static SabotageSystemType SabotageSystem => ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
    public static ReferenceDataManager referenceDataManager = DestroyableSingleton<ReferenceDataManager>.Instance;
    public static bool isAnySabotageActive => ShipStatus.Instance && SabotageSystem.AnyActive;
    public static bool isShip => ShipStatus.Instance;
    public static bool isLobby => AmongUsClient.Instance && AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Joined && !isFreePlay;
    public static bool isOnlineGame => AmongUsClient.Instance && AmongUsClient.Instance.NetworkMode == NetworkModes.OnlineGame;
    public static bool isLocalGame => AmongUsClient.Instance && AmongUsClient.Instance.NetworkMode == NetworkModes.LocalGame;
    public static bool isFreePlay => AmongUsClient.Instance && AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay;
    public static bool isPlayer => PlayerControl.LocalPlayer;
    public static bool isHost => AmongUsClient.Instance && AmongUsClient.Instance.AmHost;
    public static bool isInGame => AmongUsClient.Instance && AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started && isPlayer;
    public static bool isMeeting => MeetingHud.Instance;
    public static bool isMeetingVoting => isMeeting && MeetingHud.Instance.CurrentState is MeetingHud.VoteStates.Voted or MeetingHud.VoteStates.NotVoted;
    public static bool isMeetingProceeding => isMeeting && MeetingHud.Instance.CurrentState is MeetingHud.VoteStates.Proceeding;
    public static bool isExiling => ExileController.Instance && !(AirshipIsActive && SpawnInMinigame.Instance.isActiveAndEnabled);
    public static bool isNormalGame => GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.Normal;
    public static bool isHideNSeek => GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek;
    public static bool SkeldIsActive => (MapNames)GameOptionsManager.Instance.CurrentGameOptions.MapId == MapNames.Skeld;
    public static bool MiraHQIsActive => (MapNames)GameOptionsManager.Instance.CurrentGameOptions.MapId == MapNames.MiraHQ;
    public static bool PolusIsActive => (MapNames)GameOptionsManager.Instance.CurrentGameOptions.MapId == MapNames.Polus;
    public static bool DleksIsActive => (MapNames)GameOptionsManager.Instance.CurrentGameOptions.MapId == MapNames.Dleks;
    public static bool AirshipIsActive => (MapNames)GameOptionsManager.Instance.CurrentGameOptions.MapId == MapNames.Airship;
    public static bool FungleIsActive => (MapNames)GameOptionsManager.Instance.CurrentGameOptions.MapId == MapNames.Fungle;
    public static int GetImpNums => GameOptionsManager.Instance.CurrentGameOptions.NumImpostors;
    public static string RoomCode => isLobby ? UnityEngine.Object.FindObjectOfType<GameStartManager>().GameRoomNameCode.text : null;
    public static float MySpeed => PlayerControl.LocalPlayer.MyPhysics.Speed;
    public static float MyGhostSpeed => PlayerControl.LocalPlayer.MyPhysics.GhostSpeed;
    public static float MaxSpeed = 3f;
    public static float MaxGhostSpeed = MaxSpeed + 0.5f;
    public static float MinSpeed = 0.5f; 
    public static float MinGhostSpeed = MinSpeed + 0.5f;
    public static ClientData getClientByPlayer(PlayerControl player)
    {
        try
        {
            var allClientsField = typeof(AmongUsClient).GetField("allClients", BindingFlags.NonPublic | BindingFlags.Instance);
            var allClients = (System.Collections.Generic.IEnumerable<ClientData>)allClientsField.GetValue(AmongUsClient.Instance);

            var client = allClients.FirstOrDefault(cd => cd.Character.PlayerId == player.PlayerId);
            return client;
        }
        catch
        {
            return null;
        }
    }
    
    // Get ClientData.Id by PlayerControl
    public static int getClientIdByPlayer(PlayerControl player)
    {
        if (player == null) return -1;
        var client = getClientByPlayer(player);
        return client == null ? -1 : client.Id;
    }

    // Check if player is currently vanished
    public static bool isVanished(NetworkedPlayerInfo playerInfo)
    {
        PhantomRole phantomRole = playerInfo.Role as PhantomRole;

        if (phantomRole != null)
        {
            return phantomRole.fading || phantomRole.isInvisible;
        }

        return false;
    }

    // Custom isValidTarget method for cheats
    public static bool isValidTarget(NetworkedPlayerInfo target)
    {
        bool killAnyoneRequirements = !(target == null) && !target.Disconnected && target.Object.Visible && target.PlayerId != PlayerControl.LocalPlayer.PlayerId && !(target.Role == null) && !(target.Object == null);

        bool fullRequirements = killAnyoneRequirements && !target.IsDead && !target.Object.inVent && !target.Object.inMovingPlat && target.Role.CanBeKilled;

        if (CheatToggles.killAnyone)
        {
            return killAnyoneRequirements;
        }

        return fullRequirements;

    }

    // Adjusts HUD resolution
    // Used to fix UI problems when zooming out
    public static void adjustResolution()
    {
        ResolutionManager.ResolutionChanged.Invoke((float)Screen.width / Screen.height, Screen.width, Screen.height, Screen.fullScreen);
    }
    // Gets the room object from a Vector2 position.
    public static PlainShipRoom getRoomFromPosition(Vector2 position){
        if (ShipStatus.Instance == null) return null;

        foreach (var room in ShipStatus.Instance.AllRooms)
        {
            if (room != null && room.roomArea != null && room.roomArea.OverlapPoint(position)){
                return room;
            }
        }
        return null;
    }

    // Get RoleBehaviour from a RoleType
    public static RoleBehaviour getBehaviourByRoleType(RoleTypes roleType)
    {
        var allRoles = RoleManager.Instance.AllRoles; // keep as List<RoleBehaviour>
        for (int i = 0; i < allRoles.Count; i++)
        {
            if (allRoles[i].Role.Equals(roleType))
                return allRoles[i];
        }
        return null; // not found
    }

    // Kill any player using RPC calls
    public static void murderPlayer(PlayerControl target, MurderResultFlags result)
    {
        if (isFreePlay)
        {

            PlayerControl.LocalPlayer.MurderPlayer(target, result);
            return;
        }
        if (result == MurderResultFlags.Succeeded)
        {
            PlayerControl.LocalPlayer.RpcMurderPlayer(target, true);
        }
        //foreach (var item in PlayerControl.AllPlayerControls)
        //{
        //    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(
        //        PlayerControl.LocalPlayer.NetId,
        //        (byte)RpcCalls.MurderPlayer,
        //        SendOption.None,
        //        AmongUsClient.Instance.GetClientIdFromCharacter(item)
        //    );
        //    writer.WriteNetObject(target);
        //    writer.Write((int)result);
        //    AmongUsClient.Instance.FinishRpcImmediately(writer);
        //}
        // Removed for kicking the player
    }

    public static Il2CppSystem.Collections.Generic.List<NetworkedPlayerInfo> GetAllPlayerData()
    {
        var playerDataList = new Il2CppSystem.Collections.Generic.List<NetworkedPlayerInfo>();
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (player != null && player.Data != null)
            {
                playerDataList.Add(player.Data);
            }
        }

        return playerDataList;
    }

    public static void completeTask(PlayerTask task)
    {
        if (isFreePlay)
        {
            PlayerControl.LocalPlayer.RpcCompleteTask(task.Id);
            return;
        }

        var hostData = AmongUsClient.Instance.GetHost();
        if (hostData == null || hostData.Character.Data.Disconnected) return;

        if (task.IsComplete) return;
        foreach (var item in PlayerControl.AllPlayerControls)
        {
            var messageWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)RpcCalls.CompleteTask, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(item));
            messageWriter.WritePacked(task.Id);
            AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
        }
    }

    // Report bodies using RPC calls
    public static void reportDeadBody(NetworkedPlayerInfo playerData)
    {
        if (isFreePlay)
        {

            PlayerControl.LocalPlayer.CmdReportDeadBody(playerData);
            return;

        }

        var HostData = AmongUsClient.Instance.GetHost();
        if (HostData != null && !HostData.Character.Data.Disconnected)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(
                PlayerControl.LocalPlayer.NetId,
                (byte)RpcCalls.ReportDeadBody,
                SendOption.None,
                HostData.Id
            );
            writer.Write(playerData.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }
    }

    // Complete all of LocalPlayer's tasks using RPC calls
    public static void completeMyTasks()
    {
        if (isFreePlay)
        {
            foreach (PlayerTask task in PlayerControl.LocalPlayer.myTasks)
            {
                PlayerControl.LocalPlayer.RpcCompleteTask(task.Id);
            }
            return;
        }

        var HostData = AmongUsClient.Instance.GetHost();
        if (HostData != null && !HostData.Character.Data.Disconnected)
        {
            foreach (PlayerTask task in PlayerControl.LocalPlayer.myTasks)
            {
                if (!task.IsComplete)
                {
                    foreach (var item in PlayerControl.AllPlayerControls)
                    {
                        MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(
                            PlayerControl.LocalPlayer.NetId,
                            (byte)RpcCalls.CompleteTask,
                            SendOption.None,
                            AmongUsClient.Instance.GetClientIdFromCharacter(item)
                        );
                        messageWriter.WritePacked(task.Id);
                        AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
                    }
                }
            }
        }
    }

    // Open Chat UI
    public static void openChat()
    {
        if (!DestroyableSingleton<HudManager>.Instance.Chat.IsOpenOrOpening)
        {
            DestroyableSingleton<HudManager>.Instance.Chat.SetVisible(true);
            PlayerControl.LocalPlayer.NetTransform.Halt();
            if (DestroyableSingleton<FriendsListManager>.InstanceExists)
            {
                DestroyableSingleton<FriendsListManager>.Instance.SetFriendButtonColor(true);
            }
        }

    }

    // Draw a tracer line between two 2 GameObjects
    public static void drawTracer(GameObject sourceObject, GameObject targetObject, Color color)
    {
        LineRenderer lineRenderer = sourceObject.GetComponent<LineRenderer>();
        if (!lineRenderer)
        {
            lineRenderer = sourceObject.AddComponent<LineRenderer>();
        }

        // Set number of points
        lineRenderer.positionCount = 2;

        // Set width
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;

        // Set material
        lineRenderer.material = DestroyableSingleton<HatManager>.Instance.PlayerMaterial;

        // Set colors
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;

        // Set positions
        lineRenderer.SetPosition(0, sourceObject.transform.position);
        lineRenderer.SetPosition(1, targetObject.transform.position);
    }

    // Return if the ChatUI should be active or not
    public static bool chatUiActive()
    {
        try
        {
            return CheatToggles.alwaysChat || MeetingHud.Instance || !ShipStatus.Instance || PlayerControl.LocalPlayer.Data.IsDead;
        }
        catch
        {
            return false;
        }
    }

    // Close Chat UI
    public static void closeChat()
    {
        if (DestroyableSingleton<HudManager>.Instance.Chat.IsOpenOrOpening)
        {
            DestroyableSingleton<HudManager>.Instance.Chat.ForceClosed();
        }

    }

    // Get the distance between two players as a float
    public static float getDistanceFrom(PlayerControl target, PlayerControl source = null)
    {
        if (source != null)
        {
            source = PlayerControl.LocalPlayer;
        }

        Vector2 vector = target.GetTruePosition() - source.GetTruePosition();
        float magnitude = vector.magnitude;

        return magnitude;
    }

    // Returns a list of all the players in the game ordered from closest to farthest (from LocalPlayer by default)
    public static System.Collections.Generic.List<PlayerControl> getPlayersSortedByDistance(PlayerControl source = null)
    {

        if (source != null)
        {
            source = PlayerControl.LocalPlayer;
        }

        System.Collections.Generic.List<PlayerControl> outputList = [];

        outputList.Clear();

        Il2CppSystem.Collections.Generic.List<NetworkedPlayerInfo> allPlayers = GameData.Instance.AllPlayers;
        for (int i = 0; i < allPlayers.Count; i++)
        {
            PlayerControl player = allPlayers[i].Object;
            if (player)
            {
                outputList.Add(player);
            }
        }

        outputList = outputList.OrderBy(target => getDistanceFrom(target, source)).ToList();

        if (outputList.Count <= 0)
        {
            return null;
        }

        return outputList;
    }

    // Gets current map ID
    public static byte getCurrentMapID()
    {
        // If playing the tutorial
        if (isFreePlay)
        {
            return (byte)AmongUsClient.Instance.TutorialMapId;
        }
        else
        {
            // Works for local/online games
            return GameOptionsManager.Instance.CurrentGameOptions.MapId;
        }
    }

    // Get SystemType of the room the player is currently in
    public static SystemTypes getCurrentRoom()
    {
        return HudManager.Instance.roomTracker.LastRoom.RoomId;
    }

    // Fancy colored ping text
    public static string getColoredPingText(int ping)
    {
        if (ping <= 100)
        {
            // Green for ping < 100
            return $"<color=#00ff00ff>PING: {ping} ms</color>";
        }
        else if (ping < 400)
        {
            // Yellow for 100 < ping < 400
            return $"<color=#ffff00ff>PING: {ping} ms</color>";
        }
        else
        {
            // Red for ping > 400
            return $"<color=#ff0000ff>PING: {ping} ms</color>";
        }
    }

    // Get a UnityEngine.KeyCode from a string
    public static KeyCode stringToKeycode(string keyCodeStr)
    {
        if (!string.IsNullOrEmpty(keyCodeStr))
        {
            // Empty strings are automatically invalid
            try
            {

                // Case-insensitive parse of UnityEngine.KeyCode to check if string is validssss
                KeyCode keyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyCodeStr, true);

                return keyCode;

            }
            catch { }
        }

        return KeyCode.Delete; // If string is invalid, return Delete as the default key
    }

    // Get a platform type from a string
    public static bool stringToPlatformType(string platformStr, out Platforms? platform)
    {
        if (!string.IsNullOrEmpty(platformStr))
        {
            // Empty strings are automatically invalid
            try
            {
                // Case-insensitive parse of Platforms from string (if it valid)
                platform = (Platforms)System.Enum.Parse(typeof(Platforms), platformStr, true);

                return true; // If platform type is valid, return false
            }
            catch { }
        }

        platform = null;
        return false; // If platform type is invalid, return false
    }

    // Get the string name for a chosen player's role
    // String are automatically translated
    public static string getRoleName(NetworkedPlayerInfo playerData)
    {
        var translatedRole = DestroyableSingleton<TranslationController>.Instance.GetString(playerData.Role.StringName, Il2CppSystem.Array.Empty<Il2CppSystem.Object>());
        if (translatedRole == "STRMISS")
        {
            if (playerData.RoleWhenAlive.HasValue)
            {
                translatedRole = DestroyableSingleton<TranslationController>.Instance.GetString(getBehaviourByRoleType((RoleTypes)playerData.RoleWhenAlive.Value).StringName, Il2CppSystem.Array.Empty<Il2CppSystem.Object>());
            }
            else
            {
                translatedRole = "Ghost";
            }
        }
        return translatedRole;
    }

    public static string PlatformTypeToString(Platforms platform)
    {
        return platform switch
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
    }

    // Get the appropriate nametag for a player (seeRoles cheat)
    public static string getNameTag(NetworkedPlayerInfo playerInfo, string playerName, bool isChat = false)
    {
        var nameTag = playerName;

        if (playerInfo.Role.IsNull() || playerInfo.IsNull() || playerInfo.Disconnected ||
            playerInfo.Object.CurrentOutfit.IsNull()) return nameTag;

        var player = AmongUsClient.Instance.GetClientFromPlayerInfo(playerInfo);
        var host = AmongUsClient.Instance.GetHost();
        var level = playerInfo.PlayerLevel + 1;
        var platform = "Unknown";
        try { platform = PlatformTypeToString(player.PlatformData.Platform); } catch { }
        //var puid = player.ProductUserId;
        //var friendcode = player.FriendCode;
        var roleColor = ColorUtility.ToHtmlStringRGB(playerInfo.Role.TeamColor);

        var hostString = player == host ? "Host - " : "";

        if (CheatToggles.seeRoles)
        {

            if (CheatToggles.showPlayerInfo)
            {
                if (isChat)
                {
                    nameTag = $"<color=#{roleColor}>{nameTag} <size=70%>{getRoleName(playerInfo)}</size></color> <size=70%><color=#fb0>{hostString}Lv:{level} - {platform}</color></size>";
                    return nameTag;
                }

                nameTag =
                    $"<size=70%><color=#fb0>{hostString}Lv:{level} - {platform}</color></size>\r\n<color=#{roleColor}><size=70%>{getRoleName(playerInfo)}</size>\r\n{nameTag}</color>";
            }
            else
            {
                if (isChat)
                {
                    nameTag = $"<color=#{roleColor}>{nameTag} <size=70%>{getRoleName(playerInfo)}</size></color>";
                    return nameTag;
                }

                nameTag = $"<color=#{roleColor}><size=70%>{getRoleName(playerInfo)}</size>\r\n{nameTag}</color>";
            }
        }
        else
        {
            if (CheatToggles.showPlayerInfo)
            {
                if (PlayerControl.LocalPlayer.Data.Role.NameColor == playerInfo.Role.NameColor)
                {
                    if (isChat)
                    {
                        nameTag =
                            $"<color=#{ColorUtility.ToHtmlStringRGB(playerInfo.Role.NameColor)}>{nameTag}</color> <size=70%><color=#fb0>{hostString}Lv:{level} - {platform}</color></size>";
                        return nameTag;
                    }

                    nameTag =
                        $"<size=70%><color=#fb0>{hostString}Lv:{level} - {platform}</color></size>\r\n<color=#{ColorUtility.ToHtmlStringRGB(playerInfo.Role.NameColor)}>{nameTag}";
                }
                else
                {
                    if (isChat)
                    {
                        nameTag = $"{nameTag} <size=70%><color=#fb0>{hostString}Lv:{level} - {platform}</color></size>";
                        return nameTag;
                    }

                    nameTag = $"<size=70%><color=#fb0>{hostString}Lv:{level} - {platform}</color></size>\r\n{nameTag}";
                }
            }
            else
            {
                if (PlayerControl.LocalPlayer.Data.Role.NameColor != playerInfo.Role.NameColor || isChat)
                    return nameTag;

                nameTag = $"<color=#{ColorUtility.ToHtmlStringRGB(playerInfo.Role.NameColor)}>{nameTag}</color>";
            }
        }

        return nameTag;
    }

    // Show custom popup ingame
    // Found here: https://github.com/NuclearPowered/Reactor/blob/master/Reactor/Utilities/UI/ReactorPopup.cs
    public static void showPopup(string text)
    {
        var popup = UnityEngine.Object.Instantiate(DiscordManager.Instance.discordPopup, Camera.main!.transform);

        var background = popup.transform.Find("Background").GetComponent<SpriteRenderer>();
        var size = background.size;
        size.x *= 2.5f;
        background.size = size;

        popup.TextAreaTMP.fontSizeMin = 2;
        popup.Show(text);
    }

    // Load sprites and textures from manifest resources
    // Found here: https://github.com/Loonie-Toons/TOHE-Restored/blob/TOHE/Modules/Utils.cs
    public static System.Collections.Generic.Dictionary<string, Sprite> CachedSprites = new();
    public static Sprite LoadSprite(string path, float pixelsPerUnit = 1f)
    {
        try
        {
            if (CachedSprites.TryGetValue(path + pixelsPerUnit, out var sprite)) return sprite;

            Texture2D texture = LoadTextureFromResources(path);
            sprite = Sprite.Create(texture, new(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
            sprite.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontSaveInEditor;

            return CachedSprites[path + pixelsPerUnit] = sprite;
        }
        catch
        {
            UnityEngine.Debug.LogError($"Failed to read Texture: {path}");
        }
        return null;
    }
    public static Texture2D LoadTextureFromResources(string path)
    {
        try
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            var texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            using MemoryStream ms = new();

            stream.CopyTo(ms);
            ImageConversion.LoadImage(texture, ms.ToArray(), false);
            return texture;
        }
        catch
        {
            UnityEngine.Debug.LogError($"Failed to read Texture: {path}");
        }
        return null;
    }

    public static void OpenConfigFile()
    {
        string configFilePath = Path.Combine(Paths.ConfigPath, "ZenithX.cfg");
        if (File.Exists(configFilePath))
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = configFilePath,
                    UseShellExecute = true,
                    Verb = "edit"
                });
                return;
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError("Failed to open config file: " + ex.Message + ". If you are on Linux, this is expected.");
                return;
            }
        }
        UnityEngine.Debug.LogError("Config file does not exist.");
    }


// revive any player using RPC calls
public static void revivePlayer(PlayerControl target)
{
    if (target == null || !target.Data.IsDead) return;

        // HOST: Send custom revive RPC to ALL clients
        //MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(
            //target.NetId,
            //250,  // 250
            //SendOption.Reliable,
            //-1 // Broadcast to everyone
        //);
        //writer.Write(target.PlayerId); // Send playerId
        //AmongUsClient.Instance.FinishRpcImmediately(writer);
        target.Revive();
        target.Data.IsDead = false;
}
    private static readonly System.Random random = new();

    public static string generateToken()
    {
        byte[] tokenBytes = new byte[32];
        random.NextBytes(tokenBytes);
        return Convert.ToBase64String(tokenBytes);
    }
}