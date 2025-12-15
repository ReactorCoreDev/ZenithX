using HarmonyLib;

namespace ZenithX
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.FixedUpdate))]
    public static class ShipStatus_FixedUpdate
    {
        public static void Postfix(ShipStatus __instance)
        {
            ZenithXCheats.sabotageCheat(__instance);
            ZenithXCheats.closeMeetingCheat();
            ZenithXCheats.walkInVentCheat();
            ZenithXCheats.kickVentsCheat();
            ZenithXPPMCheats.reportBodyPPM();
        }
    }
}