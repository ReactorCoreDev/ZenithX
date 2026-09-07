using AmongUs.InnerNet.GameDataMessages;
using Hazel;
using InnerNet;

namespace ZenithX;

internal class ClientReadyCheck : GameDataCheck
{
	public override void Validate(MessageReader reader, ref bool blockMessage)
	{
		int num = reader.ReadPackedInt32();
		ClientData val = ((InnerNetClient)AmongUsClient.Instance).FindClientById(num);
		if (val == null)
		{
			AntiCheat.Flag($"Unknown ClientReady from client {num}");
			blockMessage = true;
		}
		else if (val.IsReady)
		{
			AntiCheat.Flag("Duplicate ready from " + val.PlayerName);
			blockMessage = true;
		}
	}

	public override GameDataTypes GetGameDataType()
	{
		return (GameDataTypes)7;
	}
}
