using Hazel;

namespace ZenithX;

internal class ExiledCheck : RpcCheck
{
	public override void Validate(PlayerControl player, MessageReader reader, ref bool blockRpc)
	{
		AntiCheat.Flag(player, CheatAction.InvalidRPC, player.Data.PlayerName + " sent invalid Exiled RPC");
		blockRpc = true;
	}

	public override RpcCalls GetRpcCall()
	{
		return (RpcCalls)4;
	}
}
