using System;
using System.Collections.Generic;
using InnerNet;
using UnityEngine;
using Hazel;
using HarmonyLib;
using AmongUs.GameOptions;

namespace ZenithX;

public enum CheatActions
{
    Unknown,
    Sabotage,
    Venting,
    SpeedHack,
    AbnormalName,
    AbnormalColor,
    AbnormalCosmetics,
    AbnormalScan,
    AbnormalTasks,
    AbnormalShapeshift,
    AbnormalVanish,
    TaskCompletionSpam,
    SickoMenu,
    AmongUsMenu,
    BetterAmongUs,
    KillNetwork,
    HostGuard,
    GoatNetClient,
    SickoSpam,
    Other
}

public static class State
{
    // Track detected mod/cheat action per player id
    public static Dictionary<byte, CheatActions> ModUsers = new Dictionary<byte, CheatActions>();

    // SMAC flags
    public static bool Enable_SMAC = true;
    public static bool SMAC_CheckSicko = true;
    public static bool SMAC_CheckAUM = true;
    public static bool SMAC_CheckBadNames = true;
    public static bool SMAC_CheckColor = true;
    public static bool SMAC_CheckCosmetics = true;
    public static bool SMAC_CheckScanner = true;
    public static bool SMAC_CheckTasks = true;
    public static bool SMAC_CheckShapeshift = true;
    public static bool SMAC_CheckVanish = true;
    public static bool SMAC_CheckVent = true;
    public static bool SMAC_CheckTaskCompletion = true;

    // misc flags
    public static bool SafeMode = false;
    public static bool PanicMode = false;
    public static bool ReadAndSendSickoChat = true;
    public static bool IsProcessingSickoChat = false;
    public static bool InMeeting = false;
    public static bool GameLoaded = false;

    public static class Settings
    {
        public enum MapType { Skeld, MiraHQ, Polus, Airship, Fungle }
    }
    public static Settings.MapType mapType = Settings.MapType.Skeld;

    // RPC101 / SickoChat rate-limit tracking
    public static DateTime lastRpc101WindowStart = DateTime.Now;
    public static int rpc101Counter = 0;
    public static readonly int RPC101_LIMIT = 10;
    public static Dictionary<int, DateTime> Rpc101OverloadTimestamps = new Dictionary<int, DateTime>();

    // Task completion spam detection
    public static Dictionary<byte, DateTime> lastTaskCompletionTime = new Dictionary<byte, DateTime>();
    public static Dictionary<byte, int> taskCompletionCounter = new Dictionary<byte, int>();
    public static readonly float TASK_DETECTION_WINDOW = 1.0f;
    public static readonly int MAX_TASK_COMPLETIONS = 5;

    // Speed detection baseline (set when local player's physics is available)
    public static float NormalSpeed = 1.5f;
    public static float NormalGhostSpeed = 1.8f;
}

public class CheatDetectedEvent
{
    public PlayerControl Source { get; }
    public CheatActions Action { get; }
    public DateTime Timestamp { get; }

    public CheatDetectedEvent(PlayerControl source, CheatActions action)
    {
        Source = source;
        Action = action;
        Timestamp = DateTime.UtcNow;
    }

    private static string GetActionName(CheatActions action)
    {
        return action switch
        {
            CheatActions.SpeedHack => "Speed Hacking",
            CheatActions.Venting => "Venting",
            CheatActions.Sabotage => "Sabotaging",
            CheatActions.AbnormalName => "Abnormal Name",
            CheatActions.AbnormalColor => "Abnormal Color",
            CheatActions.AbnormalCosmetics => "Abnormal Cosmetics",
            CheatActions.AbnormalScan => "Abnormal MedBay Scan",
            CheatActions.AbnormalTasks => "Abnormal Tasks",
            CheatActions.AbnormalShapeshift => "Abnormal Shapeshift",
            CheatActions.AbnormalVanish => "Abnormal Vanish/Appear",
            CheatActions.TaskCompletionSpam => "Task Completion Spam",
            CheatActions.SickoMenu => "SickoMenu Detected",
            CheatActions.AmongUsMenu => "AmongUsMenu Detected",
            CheatActions.BetterAmongUs => "BetterAmongUs Detected",
            CheatActions.KillNetwork => "KillNetwork Detected",
            CheatActions.HostGuard => "HostGuard Detected",
            CheatActions.GoatNetClient => "GoatNetClient Detected",
            CheatActions.SickoSpam => "SickoChat Spam",
            _ => "Unknown Action"
        };
    }

