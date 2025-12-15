using HarmonyLib;
using System;
using UnityEngine;
using System.Text.RegularExpressions;
using Il2CppSystem.Reflection;
using Il2CppSystem;
using Il2CppInterop.Runtime;
using BepInEx.Unity.IL2CPP.Utils;
using System.Collections.Generic;
using System.Linq;
using InnerNet;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using static ZenithX.ChatHistory_ChatController_SendChat_Prefix;

namespace ZenithX;

[HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
public static class ChatController_AddChat
{
    public static bool Prefix(PlayerControl sourcePlayer, string chatText, bool censor, ChatController __instance)
    {
        if (!CheatToggles.seeGhosts)
            return true;

        if (PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || sourcePlayer == null || sourcePlayer.Data == null)
            return true;

        var localData = PlayerControl.LocalPlayer.Data;
        var sourceData = sourcePlayer.Data;

        if (localData.IsDead)
            return true;

        if (__instance == null || __instance.scroller == null || __instance.scroller.Inner == null || __instance.chatBubblePool == null)
            return true;

        ChatBubble pooledBubble = __instance.GetPooledBubble();
        if (pooledBubble == null)
            return true;

        try
        {
            pooledBubble.transform.SetParent(__instance.scroller.Inner);
            pooledBubble.transform.localScale = Vector3.one;

            bool isLocal = sourcePlayer == PlayerControl.LocalPlayer;
            if (isLocal) pooledBubble.SetRight();
            else pooledBubble.SetLeft();

            bool didVote = MeetingHud.Instance != null && MeetingHud.Instance.DidVote(sourcePlayer.PlayerId);

            if (pooledBubble.SetCosmetics != null)
                pooledBubble.SetCosmetics(sourceData);

            __instance.SetChatBubbleName(pooledBubble, sourceData, sourceData.IsDead, didVote, PlayerNameColor.Get(sourceData), null);

            if (censor && AmongUs.Data.DataManager.Settings.Multiplayer.CensorChat)
                chatText = BlockedWords.CensorWords(chatText, false);

            pooledBubble.SetText(chatText);
            pooledBubble.AlignChildren();
            __instance.AlignAllBubbles();

            if (!__instance.IsOpenOrOpening && __instance.notificationRoutine == null)
                __instance.notificationRoutine = __instance.StartCoroutine((Il2CppSystem.Collections.IEnumerator)__instance.BounceDot());

            if (!isLocal && SoundManager.Instance != null && SoundManager.Instance.PlaySound != null)
                SoundManager.Instance.PlaySound(__instance.messageSound, false, 1f, null).pitch = 0.5f + (float)sourcePlayer.PlayerId / 15f;
        }
        catch (System.Exception ex)
        {
            ChatController.Logger.Error(ex.ToString(), null);
            if (__instance.chatBubblePool != null)
                __instance.chatBubblePool.Reclaim(pooledBubble);
        }

        return false; // Skip original AddChat
    }
}

[HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetName))]
public static class ChatBubble_SetName
{
    public static void Postfix(ChatBubble __instance){
        ZenithXESP.chatNametags(__instance);
    }
}

[HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
public static class ChatJailbreak_ChatController_Update_Postfix
{
    public static ZenithX Plugin { get; internal set; }
    // CurrentHistorySelection: -1 = no selection, 0 = first message, Count - 1 = last message
    public static int CurrentHistorySelection = -1;
    private static string inProgressMessage = "";
    private static bool isNavigatingHistory = false;

    /// <summary>
    /// Remove the chat cooldown and the character limit. Add the ability to scroll through previous chat messages using the up and down arrow keys.
    /// </summary>
    /// <param name="__instance">The <c>ChatController</c> instance.</param>
    public static void Postfix(ChatController __instance)
    {
        if (CheatToggles.chatJailbreak){
            __instance.freeChatField.textArea.characterLimit = 119; // Longer message length when chatJailbreak is enabled
        }else{
            __instance.freeChatField.textArea.characterLimit = 100;
        }
        // Set chat cooldown to 2.1s opposed to original 3s
        if (__instance.timeSinceLastMessage < 0.9f)
        {
            __instance.timeSinceLastMessage = 0.9f;
        }

        else if (CheatToggles.chatJailbreak)
        {
            __instance.freeChatField.textArea.allowAllCharacters = CheatToggles.chatJailbreak;
            __instance.freeChatField.textArea.AllowEmail = CheatToggles.chatJailbreak;
            __instance.freeChatField.textArea.AllowSymbols = CheatToggles.chatJailbreak;
            __instance.freeChatField.textArea.characterLimit = CheatToggles.chatJailbreak ? 119 : 100;
        }

        // User is trying to navigate up the chat history
        if (Input.GetKeyDown(KeyCode.UpArrow) && ChatHistory.Count > 0)
        {
            if (!isNavigatingHistory)
            {
                // Store the in-progress text so we can restore it later
                inProgressMessage = __instance.freeChatField.textArea.text;
                isNavigatingHistory = true;
            }

            if (CurrentHistorySelection == 0)
            {
                SoundManager.Instance.PlaySound(__instance.warningSound, false);
            }
            
            else
            {
                // Ensure the index (current selection) is within bounds of the ChatHistory list (0 to Count - 1)
                CurrentHistorySelection = Mathf.Clamp(--CurrentHistorySelection, 0, ChatHistory.Count - 1);
                __instance.freeChatField.textArea.SetText(ChatHistory[CurrentHistorySelection]);
            }
        }

        // User is trying to navigate down the chat history
        if (Input.GetKeyDown(KeyCode.DownArrow) && ChatHistory.Count > 0)
        {
            CurrentHistorySelection++;
            if (CurrentHistorySelection < ChatHistory.Count)
            {
                __instance.freeChatField.textArea.SetText(ChatHistory[CurrentHistorySelection]);
            }
            // User has navigated past the most recent message, restore the in-progress text
            else
            {
                __instance.freeChatField.textArea.SetText(inProgressMessage);
                isNavigatingHistory = false;
            }
        }
    }
}

[HarmonyPatch(typeof(ChatController), nameof(ChatController.SendFreeChat))]
public static class ChatController_SendFreeChat_Combined
{
    public static bool Prefix(ChatController __instance)
    {
        if (!CheatToggles.chatJailbreak) return true;

        string text = __instance.freeChatField.Text;
        string modifiedText = CensorUrlsAndEmails(text);

        ChatController.Logger.Debug("SendFreeChat() :: Sending message: '" + modifiedText + "'");
        PlayerControl.LocalPlayer.RpcSendChat(modifiedText);

        return false;
    }

    private static string CensorUrlsAndEmails(string text)
    {
        string pattern = @"(http[s]?://)?([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,6}(/[\w-./?%&=]*)?|([a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+)";
        Regex regex = new Regex(pattern);
        return regex.Replace(text, match =>
        {
            string v = match.Value;
            v = v.Replace('.', ',');
            return v;
        });
    }
}

[HarmonyPatch(typeof(FreeChatInputField), nameof(FreeChatInputField.UpdateCharCount))]
public static class EditColorIndicators_FreeChatInputField_UpdateCharCount_Postfix
{
    /// <summary>
    /// Update the character count color indicator based on the current text length.
    /// </summary>
    /// <param name="__instance">The <c>FreeChatInputField</c> instance.</param>
    public static void Postfix(FreeChatInputField __instance)
    {
        if (!CheatToggles.chatJailbreak)
        {
            return; // Only works if CheatToggles.chatJailbreak is enabled
        }

            var length = __instance.textArea.text.Length;
            // Show new character limit below text field
            __instance.charCountText.SetText($"{length}/{__instance.textArea.characterLimit}");

__instance.charCountText.color = length switch
{
    < 90 => Color.black,
    < 120 => new Color(1f, 1f, 0f, 1f),
    _ => Color.red
};

    }
}

[HarmonyPatch(typeof(ChatController), nameof(ChatController.SendChat))]
public static class ChatHistory_ChatController_SendChat_Prefix
{
    public static readonly List<string> ChatHistory = new();
    private static string TargetLanguage = "en";
    private static bool SendEnabled = false;

    // Google Translate supported languages
    private static readonly Dictionary<string, string> Languages = new()
    {
        {"auto","Automatic"},{"af","Afrikaans"},{"sq","Albanian"},{"am","Amharic"},{"ar","Arabic"},{"hy","Armenian"},{"az","Azerbaijani"},
        {"eu","Basque"},{"be","Belarusian"},{"bn","Bengali"},{"bs","Bosnian"},{"bg","Bulgarian"},{"ca","Catalan"},{"ceb","Cebuano"},{"ny","Chichewa"},
        {"zh-cn","Chinese Simplified"},{"zh-tw","Chinese Traditional"},{"co","Corsican"},{"hr","Croatian"},{"cs","Czech"},{"da","Danish"},
        {"nl","Dutch"},{"en","English"},{"eo","Esperanto"},{"et","Estonian"},{"tl","Filipino"},{"fi","Finnish"},{"fr","French"},{"fy","Frisian"},
        {"gl","Galician"},{"ka","Georgian"},{"de","German"},{"el","Greek"},{"gu","Gujarati"},{"ht","Haitian Creole"},{"ha","Hausa"},{"haw","Hawaiian"},
        {"iw","Hebrew"},{"hi","Hindi"},{"hmn","Hmong"},{"hu","Hungarian"},{"is","Icelandic"},{"ig","Igbo"},{"id","Indonesian"},{"ga","Irish"},
        {"it","Italian"},{"ja","Japanese"},{"jw","Javanese"},{"kn","Kannada"},{"kk","Kazakh"},{"km","Khmer"},{"ko","Korean"},{"ku","Kurdish (Kurmanji)"},
        {"ky","Kyrgyz"},{"lo","Lao"},{"la","Latin"},{"lv","Latvian"},{"lt","Lithuanian"},{"lb","Luxembourgish"},{"mk","Macedonian"},{"mg","Malagasy"},
        {"ms","Malay"},{"ml","Malayalam"},{"mt","Maltese"},{"mi","Maori"},{"mr","Marathi"},{"mn","Mongolian"},{"my","Myanmar (Burmese)"},{"ne","Nepali"},
        {"no","Norwegian"},{"ps","Pashto"},{"fa","Persian"},{"pl","Polish"},{"pt","Portuguese"},{"pa","Punjabi"},{"ro","Romanian"},{"ru","Russian"},
        {"sm","Samoan"},{"gd","Scots Gaelic"},{"sr","Serbian"},{"st","Sesotho"},{"sn","Shona"},{"sd","Sindhi"},{"si","Sinhala"},{"sk","Slovak"},
        {"sl","Slovenian"},{"so","Somali"},{"es","Spanish"},{"su","Sundanese"},{"sw","Swahili"},{"sv","Swedish"},{"tg","Tajik"},{"ta","Tamil"},
        {"te","Telugu"},{"th","Thai"},{"tr","Turkish"},{"uk","Ukrainian"},{"ur","Urdu"},{"uz","Uzbek"},{"vi","Vietnamese"},{"cy","Welsh"},
        {"xh","Xhosa"},{"yi","Yiddish"},{"yo","Yoruba"},{"zu","Zulu"}
    };

     public static bool Prefix(ChatController __instance)
    {
        string text = __instance.freeChatField.textArea.text;
        if (string.IsNullOrWhiteSpace(text) || __instance.quickChatField.Visible)
            return true;

        if (text.StartsWith(">")) // language command
        {
            string cmd = text[1..].ToLower();
            if (cmd == "d")
            {
                SendEnabled = false;
            }
            else if (Languages.ContainsKey(cmd))
            {
                TargetLanguage = cmd;
                SendEnabled = true;
            }

            __instance.freeChatField.Clear();
            return false;
        }

        if (SendEnabled)
        {
            string original = text;
            __instance.freeChatField.Clear(); // clear input so it doesn't send original

            Task.Run(async () =>
            {
                string translated = await TranslateText(original, TargetLanguage);
                SendTranslatedMessage(__instance, translated);
            });

            return false;
        }

        return true;
    }

    private static async Task<string> TranslateText(string text, string target)
    {
        try
        {
            using var client = new HttpClient();
            string url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl=auto&tl={target}&dt=t&q={System.Uri.EscapeDataString(text)}";
            string response = await client.GetStringAsync(url);

            var doc = JsonDocument.Parse(response);
            string translated = "";
            foreach (var item in doc.RootElement[0].EnumerateArray())
                translated += item[0].GetString();

            return translated;
        }
        catch
        {
            return text;
        }
    }

    private static void SendTranslatedMessage(ChatController chat, string msg)
    {
        // replicate original SendChat logic
        chat.timeSinceLastMessage = 3f;
        chat.freeChatField.textArea.text = msg;

        for (int i = 0; i < chat.freeChatField.textArea.text.Length; i++)
        {
            chat.SendChat(); // call the normal send logic with the translated text
        }

        chat.freeChatField.Clear();
    }
}

[HarmonyPatch(typeof(ChatController), nameof(ChatController.Awake))]
public static class ChatHistoryLimit_ChatController_Awake_Postfix
{
    /// <summary>
    /// Modify the maximum amount of chat messages to keep in the chat history.
    /// </summary>
    /// <param name="__instance">The <c>ChatController</c> instance.</param>
    /// <returns><c>false</c> to skip the original method, <c>true</c> to allow the original method to run.</returns>
    public static void Postfix(ChatController __instance)
    {
        __instance.chatBubblePool.poolSize = 20;
        // Call ReclaimOldest so the pool is re-initialized with our new size
        __instance.chatBubblePool.ReclaimOldest();
    }
}