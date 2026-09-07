using Hazel;

namespace ZenithX;

internal class ReportDeadBodyCheck : RpcCheck
{
	public override void Validate(PlayerControl player, MessageReader reader, ref bool blockRpc)
	{
		if (GameManager.Instance.IsHideAndSeek())
		{
			AntiCheat.Flag(player, CheatAction.RoleAbuse, player.Data.PlayerName + " reported in Hide and Seek");
			blockRpc = true;
		}
		AntiCheat.CheckReportAbuse(player);
	}

	public override RpcCalls GetRpcCall()
	{
		return (RpcCalls)11;
	}
}
