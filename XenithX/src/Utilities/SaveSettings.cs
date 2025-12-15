using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using UnityEngine;

namespace ZenithX
{
    public static class SaveSettings
    {
        private static Timer updateTimer;
        private static ConfigFile ConfigRef;
        private static readonly Dictionary<string, ConfigEntry<bool>> Entries = new();

        public static void LoadSettings(BasePlugin plugin)
        {
            ConfigRef = plugin.Config;
            Entries.Clear();

            var cheatFields = typeof(CheatToggles)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(bool))
                .ToArray();

            var mapping = new Dictionary<FieldInfo, string>();

            if (MenuUI.groups != null)
            {
                foreach (var group in MenuUI.groups)
                {
                    string category = "z." + group.name;
                    if (group.items == null) continue;

                    foreach (var item in group.items)
                    {
                        ProcessCreateListItem(item, category, cheatFields, mapping);
                    }
                }
            }

            foreach (var field in cheatFields)
            {
                string category = mapping.TryGetValue(field, out var c) ? c : "z.Misc";
                bool currentValue = (bool)field.GetValue(null);
                var entry = ConfigRef.Bind(category, field.Name, currentValue);
                Entries[field.Name] = entry;
                try { field.SetValue(null, entry.Value); } catch { }
            }

            try { ConfigRef.Save(); } catch { }

            StartTimer();
        }

        private static void ProcessCreateListItem(object item, string category, FieldInfo[] cheatFields, Dictionary<FieldInfo, string> mapping)
        {
            if (item == null) return;

            // If it's a ToggleInfo (struct implementing CreateList) boxed as object
            if (item is ToggleInfo t)
            {
                ResolveToggleMapping(t, cheatFields, mapping, category);
                return;
            }

            // If it's a ButtonInfo -- ignore
            if (item is ButtonInfo) return;

            // Otherwise, try to reflectively find an "items" field or property (submenu)
            var itemType = item.GetType();

            // common patterns: SubmenuInfo has public List<CreateList> items field
            var itemsField = itemType.GetField("items", BindingFlags.Public | BindingFlags.Instance);
            if (itemsField != null)
            {
                try
                {
                    var value = itemsField.GetValue(item);
                    if (value is IEnumerable enumerable)
                    {
                        foreach (var sub in enumerable)
                            ProcessCreateListItem(sub, category, cheatFields, mapping);
                    }
                }
                catch { }
                return;
            }

            // fallback: property named "items"
            var itemsProp = itemType.GetProperty("items", BindingFlags.Public | BindingFlags.Instance);
            if (itemsProp != null)
            {
                try
                {
                    var value = itemsProp.GetValue(item);
                    if (value is IEnumerable enumerable)
                    {
                        foreach (var sub in enumerable)
                            ProcessCreateListItem(sub, category, cheatFields, mapping);
                    }
                }
                catch { }
                return;
            }

            // Nothing usable found — ignore
        }

        private static void ResolveToggleMapping(ToggleInfo toggle, FieldInfo[] cheatFields, Dictionary<FieldInfo, string> mapping, string category)
        {
            bool toggleOriginal = SafeGetToggleState(toggle);

            for (int i = 0; i < cheatFields.Length; i++)
            {
                var field = cheatFields[i];
                bool fieldOriginal = (bool)field.GetValue(null);
                bool matched = false;

                try
                {
                    field.SetValue(null, !fieldOriginal);
                    bool toggleNow = SafeGetToggleState(toggle);
                    if (toggleNow != toggleOriginal) matched = true;
                }
                catch { matched = false; }
                finally
                {
                    try { field.SetValue(null, fieldOriginal); } catch { }
                }

                if (matched)
                {
                    if (!mapping.ContainsKey(field))
                        mapping[field] = category;
                    return;
                }
            }

            // fallback: attempt using setter on toggle to see which field changes
            for (int i = 0; i < cheatFields.Length; i++)
            {
                var field = cheatFields[i];
                bool fieldOriginal = (bool)field.GetValue(null);

                try
                {
                    SafeSetToggleState(toggle, !toggleOriginal);
                    bool after = (bool)field.GetValue(null);

                    if (after != fieldOriginal)
                    {
                        SafeSetToggleState(toggle, toggleOriginal);
                        if (!mapping.ContainsKey(field))
                            mapping[field] = category;
                        return;
                    }

                    SafeSetToggleState(toggle, toggleOriginal);
                }
                catch
                {
                    try { field.SetValue(null, fieldOriginal); } catch { }
                }
            }
        }

        private static bool SafeGetToggleState(ToggleInfo t)
        {
            try { return t.getState(); }
            catch { return false; }
        }

        private static void SafeSetToggleState(ToggleInfo t, bool value)
        {
            try { t.setState(value); }
            catch { }
        }

        private static void StartTimer()
        {
            updateTimer?.Dispose();
            updateTimer = new Timer(_ => UpdateConfigFromCheatToggles(), null, 0, 60000);
        }

        public static void UpdateConfigFromCheatToggles()
        {
            var cheatFields = typeof(CheatToggles)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(bool));

            foreach (var field in cheatFields)
            {
                if (Entries.TryGetValue(field.Name, out var entry))
                {
                    bool value = (bool)field.GetValue(null);
                    if (entry.Value != value)
                        entry.Value = value;
                }
            }

            try { ConfigRef.Save(); } catch { }
        }

        public static void SaveNow()
        {
            UpdateConfigFromCheatToggles();
        }
    }
}
