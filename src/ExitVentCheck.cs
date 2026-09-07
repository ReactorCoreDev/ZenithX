using System;
using Hazel;
using UnityEngine;

namespace ZenithX;

internal class ExitVentCheck : RpcCheck
{
	public override void Validate(PlayerControl player, MessageReader reader, ref bool blockRpc)
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)ShipStatus.Instance == (Object)null)
		{
			AntiCheat.Flag(player, CheatAction.Venting, player.Data.PlayerName + " exited vent with no ShipStatus");
			blockRpc = true;
		}
		else if (!player.Data.IsDead && !player.Data.Role.CanVent)
		{
			AntiCheat.Flag(player, CheatAction.Venting, player.Data.PlayerName + " exited vent without vent ability");
			blockRpc = true;
		}
		else if (GameManager.Instance.IsHideAndSeek() && RoleManager.IsImpostorRole(player.Data.RoleType))
		{
			AntiCheat.Flag(player, CheatAction.Venting, player.Data.PlayerName + " exited vent in Hide and Seek");
			blockRpc = true;
		}
	}

	public override RpcCalls GetRpcCall()
	{
		return (RpcCalls)20;
	}

	public override Type GetExpectedNetObject()
	{
		return typeof(PlayerPhysics);
	}
}
