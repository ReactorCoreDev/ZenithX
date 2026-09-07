using HarmonyLib;
using Hazel;
using InnerNet;

namespace ZenithX;

[HarmonyPatch(typeof(InnerNetClient), "HandleGameData")]
internal class HandleGameData
{
	private static bool Prefix(InnerNetClient __instance, MessageReader parentReader)
	{
		try
		{
			while (parentReader.BytesRemaining > 0)
			{
				MessageReader reader = parentReader.ReadMessageAsNewBuffer();
				int msgNum = (__instance.msgNum += 1);
				AntiCheatExtensions.HandleGameDataInner(__instance, reader, msgNum);
			}
		}
		finally
		{
			parentReader.Recycle();
		}
		return false;
	}
}
