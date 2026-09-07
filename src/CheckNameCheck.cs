using Hazel;

namespace ZenithX;

internal class CheckNameCheck : RpcCheck
{
	private const int MAX_NAME_LENGTH = 10;

	public override void Validate(PlayerControl player, MessageReader reader, ref bool blockRpc)
	{
		string text = reader.ReadString();
		if (text.Length > 10)
		{
			blockRpc = true;
			AntiCheat.Flag(player, CheatAction.AbnormalName, $"{player.Data.PlayerName} name too long: {text.Length}");
		}
		if (text.Contains('<'))
		{
			blockRpc = true;
			AntiCheat.Flag(player, CheatAction.AbnormalName, player.Data.PlayerName + " name has invalid chars");
		}
	}

	public override RpcCalls GetRpcCall()
	{
		return (RpcCalls)5;
	}
}
