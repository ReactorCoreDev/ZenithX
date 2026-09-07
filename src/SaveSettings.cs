using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;

namespace ZenithX;

public static class SaveSettings
{
	private static Timer updateTimer;

	private static int saveRequested;

	private static ConfigFile ConfigRef;

	private static readonly Dictionary<string, ConfigEntry<bool>> Entries = new Dictionary<string, ConfigEntry<bool>>();

	private static ConfigEntry<bool> AutoSaveEntry;

	private static ConfigEntry<bool> LogGamesEntry;

	private static ConfigEntry<bool> LogGamesConfiguredEntry;

	private static ConfigEntry<bool> LegacyConfigMigratedEntry;

	private static readonly string ConfigurationsPath = Path.Combine(Paths.ConfigPath, "ZenithX", "Configurations");

	private static readonly string LegacyConfigPath = Path.Combine(Paths.ConfigPath, "ZenithX.cfg");

	public static bool AutoSaveEnabled => AutoSaveEntry?.Value ?? true;

	public static bool LogGamesEnabled => LogGamesEntry?.Value ?? false;

	public static string ConfigurationName { get; set; } = "";

	public static void LoadSettings(BasePlugin plugin)
	{
		bool flag = File.Exists(LegacyConfigPath);
		ConfigRef = plugin.Config;
		Entries.Clear();
		AutoSaveEntry = ConfigRef.Bind<bool>("ZenithX.Settings", "AutoSave", true, "Automatically save ZenithX settings changes");
		LogGamesEntry = ConfigRef.Bind<bool>("ZenithX.Settings", "LogGames", false, "Allow developers to log games you join");
		LogGamesConfiguredEntry = ConfigRef.Bind<bool>("ZenithX.Settings", "LogGamesConfigured", false, "Whether game logging consent has been answered");
		LegacyConfigMigratedEntry = ConfigRef.Bind<bool>("ZenithX.Settings", "LegacyConfigMigrated", false, "Whether the legacy ZenithX.cfg was converted");
		FieldInfo[] array = (from f in typeof(CheatToggles).GetFields(BindingFlags.Static | BindingFlags.Public)
			where f.FieldType == typeof(bool)
			select f).ToArray();
		Dictionary<FieldInfo, string> dictionary = new Dictionary<FieldInfo, string>();
		if (MenuUI.groups != null)
		{
			foreach (GroupInfo group in MenuUI.groups)
			{
				string category = "z." + group.name;
				if (group.items == null)
				{
					continue;
				}
				foreach (CreateList item in group.items)
				{
					ProcessCreateListItem(item, category, array, dictionary);
				}
			}
		}
		FieldInfo[] array2 = array;
		foreach (FieldInfo fieldInfo in array2)
		{
			string value;
			string text = (dictionary.TryGetValue(fieldInfo, out value) ? value : "z.Misc");
			bool flag2 = (bool)fieldInfo.GetValue(null);
			ConfigEntry<bool> val = ConfigRef.Bind<bool>(text, fieldInfo.Name, flag2, (ConfigDescription)null);
			Entries[fieldInfo.Name] = val;
			try
			{
				fieldInfo.SetValue(null, val.Value);
			}
			catch
			{
			}
		}
		if (AutoSaveEnabled)
		{
			try
			{
				ConfigRef.Save();
			}
			catch
			{
			}
		}
		LoadFavorites();
		if (flag)
		{
			MigrateLegacyConfiguration();
		}
		ConfigurationName = "Main";
		LoadConfiguration();
		StartTimer();
	}

