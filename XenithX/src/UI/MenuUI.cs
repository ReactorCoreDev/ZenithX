using UnityEngine;
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using System.IO;

namespace ZenithX;

public class MenuUI : MonoBehaviour
{
    public static List<GroupInfo> groups = new List<GroupInfo>();
    private bool isDragging = false;
    private Rect windowRect = new Rect(10, 10, 300, 500);
    private bool isGUIActive = false;
    private static readonly Color gradientTop = new Color(0.678f, 0.847f, 0.902f); // LightBlue (173, 216, 230)
    private static readonly Color gradientBottom = Color.blue;                      // Pure Blue (0, 0, 255)
    private Texture2D whiteTex;
    private static GUIStyle buttonLabelStyle;
    private GUIStyle submenuLabelStyle;
    private GUIStyle toggleLabelStyle;
    private static Dictionary<string, float> buttonClickTimes = new Dictionary<string, float>();
    private static bool showPopup = false;
    private static string popupText = "";
    private static System.Action popupOkAction;
    private static bool showPopupButtons = false;
    private static string popupButtonsText = "";
    private static System.Action popupYesAction;
    private static System.Action popupNoAction;
    
    // Alert types enum
    [System.Serializable]
    public enum AlertType {
        Notification,
        Success,
        Warning,
        Error
    }

    class GuiNotification
    {
        public string text;
        public Color color;
       public float timer;
       public float maxTime;
       public float posY; // For animation
       public float alpha;
       public float glowTime;
       public AlertType type;
    }

    // LightBlue → Blue
    public static Color GradientColor
    {
        get { return Color.Lerp(gradientTop, gradientBottom, 0.5f); }
    }

    // Optional: get gradient color at position t (0 = LightBlue, 1 = Blue)
    public static Color GetGradientColor(float t)
    {
        return Color.Lerp(gradientTop, gradientBottom, Mathf.Clamp01(t));
    }

