using System;
using HarmonyLib;
using Il2CppSystem;

namespace ZenithX;

[HarmonyPatch(typeof(LongBoiPlayerBody), "Awake")]
public static class LongBoiPlayerBody_Awake
{
	public static bool Prefix(LongBoiPlayerBody __instance)
	{
		CosmeticsLayer cosmeticLayer = __instance.cosmeticLayer;
		cosmeticLayer.OnSetBodyAsGhost += Action.op_Implicit((Action)__instance.SetPoolableGhost);
		CosmeticsLayer cosmeticLayer2 = __instance.cosmeticLayer;
		cosmeticLayer2.OnColorChange += Action<int>.op_Implicit((Action<int>)__instance.SetHeightFromColor);
		CosmeticsLayer cosmeticLayer3 = __instance.cosmeticLayer;
		cosmeticLayer3.OnCosmeticSet += Action<string, int, CosmeticKind>.op_Implicit((Action<string, int, CosmeticKind>)__instance.OnCosmeticSet);
		return false;
	}
}
