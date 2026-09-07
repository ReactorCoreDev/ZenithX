using System;
using HarmonyLib;
using Il2CppSystem;
using UnityEngine;

namespace ZenithX;

[HarmonyPatch(typeof(PlayerControl), "CompleteTask")]
public static class PlayerControl_CompleteTask
{
	public static void Postfix(PlayerControl __instance, uint idx)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		if (CheatToggles.logTasks)
		{
			PlayerTask val = __instance.myTasks.Find(Predicate<PlayerTask>.op_Implicit((Func<PlayerTask, bool>)((PlayerTask p) => p.Id == idx)));
			PlainShipRoom roomFromPosition = Utils.getRoomFromPosition(__instance.GetTruePosition());
			string value = (((Object)(object)roomFromPosition != (Object)null) ? ((object)roomFromPosition.RoomId/*cast due to .constrained prefix*/).ToString() : "an unknown location");
			if (Object.op_Implicit((Object)(object)val))
			{
				AlertUI.Notification($"<color=#{ColorUtility.ToHtmlStringRGB(__instance.Data.Color)}>{__instance.Data.PlayerName}</color> completed task {val.TaskType} in {value}", 5f);
			}
		}
	}
}
