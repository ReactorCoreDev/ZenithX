using AmongUs.GameOptions;
using HarmonyLib;

namespace ZenithX
{
    [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.Increase))]
    public static class IncreaseWithoutLimits_NumberOption_Increase_Prefix
    {
        public static bool Prefix(NumberOption __instance)
        {
            if (!CheatToggles.noOptionsLimits) return true;

            __instance.Value += __instance.Increment;
            __instance.UpdateValue();
            __instance.OnValueChanged.Invoke(__instance);
            __instance.AdjustButtonsActiveState();
            return false;
        }
    }

    [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.Decrease))]
    public static class DecreaseWithoutLimits_NumberOption_Decrease_Prefix
    {
        public static bool Prefix(NumberOption __instance)
        {
            if (!CheatToggles.noOptionsLimits) return true;

            __instance.Value -= __instance.Increment;
            __instance.UpdateValue();
            __instance.OnValueChanged.Invoke(__instance);
            __instance.AdjustButtonsActiveState();
            return false;
        }
    }

    [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.Initialize))]
    public static class UnlimitedRange_NumberOption_Initialize_Postfix
    {
        public static void Postfix(NumberOption __instance)
        {
            if (CheatToggles.noOptionsLimits)
            {
                __instance.ValidRange = new FloatRange(float.MinValue, float.MaxValue);
            }
        }
    }

    [HarmonyPatch(typeof(IGameOptionsExtensions), nameof(IGameOptionsExtensions.GetAdjustedNumImpostors))]
    public static class IGameOptionsExtensions_GetAdjustedNumImpostors
    {
        public static bool Prefix(IGameOptions __instance, int playerCount, ref int __result)
        {
            if (!CheatToggles.noOptionsLimits) return true;

            __result = GameOptionsManager.Instance.CurrentGameOptions.NumImpostors;
            return false;
        }
    }
}