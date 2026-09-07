using HarmonyLib;
using Hazel;

namespace ZenithX;

[HarmonyPatch(typeof(CustomNetworkTransform), "HandleRpc")]
internal class OnNetTransformRPC
{
	private static bool Prefix(CustomNetworkTransform __instance, byte callId, MessageReader reader)
	{
		return AntiCheatExtensions.HandleRpc(typeof(CustomNetworkTransform), __instance.myPlayer, (RpcCalls)callId, reader);
	}
}
