using System;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using UnityEngine;

namespace ZenithX;

public class TeleportManager
{
    private const float MAX_TELEPORT_DISTANCE = 50f;
    private const float MIN_TELEPORT_INTERVAL = 0.5f;

    private static readonly Dictionary<SystemTypes, Vector2> locations = new()
    {
        { SystemTypes.Electrical, new Vector2(-8.6f, -8.3f) },
        { SystemTypes.Security,   new Vector2(-13.3f, -5.9f) },
        { SystemTypes.MedBay,     new Vector2(-10.5f, -3.5f) },
        { SystemTypes.Storage,    new Vector2(-3.5f, -11.9f) },
        { SystemTypes.Cafeteria,  new Vector2(0f, 0f) },
        { SystemTypes.Admin,      new Vector2(3.5f, -7.5f) },
        { SystemTypes.Weapons,    new Vector2(4.5f, 4f) },
        { SystemTypes.Nav,        new Vector2(16.7f, -4.8f) },
        { SystemTypes.Shields,    new Vector2(9.3f, -11.3f) }
    };

    private static readonly Dictionary<byte, DateTime> lastPlayerTeleports = new();

    private static float lastTeleportTime;

    public IReadOnlyDictionary<SystemTypes, Vector2> Locations => locations;

    public static void TeleportToLocation(SystemTypes location)
    {
        if (!ValidateGameState() || !ValidateTeleportCooldown()) return;
        if (locations.TryGetValue(location, out Vector2 position))
        {
            ExecuteTeleport(position);
        }
    }

    public static void TeleportToPlayer(PlayerControl target)
    {
        if (!ValidateGameState() || !ValidateTeleportCooldown() || !ValidateTargetPlayer(target)) return;

        Vector2 targetPosition = target.GetTruePosition();
        if (Vector2.Distance(PlayerControl.LocalPlayer.GetTruePosition(), targetPosition) <= MAX_TELEPORT_DISTANCE)
        {
            ExecuteTeleport(targetPosition);
        }
    }

    public static void TeleportAllToMe()
    {
        if (!Utils.isHost) return;

        Vector2 myPosition = PlayerControl.LocalPlayer.GetTruePosition();

        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (ValidateTargetPlayer(player) && !IsPlayerTeleportOnCooldown(player.PlayerId))
            {
                SendSnapToRpc(player.PlayerId, myPosition);
                lastPlayerTeleports[player.PlayerId] = DateTime.UtcNow;
            }
        }
    }
    
    public PlayerControl GetClosestPlayer()
    {
    if (!ValidateGameState()) return null;

    Vector2 localPos = PlayerControl.LocalPlayer.GetTruePosition();
    return ((IEnumerable<PlayerControl>)PlayerControl.AllPlayerControls)
        .Where(p => ValidateTargetPlayer(p))
        .OrderBy(p => Vector2.Distance(p.GetTruePosition(), localPos))
        .FirstOrDefault();
    }

    private static bool ValidateGameState() =>
        PlayerControl.LocalPlayer != null &&
        AmongUsClient.Instance != null &&
        AmongUsClient.Instance.AmConnected;

    private static bool ValidateTeleportCooldown() =>
        Time.time - lastTeleportTime >= MIN_TELEPORT_INTERVAL;

    private static bool IsPlayerTeleportOnCooldown(byte playerId)
    {
        if (lastPlayerTeleports.TryGetValue(playerId, out DateTime lastTeleport))
            return (DateTime.UtcNow - lastTeleport).TotalSeconds < MIN_TELEPORT_INTERVAL;
        return false;
    }

    private static bool ValidateTargetPlayer(PlayerControl target) =>
        target != null &&
        target != PlayerControl.LocalPlayer &&
        !target.Data.IsDead &&
        !target.Data.Disconnected;

    private static void ExecuteTeleport(Vector2 position)
    {
        if (PlayerControl.LocalPlayer.inVent)
            PlayerControl.LocalPlayer.MyPhysics.ExitAllVents();

        PlayerControl.LocalPlayer.NetTransform.SnapTo(position);
        lastTeleportTime = Time.time;
    }

    private static void SendSnapToRpc(byte playerId, Vector2 position)
    {
        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(
            PlayerControl.LocalPlayer.NetId,
            (byte)RpcCalls.SnapTo,
            SendOption.Reliable,
            -1
        );

        writer.Write(playerId);
        writer.Write(position.x);
        writer.Write(position.y);
        writer.Write(Utils.generateToken());
        AmongUsClient.Instance.FinishRpcImmediately(writer);
    }
}