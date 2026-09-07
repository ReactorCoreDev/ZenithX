using System;
using HarmonyLib;
using UnityEngine;

namespace ZenithX;

internal class SkipAnimationPatch
{
	[HarmonyPatch(typeof(ShhhBehaviour), "PlayAnimation")]
	public static class SkipShhhAnimPatch
	{
		[HarmonyPatch(typeof(LogicOptionsHnS), "GetCrewmateLeadTime")]
		public static class SkipSeekerAnimPatch
		{
			public static bool Prefix(ref int __result)
			{
				if (!CheatToggles.noSeekerAnimation)
				{
					return true;
				}
				__result = 0;
				return false;
			}
		}

		[HarmonyPatch(typeof(KillOverlay), "ShowKillAnimation")]
		[HarmonyPatch(new Type[]
		{
			typeof(NetworkedPlayerInfo),
			typeof(NetworkedPlayerInfo)
		})]
		public static class SkipKillAnimationPatch
		{
			[HarmonyPrefix]
			public static bool Prefix(NetworkedPlayerInfo killer, NetworkedPlayerInfo victim)
			{
				if (!CheatToggles.noKillAnimation)
				{
					return true;
				}
				return false;
			}
		}

		public static bool Prefix()
		{
			if (!CheatToggles.noShhScreenAnimation)
			{
				return true;
			}
			HudManager instance = DestroyableSingleton<HudManager>.Instance;
			if ((Object)(object)((instance != null) ? instance.shhhEmblem : null) != (Object)null)
			{
				((Component)DestroyableSingleton<HudManager>.Instance.shhhEmblem).gameObject.SetActive(false);
			}
			return false;
		}
	}
}
