using HarmonyLib;
using UnityEngine;

namespace ZenithX
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public static class HudManager_Start
    {
        public static void Postfix(HudManager __instance)
        {
            if (__instance == null || __instance.MapButton == null) return;

            __instance.MapButton.OnClick.RemoveAllListeners();
            if (!__instance.MapButton.isActiveAndEnabled)
                __instance.MapButton.gameObject.SetActive(true);

            __instance.MapButton.OnClick.AddListener((UnityEngine.Events.UnityAction)(() =>
            {
                if (CheatToggles.changeMapToSabotage)
                {
                    __instance.ToggleMapVisible(new MapOptions
                    {
                        Mode = MapOptions.Modes.Sabotage
                    });
                }
                else
                {
                    __instance.ToggleMapVisible(new MapOptions
                    {
                        Mode = MapOptions.Modes.Normal
                    });
                }
            }));
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManager_Update
    {
        public static void Postfix(HudManager __instance)
        {
            __instance.ShadowQuad.gameObject.SetActive(!ZenithXESP.fullBrightActive());

            if (Utils.chatUiActive())
                __instance.Chat.gameObject.SetActive(true);
            else
            {
                Utils.closeChat();
                __instance.Chat.gameObject.SetActive(false);
            }

            ZenithXCheats.useVentCheat(__instance);
            ZenithXESP.zoomOut(__instance);
            ZenithXESP.freecamCheat();

            if (PlayerPickMenu.playerpickMenu != null && CheatToggles.shouldPPMClose())
                PlayerPickMenu.playerpickMenu.Close();
        }
    }
}