    public static bool TryParseHtmlString(string hex, out Color uiColor)
    {
        uiColor = Color.white;

        if (string.IsNullOrEmpty(hex))
            return false;

        try
        {
            if (hex.StartsWith("#"))
                hex = hex.Substring(1);

            if (hex.Length != 6 && hex.Length != 8)
                return false;

            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            byte a = 255;

            if (hex.Length == 8)
                a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);

            uiColor = new Color32(r, g, b, a);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private Texture2D MakeSolidTexture(int width, int height, Color color)
    {
        Texture2D tex = new Texture2D(width, height);
        Color[] colors = new Color[width * height];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = color;
        }
        tex.SetPixels(colors);
        tex.Apply();
        return tex;
    }
    private void Other()
    {
        whiteTex = MakeSolidTexture(2, 2, Color.white);

        buttonLabelStyle = new GUIStyle();
        buttonLabelStyle.normal.textColor = Color.white;
        buttonLabelStyle.alignment = TextAnchor.MiddleCenter;
        buttonLabelStyle.fontSize = 17;

        submenuLabelStyle = new GUIStyle(buttonLabelStyle);
        submenuLabelStyle.fontSize = 15;

        toggleLabelStyle = new GUIStyle();
        toggleLabelStyle.normal.textColor = Color.white;
        toggleLabelStyle.alignment = TextAnchor.MiddleLeft;
        toggleLabelStyle.fontSize = 17;
    }
    // Create all groups (buttons) and their toggles on start
    private void Start()
    {
        groups.Add(new GroupInfo("Player", false, new List<CreateList>() {
            new ToggleInfo(" NoClip", () => CheatToggles.noClip, x => CheatToggles.noClip = x),
            new ToggleInfo(" SpeedHack", () => CheatToggles.speedBoost, x => CheatToggles.speedBoost = x),
            new ToggleInfo(" Revive self", () => CheatToggles.revive, x => CheatToggles.revive = x),
            new ToggleInfo(" Force Aum Rpc For Everyone", () => CheatToggles.forceAumRpcForEveryone, x => CheatToggles.forceAumRpcForEveryone = x),
            new ToggleInfo(" No Ability Cooldown", () => CheatToggles.noAbilityCD, x => CheatToggles.noAbilityCD = x),
            new ToggleInfo(" Invert Controls", () => CheatToggles.invertControls, x => CheatToggles.invertControls = x)
            }, new List<SubmenuInfo> {
            new SubmenuInfo("Teleport", false, new List<CreateList>() {
                new ToggleInfo(" to Cursor", () => CheatToggles.teleportCursor, x => CheatToggles.teleportCursor = x),
                new ToggleInfo(" to Player", () => CheatToggles.teleportPlayer, x => CheatToggles.teleportPlayer = x),
            }),
            new SubmenuInfo("Murder", false, new List<CreateList>() {
                new ToggleInfo(" Murder Player", () => CheatToggles.murderPlayer, x => CheatToggles.murderPlayer = x),
                new ToggleInfo(" Murder All", () => CheatToggles.murderAll, x => CheatToggles.murderAll = x),
            }),
        }));

        groups.Add(new GroupInfo("ESP", false, new List<CreateList>() {
            new ToggleInfo(" Show Player Info", () => CheatToggles.showPlayerInfo, x => CheatToggles.showPlayerInfo = x),
            new ToggleInfo(" See Roles", () => CheatToggles.seeRoles, x => CheatToggles.seeRoles = x),
            new ToggleInfo(" See Ghosts", () => CheatToggles.seeGhosts, x => CheatToggles.seeGhosts = x),
            new ToggleInfo(" No Shadows", () => CheatToggles.fullBright, x => CheatToggles.fullBright = x),
            new ToggleInfo(" Reveal Votes", () => CheatToggles.revealVotes, x => CheatToggles.revealVotes = x),
            new ToggleInfo(" More Lobby Info", () => CheatToggles.moreLobbyInfo, x => CheatToggles.moreLobbyInfo = x)
        }, new List<SubmenuInfo> {
            new SubmenuInfo("Camera", false, new List<CreateList>() {
                new ToggleInfo(" Zoom Out", () => CheatToggles.zoomOut, x => CheatToggles.zoomOut = x),
                new ToggleInfo(" Spectate", () => CheatToggles.spectate, x => CheatToggles.spectate = x),
                new ToggleInfo(" Freecam", () => CheatToggles.freecam, x => CheatToggles.freecam = x)
            }),
            new SubmenuInfo("Tracers", false, new List<CreateList>() {
                new ToggleInfo(" Crewmates", () => CheatToggles.tracersCrew, x => CheatToggles.tracersCrew = x),
                new ToggleInfo(" Impostors", () => CheatToggles.tracersImps, x => CheatToggles.tracersImps = x),
                new ToggleInfo(" Ghosts", () => CheatToggles.tracersGhosts, x => CheatToggles.tracersGhosts = x),
                new ToggleInfo(" Dead Bodies", () => CheatToggles.tracersBodies, x => CheatToggles.tracersBodies = x),
                new ToggleInfo(" Color-based", () => CheatToggles.colorBasedTracers, x => CheatToggles.colorBasedTracers = x),
            }),
            new SubmenuInfo("Minimap", false, new List<CreateList>() {
                new ToggleInfo(" Crewmates", () => CheatToggles.mapCrew, x => CheatToggles.mapCrew = x),
                new ToggleInfo(" Impostors", () => CheatToggles.mapImps, x => CheatToggles.mapImps = x),
                new ToggleInfo(" Ghosts", () => CheatToggles.mapGhosts, x => CheatToggles.mapGhosts = x),
                new ToggleInfo(" Color-based", () => CheatToggles.colorBasedMap, x => CheatToggles.colorBasedMap = x)
            }),
        }));

        groups.Add(new GroupInfo("Roles", false, new List<CreateList>() {
            new ToggleInfo(" Set Fake Role", () => CheatToggles.changeRole, x => CheatToggles.changeRole = x),
        },
            new List<SubmenuInfo> {
                new SubmenuInfo("Impostor", false, new List<CreateList>() {
                    new ToggleInfo(" Kill Reach", () => CheatToggles.killReach, x => CheatToggles.killReach = x),
                    new ToggleInfo(" Kill Anyone", () => CheatToggles.killAnyone, x => CheatToggles.killAnyone = x),
                    new ToggleInfo(" No Kill Cooldown", () => CheatToggles.zeroKillCd, x => CheatToggles.zeroKillCd = x),
                    new ToggleInfo(" Auto Kill Nearby", () => CheatToggles.autoKillNearby, x => CheatToggles.autoKillNearby = x),
                }),
                new SubmenuInfo("Shapeshifter", false, new List<CreateList>() {
                    new ToggleInfo(" No Ss Animation", () => CheatToggles.noShapeshiftAnim, x => CheatToggles.noShapeshiftAnim = x),
                    new ToggleInfo(" Endless Ss Duration", () => CheatToggles.endlessSsDuration, x => CheatToggles.endlessSsDuration = x),
                }),
                new SubmenuInfo("Crewmate", false, new List<CreateList>() {
                    new ToggleInfo(" Task Menu", () => TasksUI.isVisible, x => TasksUI.isVisible = x),
                }),
                new SubmenuInfo("Tracker", false, new List<CreateList>() {
                    new ToggleInfo(" Endless Tracking", () => CheatToggles.endlessTracking, x => CheatToggles.endlessTracking = x),
                    new ToggleInfo(" No Track Delay", () => CheatToggles.noTrackingDelay, x => CheatToggles.noTrackingDelay = x),
                    new ToggleInfo(" No Track Cooldown", () => CheatToggles.noTrackingCooldown, x => CheatToggles.noTrackingCooldown = x),
                }),
                new SubmenuInfo("Engineer", false, new List<CreateList>() {
                    new ToggleInfo(" Endless Vent Time", () => CheatToggles.endlessVentTime, x => CheatToggles.endlessVentTime = x),
                    new ToggleInfo(" No Vent Cooldown", () => CheatToggles.noVentCooldown, x => CheatToggles.noVentCooldown = x),
                }),
                new SubmenuInfo("Scientist", false, new List<CreateList>() {
                    new ToggleInfo(" Endless Battery", () => CheatToggles.endlessBattery, x => CheatToggles.endlessBattery = x),
                    new ToggleInfo(" No Vitals Cooldown", () => CheatToggles.noVitalsCooldown, x => CheatToggles.noVitalsCooldown = x),
                }),
            }));

        groups.Add(new GroupInfo("Ship", false, new List<CreateList> {
            new ToggleInfo(" Change map to sabotage map", () => CheatToggles.changeMapToSabotage, x => CheatToggles.changeMapToSabotage = x),
            new ToggleInfo(" Auto-Open Doors On Use", () => CheatToggles.autoOpenDoorsOnUse, x => CheatToggles.autoOpenDoorsOnUse = x),
            new ToggleInfo(" Unfixable Lights", () => CheatToggles.unfixableLights, x => CheatToggles.unfixableLights = x),
            new ToggleInfo(" Infinite Doors", () => CheatToggles.spamCloseAllDoors, x => CheatToggles.spamCloseAllDoors = x),
            new ToggleInfo(" Report Body", () => CheatToggles.reportBody, x => CheatToggles.reportBody = x),
            new ToggleInfo(" Close Meeting", () => CheatToggles.closeMeeting, x => CheatToggles.closeMeeting = x),
        }, new List<SubmenuInfo> {
            new SubmenuInfo("Sabotage", false, new List<CreateList>() {
                new ToggleInfo(" Reactor", () => CheatToggles.reactorSab, x => CheatToggles.reactorSab = x),
                new ToggleInfo(" Oxygen", () => CheatToggles.oxygenSab, x => CheatToggles.oxygenSab = x),
                new ToggleInfo(" Lights", () => CheatToggles.elecSab, x => CheatToggles.elecSab = x),
                new ToggleInfo(" Comms", () => CheatToggles.commsSab, x => CheatToggles.commsSab = x),
                new ToggleInfo(" Doors Menu", () => DoorsUI.isVisible, x => DoorsUI.isVisible = x),
                new ToggleInfo(" MushroomMixup", () => CheatToggles.mushSab, x => CheatToggles.mushSab = x),
            }),
            new SubmenuInfo("Vents", false, new List<CreateList>() {
                new ToggleInfo(" Unlock Vents", () => CheatToggles.useVents, x => CheatToggles.useVents = x),
                new ToggleInfo(" Kick All From Vents", () => CheatToggles.kickVents, x => CheatToggles.kickVents = x),
                new ToggleInfo(" Walk In Vents", () => CheatToggles.walkVent, x => CheatToggles.walkVent = x)
            }),
        }));

        groups.Add(new GroupInfo("Chat", false, new List<CreateList>() {
            new ToggleInfo(" Enable Chat", () => CheatToggles.alwaysChat, x => CheatToggles.alwaysChat = x),
            new ToggleInfo(" Chat Features", () => CheatToggles.chatJailbreak, x => CheatToggles.chatJailbreak = x)
        }, []));

        groups.Add(new GroupInfo("Host-Only", false, new List<CreateList>
        {
            new ButtonInfo(" Force Start", () => ZenithXCheats.forceStartGameCheat()),
            new ButtonInfo(" Dead Troll", () => ZenithXCheats.callMeeting()),
            new ButtonInfo(" Increase lobby timer", () => ZenithXCheats.extendLobbyTimer()),
            new ButtonInfo(" Skip Meeting", () => ZenithXCheats.skipMeetingCheat()),
            new ToggleInfo(" Kill While Vanished", () => CheatToggles.killVanished, x => CheatToggles.killVanished = x),
            new ToggleInfo(" Kill Anyone", () => CheatToggles.killAnyone, x => CheatToggles.killAnyone = x),
            new ToggleInfo(" No Options Limits", () => CheatToggles.noOptionsLimits, x => CheatToggles.noOptionsLimits = x),
            new ButtonInfo(" Revive Any", () => ZenithXCheats.reviveAny()),
            new ButtonInfo(" Revive All", () => ZenithXCheats.reviveAll()),
            new ButtonInfo(" Force Imposter", () => ZenithXCheats.requestImpostorRole()),
            new ButtonInfo(" Teleport All", () => TeleportManager.TeleportAllToMe()),
        },
        new List<SubmenuInfo>
        {
            new SubmenuInfo("Buggy", false, new List<CreateList>
            {
                new ToggleInfo(" Impostor Hack", () => CheatToggles.impostorHack, x => CheatToggles.impostorHack = x),
                new ToggleInfo(" God mode", () => CheatToggles.godMode, x => CheatToggles.godMode = x),
                new ToggleInfo(" Evil Vote", () => CheatToggles.evilVote, x => CheatToggles.evilVote = x),
                new ToggleInfo(" Vote Immune", () => CheatToggles.voteImmune, x => CheatToggles.voteImmune = x)
            }),
            new SubmenuInfo("Murder", false, new List<CreateList>
           {
                new ToggleInfo(" Kill Player", () => CheatToggles.killPlayer, x => CheatToggles.killPlayer = x),
                new ToggleInfo(" Telekill Player", () => CheatToggles.telekillPlayer, x => CheatToggles.telekillPlayer = x),
                new ButtonInfo(" Kill All Crewmates", () => ZenithXCheats.killAllCrewCheat()),
                new ButtonInfo(" Kill All Impostors", () => ZenithXCheats.killAllImpsCheat()),
                new ButtonInfo(" Kill Everyone", () => ZenithXCheats.killAllCheat()),
            })
        }));

        groups.Add(new GroupInfo("Passive", false, new List<CreateList>() {
            new ToggleInfo(" Free Cosmetics", () => CheatToggles.freeCosmetics, x => CheatToggles.freeCosmetics = x),
            new ToggleInfo(" Avoid Penalties", () => CheatToggles.avoidBans, x => CheatToggles.avoidBans = x),
            new ToggleInfo(" Unlock Extra Features", () => CheatToggles.unlockFeatures, x => CheatToggles.unlockFeatures = x),
        }, []));

        groups.Add(new GroupInfo("Animations", false, [
            new ToggleInfo(" Shields", () => CheatToggles.animShields, x => CheatToggles.animShields = x),
            new ToggleInfo(" Asteroids", () => CheatToggles.animAsteroids, x => CheatToggles.animAsteroids = x),
            new ToggleInfo(" Empty Garbage", () => CheatToggles.animEmptyGarbage, x => CheatToggles.animEmptyGarbage = x),
            new ToggleInfo(" Medbay Scan", () => CheatToggles.animScan, x => CheatToggles.animScan = x),
            new ToggleInfo(" Fake Cams In Use", () => CheatToggles.animCamsInUse, x => CheatToggles.animCamsInUse = x)
        ], []));

        groups.Add(new GroupInfo("Config", false, new List<CreateList>() {
            new ToggleInfo(" Open config file", () => true, x => Utils.OpenConfigFile()),
            new ToggleInfo(" RGB Mode", () => CheatToggles.RGBMode, x => CheatToggles.RGBMode = x),
        }, []));

        groups.Add(new GroupInfo("Other", false, new List<CreateList>() {
            new ToggleInfo(" Console", () => ZenithX.consoleUI.isVisible, x => ZenithX.consoleUI.isVisible = x),
            new ButtonInfo(" Assign clean filter task", () => ZenithXCheats.assignCleanFilterTask()),
        }, new List<SubmenuInfo>
        {
            new SubmenuInfo("Notifications test", false, new List<CreateList>
            {
                new ButtonInfo(" Notification", () => alert.Notification("Notification Example")),
                new ButtonInfo(" Warning", () => alert.Warning("Warning Example")),
                new ButtonInfo(" Error", () => alert.Error("Error Example")),
                new ButtonInfo(" Popup", () => ShowPopup("Popup Example")),
                new ButtonInfo(" Popup with buttons", () => 
                    ShowPopupButtons("Popup with buttons example", 
                        () => alert.Notification("You clicked yes"), 
                        () => alert.Notification("You clicked no")
                    )
                )
            })
        }));
        
        groups.Add(new GroupInfo("Notifications", false, [
            new ToggleInfo(" On Player Death", () => CheatToggles.notifyOnDeath, x => CheatToggles.notifyOnDeath = x),
            new ToggleInfo(" On Player Disconnect", () => CheatToggles.notifyOnDisconnect, x => CheatToggles.notifyOnDisconnect = x),
            new ToggleInfo(" On Vent Usage", () => CheatToggles.notifyOnVent, x => CheatToggles.notifyOnVent = x),
        ], []));

        var submenuInfos = new List<SubmenuInfo>();

        for (int i = 0; i < PlayerStatsHandler.stats.Count; i++)
        {
            string statKey = PlayerStatsHandler.stats[i];
            string statDisplayName = PlayerStatsHandler.formattedList[i];
            submenuInfos.Add(new SubmenuInfo(statDisplayName, false, new List<CreateList>
                {
                    new ButtonInfo(" Increase", () => PlayerStatsHandler.Increase(statKey)),
                    new ButtonInfo(" Decrease", () => PlayerStatsHandler.Decrease(statKey))
                }
            ));
        }

        groups.Add(new GroupInfo(
            "Player Stats",
            false,
            new List<CreateList>(), submenuInfos
        ));

        Other();
    }

