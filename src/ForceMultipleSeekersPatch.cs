using HarmonyLib;
using InnerNet;
using UnityEngine;

namespace ZenithX;

[HarmonyPatch(typeof(AmongUsClient), "CoStartGame")]
public static class ForceMultipleSeekersPatch
{
	public static void Postfix()
	{
		if (!((Object)(object)AmongUsClient.Instance == (Object)null) && ((InnerNetClient)AmongUsClient.Instance).AmHost && Utils.isHideNSeek && CheatToggles.customSeekers)
		{
			ZenithXCheats.ApplyCustomSeekers();
		}
	}
}
