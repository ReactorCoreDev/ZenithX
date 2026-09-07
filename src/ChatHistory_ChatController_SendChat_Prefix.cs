using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(ChatController), "SendChat")]
public static class ChatHistory_ChatController_SendChat_Prefix
{
	public static readonly Dictionary<string, string> Languages = new Dictionary<string, string>
	{
		{ "auto", "Automatic" },
		{ "af", "Afrikaans" },
		{ "sq", "Albanian" },
		{ "am", "Amharic" },
		{ "ar", "Arabic" },
		{ "hy", "Armenian" },
		{ "az", "Azerbaijani" },
		{ "eu", "Basque" },
		{ "be", "Belarusian" },
		{ "bn", "Bengali" },
		{ "bs", "Bosnian" },
		{ "bg", "Bulgarian" },
		{ "ca", "Catalan" },
		{ "ceb", "Cebuano" },
		{ "ny", "Chichewa" },
		{ "zh-cn", "Chinese Simplified" },
		{ "zh-tw", "Chinese Traditional" },
		{ "co", "Corsican" },
		{ "hr", "Croatian" },
		{ "cs", "Czech" },
		{ "da", "Danish" },
		{ "nl", "Dutch" },
		{ "en", "English" },
		{ "eo", "Esperanto" },
		{ "et", "Estonian" },
		{ "tl", "Filipino" },
		{ "fi", "Finnish" },
		{ "fr", "French" },
		{ "fy", "Frisian" },
		{ "gl", "Galician" },
		{ "ka", "Georgian" },
		{ "de", "German" },
		{ "el", "Greek" },
		{ "gu", "Gujarati" },
		{ "ht", "Haitian Creole" },
		{ "ha", "Hausa" },
		{ "haw", "Hawaiian" },
		{ "iw", "Hebrew" },
		{ "hi", "Hindi" },
		{ "hmn", "Hmong" },
		{ "hu", "Hungarian" },
		{ "is", "Icelandic" },
		{ "ig", "Igbo" },
		{ "id", "Indonesian" },
		{ "ga", "Irish" },
		{ "it", "Italian" },
		{ "ja", "Japanese" },
		{ "jw", "Javanese" },
		{ "kn", "Kannada" },
		{ "kk", "Kazakh" },
		{ "km", "Khmer" },
		{ "ko", "Korean" },
		{ "ku", "Kurdish (Kurmanji)" },
		{ "ky", "Kyrgyz" },
		{ "lo", "Lao" },
		{ "la", "Latin" },
		{ "lv", "Latvian" },
		{ "lt", "Lithuanian" },
		{ "lb", "Luxembourgish" },
		{ "mk", "Macedonian" },
		{ "mg", "Malagasy" },
		{ "ms", "Malay" },
		{ "ml", "Malayalam" },
		{ "mt", "Maltese" },
		{ "mi", "Maori" },
		{ "mr", "Marathi" },
		{ "mn", "Mongolian" },
		{ "my", "Myanmar (Burmese)" },
		{ "ne", "Nepali" },
		{ "no", "Norwegian" },
		{ "ps", "Pashto" },
		{ "fa", "Persian" },
		{ "pl", "Polish" },
		{ "pt", "Portuguese" },
		{ "pa", "Punjabi" },
		{ "ro", "Romanian" },
		{ "ru", "Russian" },
		{ "sm", "Samoan" },
		{ "gd", "Scots Gaelic" },
		{ "sr", "Serbian" },
		{ "st", "Sesotho" },
		{ "sn", "Shona" },
		{ "sd", "Sindhi" },
		{ "si", "Sinhala" },
		{ "sk", "Slovak" },
		{ "sl", "Slovenian" },
		{ "so", "Somali" },
		{ "es", "Spanish" },
		{ "su", "Sundanese" },
		{ "sw", "Swahili" },
		{ "sv", "Swedish" },
		{ "tg", "Tajik" },
		{ "ta", "Tamil" },
		{ "te", "Telugu" },
		{ "th", "Thai" },
		{ "tr", "Turkish" },
		{ "uk", "Ukrainian" },
		{ "ur", "Urdu" },
		{ "uz", "Uzbek" },
		{ "vi", "Vietnamese" },
		{ "cy", "Welsh" },
		{ "xh", "Xhosa" },
		{ "yi", "Yiddish" },
		{ "yo", "Yoruba" },
		{ "zu", "Zulu" }
	};

	public static readonly List<string> ChatHistory = new List<string>();

