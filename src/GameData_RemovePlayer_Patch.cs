using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace ZenithX;

[HarmonyPatch(typeof(GameData), "RemovePlayer")]
public static class GameData_RemovePlayer_Patch
{
	public static readonly HashSet<byte> notifiedDisconnects = new HashSet<byte>();

	public static void Prefix(GameData __instance, byte playerId)
	{
		if (CheatToggles.notifyOnDisconnect && Utils.isInGame && !notifiedDisconnects.Contains(playerId))
		{
			NetworkedPlayerInfo playerById = __instance.GetPlayerById(playerId);
			if ((Object)(object)playerById != (Object)null)
			{
				NotificationHandler.HandlePlayerDisconnect(playerById);
				notifiedDisconnects.Add(playerId);
			}
		}
	}
}
