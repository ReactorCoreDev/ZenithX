using Hazel;
using InnerNet;

namespace ZenithX;

internal class SetStartCounterCheck : RpcCheck
{
	public override void Validate(PlayerControl player, MessageReader reader, ref bool blockRpc)
	{
		reader.ReadPackedInt32();
		sbyte b = reader.ReadSByte();
		if (((InnerNetObject)player).OwnerId != ((InnerNetClient)AmongUsClient.Instance).HostId && b != -1)
		{
			AntiCheat.Flag(player, CheatAction.InvalidRPC, $"{player.Data.PlayerName} invalid start counter {b}");
			blockRpc = true;
		}
	}

	public override RpcCalls GetRpcCall()
	{
		return (RpcCalls)18;
	}
}
