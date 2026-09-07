using HarmonyLib;
using Hazel;
using Il2CppSystem.Collections.Generic;
using InnerNet;
using UnityEngine;

namespace ZenithX;

[HarmonyPatch(typeof(PlayerControl), "RpcSendChat")]
public static class RPC_SpamTextPostfix
{
	public static string spamText;

	private static float lastChatTime = 0f;

	private static float chatDelay = 0.5f;

	public static bool Prefix(string chatText, PlayerControl __instance)
	{
		if (CheatToggles.spamChat)
		{
			spamText = chatText;
			return false;
		}
		return true;
	}

	[HarmonyPrefix]
	public static void Update()
	{
		if (CheatToggles.spamChat)
		{
			if (CheatToggles.chatMimic)
			{
				CheatToggles.chatMimic = false;
			}
			if (spamText != null && Time.time - lastChatTime >= chatDelay)
			{
				lastChatTime = Time.time;
				SendSpamChat();
			}
		}
	}

	private static void SendSpamChat()
	{
		ClientData host = ((InnerNetClient)AmongUsClient.Instance).GetHost();
		if (host == null || host.Character.Data.Disconnected)
		{
			return;
		}
		Enumerator<PlayerControl> enumerator = PlayerControl.AllPlayerControls.GetEnumerator();
		while (enumerator.MoveNext())
		{
			PlayerControl current = enumerator.Current;
			Enumerator<PlayerControl> enumerator2 = PlayerControl.AllPlayerControls.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				PlayerControl current2 = enumerator2.Current;
				MessageWriter val = ((InnerNetClient)AmongUsClient.Instance).StartRpcImmediately(((InnerNetObject)current).NetId, (byte)13, (SendOption)1, ((InnerNetClient)AmongUsClient.Instance).GetClientIdFromCharacter(current2));
				val.Write(spamText);
				((InnerNetClient)AmongUsClient.Instance).FinishRpcImmediately(val);
			}
		}
	}
}