    public void Output()
    {
        if (HudManager.Instance == null || Source == null) return;
        string playerName = Source.Data?.PlayerName ?? "Unknown";
        string actionName = GetActionName(Action);
        HudManager.Instance.Notifier?.AddDisconnectMessage($"Player: {playerName} Action: {actionName}");
    }
}

public struct EventPlayer
{
    public string PlayerName;
    public string ColorName;
    public bool IsProtected;
}

public class KillEvent
{
    private readonly EventPlayer Source;
    private readonly EventPlayer Target;
    private readonly Vector2 Position;
    private readonly Vector2 TargetPosition;
    private readonly DateTime Timestamp;

    public KillEvent(EventPlayer source, EventPlayer target, Vector2 position, Vector2 targetPosition)
    {
        Source = source;
        Target = target;
        Position = position;
        TargetPosition = targetPosition;
        Timestamp = DateTime.UtcNow;
    }

    public void Output()
    {
        var notifier = HudManager.Instance?.Notifier;
        if (notifier == null) return;
        string src = !string.IsNullOrEmpty(Source.PlayerName) ? Source.PlayerName : "Unknown";
        string tgt = !string.IsNullOrEmpty(Target.PlayerName) ? Target.PlayerName : "Unknown";
        string sys = $"({TranslatePosition(Position)})";
        string prot = Target.IsProtected ? " [Protected]" : "";
        notifier.AddDisconnectMessage($"Kill: {src} > {tgt} {sys}{prot}");
    }

    private static string TranslatePosition(Vector2 pos) => $"x:{pos.x:0.0},y:{pos.y:0.0}";
}

public static class ProtectionManager
{
    private static readonly Dictionary<byte, Color> OriginalColors = new Dictionary<byte, Color>();

    // A player is considered protected if ModUsers contains them (detected cheater)
    public static bool IsProtected(byte playerId) => State.ModUsers.ContainsKey(playerId);

    public static void SetProtected(byte playerId, bool protect)
    {
        var p = GetPlayerById(playerId);
        if (p == null) return;
        var sr = p.GetComponent<SpriteRenderer>();
        if (sr == null) return;

        if (protect)
        {
            if (!OriginalColors.ContainsKey(playerId)) OriginalColors[playerId] = sr.color;
            sr.color = Color.cyan;
        }
        else
        {
            if (OriginalColors.TryGetValue(playerId, out var orig)) sr.color = orig;
            OriginalColors.Remove(playerId);
        }
    }

    private static PlayerControl GetPlayerById(byte id)
    {
        foreach (var pl in PlayerControl.AllPlayerControls)
            if (pl != null && pl.PlayerId == id) return pl;
        return null;
    }
}

public static class CustomRpcHandler
{
    // Centralized cheat-reporting: record action + notify + protect
    public static void SMAC_OnCheatDetected(PlayerControl player, CheatActions action)
    {
        if (player == null) return;
        byte pid = player.PlayerId;
        State.ModUsers[pid] = action;
        ProtectionManager.SetProtected(pid, true);

        var evt = new CheatDetectedEvent(player, action);
        evt.Output();
    }

    // Handle a few known RPCs (sets ModUsers appropriately)
    public static void HandleRpc(PlayerControl player, byte callId, MessageReader reader)
    {
        if (player == null) return;
        switch (callId)
        {
            // custom mod RPCs known to indicate mods
            case 200: // SickoMenu-ish
                if (State.SMAC_CheckSicko && reader.BytesRemaining == 0)
                    SMAC_OnCheatDetected(player, CheatActions.SickoMenu);
                break;
            case 201: // AmongUsMenu-ish
                if (State.SMAC_CheckAUM)
                    SMAC_OnCheatDetected(player, CheatActions.AmongUsMenu);
                break;
            case 150: // BetterAmongUs
                SMAC_OnCheatDetected(player, CheatActions.BetterAmongUs);
                break;
            case 250: // KillNetwork
                SMAC_OnCheatDetected(player, CheatActions.KillNetwork);
                break;
            case 176: // HostGuard
                SMAC_OnCheatDetected(player, CheatActions.HostGuard);
                break;
            case 202: // GoatNetClient
                SMAC_OnCheatDetected(player, CheatActions.GoatNetClient);
                break;
            case 101:
                // SickoChat / custom chat - rate-limit detection
                HandleRpc101(player, reader);
                break;
        }
    }

