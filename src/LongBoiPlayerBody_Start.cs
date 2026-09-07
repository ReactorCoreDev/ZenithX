using HarmonyLib;
using UnityEngine;

namespace ZenithX;

[HarmonyPatch(typeof(LongBoiPlayerBody), "Start")]
public static class LongBoiPlayerBody_Start
{
	public static bool Prefix(LongBoiPlayerBody __instance)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Invalid comparison between Unknown and I4
		__instance.ShouldLongAround = true;
		if (__instance.hideCosmeticsQC)
		{
			__instance.cosmeticLayer.SetHatVisorVisible(false);
		}
		__instance.SetupNeckGrowth(false, true);
		if (__instance.isExiledPlayer)
		{
			ShipStatus instance = ShipStatus.Instance;
			if (!Object.op_Implicit((Object)(object)instance) || (int)instance.Type != 3)
			{
				__instance.cosmeticLayer.AdjustCosmeticRotations(-17.75f);
			}
		}
		if (!__instance.isPoolablePlayer)
		{
			__instance.cosmeticLayer.ValidateCosmetics();
		}
		return false;
	}
}
