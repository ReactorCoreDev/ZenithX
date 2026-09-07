using Hazel;
using InnerNet;

namespace ZenithX;

internal class AddVoteCheck : RpcCheck
{
	public override void Validate(PlayerControl player, MessageReader reader, ref bool blockRpc)
	{
		int num = reader.ReadPackedInt32();
		int value = reader.ReadPackedInt32();
		if (((InnerNetClient)AmongUsClient.Instance).FindClientById(num) == null)
		{
			AntiCheat.Flag(player, CheatAction.VoteAbuse, $"Unknown client {num} voted for {value}");
			blockRpc = true;
		}
		AntiCheat.CheckVoteAbuse(player);
	}

	public override RpcCalls GetRpcCall()
	{
		return (RpcCalls)26;
	}
}