    private static void HandleRpc101(PlayerControl player, MessageReader reader)
    {
        var now = DateTime.Now;
        if ((now - State.lastRpc101WindowStart).TotalSeconds >= 1)
        {
            State.lastRpc101WindowStart = now;
            State.rpc101Counter = 0;
        }

        if (++State.rpc101Counter > State.RPC101_LIMIT)
        {
            if (player != null && player != Game.pLocalPlayer)
            {
                int playerId = player.PlayerId;
                if (!State.Rpc101OverloadTimestamps.ContainsKey(playerId) ||
                    (now - State.Rpc101OverloadTimestamps[playerId]).TotalSeconds >= 3)
                {
                    State.Rpc101OverloadTimestamps[playerId] = now;
                    SMAC_OnCheatDetected(player, CheatActions.SickoSpam);
                }
            }
            return;
        }

        // Optionally relay the SickoChat content to local chat (original behavior)
        if (reader.BytesRemaining > 0 && State.ReadAndSendSickoChat)
        {
            try
            {
                string playerName = reader.ReadString();
                string message = reader.ReadString();
                // uint colorId = reader.ReadUInt32(); // not used here
                if (!string.IsNullOrEmpty(message) && !State.PanicMode)
                {
                    HudManager.Instance?.Chat?.AddChat(player, message);
                }
            }
            catch { }
        }
    }