	private static void ProcessCreateListItem(object item, string category, FieldInfo[] cheatFields, Dictionary<FieldInfo, string> mapping)
	{
		if (item == null)
		{
			return;
		}
		if (item is ToggleInfo toggle)
		{
			ResolveToggleMapping(toggle, cheatFields, mapping, category);
		}
		else
		{
			if (item is ButtonInfo)
			{
				return;
			}
			Type type = item.GetType();
			FieldInfo field = type.GetField("items", BindingFlags.Instance | BindingFlags.Public);
			if (field != null)
			{
				try
				{
					if (field.GetValue(item) is IEnumerable enumerable)
					{
						{
							foreach (object item2 in enumerable)
							{
								ProcessCreateListItem(item2, category, cheatFields, mapping);
							}
							return;
						}
					}
					return;
				}
				catch
				{
					return;
				}
			}
			PropertyInfo property = type.GetProperty("items", BindingFlags.Instance | BindingFlags.Public);
			if (!(property != null))
			{
				return;
			}
			try
			{
				if (!(property.GetValue(item) is IEnumerable enumerable2))
				{
					return;
				}
				foreach (object item3 in enumerable2)
				{
					ProcessCreateListItem(item3, category, cheatFields, mapping);
				}
			}
			catch
			{
			}
		}
	}

	private static void ResolveToggleMapping(ToggleInfo toggle, FieldInfo[] cheatFields, Dictionary<FieldInfo, string> mapping, string category)
	{
		bool flag = SafeGetToggleState(toggle);
		foreach (FieldInfo fieldInfo in cheatFields)
		{
			bool flag2 = (bool)fieldInfo.GetValue(null);
			bool flag3 = false;
			try
			{
				fieldInfo.SetValue(null, !flag2);
				if (SafeGetToggleState(toggle) != flag)
				{
					flag3 = true;
				}
			}
			catch
			{
				flag3 = false;
			}
			finally
			{
				try
				{
					fieldInfo.SetValue(null, flag2);
				}
				catch
				{
				}
			}
			if (flag3)
			{
				if (!mapping.ContainsKey(fieldInfo))
				{
					mapping[fieldInfo] = category;
				}
				return;
			}
		}
		foreach (FieldInfo fieldInfo2 in cheatFields)
		{
			bool flag4 = (bool)fieldInfo2.GetValue(null);
			try
			{
				SafeSetToggleState(toggle, !flag);
				if ((bool)fieldInfo2.GetValue(null) != flag4)
				{
					SafeSetToggleState(toggle, flag);
					if (!mapping.ContainsKey(fieldInfo2))
					{
						mapping[fieldInfo2] = category;
					}
					break;
				}
				SafeSetToggleState(toggle, flag);
			}
			catch
			{
				try
				{
					fieldInfo2.SetValue(null, flag4);
				}
				catch
				{
				}
			}
		}
	}

	private static bool SafeGetToggleState(ToggleInfo t)
	{
		try
		{
			return t.getState();
		}
		catch
		{
			return false;
		}
	}

	private static void SafeSetToggleState(ToggleInfo t, bool value)
	{
		try
		{
			t.setState(value);
		}
		catch
		{
		}
	}

	private static void StartTimer()
	{
		updateTimer?.Dispose();
		saveRequested = 0;
		if (AutoSaveEnabled)
		{
			updateTimer = new Timer(delegate
			{
				Interlocked.Exchange(ref saveRequested, 1);
			}, null, 60000, 60000);
		}
	}

	public static void Update()
	{
		if (Interlocked.Exchange(ref saveRequested, 0) == 1)
		{
			UpdateConfigFromCheatToggles();
		}
	}

	public static void UpdateConfigFromCheatToggles()
	{
		if (!AutoSaveEnabled || ConfigRef == null)
		{
			return;
		}
		foreach (FieldInfo item in from f in typeof(CheatToggles).GetFields(BindingFlags.Static | BindingFlags.Public)
			where f.FieldType == typeof(bool)
			select f)
		{
			if (Entries.TryGetValue(item.Name, out var value))
			{
				bool flag = (bool)item.GetValue(null);
				if (value.Value != flag)
				{
					value.Value = flag;
				}
			}
		}
		try
		{
			ConfigRef.Save();
		}
		catch
		{
		}
	}

	public static void SaveNow()
	{
		UpdateConfigFromCheatToggles();
	}

	public static void SetAutoSave(bool enabled)
	{
		if (AutoSaveEntry == null)
		{
			return;
		}
		AutoSaveEntry.Value = enabled;
		if (enabled)
		{
			StartTimer();
		}
		else
		{
			updateTimer?.Dispose();
		}
		try
		{
			ConfigRef.Save();
		}
		catch
		{
		}
	}

