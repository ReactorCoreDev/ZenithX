using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace ZenithX;

[HarmonyPatch(typeof(PlayerControl), "MurderPlayer")]
public static class PlayerControl_MurderPlayer_Patch
{
	public static readonly HashSet<byte> notifiedKilledVictims = new HashSet<byte>();

	public static void Prefix(PlayerControl __instance, PlayerControl target)
	{
		if ((Object)(object)target == (Object)null)
		{
			return;
		}
		if (target.protectedByGuardianId != -1)
		{
			NotificationHandler.HandleGuardianAngelSave(__instance, target);
		}
		else
		{
			if (notifiedKilledVictims.Contains(target.PlayerId))
			{
				return;
			}
			NotificationHandler.HandlePlayerKill(__instance, target);
			notifiedKilledVictims.Add(target.PlayerId);
			try
			{
				if (!((Object)(object)__instance == (Object)null) && !((Object)(object)target == (Object)null))
				{
					EventPlayer eventPlayer = default(EventPlayer);
					NetworkedPlayerInfo data = __instance.Data;
					eventPlayer.PlayerName = ((data != null) ? data.PlayerName : null) ?? "Unknown";
					NetworkedPlayerInfo data2 = __instance.Data;
					eventPlayer.ColorName = ((data2 != null) ? data2.ColorName : null) ?? "white";
					eventPlayer.IsProtected = false;
					eventPlayer = default(EventPlayer);
					NetworkedPlayerInfo data3 = target.Data;
					eventPlayer.PlayerName = ((data3 != null) ? data3.PlayerName : null) ?? "Unknown";
					NetworkedPlayerInfo data4 = target.Data;
					eventPlayer.ColorName = ((data4 != null) ? data4.ColorName : null) ?? "white";
					eventPlayer.IsProtected = Utils.isProtected(target);
					if (eventPlayer.IsProtected)
					{
						AntiCheat.Flag(__instance, CheatAction.KillProtected, __instance.Data.PlayerName + " killed a protected player");
					}
				}
			}
			catch
			{
			}
		}
	}
}
