using HarmonyLib;
using UnityEngine;

namespace ZenithX;

[HarmonyPatch(typeof(BanMenu), "SetVisible")]
public static class BanMenu_SetVisible
{
	public static bool Prefix(BanMenu __instance, bool show)
	{
		if ((Object)(object)__instance == (Object)null)
		{
			return true;
		}
		PlayerControl localPlayer = PlayerControl.LocalPlayer;
		if ((Object)(object)localPlayer == (Object)null || (Object)(object)localPlayer.Data == (Object)null)
		{
			show = false;
		}
		if ((Object)(object)__instance.BanButton != (Object)null)
		{
			((Component)__instance.BanButton).gameObject.SetActive(show && Utils.isHost);
		}
		if ((Object)(object)__instance.KickButton != (Object)null)
		{
			((Component)__instance.KickButton).gameObject.SetActive(show);
		}
		if ((Object)(object)__instance.MenuButton != (Object)null)
		{
			((Component)__instance.MenuButton).gameObject.SetActive(show);
		}
		return false;
	}
}
