using Hazel;
using InnerNet;

namespace ZenithX;

internal class SetNameCheck : RpcCheck
{
	private const int MAX_NAME_LENGTH = 12;

	public override void Validate(PlayerControl player, MessageReader reader, ref bool blockRpc)
	{
		uint num = reader.ReadUInt32();
		string text = reader.ReadString();
		if (num != ((InnerNetObject)player.Data).NetId)
		{
			blockRpc = true;
			AntiCheat.Flag(player, CheatAction.NameSpoofing, player.Data.PlayerName + " SetName bad net id");
		}
		if (text.Length > 12)
		{
			blockRpc = true;
			AntiCheat.Flag(player, CheatAction.AbnormalName, player.Data.PlayerName + " name too long");
		}
		if (text.Contains('<'))
		{
			blockRpc = true;
			AntiCheat.Flag(player, CheatAction.AbnormalName, player.Data.PlayerName + " name has invalid chars");
		}
	}

	public override RpcCalls GetRpcCall()
	{
		return (RpcCalls)6;
	}
}