    // SMAC-like checks for normal RpcCalls (SetName, CheckColor, etc.)
    public static void SMAC_HandleRpc(PlayerControl player, byte callId, MessageReader reader)
    {
        if (!State.Enable_SMAC || player == Game.pLocalPlayer) return;
        var pData = Game.GetPlayerData(player);

        switch (callId)
        {
            case (byte)RpcCalls.CheckName:
            case (byte)RpcCalls.SetName:
                if (State.SMAC_CheckBadNames && Game.IsInGame())
                {
                    // basic validation read (guarded)
                    try
                    {
                        string name = reader.ReadString();
                        if (string.IsNullOrEmpty(name)) return;
                        if (name != Utility.RemoveHtmlTags(name) || !Utility.IsNameValid(name))
                        {
                            SMAC_OnCheatDetected(player, CheatActions.AbnormalName);
                        }
                    }
                    catch { }
                }
                break;

            case (byte)RpcCalls.CheckColor:
                if (State.SMAC_CheckColor && Game.IsInGame())
                    SMAC_OnCheatDetected(player, CheatActions.AbnormalColor);
                break;

            case (byte)RpcCalls.SetHat_Deprecated:
            case (byte)RpcCalls.SetHatStr:
            case (byte)RpcCalls.SetVisor_Deprecated:
            case (byte)RpcCalls.SetVisorStr:
            case (byte)RpcCalls.SetSkin_Deprecated:
            case (byte)RpcCalls.SetSkinStr:
            case (byte)RpcCalls.SetPet_Deprecated:
            case (byte)RpcCalls.SetPetStr:
            case (byte)RpcCalls.SetNamePlate_Deprecated:
            case (byte)RpcCalls.SetNamePlateStr:
                if (State.SMAC_CheckCosmetics && Game.IsInGame())
                    SMAC_OnCheatDetected(player, CheatActions.AbnormalCosmetics);
                break;

            case (byte)RpcCalls.SetScanner:
                if (State.SMAC_CheckScanner &&
                    (Game.IsInLobby() || State.mapType == State.Settings.MapType.Airship || State.mapType == State.Settings.MapType.Fungle))
                {
                    // reader.ReadBoolean() would indicate scan result; if suspicious mark it
                    try
                    {
                        bool scanned = reader.ReadBoolean();
                        if (!scanned) SMAC_OnCheatDetected(player, CheatActions.AbnormalScan);
                    }
                    catch { SMAC_OnCheatDetected(player, CheatActions.AbnormalScan); }
                }
                break;

            case (byte)RpcCalls.SetTasks:
                if (State.SMAC_CheckTasks && (Game.IsInLobby() || State.InMeeting))
                    SMAC_OnCheatDetected(player, CheatActions.AbnormalTasks);
                break;

            case (byte)RpcCalls.Shapeshift:
            case (byte)RpcCalls.CheckShapeshift:
            case (byte)RpcCalls.RejectShapeshift:
                if (State.SMAC_CheckShapeshift &&
                    (Game.IsInLobby() || (pData != null && !pData.RoleType.Equals(RoleTypes.Shapeshifter))))
                    SMAC_OnCheatDetected(player, CheatActions.AbnormalShapeshift);
                break;

            case (byte)RpcCalls.StartVanish:
            case (byte)RpcCalls.StartAppear:
            case (byte)RpcCalls.CheckVanish:
            case (byte)RpcCalls.CheckAppear:
                if (State.SMAC_CheckVanish &&
                    (Game.IsInLobby() || (pData != null && !pData.RoleType.Equals(RoleTypes.Phantom))))
                    SMAC_OnCheatDetected(player, CheatActions.AbnormalVanish);
                break;

            case (byte)RpcCalls.EnterVent:
                if (State.SMAC_CheckVent &&
                    (Game.IsInLobby() || (pData != null && pData.IsDead) ||
                    !(Game.PlayerIsImpostor(pData) || (pData != null && pData.RoleType.Equals(RoleTypes.Engineer)))))
                    SMAC_OnCheatDetected(player, CheatActions.Venting);
                break;

            case (byte)RpcCalls.CompleteTask:
                if (State.SMAC_CheckTaskCompletion && Game.IsInGame())
                {
                    byte playerId = player.PlayerId;
                    var now = DateTime.Now;
                    if (State.lastTaskCompletionTime.ContainsKey(playerId))
                    {
                        float elapsedTime = (float)(now - State.lastTaskCompletionTime[playerId]).TotalSeconds;
                        if (elapsedTime < State.TASK_DETECTION_WINDOW)
                        {
                            State.taskCompletionCounter[playerId]++;
                            if (State.taskCompletionCounter[playerId] > State.MAX_TASK_COMPLETIONS)
                            {
                                SMAC_OnCheatDetected(player, CheatActions.TaskCompletionSpam);
                                return;
                            }
                        }
                        else State.taskCompletionCounter[playerId] = 1;
                    }
                    else State.taskCompletionCounter[playerId] = 1;

                    State.lastTaskCompletionTime[playerId] = now;
                }
                break;
        }
    }

