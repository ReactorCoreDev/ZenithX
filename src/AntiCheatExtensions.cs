using System;
using AmongUs.InnerNet.GameDataMessages;
using Hazel;
using InnerNet;
using UnityEngine;

namespace ZenithX;

public static class AntiCheatExtensions
{
	public static void HandleGameDataInner(InnerNetClient innerNetClient, MessageReader reader, int msgNum)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		GameDataTypes key = (GameDataTypes)reader.Tag;
		bool flag = true;
		if (AntiCheat.GameDataHandlers.TryGetValue(key, out var value) && AntiCheatConfig.Enabled && value.Enabled)
		{
			int position = reader.Position;
			bool blockMessage = false;
			value.Validate(reader, ref blockMessage);
			if (AntiCheatConfig.DiscardMaliciousRpc && blockMessage)
			{
				flag = false;
			}
			reader.Position = position;
		}
		if (!flag)
		{
			reader.Recycle();
		}
		else
		{
			((MonoBehaviour)innerNetClient).StartCoroutine(innerNetClient.HandleGameDataInner(reader, msgNum));
		}
	}

	public static bool HandleRpc(Type sourceNetObj, PlayerControl player, RpcCalls rpc, MessageReader reader)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected I4, but got Unknown
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		if (!AntiCheatConfig.Enabled)
		{
			return true;
		}
		byte b = (byte)(int)rpc;
		AntiCheat.CheckCheatClientRPC(player, b);
		if (AntiCheatConfig.DetectInvalidRpcs && !AntiCheat.IsTrustedRPC(b))
		{
			AntiCheat.Flag(player, CheatAction.InvalidRPC, $"{player.Data.PlayerName} sent unregistered RPC: {b}");
			return false;
		}
		if (!AntiCheat.CheckGameStateRPC(player, b))
		{
			return false;
		}
		if (!AntiCheat.RpcHandlers.TryGetValue(rpc, out var value) || !value.Enabled)
		{
			return true;
		}
		if (sourceNetObj != value.GetExpectedNetObject())
		{
			return false;
		}
		if ((Object)(object)player != (Object)null && ((InnerNetClient)AmongUsClient.Instance).AmHost && value.IsHostOnly())
		{
			AntiCheat.Flag(player, CheatAction.InvalidRPC, $"{player.Data.PlayerName} sent host-only RPC {rpc}");
			return false;
		}
		int position = reader.Position;
		bool blockRpc = false;
		value.Validate(player, reader, ref blockRpc);
		if (AntiCheatConfig.DiscardMaliciousRpc && blockRpc)
		{
			reader.Position = position;
			return false;
		}
		reader.Position = position;
		return true;
	}
}
