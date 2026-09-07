using Hazel;
using UnityEngine;

namespace ZenithX;

internal class PlayAnimationCheck : RpcCheck
{
	public override void Validate(PlayerControl player, MessageReader reader, ref bool blockRpc)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		reader.ReadByte();
		if ((Object)(object)LobbyBehaviour.Instance != (Object)null)
		{
			AntiCheat.Flag(player, CheatAction.AbnormalTasks, player.Data.PlayerName + " used PlayAnimation in lobby");
			blockRpc = true;
		}
		if (RoleManager.IsImpostorRole(player.Data.RoleType))
		{
			AntiCheat.Flag(player, CheatAction.AbnormalTasks, player.Data.PlayerName + " used PlayAnimation as impostor");
			blockRpc = true;
		}
		if (!GameManager.Instance.LogicOptions.GetVisualTasks())
		{
			AntiCheat.Flag(player, CheatAction.AbnormalTasks, player.Data.PlayerName + " used PlayAnimation with visual tasks disabled");
			blockRpc = true;
		}
	}

	public override RpcCalls GetRpcCall()
	{
		return (RpcCalls)0;
	}
}
