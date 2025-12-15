using System;
using System.Linq;
using AmongUs.GameOptions;
using HarmonyLib;
using Hazel;
using UnityEngine;
using Il2CppSystem.Collections.Generic;

namespace ZenithX;

public enum MessageType : byte
{
    Normal = 0,
    Command = 1,
    System = 2,
    Private = 3,
    Broadcast = 4
}

public class CustomMessage
{
    public byte Tag { get; set; }
    public int SenderId { get; set; }
    public string SenderName { get; set; }
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
    public MessageType Type { get; set; }

    public CustomMessage(byte tag, int senderId, string senderName, string content, MessageType type)
    {
        Tag = tag;
        SenderId = senderId;
        SenderName = senderName;
        Content = content;
        Timestamp = DateTime.UtcNow;
        Type = type;
    }

    public void Serialize(MessageWriter writer)
    {
        writer.Write(Tag);
        writer.WritePacked(SenderId);
        writer.Write(SenderName ?? "");
        writer.Write(Content ?? "");
        writer.Write(Timestamp.ToBinary());
        writer.Write((byte)Type);
    }

    public static CustomMessage Deserialize(MessageReader reader)
    {
        var message = new CustomMessage(
            reader.ReadByte(),
            reader.ReadPackedInt32(),
            reader.ReadString(),
            reader.ReadString(),
            (MessageType)reader.ReadByte()
        );
        message.Timestamp = DateTime.FromBinary((long)reader.ReadUInt64());
        return message;
    }

    public void SendBypass()
    {
        if (AmongUsClient.Instance == null || !AmongUsClient.Instance.AmConnected) return;
        if (PlayerControl.LocalPlayer == null) return;

        var writer = AmongUsClient.Instance.StartRpcImmediately(
            PlayerControl.LocalPlayer.NetId,
            (byte)CustomRpcCalls.BroadcastMessage,
            SendOption.Reliable,
            -1
        );

        Serialize(writer);
        AmongUsClient.Instance.FinishRpcImmediately(writer);
    }

    public static void HandleBypass(MessageReader reader)
    {
        try
        {
            var message = Deserialize(reader);
            if (HudManager.Instance?.Chat != null)
            {
                var player = PlayerControl.AllPlayerControls.ToArray()
                    .FirstOrDefault(p => p.PlayerId == message.SenderId);

                if (player != null)
                {
                    HudManager.Instance.Chat.AddChat(player, message.Content);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[CustomMessage] Failed to handle message: {e.Message}");
        }
    }

    public static void SendMessageToAll(string messageContent)
    {
        if (PlayerControl.LocalPlayer == null) return;

        var message = new CustomMessage(
            0,
            PlayerControl.LocalPlayer.PlayerId,
            PlayerControl.LocalPlayer.Data.PlayerName,
            messageContent,
            MessageType.Broadcast
        );

        message.SendBypass();
    }

    public static void SendPrivateMessage(string messageContent, PlayerControl targetPlayer)
    {
        if (PlayerControl.LocalPlayer == null || targetPlayer == null) return;

        var message = new CustomMessage(
            0,
            PlayerControl.LocalPlayer.PlayerId,
            PlayerControl.LocalPlayer.Data.PlayerName,
            messageContent,
            MessageType.Private
        );

        message.SendBypass();
    }
}

public enum CustomRpcCalls : byte
{
    BroadcastMessage = 201
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
public static class RpcPatch
{
    public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] Hazel.MessageReader reader)
    {
        if (callId == (byte)CustomRpcCalls.BroadcastMessage)
        {
            CustomMessage.HandleBypass(reader);
        }

        if (callId == (byte)RpcCalls.SetRole && AmongUsClient.Instance != null && !AmongUsClient.Instance.AmHost)
        {
            byte playerId = reader.ReadByte();
            byte roleId = reader.ReadByte();
            if (roleId == (byte)RoleTypes.Impostor)
            {
                var targetPlayer = PlayerControl.AllPlayerControls
                    .ToArray()
                    .FirstOrDefault(p => p.PlayerId == playerId);

                targetPlayer?.RpcSetRole(RoleTypes.Impostor);
            }
        }
    }
}