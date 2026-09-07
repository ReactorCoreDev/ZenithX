using HarmonyLib;
using Hazel;

namespace ZenithX;

[HarmonyPatch(typeof(PlayerControl), "HandleRpc")]
internal class OnPlayerControlRPC
{
	private static bool Prefix(PlayerControl __instance, byte callId, MessageReader reader)
	{
		return AntiCheatExtensions.HandleRpc(typeof(PlayerControl), __instance, (RpcCalls)callId, reader);
	}
}
