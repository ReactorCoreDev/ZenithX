using Hazel;
using UnityEngine;

namespace ZenithX;

internal class CompleteTaskCheck : RpcCheck
{
	public override void Validate(PlayerControl player, MessageReader reader, ref bool blockRpc)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		uint num = reader.ReadPackedUInt32();
		if ((Object)(object)ShipStatus.Instance == (Object)null)
		{
			AntiCheat.Flag(player, CheatAction.AbnormalTasks, player.Data.PlayerName + " completed task with no ShipStatus");
			blockRpc = true;
		}
		if (RoleManager.IsImpostorRole(player.Data.RoleType))
		{
			AntiCheat.Flag(player, CheatAction.AbnormalTasks, player.Data.PlayerName + " completed task as impostor");
			blockRpc = true;
		}
		if (num + 1 > player.Data.Tasks.Count)
		{
			AntiCheat.Flag(player, CheatAction.AbnormalTasks, player.Data.PlayerName + " completed nonexistent task");
			blockRpc = true;
		}
	}

	public override RpcCalls GetRpcCall()
	{
		return (RpcCalls)1;
	}
}
