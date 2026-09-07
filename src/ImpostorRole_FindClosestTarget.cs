using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using Sentry.Internal.Extensions;
using UnityEngine;

namespace ZenithX;

[HarmonyPatch(typeof(ImpostorRole), "FindClosestTarget")]
public static class ImpostorRole_FindClosestTarget
{
	public static bool Prefix(ImpostorRole __instance, ref PlayerControl __result)
	{
		if (!CheatToggles.killReach)
		{
			return true;
		}
		List<PlayerControl> list = (from player in Utils.getPlayersSortedByDistance()
			where !MiscExtensions.IsNull((Object)(object)player) && ((RoleBehaviour)__instance).IsValidTarget(player.Data) && ((Behaviour)player.Collider).enabled
			select player).ToList();
		if (list.Count == 0)
		{
			return true;
		}
		__result = list[0];
		return false;
	}

	public static void Postfix(ref PlayerControl __result)
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		if (!CheatToggles.killOtherImpostors || (Object)(object)PlayerControl.LocalPlayer == (Object)null)
		{
			return;
		}
		PlayerControl val = null;
		float num = 1.8f;
		Enumerator<PlayerControl> enumerator = PlayerControl.AllPlayerControls.GetEnumerator();
		while (enumerator.MoveNext())
		{
			PlayerControl current = enumerator.Current;
			if (!((Object)(object)current == (Object)null) && !((Object)(object)current == (Object)(object)PlayerControl.LocalPlayer) && !((Object)(object)current.Data == (Object)null) && !current.Data.IsDead && !((Object)(object)current.Data.Role == (Object)null) && current.Data.Role.IsImpostor)
			{
				float num2 = Vector2.Distance(current.GetTruePosition(), PlayerControl.LocalPlayer.GetTruePosition());
				if (!(num2 >= num))
				{
					num = num2;
					val = current;
				}
			}
		}
		if ((Object)(object)val != (Object)null)
		{
			__result = val;
		}
	}
}
