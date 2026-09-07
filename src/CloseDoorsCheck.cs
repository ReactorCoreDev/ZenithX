using System;
using Hazel;

namespace ZenithX;

internal class CloseDoorsCheck : RpcCheck
{
	public override void Validate(PlayerControl player, MessageReader reader, ref bool blockRpc)
	{
		if (GameManager.Instance.IsHideAndSeek())
		{
			AntiCheat.Flag("Door closure blocked in Hide and Seek");
			blockRpc = true;
		}
	}

	public override RpcCalls GetRpcCall()
	{
		return (RpcCalls)27;
	}

	public override Type GetExpectedNetObject()
	{
		return typeof(ShipStatus);
	}
}
