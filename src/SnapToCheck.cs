using System;
using Hazel;
using UnityEngine;

namespace ZenithX;

internal class SnapToCheck : RpcCheck
{
	public override void Validate(PlayerControl player, MessageReader reader, ref bool blockRpc)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		Vector2 newPosition = default(Vector2);
		((Vector2)(ref newPosition))._002Ector(reader.ReadSingle(), reader.ReadSingle());
		if ((Object)(object)LobbyBehaviour.Instance != (Object)null)
		{
			AntiCheat.Flag(player, CheatAction.Teleportation, player.Data.PlayerName + " SnapTo in lobby");
			blockRpc = true;
		}
		AntiCheat.CheckTeleportation(player, newPosition);
	}

	public override RpcCalls GetRpcCall()
	{
		return (RpcCalls)21;
	}

	public override Type GetExpectedNetObject()
	{
		return typeof(CustomNetworkTransform);
	}
}
