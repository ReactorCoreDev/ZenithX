using BepInEx;
using BepInEx.Unity.IL2CPP;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZenithX;
using Il2CppSystem;
using System.Linq;
using UnityEngine.Analytics;
using UnityEngine.CrashReportHandler;
using System.IO;
using System.Net;
using System.Net.Http;

namespace ZenithX;

[BepInAutoPlugin()]
[BepInProcess("Among Us.exe")]
public partial class ZenithX : BasePlugin
{
    public Harmony Harmony { get; } = new(Id);

    public static string ZenithXVersion = "2.7.4";
    public static List<string> supportedAU = new() { "2025.9.9" };
    public static MenuUI menuUI;
    public static ConsoleUI consoleUI;
    public static CoroutineRunner coroutineRunner;
    public static DoorsUI doorsUI;
    public static TasksUI tasksUI;
    public static ConfigEntry<string> menuKeybind;
    public static ConfigEntry<string> menuHtmlColor;
    public static ConfigEntry<string> spoofLevel;
    public static ConfigEntry<string> spoofPlatform;
    public static ConfigEntry<bool> spoofDeviceId;
    public static ConfigEntry<string> guestFriendCode;
    public static ConfigEntry<bool> guestMode;
    public static ConfigEntry<bool> noTelemetry;
    public static ConfigEntry<string> toggleZoom;
    public static ConfigEntry<bool> freeCosmetics;
    public static ConfigEntry<bool> avoidBans;
    public static ConfigEntry<bool> unlockFeatures;

    public static readonly HashSet<string> LoggedMessages = new();

    public static new void Log(string message)
    {
        if (!LoggedMessages.Add(message)) return;
        UnityEngine.Debug.Log($"[ZenithX] Info: {message}");
        consoleUI?.Log($"[Info] {message}");
    }

    public static void Warning(string message)
    {
        if (!LoggedMessages.Add(message)) return;
        UnityEngine.Debug.LogWarning($"[ZenithX] Warning: {message}");
        consoleUI?.Log($"[Warning] {message}");
    }

    public static void Error(string message)
    {
        if (!LoggedMessages.Add(message)) return;
        UnityEngine.Debug.LogError($"[ZenithX] Error: {message}");
        consoleUI?.Log($"[Error] {message}");
    }

    public static void ClearLoggedMessages() => LoggedMessages.Clear();

    private static readonly string BaseUrl = "https://github.com/ReactorCoreDev/ZenithX/raw/refs/heads/main/ZenithXFolder/Sounds/";
    private static readonly string[] SoundFiles = new string[]
    {
        "Error.wav",
        "Notification.wav",
        "Success.wav",
        "Warning.wav"
    };
    
    public override void Load()
    {
        try
        {
            Harmony.PatchAll();
            Log("Harmony patches applied.");
        }
        catch (System.Exception ex)
        {
            Error($"Harmony patch failed: {ex}");
        }

        menuKeybind = Config.Bind("ZenithX.GUI", "Keybind", "Delete", "Key to toggle the GUI on/off");
        menuHtmlColor = Config.Bind("ZenithX.GUI", "Color", "", "HTML color for GUI");

        guestMode = Config.Bind("ZenithX.GuestMode", "GuestMode", false, "Enable guest mode for bypassing bans");
        guestFriendCode = Config.Bind("ZenithX.GuestMode", "FriendName", "", "Guest friend code (≤10 chars)");

        spoofLevel = Config.Bind("ZenithX.Spoofing", "Level", "", "Custom player level to display");
        spoofPlatform = Config.Bind("ZenithX.Spoofing", "Platform", "", "Custom platform to display");

        spoofDeviceId = Config.Bind("ZenithX.Privacy", "HideDeviceId", true, "Hide unique device ID");
        noTelemetry = Config.Bind("ZenithX.Privacy", "NoTelemetry", true, "Disable Unity Analytics telemetry");

        toggleZoom = Config.Bind("ZenithX.Cheats", "ToggleZoom", "c", "Key to toggle zoom hack");

        freeCosmetics = Config.Bind("ZenithX.FreeCosmetics", "FreeCosmetics", true, "Access all cosmetics for free");
        avoidBans = Config.Bind("ZenithX.AvoidBans", "AvoidBans", true, "Remove disconnect penalties");
        unlockFeatures = Config.Bind("ZenithX.UnlockFeatures", "UnlockFeatures", true, "Unlock special features automatically");

        CheatToggles.unlockFeatures = unlockFeatures.Value;
        CheatToggles.freeCosmetics = freeCosmetics.Value;
        CheatToggles.avoidBans = avoidBans.Value;

        // Base ZenithX folder
        string basePath = System.IO.Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData),
            "ZenithX"
        );

        // Sounds folder inside ZenithX
        string soundsPath = Path.Combine(basePath, "Sounds");

        // Create folders if they don't exist
        if (!Directory.Exists(basePath))
        {
            Debug.LogWarning("[ZenithX] Main folder does not exist. Creating new.");
            Directory.CreateDirectory(basePath);
        }

        if (!Directory.Exists(soundsPath))
        {
            Debug.LogWarning("[ZenithX] Sounds folder does not exist. Creating new.");
            Directory.CreateDirectory(soundsPath);
        }

        using HttpClient client = new HttpClient();

        foreach (string fileName in SoundFiles)
        {
            string localPath = Path.Combine(soundsPath, fileName);
            string url = BaseUrl + fileName;

            if (!File.Exists(localPath))
            {
                try
                {
                    // Synchronous download
                    byte[] data = client.GetByteArrayAsync(url).Result;
                    File.WriteAllBytes(localPath, data);
                    Debug.Log($"[ZenithX] Downloaded {fileName} to {localPath}");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[ZenithX] Failed to download {fileName}: {e.Message}");
                }
            }
            else
            {
                Debug.Log($"[ZenithX] {fileName} already exists, skipping.");
            }
        }
    
        SaveSettings.LoadSettings(this);

        menuUI = AddComponent<MenuUI>();
        consoleUI = AddComponent<ConsoleUI>();
        coroutineRunner = AddComponent<CoroutineRunner>();
        doorsUI = AddComponent<DoorsUI>();
        tasksUI = AddComponent<TasksUI>();

        ZenithXSoundManager.Initialize();

        if (noTelemetry.Value)
        {
            Analytics.deviceStatsEnabled = false;
            Analytics.enabled = false;
            Analytics.initializeOnStartup = false;
            Analytics.limitUserTracking = true;
            CrashReportHandler.enableCaptureExceptions = false;
            PerformanceReporting.enabled = false;
            // If Among Us updates their IAP
              // using Unity.Services.Analytics;
              // using Unity.Services.Core;
              // AnalyticsService.Instance.OptOut();
            // More Info: https://discussions.unity.com/t/iap-privacy-issue/881743
        }

        SceneManager.add_sceneLoaded((System.Action<Scene, LoadSceneMode>)((scene, _) =>
        {
            if (scene.name != "MainMenu") return;

            ModManager.Instance.ShowModStamp();

            if (!supportedAU.Contains(Application.version))
                Utils.showPopup("\nThis version of ZenithX and Among Us are incompatible\n\nInstall the right version of ZenithX to avoid problems");

            Log($"ZenithX v{ZenithXVersion} loaded");
            Log($"Among Us Version: {Application.version}");

            TestAccessHelper.Start();

            CheatToggles.init = true;
        }));
    }
}