    public static bool SMAC_HandleUpdateSystem(PlayerControl player, SystemTypes sysType, byte reader) => false;
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
public static class PlayerControl_HandleRpcPatch
{
    // Prefix ensures we inspect incoming RPCs before game logic (non-intrusive)
    public static void Prefix(PlayerControl __instance, byte callId, MessageReader reader)
    {
        try
        {
            CustomRpcHandler.HandleRpc(__instance, callId, reader);
            CustomRpcHandler.SMAC_HandleRpc(__instance, callId, reader);
        }
        catch { }
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
public static class PlayerControl_MurderPatch
{
    public static void Postfix(PlayerControl __instance, PlayerControl target)
    {
        try
        {
            if (__instance == null || target == null) return;
            var src = new EventPlayer
            {
                PlayerName = __instance.Data?.PlayerName ?? "Unknown",
                ColorName = __instance.Data?.ColorName ?? "white",
                IsProtected = false
            };
            var tgt = new EventPlayer
            {
                PlayerName = target.Data?.PlayerName ?? "Unknown",
                ColorName = target.Data?.ColorName ?? "white",
                IsProtected = ProtectionManager.IsProtected(target.PlayerId)
            };
            var evt = new KillEvent(src, tgt, __instance.transform.position, target.transform.position);
            evt.Output();

            if (tgt.IsProtected)
            {
                // report the killer because they killed a protected player
                CustomRpcHandler.SMAC_OnCheatDetected(__instance, CheatActions.Other);
            }
        }
        catch { }
    }
}

public class ZenithX_AntiCheatBehaviour : MonoBehaviour
{
    private float _tick;
    private HashSet<byte> highlighted = new HashSet<byte>();

    private void Update()
    {
        _tick += Time.deltaTime;
        if (_tick < 0.25f) return;
        _tick = 0f;

        try
        {
            // Initialize normal speeds once when local player's physics is available
            if (!State.GameLoaded && PlayerControl.LocalPlayer != null && PlayerControl.LocalPlayer.MyPhysics != null)
            {
                State.NormalSpeed = PlayerControl.LocalPlayer.MyPhysics.Speed;
                State.NormalGhostSpeed = PlayerControl.LocalPlayer.MyPhysics.GhostSpeed;
                State.GameLoaded = true;
            }
            // Iterate safely over players (IL2CPP-friendly)
            foreach (var pl in PlayerControl.AllPlayerControls)
            {
                if (pl == null) continue;
                byte id = pl.PlayerId;

                // If this player has been detected previously, ensure protection and show message once
                if (State.ModUsers.ContainsKey(id) && !highlighted.Contains(id))
                {
                    ProtectionManager.SetProtected(id, true);
                    highlighted.Add(id);

                    var action = State.ModUsers[id];
                    HudManager.Instance?.Notifier?.AddDisconnectMessage(
                        $"Player: {pl.Data?.PlayerName ?? "Unknown"} Action: {GetActionString(action)}"
                    );
                }

                // If they no longer appear in ModUsers but are highlighted, remove highlighting
                if (!State.ModUsers.ContainsKey(id) && highlighted.Contains(id))
                {
                    ProtectionManager.SetProtected(id, false);
                    highlighted.Remove(id);
                }

                // Speed hack detection (compare physics values)
                if (pl.MyPhysics != null)
                {
                    bool speedMismatch = Math.Abs(pl.MyPhysics.Speed - State.NormalSpeed) > 0.01f;
                    bool ghostMismatch = Math.Abs(pl.MyPhysics.GhostSpeed - State.NormalGhostSpeed) > 0.01f;
                    if ((speedMismatch || ghostMismatch) && !State.ModUsers.ContainsKey(id))
                    {
                        State.ModUsers[id] = CheatActions.SpeedHack;
                        ProtectionManager.SetProtected(id, true);
                        highlighted.Add(id);

                        HudManager.Instance?.Notifier?.AddDisconnectMessage(
                            $"Player: {pl.Data?.PlayerName ?? "Unknown"} Action: {GetActionString(CheatActions.SpeedHack)}"
                        );
                    }
                }
            }
        }
        catch { }
    }

    private static string GetActionString(CheatActions action)
    {
        return action switch
        {
            CheatActions.SpeedHack => "Speed Hacking",
            CheatActions.Venting => "Venting",
            CheatActions.SickoMenu => "SickoMenu Detected",
            CheatActions.AmongUsMenu => "AmongUsMenu Detected",
            CheatActions.BetterAmongUs => "BetterAmongUs Detected",
            CheatActions.KillNetwork => "KillNetwork Detected",
            CheatActions.HostGuard => "HostGuard Detected",
            CheatActions.GoatNetClient => "GoatNetClient Detected",
            CheatActions.SickoSpam => "SickoChat Spam",
            CheatActions.AbnormalName => "Abnormal Name",
            CheatActions.AbnormalColor => "Abnormal Color",
            CheatActions.AbnormalCosmetics => "Abnormal Cosmetics",
            CheatActions.AbnormalScan => "Abnormal MedBay Scan",
            CheatActions.AbnormalTasks => "Abnormal Set Tasks",
            CheatActions.AbnormalShapeshift => "Abnormal Shapeshift",
            CheatActions.AbnormalVanish => "Abnormal Vanish/Appear",
            CheatActions.TaskCompletionSpam => "Task Completion Spam",
            _ => "Unknown Action"
        };
    }
}