    private void Update()
    {
        if (Input.GetKeyDown(Utils.stringToKeycode(ZenithX.menuKeybind.Value)))
        {
            //Enable-disable GUI with DELETE key
            isGUIActive = !isGUIActive;

            //Also teleport the window to the mouse for immediate use
            Vector2 mousePosition = Input.mousePosition;
            windowRect.position = new Vector2(mousePosition.x, Screen.height - mousePosition.y);
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            ZenithXCheats.SetCode();
        }

        if (Input.GetKeyDown(Utils.stringToKeycode(ZenithX.toggleZoom.Value)) && DestroyableSingleton<HudManager>.Instance.Chat.IsClosedOrClosing)
        {
            // Toggle zoom hack with c key
            CheatToggles.zoomOut = !CheatToggles.zoomOut;

            // Also zoom out 2 times for immediate use and easy usablity 
            ZenithXESP.zoomToggle(DestroyableSingleton<HudManager>.Instance);
        }
        else if (Input.GetKeyUp(Utils.stringToKeycode(ZenithX.toggleZoom.Value)) && DestroyableSingleton<HudManager>.Instance.Chat.IsClosedOrClosing)
        {
            CheatToggles.zoomOut = !CheatToggles.zoomOut;
        }

        CheatToggles.unlockFeatures = ZenithX.unlockFeatures.Value;
        CheatToggles.freeCosmetics = ZenithX.freeCosmetics.Value;
        CheatToggles.avoidBans = ZenithX.avoidBans.Value;

        if (!Utils.isPlayer)
        {
            CheatToggles.changeRole = CheatToggles.murderAll = CheatToggles.teleportPlayer = CheatToggles.spectate = CheatToggles.freecam = CheatToggles.murderPlayer = false;
        }

        if (!Utils.isHost && !Utils.isFreePlay)
        {
            CheatToggles.telekillPlayer = CheatToggles.killPlayer = CheatToggles.zeroKillCd = CheatToggles.killAnyone = CheatToggles.killVanished = false;
        }

        //Host-only cheats are turned off if LocalPlayer is not the game's host
        if (!Utils.isHost) {
            CheatToggles.voteImmune = CheatToggles.godMode = CheatToggles.impostorHack = CheatToggles.evilVote = false;
        }
        //Almost all of them now have host checking

        //Some cheats only work if the ship is present, so they are turned off if it is not
        if (!Utils.isShip)
        {
            CheatToggles.unfixableLights = CheatToggles.completeMyTasks = CheatToggles.kickVents = CheatToggles.reportBody = CheatToggles.closeMeeting = CheatToggles.reactorSab = CheatToggles.oxygenSab = CheatToggles.commsSab = CheatToggles.elecSab = CheatToggles.mushSab = CheatToggles.doorsSab = false;
        }

        if (CheatToggles.RGBMode)
        {
            this.hue += Time.deltaTime * 0.3f;
            if (this.hue > 1f)
            {
                this.hue -= 1f;
            }
        }
    }
    public void OnGUI()
    {
        if (!isGUIActive) return;

        DrawPopups();

        Color uiColor;

        string configHtmlColor = ZenithX.menuHtmlColor.Value;

        if (!TryParseHtmlString(configHtmlColor, out uiColor))
        {
            if (!configHtmlColor.StartsWith("#"))
            {
                if (TryParseHtmlString("#" + configHtmlColor, out uiColor))
                {
                    GUI.backgroundColor = uiColor;
                }
            }
        }
        else
        {
            GUI.backgroundColor = GradientColor;
        }

        if (CheatToggles.init)
        {
            GUI.backgroundColor = GradientColor;
        }

        if (CheatToggles.RGBMode)
        {
            GUI.backgroundColor = Color.HSVToRGB(this.hue, 1f, 1f);
        }

        //Only change the window height while the user is not dragging it
        //Or else dragging breaks
        if (!isDragging)
        {
            int windowHeight = CalculateWindowHeight();
            windowRect.height = windowHeight;
        }

        windowRect = GUI.Window(
            0,
            windowRect,
            (GUI.WindowFunction)WindowFunction,
            new GUIContent("ZenithX v" + ZenithX.ZenithXVersion),
            GUI.skin.window
        );
    }

public static void ShowPopup(string text, System.Action onOk = null)
{
    showPopup = true;
    popupText = text;
    popupOkAction = onOk;
    ZenithXSoundManager.PlaySound("Notification", 1f);
    ShowNotification(text, AlertType.Notification, 2.5f);
}

public static void ShowPopupButtons(string text, System.Action onYes, System.Action onNo = null)
{
    showPopupButtons = true;
    popupButtonsText = text;
    popupYesAction = onYes;
    popupNoAction = onNo;
    ZenithXSoundManager.PlaySound("Notification", 1f);
    ShowNotification(text, AlertType.Warning, 3f);
}

public static class alert
{
    public static void Notification(string text)
    {
        ZenithXSoundManager.PlaySound("Notification", 1f);
        ShowNotification(text, AlertType.Notification, 2.5f);
    }

