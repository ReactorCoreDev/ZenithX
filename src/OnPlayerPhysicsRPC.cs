using HarmonyLib;
using Hazel;

namespace ZenithX;

[HarmonyPatch(typeof(PlayerPhysics), "HandleRpc")]
internal class OnPlayerPhysicsRPC
{
	private static bool Prefix(PlayerPhysics __instance, byte callId, MessageReader reader)
	{
		return AntiCheatExtensions.HandleRpc(typeof(PlayerPhysics), __instance.myPlayer, (RpcCalls)callId, reader);
	}
}
