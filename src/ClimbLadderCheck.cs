using Hazel;

namespace ZenithX;

internal class ClimbLadderCheck : RpcCheck
{
	public override void Validate(PlayerControl player, MessageReader reader, ref bool blockRpc)
	{
	}

	public override RpcCalls GetRpcCall()
	{
		return (RpcCalls)31;
	}
}
