using Hazel;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using InnerNet;

namespace ZenithX;

internal class SetColorCheck : RpcCheck
{
	public override void Validate(PlayerControl player, MessageReader reader, ref bool blockRpc)
	{
		uint num = reader.ReadUInt32();
		byte b = reader.ReadByte();
		if (num != ((InnerNetObject)player.Data).NetId)
		{
			blockRpc = true;
			AntiCheat.Flag(player, CheatAction.AbnormalColor, player.Data.PlayerName + " SetColor bad net id");
		}
		if (b >= ((Il2CppArrayBase<StringNames>)(object)Palette.ColorNames).Length)
		{
			blockRpc = true;
			AntiCheat.Flag(player, CheatAction.AbnormalColor, $"{player.Data.PlayerName} invalid color {b}");
		}
		AntiCheat.CheckColorChangeSpam(player);
		if (blockRpc)
		{
			player.SetColor(0);
		}
	}

	public override RpcCalls GetRpcCall()
	{
		return (RpcCalls)8;
	}

	public override bool IsHostOnly()
	{
		return true;
	}
}
