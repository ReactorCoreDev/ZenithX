using System;
using HarmonyLib;
using UnityEngine;

namespace ZenithX;

[HarmonyPatch(typeof(HudManager), "SetHudActive", new Type[]
{
	typeof(PlayerControl),
	typeof(RoleBehaviour),
	typeof(bool)
})]
public static class HudManager_SetHudActive
{
	public static void Postfix(HudManager __instance, RoleBehaviour role, bool isActive)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (CheatToggles.showTasksInMeetings && Object.op_Implicit((Object)(object)MeetingHud.Instance))
		{
			Vector3 openPosition = __instance.TaskPanel.openPosition;
			openPosition.z = -20f;
			__instance.TaskPanel.openPosition = openPosition;
			((Component)__instance.TaskPanel).gameObject.SetActive(true);
		}
	}
}
