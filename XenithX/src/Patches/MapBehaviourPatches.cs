using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace ZenithX
{
    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.ShowNormalMap))]
    public static class MapBehaviour_ShowNormalMap
    {
        public static void Postfix(MapBehaviour __instance)
        {
            MinimapHandler.minimapActive = MinimapHandler.isCheatEnabled();
            if (!MinimapHandler.minimapActive) return;

            __instance.ColorControl.SetColor(Palette.Purple);
            __instance.DisableTrackerOverlays();

            try
            {
                MinimapHandler.herePoints.ForEach(x => Object.Destroy(x.sprite.gameObject));
                MinimapHandler.herePoints.Clear();
            }
            catch { }

            var temp = new List<HerePoint>();
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!player.AmOwner)
                {
                    var herePoint = Object.Instantiate(__instance.HerePoint, __instance.HerePoint.transform.parent);
                    temp.Add(new HerePoint(player, herePoint));
                }
            }

            MinimapHandler.herePoints = temp;
        }
    }

    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.FixedUpdate))]
    public static class MapBehaviour_FixedUpdate
    {
        public static void Postfix(MapBehaviour __instance)
        {
            if (MinimapHandler.isCheatEnabled() != MinimapHandler.minimapActive)
            {
                if (!__instance.infectedOverlay.gameObject.activeSelf)
                {
                    __instance.Close();
                    __instance.ShowNormalMap();
                }
            }

            foreach (var herePoint in MinimapHandler.herePoints)
            {
                MinimapHandler.handleHerePoint(herePoint);
            }

            foreach (var removePoint in MinimapHandler.herePointsToRemove)
            {
                MinimapHandler.herePoints.Remove(removePoint);
            }
        }
    }

    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.Close))]
    public static class MapBehaviour_Close
    {
        public static void Postfix(MapBehaviour __instance)
        {
            try
            {
                MinimapHandler.herePoints.ForEach(x => Object.Destroy(x.sprite.gameObject));
                MinimapHandler.herePoints.Clear();
            }
            catch { }
        }
    }
}