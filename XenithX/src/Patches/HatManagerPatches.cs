using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AmongUs.Data;
using HarmonyLib;
using UnityEngine;
using DateTime = Il2CppSystem.DateTime;

namespace ZenithX
{
    [HarmonyPatch(typeof(HatManager), nameof(HatManager.Initialize))]
    public static class CosmeticsUnlockPatch
    {
        private static readonly Dictionary<(Type, string), MemberInfo> memberCache = new();
        private static readonly BindingFlags CaseInsensitiveInstanceFlags =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase;

        private static object ConvertIfNeeded(object value, Type targetType)
        {
            if (value == null) return null;
            if (targetType.IsAssignableFrom(value.GetType())) return value;
            try { return Convert.ChangeType(value, targetType); } catch { return value; }
        }

        private static MemberInfo GetCachedMember(Type type, string name)
        {
            var key = (type, name.ToLowerInvariant());
            if (memberCache.TryGetValue(key, out var mi)) return mi;

            var prop = type.GetProperty(name, CaseInsensitiveInstanceFlags);
            if (prop != null) { memberCache[key] = prop; return prop; }

            var field = type.GetField(name, CaseInsensitiveInstanceFlags);
            if (field != null) { memberCache[key] = field; return field; }

            memberCache[key] = null;
            return null;
        }

        private static bool TrySetPropertyOrField(object target, object value, params string[] candidateNames)
        {
            if (target == null || candidateNames == null || candidateNames.Length == 0) return false;
            var type = target.GetType();
            foreach (var name in candidateNames)
            {
                var mi = GetCachedMember(type, name);
                if (mi is PropertyInfo prop && prop.CanWrite)
                {
                    try { prop.SetValue(target, ConvertIfNeeded(value, prop.PropertyType), null); return true; } catch { }
                }
                if (mi is FieldInfo field && !field.IsInitOnly)
                {
                    try { field.SetValue(target, ConvertIfNeeded(value, field.FieldType)); return true; } catch { }
                }
            }
            return false;
        }

        private static string GetStringPropertyOrFieldOrDefault(object target, string defaultValue, params string[] candidateNames)
        {
            if (target == null || candidateNames == null || candidateNames.Length == 0) return defaultValue;
            var type = target.GetType();
            foreach (var name in candidateNames)
            {
                var mi = GetCachedMember(type, name);
                if (mi is PropertyInfo prop && prop.CanRead)
                {
                    try { var objVal = prop.GetValue(target, null); if (objVal is string s && !string.IsNullOrEmpty(s)) return s; } catch { }
                }
                if (mi is FieldInfo field)
                {
                    try { var objVal = field.GetValue(target); if (objVal is string s && !string.IsNullOrEmpty(s)) return s; } catch { }
                }
            }
            return defaultValue;
        }

        public static void Postfix(HatManager __instance)
        {
            CosmeticsUnlocker.unlockCosmetics(__instance);

            var playerData = DataManager.Player;
            if (playerData == null) return;

            var storeData = playerData.Store;
            if (storeData != null)
            {
                storeData.LastBundlesViewDate = DateTime.Now;
                storeData.LastHatsViewDate = DateTime.Now;
                storeData.LastOutfitsViewDate = DateTime.Now;
                storeData.LastVisorsViewDate = DateTime.Now;
                storeData.LastPetsViewDate = DateTime.Now;
                storeData.LastNameplatesViewDate = DateTime.Now;
                storeData.LastCosmicubeViewDate = DateTime.Now;
            }

            playerData.Save();
        }
    }

    [HarmonyPatch(typeof(Constants), "IsVersionModded")]
    public static class Constants_IsVersionModded_Patch
    {
        [HarmonyPrefix]
        public static bool ForceReturnFalse(ref bool __result)
        {
            __result = false;
            return false;
        }
    }

    [HarmonyPatch(typeof(HatManager), nameof(HatManager.CheckValidCosmetic))]
    public static class Patch_IgnoreBlacklist
    {
        public static bool Prefix(ref bool __result)
        {
            __result = true;
            return false;
        }
    }
}