    public static void Success(string text)
    {
        ZenithXSoundManager.PlaySound("Success", 1f);
        ShowNotification(text, AlertType.Success, 3f);
    }

    public static void Warning(string text)
    {
        ZenithXSoundManager.PlaySound("Warning", 0.5f);
        ShowNotification(text, AlertType.Warning, 3f);
    }

    public static void Error(string text)
    {
        ZenithXSoundManager.PlaySound("Error", 1f);
        ShowNotification(text, AlertType.Error, 4f);
    }
}

static List<GuiNotification> notifications = new();

// Call to create a new notification
public static void ShowNotification(string text, AlertType type, float duration = 2.5f)
{
    Color c = type switch {
        AlertType.Notification => new Color(0.211f, 0.475f, 1f),
        AlertType.Success => Color.green,
        AlertType.Warning => Color.yellow,
        AlertType.Error => Color.red,
        _ => new Color(0.211f, 0.475f, 1f)
    };

    notifications.Add(new GuiNotification {
        text = text,
        color = c,
        timer = duration,
        maxTime = duration,
        posY = Screen.height + 60,
        alpha = 0f,
        glowTime = 0f,
        type = type
    });
}

private static void DrawPopups()
{
    if (showPopup || showPopupButtons)
    {
        GUI.backgroundColor = GradientColor;
        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");
    }

    if (showPopup)
    {
        Rect popupRect = new Rect((Screen.width - 400) / 2, (Screen.height - 200) / 2, 400, 200);
        GUI.backgroundColor = GradientColor;
        GUI.ModalWindow(1001, popupRect, (GUI.WindowFunction)(id =>
        {
            GUI.Label(new Rect(20, 40, popupRect.width - 40, 60), popupText, buttonLabelStyle);
            if (ManualButton(new Rect(160, 130, 80, 35), "Ok", buttonLabelStyle))
            {
                popupOkAction?.Invoke();
                showPopup = false;
            }
        }), "");
    }

    if (showPopupButtons)
    {
        Rect popupRect = new Rect((Screen.width - 440) / 2, (Screen.height - 240) / 2, 440, 240);
        GUI.backgroundColor = GradientColor;
        GUI.ModalWindow(1002, popupRect, (GUI.WindowFunction)(id =>
        {
            GUI.Label(new Rect(20, 40, popupRect.width - 40, 80), popupButtonsText, buttonLabelStyle);

            GUI.backgroundColor = Color.green;
            if (ManualButton(new Rect(40, 150, 90, 40), "Yes", buttonLabelStyle))
            {
                popupYesAction?.Invoke();
                showPopupButtons = false;
            }

            GUI.backgroundColor = Color.red;
            if (ManualButton(new Rect(310, 150, 90, 40), "No", buttonLabelStyle))
            {
                popupNoAction?.Invoke();
                showPopupButtons = false;
            }

            GUI.backgroundColor = GradientColor;
        }), "");
    }

    DrawNotifications();
}

private static void DrawNotifications()
{
    float thickness = 3f;
    float notifHeight = 44f;
    float notifWidth = 315f;
    float notifSpacing = 8f;
    float baseY = Screen.height - 80;

    for(int i = notifications.Count - 1; i >= 0; i--)
    {
        var n = notifications[i];
        n.timer -= Time.unscaledDeltaTime;

        float targetY = baseY - i * (notifHeight + notifSpacing);
        n.posY = Mathf.Lerp(n.posY, targetY, 10f * Time.unscaledDeltaTime);

        if (n.timer > n.maxTime - 0.4f)
            n.alpha = Mathf.Min(1f, n.alpha + Time.unscaledDeltaTime * 2.5f);
        else if (n.timer < 0.5f)
            n.alpha = Mathf.Max(0f, n.timer / 0.5f);
        else
            n.alpha = 1f;

        if (n.timer <= 0 || n.alpha <= 0.01f)
        {
            notifications.RemoveAt(i);
            continue;
        }

        n.glowTime += Time.unscaledDeltaTime * 2f;
        float glowLerp = 0.26f + 0.16f * Mathf.Sin(n.glowTime);
        Color glowColor = Color.Lerp(n.color, Color.white, glowLerp);

        Rect rect = new Rect(22, n.posY - thickness, notifWidth + thickness * 2, notifHeight + thickness * 2);
        GUI.backgroundColor = Color.black;
        GUI.Box(rect, "");

        GUI.backgroundColor = glowColor * new Color(1f, 1f, 1f, n.alpha);
        GUI.Box(new Rect(22 + thickness, n.posY, notifWidth, notifHeight), "");

GUIStyle tmp = new GUIStyle(GUI.skin.label)
{
    fontSize = 18,
    alignment = TextAnchor.MiddleCenter,
    fontStyle = FontStyle.Bold
};

        tmp.normal.textColor = new Color(0, 0, 0, n.alpha);
        Rect trect = new Rect(22 + thickness, n.posY, notifWidth, notifHeight);

        for(int o = -1; o <= 1; o++)
        for(int p = -1; p <= 1; p++)
            if(o != 0 || p != 0)
                GUI.Label(new Rect(trect.x + o * 0.8f, trect.y + p * 0.8f, trect.width, trect.height), n.text, tmp);

        tmp.normal.textColor = new Color(n.color.r, n.color.g, n.color.b, n.alpha);
        GUI.Label(trect, n.text, tmp);
    }
}

private static bool ManualButton(Rect rect, string label, GUIStyle lblStyle, Action onClick = null)
{
    Color normalBgColor = gradientTop;
    Color hoverBgColor = Color.Lerp(gradientTop, Color.white, 0.2f);
    Color activeBgColor = Color.Lerp(gradientTop, gradientBottom, 0.3f);

    Event e = Event.current;
    bool isHover = rect.Contains(e.mousePosition);
    
    // Track button state to detect press (down transition)
    bool isButtonDown = false; // Static to persist across frames
    bool wasButtonDown = isButtonDown;
    isButtonDown = e.type == EventType.MouseDown && e.button == 0 && isHover;
    
    bool isClick = !wasButtonDown && isButtonDown; // Only true on the down transition
    
    if (isClick)
    {
        e.Use();
        
        // Record click time for 0.15s outline
        buttonClickTimes[label] = Time.time;
        
        // Invoke the action when clicked
        onClick?.Invoke();
    }

    // Check if button should have outline (within 0.15s of click)
    bool hasOutline = false;
    if (buttonClickTimes.ContainsKey(label))
    {
        float timeSinceClick = Time.time - buttonClickTimes[label];
        hasOutline = timeSinceClick < 0.15f;
        if (timeSinceClick >= 0.15f)
        {
            buttonClickTimes.Remove(label); // Clean up
        }
    }

    Color bgColor = isButtonDown ? activeBgColor : (isHover ? hoverBgColor : normalBgColor);
    Color originalBgColor = GUI.backgroundColor;
    Color originalColor = GUI.color;

    // Draw outline first (behind button)
    if (hasOutline)
    {
        DrawLightBlueOutline(rect, 2f);
    }

    // Draw background box
    GUI.backgroundColor = bgColor;
    GUI.Box(rect, "");

    // Draw the label with proper text color
    Color textColor = GetContrastingTextColor(bgColor);
    GUIStyle labelStyle = new GUIStyle(lblStyle);
    labelStyle.normal.textColor = Color.white;

    // Adjust label rect to center it properly
    Rect labelRect = rect;
    labelRect.x += 5; // Small padding
    GUI.Label(labelRect, label, labelStyle);

    // Restore original background color
    GUI.backgroundColor = originalBgColor;
    GUI.color = originalColor;

    return isClick;
}

private static bool ManualToggle(Rect rect, bool currentState, string label, GUIStyle lblStyle, Action<bool> onValueChanged = null)
{
    Color normalBgColor = currentState ? Color.Lerp(gradientTop, gradientBottom, 0.2f) : gradientTop;
    Color hoverBgColor = Color.Lerp(normalBgColor, Color.white, 0.2f);
    Color activeBgColor = Color.Lerp(normalBgColor, gradientBottom, 0.3f);

    Event e = Event.current;
    bool isHover = rect.Contains(e.mousePosition);
    bool isClick = false;

    if (isHover && e.type == EventType.MouseDown && e.button == 0)
    {
        isClick = true;
        e.Use();
        // Toggle state and invoke action with new state
        bool newState = !currentState;
        onValueChanged?.Invoke(newState);
    }

    Color bgColor = isClick ? activeBgColor : (isHover ? hoverBgColor : normalBgColor);
    Color originalBgColor = GUI.backgroundColor;
    Color originalColor = GUI.color;

    // Draw outline if toggle is TRUE
    if (currentState)
    {
        DrawLightBlueOutline(rect, 2f);
    }

    // Draw background box
    GUI.backgroundColor = bgColor;
    GUI.Box(rect, "");

    // Draw label with proper text color
    Color textColor = GetContrastingTextColor(bgColor);
    GUIStyle labelStyle = new GUIStyle(lblStyle);
    labelStyle.normal.textColor = Color.white;

    // Adjust label rect to center it properly
    Rect labelRect = rect;
    labelRect.x += 5; // Small padding
    GUI.Label(labelRect, label, labelStyle);

    // Restore original background color
    GUI.backgroundColor = originalBgColor;
    GUI.color = originalColor;

    return isClick ? !currentState : currentState;
}

// Light blue outline drawing method
private static void DrawLightBlueOutline(Rect rect, float thickness = 2f)
{
    Color originalColor = GUI.color;
    Color originalBgColor = GUI.backgroundColor;
    
    // YOUR exact light blue from gradientTop
    Color lightBlue = gradientTop;
    
    // Create a slightly larger rect for the outline
    Rect outlineRect = new Rect(
        rect.x - thickness, 
        rect.y - thickness, 
        rect.width + (thickness * 2), 
        rect.height + (thickness * 2)
    );
    
    // Draw the main button rect first (black background)
    GUI.backgroundColor = Color.black;
    GUI.Box(outlineRect, "");
    
    // Now draw the light blue outline on top
    GUI.backgroundColor = lightBlue;
    GUI.Box(rect, "");
    
    // Restore colors
    GUI.backgroundColor = originalBgColor;
    GUI.color = originalColor;
}

private static Color GetContrastingTextColor(Color backgroundColor)
{
    // Calculate luminance to determine if we need light or dark text
    float luminance = 0.299f * backgroundColor.r + 0.587f * backgroundColor.g + 0.114f * backgroundColor.b;
    
    return Color.white; // Just return white
}
    public void WindowFunction(int windowID)
    {
        int groupSpacing = 50;
        int toggleSpacing = 40;
        int buttonSpacing = 40;
        int submenuSpacing = 40;
        int currentYPosition = 20;

        for (int groupId = 0; groupId < groups.Count; groupId++)
        {
            GroupInfo group = groups[groupId];

            Rect groupRect = new Rect(10, currentYPosition, 280, 40);
            if (ManualButton(groupRect, group.name, buttonLabelStyle))
            {
                group.isExpanded = !group.isExpanded;
                groups[groupId] = group;
                CloseAllGroupsExcept(groupId);
            }
            currentYPosition += groupSpacing;

            if (!group.isExpanded) continue;

            // Draw group items
            foreach (var item in group.items)
            {
                Rect itemRect = new Rect(20, currentYPosition, 260, 30);
                if (item is ToggleInfo toggle)
                {
                    bool current = toggle.getState();
                    bool newState = ManualToggle(itemRect, current, toggle.label, toggleLabelStyle);
                    if (newState != current) toggle.setState(newState);
                    currentYPosition += toggleSpacing;
                }
                else if (item is ButtonInfo button)
                {
                    if (ManualButton(itemRect, button.label, buttonLabelStyle))
                        button.action?.Invoke();
                    currentYPosition += buttonSpacing;
                }
            }

            // Draw submenus
            for (int submenuId = 0; submenuId < group.submenus.Count; submenuId++)
            {
                var submenu = group.submenus[submenuId];

                Rect submenuRect = new Rect(20, currentYPosition, 260, 30);
                if (ManualButton(submenuRect, submenu.name, submenuLabelStyle))
                {
                    submenu.isExpanded = !submenu.isExpanded;
                    group.submenus[submenuId] = submenu;
                    if (submenu.isExpanded)
                        CloseAllSubmenusExcept(group, submenuId);
                }
                currentYPosition += submenuSpacing;

                if (!submenu.isExpanded) continue;

                // Draw submenu items
                foreach (var item in submenu.items)
                {
                    Rect subItemRect = new Rect(30, currentYPosition, 250, 30);
                    if (item is ToggleInfo toggle)
                    {
                        bool current = toggle.getState();
                        bool newState = ManualToggle(subItemRect, current, toggle.label, toggleLabelStyle);
                        if (newState != current) toggle.setState(newState);
                        currentYPosition += toggleSpacing;
                    }
                    else if (item is ButtonInfo button)
                    {
                        if (ManualButton(subItemRect, button.label, buttonLabelStyle))
                            button.action?.Invoke();
                        currentYPosition += buttonSpacing;
                    }
                }
            }
        }


        if (Event.current.type == EventType.MouseDrag)
        {
            isDragging = true;
        }

        if (Event.current.type == EventType.MouseUp)
        {
            isDragging = false;
        }

        GUI.DragWindow(); //Allows dragging the GUI window with mouse
    }


