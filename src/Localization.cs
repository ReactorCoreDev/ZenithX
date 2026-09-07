using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ZenithX;

public static class Localization
{
	private static readonly HttpClient Client = new HttpClient
	{
		Timeout = TimeSpan.FromSeconds(6.0)
	};

	private static readonly Dictionary<string, Dictionary<string, string>> Cache = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

	private static readonly Dictionary<string, HashSet<string>> InFlight = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

	private static readonly object CacheLock = new object();

	public static string CurrentLanguage
	{
		get
		{
			return ZenithX.selectedLanguage.Value;
		}
		set
		{
			string value2 = ZenithX.selectedLanguage.Value;
			ZenithX.selectedLanguage.Value = value;
			if (!string.Equals(value2, value, StringComparison.OrdinalIgnoreCase) && CheatToggles.debugMode)
			{
				ZenithX.Log("[Localization] Language changed: " + value2 + " -> " + value);
			}
		}
	}

	public static string Translate(string key)
	{
		if (string.IsNullOrWhiteSpace(key))
		{
			return key;
		}
		key = key.Trim();
		string text = CurrentLanguage?.Trim().ToLowerInvariant();
		if (string.IsNullOrEmpty(text) || text == "auto" || text == "en")
		{
			return key;
		}
		lock (CacheLock)
		{
			if (Cache.TryGetValue(text, out var value) && value.TryGetValue(key, out var value2))
			{
				return value2;
			}
			if (!InFlight.TryGetValue(text, out var value3))
			{
				value3 = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
				InFlight[text] = value3;
			}
			if (!value3.Contains(key))
			{
				value3.Add(key);
				TranslateAsync(key, text);
			}
		}
		return key;
	}

	private static async Task TranslateAsync(string key, string language)
	{
		try
		{
			string requestUri = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl=auto&tl={Uri.EscapeDataString(language)}&dt=t&q={Uri.EscapeDataString(key)}";
			string text = ParseTranslation(await Client.GetStringAsync(requestUri));
			if (string.IsNullOrWhiteSpace(text))
			{
				text = key;
			}
			text = text.Trim();
			lock (CacheLock)
			{
				if (!Cache.TryGetValue(language, out var value))
				{
					value = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
					Cache[language] = value;
				}
				value[key] = text;
				if (InFlight.TryGetValue(language, out var value2))
				{
					value2.Remove(key);
					if (value2.Count == 0)
					{
						InFlight.Remove(language);
					}
				}
			}
			if (CheatToggles.debugMode)
			{
				ZenithX.Log($"[Localization] {key} -> {text} ({language})");
			}
		}
		catch (Exception ex)
		{
			lock (CacheLock)
			{
				if (InFlight.TryGetValue(language, out var value3))
				{
					value3.Remove(key);
					if (value3.Count == 0)
					{
						InFlight.Remove(language);
					}
				}
			}
			if (CheatToggles.debugMode)
			{
				ZenithX.Error($"[Localization] Failed translating '{key}' ({language}): {ex.Message}");
			}
		}
	}

	private static string ParseTranslation(string response)
	{
		try
		{
			using JsonDocument jsonDocument = JsonDocument.Parse(response);
			if (jsonDocument.RootElement.ValueKind != JsonValueKind.Array)
			{
				return "";
			}
			if (jsonDocument.RootElement.GetArrayLength() == 0)
			{
				return "";
			}
			JsonElement jsonElement = jsonDocument.RootElement[0];
			if (jsonElement.ValueKind != JsonValueKind.Array)
			{
				return "";
			}
			string text = "";
			foreach (JsonElement item in jsonElement.EnumerateArray())
			{
				if (item.ValueKind == JsonValueKind.Array && item.GetArrayLength() != 0)
				{
					JsonElement jsonElement2 = item[0];
					if (jsonElement2.ValueKind == JsonValueKind.String)
					{
						text += jsonElement2.GetString();
					}
				}
			}
			return text.Trim();
		}
		catch
		{
			return "";
		}
	}

	public static bool TryGetCached(string key, out string translated)
	{
		translated = null;
		if (string.IsNullOrWhiteSpace(key))
		{
			return false;
		}
		key = key.Trim();
		string text = CurrentLanguage?.Trim().ToLowerInvariant();
		if (string.IsNullOrEmpty(text) || text == "auto" || text == "en")
		{
			translated = key;
			return true;
		}
		lock (CacheLock)
		{
			if (Cache.TryGetValue(text, out var value) && value.TryGetValue(key, out translated))
			{
				return true;
			}
		}
		return false;
	}

	public static void ClearCache()
	{
		lock (CacheLock)
		{
			Cache.Clear();
		}
	}

	public static void ClearLanguageCache(string language)
	{
		if (string.IsNullOrWhiteSpace(language))
		{
			return;
		}
		language = language.Trim().ToLowerInvariant();
		lock (CacheLock)
		{
			Cache.Remove(language);
			InFlight.Remove(language);
		}
	}
}
