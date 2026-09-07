using Hazel;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace ZenithX;

internal class SetScannerCheck : RpcCheck
{
	public override void Validate(PlayerControl player, MessageReader reader, ref bool blockRpc)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		bool flag = reader.ReadBoolean();
		if ((Object)(object)ShipStatus.Instance == (Object)null && flag)
		{
			AntiCheat.Flag(player, CheatAction.AbnormalScan, player.Data.PlayerName + " scanned before map spawn");
			blockRpc = true;
		}
		if (RoleManager.IsImpostorRole(player.Data.RoleType) && flag)
		{
			AntiCheat.Flag(player, CheatAction.AbnormalScan, player.Data.PlayerName + " impostor scanned");
			blockRpc = true;
		}
		if (!GameManager.Instance.LogicOptions.GetVisualTasks())
		{
			AntiCheat.Flag(player, CheatAction.AbnormalScan, player.Data.PlayerName + " scanned with visual tasks disabled");
			blockRpc = true;
		}
		bool flag2 = false;
		Enumerator<TaskInfo> enumerator = player.Data.Tasks.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (enumerator.Current.Id == 0)
			{
				flag2 = true;
				break;
			}
		}
		if (!flag2 && flag)
		{
			AntiCheat.Flag(player, CheatAction.AbnormalScan, player.Data.PlayerName + " scanned without scan task");
			blockRpc = true;
		}
	}

	public override RpcCalls GetRpcCall()
	{
		return (RpcCalls)15;
	}
}
