using HarmonyLib;
using UnityEngine;

namespace ZenithX;

[HarmonyPatch(typeof(HudManager), "Update")]
public static class HudManager_Update
{
	public static void Postfix(HudManager __instance)
	{
		((Component)__instance.ShadowQuad).gameObject.SetActive(!ZenithXESP.fullBrightActive());
		if (Utils.chatUiActive())
		{
			((Component)__instance.Chat).gameObject.SetActive(true);
		}
		else
		{
			Utils.closeChat();
			((Component)__instance.Chat).gameObject.SetActive(false);
		}
		ZenithXCheats.useVentCheat(__instance);
		ZenithXESP.zoomOut(__instance);
		ZenithXESP.freecamCheat();
		if ((Object)(object)PlayerPickMenu.playerpickMenu != (Object)null && CheatToggles.shouldPPMClose())
		{
			((Minigame)PlayerPickMenu.playerpickMenu).Close();
		}
	}
}