	public static void SaveConfiguration()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		string configurationPath = GetConfigurationPath(ConfigurationName);
		if (configurationPath == null)
		{
			return;
		}
		Directory.CreateDirectory(ConfigurationsPath);
		ConfigFile val = new ConfigFile(configurationPath, false);
		try
		{
			foreach (FieldInfo cheatField in GetCheatFields())
			{
				val.Bind<bool>("CheatToggles", cheatField.Name, (bool)cheatField.GetValue(null), (ConfigDescription)null).Value = (bool)cheatField.GetValue(null);
			}
			SaveFavoritesTo(val);
			val.Save();
			ZenithX.Log("Configuration saved: " + ConfigurationName);
		}
		catch (Exception ex)
		{
			ZenithX.Error("Failed to save configuration '" + ConfigurationName + "': " + ex.Message);
		}
	}

	public static void PromptSaveConfiguration()
	{
		AlertUI.ShowPopupInput("Enter a name for this configuration", ConfigurationName, delegate(string name)
		{
			ConfigurationName = name;
			SaveConfiguration();
		});
	}

	public static void PromptLoadConfiguration()
	{
		AlertUI.ShowPopupInput("Enter the configuration name to load", ConfigurationName, delegate(string name)
		{
			ConfigurationName = name;
			LoadConfiguration();
		}, "Load");
	}

	public static void PromptDeleteConfiguration()
	{
		AlertUI.ShowPopupInput("Enter the configuration name to delete", ConfigurationName, delegate(string name)
		{
			ConfigurationName = name;
			DeleteConfiguration();
		}, "Delete");
	}

	public static void InitializeLogGamesConsent()
	{
		if (LogGamesConfiguredEntry != null && !LogGamesConfiguredEntry.Value)
		{
			AlertUI.ShowPopupButtons("Do you want ZenithX developers to log the games you join?\nThis is used to help developers join you.", delegate
			{
				SetLogGamesConsent(enabled: true);
			}, delegate
			{
				SetLogGamesConsent(enabled: false);
			});
		}
	}

	private static void SetLogGamesConsent(bool enabled)
	{
		LogGamesEntry.Value = enabled;
		LogGamesConfiguredEntry.Value = true;
		try
		{
			ConfigRef.Save();
		}
		catch
		{
		}
	}

	public static void LoadConfiguration()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		string configurationPath = GetConfigurationPath(ConfigurationName);
		if (configurationPath == null || !File.Exists(configurationPath))
		{
			return;
		}
		ConfigFile val = new ConfigFile(configurationPath, false);
		try
		{
			foreach (FieldInfo cheatField in GetCheatFields())
			{
				ConfigEntry<bool> val2 = val.Bind<bool>("CheatToggles", cheatField.Name, (bool)cheatField.GetValue(null), (ConfigDescription)null);
				cheatField.SetValue(null, val2.Value);
				if (Entries.TryGetValue(cheatField.Name, out var value))
				{
					value.Value = val2.Value;
				}
			}
			LoadFavoritesFrom(val);
			ZenithX.Log("Configuration loaded: " + ConfigurationName);
		}
		catch (Exception ex)
		{
			ZenithX.Error("Failed to load configuration '" + ConfigurationName + "': " + ex.Message);
		}
	}

	public static void DeleteConfiguration()
	{
		string configurationPath = GetConfigurationPath(ConfigurationName);
		if (configurationPath == null)
		{
			return;
		}
		try
		{
			if (File.Exists(configurationPath))
			{
				File.Delete(configurationPath);
			}
			ZenithX.Log("Configuration deleted: " + ConfigurationName);
		}
		catch (Exception ex)
		{
			ZenithX.Error("Failed to delete configuration '" + ConfigurationName + "': " + ex.Message);
		}
	}

	public static void OpenConfigurationsFolder()
	{
		Directory.CreateDirectory(ConfigurationsPath);
		try
		{
			Process.Start(new ProcessStartInfo
			{
				FileName = ConfigurationsPath,
				UseShellExecute = true
			});
		}
		catch (Exception ex)
		{
			ZenithX.Error("Failed to open configurations folder: " + ex.Message);
		}
	}

	private static void MigrateLegacyConfiguration()
	{
		if (LegacyConfigMigratedEntry.Value)
		{
			return;
		}
		string configurationPath = GetConfigurationPath("Main");
		try
		{
			if (!File.Exists(configurationPath))
			{
				string configurationName = ConfigurationName;
				ConfigurationName = "Main";
				SaveConfiguration();
				ConfigurationName = configurationName;
			}
			LegacyConfigMigratedEntry.Value = true;
			ConfigRef.Save();
			ZenithX.Log("Converted legacy configuration to Configurations/Main.cfg");
		}
		catch (Exception ex)
		{
			ZenithX.Error("Failed to convert legacy configuration: " + ex.Message);
		}
	}

	private static IEnumerable<FieldInfo> GetCheatFields()
	{
		return from field in typeof(CheatToggles).GetFields(BindingFlags.Static | BindingFlags.Public)
			where field.FieldType == typeof(bool)
			select field;
	}

	private static string GetConfigurationPath(string name)
	{
		string text = name?.Trim();
		if (string.IsNullOrWhiteSpace(text))
		{
			return null;
		}
		char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
		foreach (char oldChar in invalidFileNameChars)
		{
			text = text.Replace(oldChar, '_');
		}
		if (text == "." || text == "..")
		{
			return null;
		}
		return Path.Combine(ConfigurationsPath, text + ".cfg");
	}

	private static void SaveFavoritesTo(ConfigFile configuration)
	{
		ConfigEntry<string> val = configuration.Bind<string>("ZenithX.Favorites", "SavedFavorites", "", (ConfigDescription)null);
		List<string> list = new List<string>();
		if (MenuUI.favoritesGroup.items != null)
		{
			foreach (CreateList item in MenuUI.favoritesGroup.items)
			{
				if (item is ToggleInfo toggleInfo)
				{
					list.Add(toggleInfo.label.Trim());
				}
			}
		}
		val.Value = string.Join(",", list);
	}

	private static void LoadFavoritesFrom(ConfigFile configuration)
	{
		ApplyFavorites(configuration.Bind<string>("ZenithX.Favorites", "SavedFavorites", "", (ConfigDescription)null).Value?.Split(',') ?? Array.Empty<string>());
	}

	private static void ApplyFavorites(IEnumerable<string> savedFavorites)
	{
		HashSet<string> hashSet = new HashSet<string>(savedFavorites.Select((string favorite) => favorite.Trim()), StringComparer.Ordinal);
		MenuUI.favoritesGroup.items.Clear();
		MenuUI.favoritesGroup.toggles.Clear();
		foreach (GroupInfo group in MenuUI.groups)
		{
			if (group.name == "Favorites" || group.items == null)
			{
				continue;
			}
			foreach (CreateList item in group.items)
			{
				if (item is ToggleInfo toggleInfo && hashSet.Contains(toggleInfo.label.Trim()))
				{
					MenuUI.favoritesGroup.items.Add(toggleInfo);
					MenuUI.favoritesGroup.toggles.Add(toggleInfo);
				}
			}
		}
	}

	public static void SaveFavorites()
	{
		ConfigEntry<string> val = ConfigRef.Bind<string>("ZenithX.Favorites", "SavedFavorites", "", "Comma-separated list of favorite toggle names");
		List<string> list = new List<string>();
		foreach (CreateList item in MenuUI.favoritesGroup.items)
		{
			if (item is ToggleInfo toggleInfo)
			{
				list.Add(toggleInfo.label.Trim());
			}
		}
		val.Value = string.Join(",", list);
		if (AutoSaveEnabled)
		{
			ConfigRef.Save();
		}
	}

	public static void LoadFavorites()
	{
		ConfigEntry<string> val = ConfigRef.Bind<string>("ZenithX.Favorites", "SavedFavorites", "", "Comma-separated list of favorite toggle names");
		if (!string.IsNullOrEmpty(val.Value))
		{
			ApplyFavorites(val.Value.Split(','));
		}
	}
}