    // Dynamically calculate the window's height depending on
    // The number of toggles & group expansion
    private int CalculateWindowHeight()
    {
        int totalHeight = 70; // Base height for the window
        int groupHeight = 50; // Height for each group title
        int itemHeight = 40; // Height for each item (toggle or button)
        int submenuHeight = 40; // Height for each submenu title

        foreach (GroupInfo group in groups)
        {
            totalHeight += groupHeight; // Always add height for the group title

            if (group.isExpanded)
            {
                totalHeight += group.items.Count * itemHeight; // Add height for items in the group

                foreach (SubmenuInfo submenu in group.submenus)
                {
                    totalHeight += submenuHeight; // Always add height for the submenu title

                    if (submenu.isExpanded)
                    {
                        totalHeight += submenu.items.Count * itemHeight; // Add height for items in the expanded submenu
                    }
                }
            }
        }

        return totalHeight;
    }


    // Closes all expanded groups other than indexToKeepOpen
    private void CloseAllGroupsExcept(int indexToKeepOpen)
    {
        for (int i = 0; i < groups.Count; i++)
        {
            if (i != indexToKeepOpen)
            {
                GroupInfo group = groups[i];
                group.isExpanded = false;
                groups[i] = group;
            }
        }
    }

    private void CloseAllSubmenusExcept(GroupInfo group, int submenuIndexToKeepOpen)
    {
        for (int i = 0; i < group.submenus.Count; i++)
        {
            if (i != submenuIndexToKeepOpen)
            {
                var submenu = group.submenus[i];
                submenu.isExpanded = false;
                group.submenus[i] = submenu;
            }
        }
    }
    
    private float hue;
}