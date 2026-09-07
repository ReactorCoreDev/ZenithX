using HarmonyLib;
using Hazel;
using InnerNet;
using UnityEngine;

namespace ZenithX;

[HarmonyPatch(typeof(ShipStatus), "HandleRpc")]
internal class OnShipStatusRPC
{
	private static bool Prefix(byte callId, MessageReader reader)
	{
		if (!AntiCheatExtensions.HandleRpc(typeof(ShipStatus), null, (RpcCalls)callId, reader))
		{
			return false;
		}
		if (!AntiCheatConfig.Enabled || !AntiCheatConfig.CheckHostRPCs || callId != 35)
		{
			return true;
		}
		int position = reader.Position;
		try
		{
			byte num = reader.ReadByte();
			PlayerControl val = MessageExtensions.ReadNetObject<PlayerControl>(reader);
			if (num == 37 && !((InnerNetClient)AmongUsClient.Instance).AmHost)
			{
				if ((Object)(object)val != (Object)null && (Object)(object)val.Data != (Object)null)
				{
					AlertUI.Warning(val.Data.PlayerName + " attempted to use the VentilationSystem kick exploit on you!");
				}
				return false;
			}
		}
		finally
		{
			reader.Position = position;
		}
		return true;
	}
}
