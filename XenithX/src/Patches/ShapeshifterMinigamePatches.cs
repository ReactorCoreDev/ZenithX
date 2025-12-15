using AmongUs.Data;
using HarmonyLib;
using UnityEngine;
using Il2CppSystem.Collections.Generic;

namespace ZenithX
{
    [HarmonyPatch(typeof(ShapeshifterMinigame), nameof(ShapeshifterMinigame.Begin))]
    public static class ShapeshifterMinigame_Begin
    {
        public static bool Prefix(ShapeshifterMinigame __instance)
        {
            if (!PlayerPickMenu.IsActive) return true;

            var list = PlayerPickMenu.customPlayerList;
            var panelList = new Il2CppSystem.Collections.Generic.List<ShapeshifterPanel>();
            var buttonsList = new Il2CppSystem.Collections.Generic.List<UiElement>();

            for (int i = 0; i < list.Count; i++)
            {
                var playerData = list[i];
                int col = i % 3;
                int row = i / 3;

                var panel = Object.Instantiate(__instance.PanelPrefab, __instance.transform);
                panel.transform.localPosition = new Vector3(__instance.XStart + col * __instance.XOffset,
                                                            __instance.YStart + row * __instance.YOffset, -1f);

                panel.SetPlayer(i, playerData, (System.Action)(() =>
                {
                    PlayerPickMenu.targetPlayerData = playerData;
                    PlayerPickMenu.customAction?.Invoke();
                    __instance.Close();
                }));

                if (playerData?.Object != null)
                {
                    panel.NameText.text = Utils.getNameTag(playerData, playerData.DefaultOutfit.PlayerName);
                }

                panelList.Add(panel);
                buttonsList.Add(panel.Button);
            }

            ControllerManager.Instance.OpenOverlayMenu(__instance.name, __instance.BackButton,
                                                       __instance.DefaultButtonSelected, buttonsList, false);

            PlayerPickMenu.IsActive = false;

            return false; // skip original method
        }
    }

    [HarmonyPatch(typeof(ShapeshifterPanel), nameof(ShapeshifterPanel.SetPlayer))]
    public static class ShapeshifterPanel_SetPlayer
    {
        public static bool Prefix(ShapeshifterPanel __instance, int index, NetworkedPlayerInfo playerInfo, Il2CppSystem.Action onShift)
        {
            if (!PlayerPickMenu.IsActive) return true;

            __instance.PlayerIcon?.SetFlipX(false);
            __instance.PlayerIcon?.ToggleName(false);

            var renderers = __instance.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i]?.material.SetInt(PlayerMaterial.MaskLayer, index + 2);
            }

            __instance.PlayerIcon?.SetMaskLayer(index + 2);
            __instance.PlayerIcon?.UpdateFromEitherPlayerDataOrCache(playerInfo, PlayerOutfitType.Default, PlayerMaterial.MaskType.ComplexUI, false, null);

            if (__instance.LevelNumberText != null)
                __instance.LevelNumberText.text = ProgressionManager.FormatVisualLevel(playerInfo.PlayerLevel);

            if (__instance.NameText != null)
                __instance.NameText.text = playerInfo.PlayerName;

            return false; // skip original method
        }
    }
}