	public static string TargetLanguage = "en";

	public static string SavedLanguage = "en";

	public static bool SendEnabled = false;

	public static string Translate(string text, string target)
	{
		try
		{
			using HttpClient httpClient = new HttpClient();
			httpClient.Timeout = TimeSpan.FromSeconds(6.0);
			string requestUri = "https://translate.googleapis.com/translate_a/single?client=gtx&sl=auto&tl=" + target + "&dt=t&q=" + Uri.EscapeDataString(text);
			string result = httpClient.GetStringAsync(requestUri).GetAwaiter().GetResult();
			if (CheatToggles.debugMode)
			{
				ZenithX.Log("[Translater] Response: " + result);
			}
			using JsonDocument jsonDocument = JsonDocument.Parse(result);
			string text2 = "";
			if (CheatToggles.debugMode)
			{
				ZenithX.Log($"[Translater] Json: {jsonDocument}");
			}
			if (jsonDocument.RootElement.ValueKind == JsonValueKind.Array && jsonDocument.RootElement.GetArrayLength() > 0)
			{
				foreach (JsonElement item in jsonDocument.RootElement[0].EnumerateArray())
				{
					if (item.ValueKind == JsonValueKind.Array && item.GetArrayLength() > 0)
					{
						text2 += item[0].GetString();
					}
				}
			}
			if (CheatToggles.debugMode)
			{
				ZenithX.Log("[Translater] Translated: " + text2.Trim());
			}
			return text2.Trim();
		}
		catch (Exception ex)
		{
			ZenithX.Error("[Translator] Failed: " + ex.Message + " | Text: " + text);
			return null;
		}
	}

	public static bool Prefix(ChatController __instance)
	{
		if (((AbstractChatInputField)__instance.quickChatField).Visible)
		{
			if (CheatToggles.debugMode)
			{
				ZenithX.Log("[Translator] Quick Chat mode skipping translation");
			}
			return true;
		}
		string text = __instance.freeChatField.textArea.text?.Trim();
		if (string.IsNullOrWhiteSpace(text))
		{
			return true;
		}
		if (text.StartsWith("!t ") || text.StartsWith("!translator "))
		{
			string cmdRaw = (text.Contains("!translator ") ? text.Substring(12).Trim() : text.Substring(3).Trim());
			string text2 = cmdRaw.ToLowerInvariant();
			if (text2 == "d" || text2 == "disable")
			{
				SendEnabled = false;
				SavedLanguage = null;
				((AbstractChatInputField)__instance.freeChatField).Clear();
				__instance.AddChat(PlayerControl.LocalPlayer, "<color=#00FF00>[Translator] Disabled</color>", true);
				return false;
			}
			string text3 = null;
			if (Languages.TryGetValue(text2, out var _))
			{
				text3 = text2;
			}
			else
			{
				KeyValuePair<string, string> keyValuePair = Languages.FirstOrDefault((KeyValuePair<string, string> x) => x.Value.Equals(cmdRaw, StringComparison.OrdinalIgnoreCase));
				if (keyValuePair.Key != null)
				{
					text3 = keyValuePair.Key;
				}
			}
			if (text3 != null)
			{
				TargetLanguage = text3;
				SendEnabled = true;
				SavedLanguage = text3;
				((AbstractChatInputField)__instance.freeChatField).Clear();
				__instance.AddChat(PlayerControl.LocalPlayer, $"<color=#00FF00>[Translator] Set to {Languages[text3]} ({text3})</color>", true);
			}
			else
			{
				__instance.AddChat(PlayerControl.LocalPlayer, "<color=#FF0000>[Translator] Unknown language: " + cmdRaw + "</color>", true);
			}
			return false;
		}
		if (!SendEnabled)
		{
			return true;
		}
		string text4 = Translate(text, TargetLanguage);
		if (string.IsNullOrEmpty(text4) || text4 == text)
		{
			if (CheatToggles.debugMode)
			{
				ZenithX.Log("[Translator] Translation failed sending original");
			}
			return true;
		}
		((AbstractChatInputField)__instance.freeChatField).Clear();
		string item = "<color=#FFD700>[TR " + TargetLanguage.ToUpper() + "]</color> " + text4;
		Utils.SendMessage(text4);
		ChatHistory.Add(item);
		if (CheatToggles.debugMode)
		{
			ZenithX.Log($"[Translator] Sent: {text4} (original: {text})");
		}
		return false;
	}
}
