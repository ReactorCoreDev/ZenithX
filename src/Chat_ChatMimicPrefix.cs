using HarmonyLib;
using Hazel;
using Il2CppSystem.Collections.Generic;
using InnerNet;
using UnityEngine;

namespace ZenithX;

[HarmonyPatch(typeof(PlayerControl), "RpcSendChat")]
public static class Chat_ChatMimicPrefix
{
	public static PlayerControl chatMimicTarget;

	public static bool Prefix(string chatText, PlayerControl __instance)
	{
		if ((Object)(object)chatMimicTarget == (Object)null)
		{
			return true;
		}
		ClientData host = ((InnerNetClient)AmongUsClient.Instance).GetHost();
		if (host != null && !host.Character.Data.Disconnected)
		{
			Enumerator<PlayerControl> enumerator = PlayerControl.AllPlayerControls.GetEnumerator();
			while (enumerator.MoveNext())
			{
				PlayerControl current = enumerator.Current;
				MessageWriter val = ((InnerNetClient)AmongUsClient.Instance).StartRpcImmediately(((InnerNetObject)chatMimicTarget).NetId, (byte)13, (SendOption)1, ((InnerNetClient)AmongUsClient.Instance).GetClientIdFromCharacter(current));
				val.Write(chatText);
				((InnerNetClient)AmongUsClient.Instance).FinishRpcImmediately(val);
			}
		}
		return false;
	}
}
