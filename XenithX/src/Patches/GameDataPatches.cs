using HarmonyLib;
using Il2CppInterop.Runtime;

namespace ZenithX
{
    [HarmonyPatch(typeof(GameData))]
    public static class GameDataVoteKickPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameData), nameof(GameData.HandleDisconnect), new[] { typeof(PlayerControl), typeof(DisconnectReasons) })]
        public static bool PreventAllDisconnects(GameData __instance, PlayerControl player, DisconnectReasons reason)
        {
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameData), nameof(GameData.ShowNotification), new[] { typeof(string), typeof(DisconnectReasons) })]
        public static bool SuppressAllNotifications(GameData __instance, string playerName, DisconnectReasons reason)
        {
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameData), nameof(GameData.OnDisconnected))]
        public static bool SkipAllOnDisconnected()
        {
            return false;
        }
    }
}