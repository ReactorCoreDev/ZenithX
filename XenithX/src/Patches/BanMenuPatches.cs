using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace ZenithX;

[HarmonyPatch(typeof(BanMenu), nameof(BanMenu.Select))]
class BanMenuSelectPatch
{
    public static void Postfix(BanMenu __instance, int clientId)
    {
        if (__instance == null || !AmongUsClient.Instance) return;

        InnerNet.ClientData recentClient = AmongUsClient.Instance.GetRecentClient(clientId);
        if (recentClient == null) return;

        var hud = HudManager.Instance;
        if (hud == null || hud.Chat == null) return;

        var chatBanButton = hud.Chat.banButton;
        if (chatBanButton != null)
        {
            chatBanButton.gameObject.SetActive(true);

            var kickBtn = chatBanButton.KickButton;
            if (kickBtn != null)
            {
                kickBtn.gameObject.SetActive(true);
            }
        }

        var banBtn = __instance.BanButton;
        if (banBtn != null)
        {
            banBtn.gameObject.SetActive(true);

            var btnComponent = banBtn.GetComponent<Button>();
            if (btnComponent != null)
                btnComponent.interactable = true;

            var rollover = banBtn.GetComponent<ButtonRolloverHandler>();
            if (rollover != null)
            {
                rollover.enabled = true;
                rollover.SetEnabledColors();
            }
        }
    }
}