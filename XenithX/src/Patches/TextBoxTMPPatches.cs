using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace ZenithX;

[HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.Update))]
public static class TextBoxTMPUpdate
{
    public static void Postfix(TextBoxTMP __instance)
    {
        if (!CheatToggles.chatJailbreak || !__instance.hasFocus) return;

        bool ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        if (ctrl && Input.GetKeyDown(KeyCode.C))
        {
            ClipboardHelper.PutClipboardString(__instance.text);
        }
        else if (ctrl && Input.GetKeyDown(KeyCode.V))
        {
            var chat = DestroyableSingleton<HudManager>.Instance.Chat.freeChatField.textArea;
            chat.SetText(chat.text + ClipboardHelper.GetClipboardString());
        }
        else if (ctrl && Input.GetKeyDown(KeyCode.X))
        {
            DestroyableSingleton<HudManager>.Instance.Chat.freeChatField.textArea.Clear();
        }
    }
}

[HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.Start))]
public static class AllowPaste_TextBoxTMP_Start_Postfix
{
    public static void Postfix(TextBoxTMP __instance)
    {
        if (!CheatToggles.chatJailbreak) return;

        __instance.allowAllCharacters = true;
        __instance.AllowEmail = true;
        __instance.AllowSymbols = true;
    }
}

[HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.IsCharAllowed))]
public static class TextBoxTMP_IsCharAllowed
{
    public static bool Prefix(TextBoxTMP __instance, char i, ref bool __result)
    {
        if (!CheatToggles.chatJailbreak) return true;

        // Block only control characters
        if (i == '\b' || i == '\r')
        {
            __result = false;
            return false;
        }

        __result = true;
        return false;
    }
}