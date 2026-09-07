using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Il2CppInterop.Runtime.Attributes;
using Il2CppSystem;
using UnityEngine;
using UnityEngine.Video;

namespace ZenithX;

public class MenuUI : MonoBehaviour
{
	public static List<GroupInfo> groups = new List<GroupInfo>();

	private bool isDragging;

	public static Rect WindowRect = new Rect(10f, 10f, 700f, 550f);

	public static bool isGUIActive;

	private string searchQuery = "";

	public static float hue;

	public static GroupInfo favoritesGroup = new GroupInfo("Favorites", "favorites", isExpanded: false, new List<CreateList>(), new List<SubmenuInfo>());

	private GUIStyle _windowBg;

	private Vector2 _contentScroll = Vector2.zero;

	private GUIStyle _headerLabelStyle;

	private GUIStyle _groupTitleStyle;

	private GUIStyle _itemLabelStyle;

	private GUIStyle _subItemLabelStyle;

	private GUIStyle _buttonStyle;

	private GUIStyle _checkmarkStyle;

	private GUIStyle _searchStyle;

	private GUIStyle _versionStyle;

	private GUIStyle _submenuIndicatorStyle;

	private GUIStyle _sliderLabelStyle;

	private GUIStyle _sliderValueStyle;

	private GUIStyle _smallBoldLabelStyle;

	private GUIStyle _iconStyle;

	private GUIStyle _sidebarGroupLabelStyle;

	private GUIStyle _groupLabelStyle;

	private GUIStyle _keybindValueStyle;

	private GUIStyle _colorHexStyle;

	private GUIStyle _emptyStateStyle;

	private bool _stylesInitialized;

	private readonly Dictionary<string, GUIContent> CachedContents = new Dictionary<string, GUIContent>();

	private float _menuAnim;

	private float _menuVel;

	private Vector2 _scrollPosition = Vector2.zero;

	private Vector2 _sidebarScrollPosition = Vector2.zero;

	private readonly Dictionary<int, float> _groupAnim = new Dictionary<int, float>();

	private readonly Dictionary<int, float> _groupVel = new Dictionary<int, float>();

	private static readonly Color UIAccent = new Color(0.7f, 0.9f, 1f);

	private static readonly Color UIAccentLight = new Color(0.85f, 0.95f, 1f);

	private static readonly Color UIWindow = new Color(0.92f, 0.97f, 0.98f);

	private static readonly Color UIPanel = new Color(0.96f, 0.99f, 1f);

	private static readonly Color UIText = new Color(0.09f, 0.27f, 0.36f);

	private static readonly Color UIMuted = new Color(0.37f, 0.53f, 0.63f);

	public static readonly Color UIDisabledGrey = new Color(0.5f, 0.5f, 0.5f);

	public static readonly Color UIAcceptedGreen = new Color(0.2f, 0.8f, 0.4f);

	private static readonly Color UIGradientStart = new Color(0.93f, 0.98f, 1f);

	private static readonly Color UIGradientEnd = new Color(0.77f, 0.94f, 1f);

	private static readonly Color TextDark = new Color(0.08f, 0.12f, 0.18f, 1f);

	private static readonly Color TextMutedDark = new Color(0.35f, 0.45f, 0.58f, 1f);

	private static readonly Color ButtonLight = new Color(0.9f, 0.93f, 0.99f, 1f);

	private static readonly Color ButtonHover = new Color(0.82f, 0.87f, 0.96f, 1f);

	private static readonly Color ButtonPressed = new Color(0.74f, 0.8f, 0.9f, 1f);

	private static UILibrary.UIWindowData _windowData;

	private static readonly Color ToggleEnabledOutline = new Color(0.48f, 0.7f, 1f, 1f);

	private static readonly Color ToggleEnabledFill = new Color(0.72f, 0.86f, 1f, 0.42f);

	private const float HeaderHeight = 48f;

	private const float HeaderCorner = 12f;

	private const float GroupCorner = 8f;

	private const float ItemCorner = 7f;

	private static float _customHue = 0.6f;

	private static float _customSaturation = 0.8f;

	private static float _customBrightness = 0.9f;

	private static bool _useCustomColor;

	private static bool _enableAnimations = true;

	private static bool _enableShadows = true;

	private static bool _enableGlowEffects = true;

	private static bool _enableRoundedCorners = true;

	private KeyCode CachedMenuKey;

	private string LastMenuKeybind;

	private KeyCode _cachedToggleZoomKey;

	private string _lastToggleZoomKeybind;

	private ZPalette _palette;

	private Color _cachedAccent;

	private bool _paletteInitialized;

	private bool _groupsInitialized;

	private float _cachedSidebarHeight = -1f;

	private int _cachedSidebarGroupCount = -1;

	private float _cachedContentHeight = -1f;

	private int _cachedContentGroupId = -1;

	private float _windowWidth = 820f;

	private float _windowHeight = 650f;

	private int _editMode;

	private string _editVideoPath = "";

	private string _editAudioPath = "";

	private AudioSource _editAudioSource;

	private VideoPlayer _editVideoPlayer;

	private RenderTexture _videoRenderTexture;

	private string _currentAudioPath = "";

	private readonly HashSet<int> _visibleGroupIds = new HashSet<int>();

	private string _lastSearchQuery = "";

	private bool _searchDirty = true;

	private Color _currentColor = Color.white;

	public static Vector2 GetMouseGuiPosition(Vector2 mousePosition)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (GUI.matrix == Matrix4x4.identity)
		{
			return mousePosition;
		}
		Matrix4x4 val = GUI.matrix;
		val = ((Matrix4x4)(ref val)).inverse;
		Vector3 val2 = ((Matrix4x4)(ref val)).MultiplyPoint3x4(Vector2.op_Implicit(mousePosition));
		return new Vector2(val2.x, val2.y);
	}

	public static bool ContainsMouse(Rect rect, Vector2? mousePosition = null, float yOffset = 0f)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		Vector2 mouseGuiPosition = GetMouseGuiPosition((Vector2)(((_003F?)mousePosition) ?? Event.current.mousePosition));
		mouseGuiPosition.y += yOffset;
		return ((Rect)(ref rect)).Contains(mouseGuiPosition);
	}

	private void Start()
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Expected O, but got Unknown
		_editAudioSource = ((Component)this).gameObject.AddComponent<AudioSource>();
		_editAudioSource.playOnAwake = false;
		_editAudioSource.loop = false;
		_editVideoPlayer = ((Component)this).gameObject.AddComponent<VideoPlayer>();
		_editVideoPlayer.playOnAwake = false;
		_editVideoPlayer.isLooping = true;
		_editVideoPlayer.renderMode = (VideoRenderMode)2;
		_videoRenderTexture = new RenderTexture(1280, 720, 24)
		{
			name = "EditVideoTexture"
		};
		_editVideoPlayer.targetTexture = _videoRenderTexture;
		_windowData = UILibrary.UI.CreateUI("ZenithX", draggable: true, "", 0, "v" + ZenithX.ZenithXVersion, "Players: " + PlayerCountAPI.PlayerCount);
		if (groups == null)
		{
			groups = _windowData.Groups;
		}
		if (!groups.Contains(favoritesGroup))
		{
			groups.Add(favoritesGroup);
		}
	}

	private void InitializeGroups()
	{
		if (_groupsInitialized)
		{
			return;
		}
		_groupsInitialized = true;
		List<SubmenuInfo> list = new List<SubmenuInfo>();
		for (int i = 0; i < PlayerStatsHandler.stats.Count; i++)
		{
			string statKey = PlayerStatsHandler.stats[i];
			string name = ((i < PlayerStatsHandler.formattedList.Count) ? PlayerStatsHandler.formattedList[i] : statKey);
			list.Add(new SubmenuInfo(name, isExpanded: false, new List<CreateList>
			{
				new ButtonInfo(" Increase", delegate
				{
					PlayerStatsHandler.Increase(statKey);
				}),
				new ButtonInfo(" Decrease", delegate
				{
					PlayerStatsHandler.Decrease(statKey);
				})
			}));
		}
		groups.Add(new GroupInfo("Player Stats", "stats", isExpanded: false, new List<CreateList>(), list));
		groups.Add(UILibrary.UI.Group("Player", "user", isExpanded: false, new List<CreateList>
		{
			UILibrary.UI.Toggle(" NoClip", () => CheatToggles.noClip, delegate(bool x)
			{
				CheatToggles.noClip = x;
			}),
			UILibrary.UI.Toggle(" SpeedHack", () => CheatToggles.speedBoost, delegate(bool x)
			{
				CheatToggles.speedBoost = x;
			}),
			UILibrary.UI.Toggle(" Revive self", () => CheatToggles.revive, delegate(bool x)
			{
				CheatToggles.revive = x;
			}),
			UILibrary.UI.Toggle(" No Ability Cooldown", () => CheatToggles.noAbilityCD, delegate(bool x)
			{
				CheatToggles.noAbilityCD = x;
			}),
			UILibrary.UI.Toggle(" Invert Controls", () => CheatToggles.invertControls, delegate(bool x)
			{
				CheatToggles.invertControls = x;
			}),
			UILibrary.UI.Toggle(" Uncapped FPS", () => CheatToggles.uncappedFPS, delegate(bool x)
			{
				CheatToggles.uncappedFPS = x;
			}),
			UILibrary.UI.Toggle(" Show Tasks in Meetings", () => CheatToggles.showTasksInMeetings, delegate(bool x)
			{
				CheatToggles.showTasksInMeetings = x;
			}),
			UILibrary.UI.Toggle(" AntiCheat", () => CheatToggles.AntiCheatEnabled, delegate(bool x)
			{
				CheatToggles.AntiCheatEnabled = x;
			}),
			UILibrary.UI.Toggle(" Immortality", () => CheatToggles.Immortality, delegate(bool x)
			{
				CheatToggles.Immortality = x;
			}),
			UILibrary.UI.Button(" Increase Level", delegate
			{
				ZenithXCheats.increaseLevel();
			}),
			UILibrary.UI.Button(" Disable All Cheats", delegate
			{
				CheatToggles.DisableAll();
			}),
			UILibrary.UI.Button(" Panic", delegate
			{
				Utils.Panic();
			})
		}, new List<SubmenuInfo>
		{
			UILibrary.UI.Submenu("Teleport", false, null, UILibrary.UI.Toggle(" to Cursor", () => CheatToggles.teleportCursor, delegate(bool x)
			{
				CheatToggles.teleportCursor = x;
			}), UILibrary.UI.Toggle(" to Player", () => CheatToggles.teleportPlayer, delegate(bool x)
			{
				CheatToggles.teleportPlayer = x;
			})),
			UILibrary.UI.Submenu("Murder", false, null, UILibrary.UI.Toggle(" Murder Player", () => CheatToggles.murderPlayer, delegate(bool x)
			{
				CheatToggles.murderPlayer = x;
			}), UILibrary.UI.Toggle(" Murder All", () => CheatToggles.murderAll, delegate(bool x)
			{
				CheatToggles.murderAll = x;
			})),
			UILibrary.UI.Submenu("Body Types (Client)", false, null, UILibrary.UI.Button(" Default", delegate
			{
				CheatToggles.bodyType = "Default";
			}), UILibrary.UI.Button(" Normal", delegate
			{
				CheatToggles.bodyType = "Normal";
			}), UILibrary.UI.Button(" Horse", delegate
			{
				CheatToggles.bodyType = "Horse";
			}), UILibrary.UI.Button(" Seeker", delegate
			{
				CheatToggles.bodyType = "Seeker";
			}), UILibrary.UI.Button(" Long", delegate
			{
				CheatToggles.bodyType = "Long";
			}), UILibrary.UI.Button(" Long Horse", delegate
			{
				CheatToggles.bodyType = "Long Horse";
			}), UILibrary.UI.Button(" Classic", delegate
			{
				CheatToggles.bodyType = "Classic";
			}))
		}));
		groups.Add(UILibrary.UI.Group("ESP & Vision", "eye", isExpanded: false, new List<CreateList>
		{
			UILibrary.UI.Toggle(" Nametag Vision", () => CheatToggles.nametagVision, delegate(bool x)
			{
				CheatToggles.nametagVision = x;
			}),
			UILibrary.UI.Toggle(" See Roles", () => CheatToggles.seeRoles, delegate(bool x)
			{
				CheatToggles.seeRoles = x;
			}),
			UILibrary.UI.Toggle(" See Ghosts", () => CheatToggles.seeGhosts, delegate(bool x)
			{
				CheatToggles.seeGhosts = x;
			}),
			UILibrary.UI.Toggle(" No Shadows (Fullbright)", () => CheatToggles.fullBright, delegate(bool x)
			{
				CheatToggles.fullBright = x;
			}),
			UILibrary.UI.Toggle(" Reveal Votes", () => CheatToggles.revealVotes, delegate(bool x)
			{
				CheatToggles.revealVotes = x;
			}),
			UILibrary.UI.Toggle(" Show Player Info", () => CheatToggles.showPlayerInfo, delegate(bool x)
			{
				CheatToggles.showPlayerInfo = x;
			}),
			UILibrary.UI.Toggle(" More Lobby Info", () => CheatToggles.moreLobbyInfo, delegate(bool x)
			{
				CheatToggles.moreLobbyInfo = x;
			}),
			UILibrary.UI.Toggle(" Show Lobby Timer", () => CheatToggles.showLobbyTimer, delegate(bool x)
			{
				CheatToggles.showLobbyTimer = x;
			})
		}, new List<SubmenuInfo>
		{
			UILibrary.UI.Submenu("Camera", false, null, UILibrary.UI.Toggle(" Zoom Out", () => CheatToggles.zoomOut, delegate(bool x)
			{
				CheatToggles.zoomOut = x;
			}), UILibrary.UI.Toggle(" Spectate", () => CheatToggles.spectate, delegate(bool x)
			{
				CheatToggles.spectate = x;
			}), UILibrary.UI.Toggle(" Freecam", () => CheatToggles.freecam, delegate(bool x)
			{
				CheatToggles.freecam = x;
			})),
			UILibrary.UI.Submenu("Tracers", false, null, UILibrary.UI.Toggle(" Crewmates", () => CheatToggles.tracersCrew, delegate(bool x)
			{
				CheatToggles.tracersCrew = x;
			}), UILibrary.UI.Toggle(" Impostors", () => CheatToggles.tracersImps, delegate(bool x)
			{
				CheatToggles.tracersImps = x;
			}), UILibrary.UI.Toggle(" Ghosts", () => CheatToggles.tracersGhosts, delegate(bool x)
			{
				CheatToggles.tracersGhosts = x;
			}), UILibrary.UI.Toggle(" Dead Bodies", () => CheatToggles.tracersBodies, delegate(bool x)
			{
				CheatToggles.tracersBodies = x;
			}), UILibrary.UI.Toggle(" Color Based", () => CheatToggles.colorBasedTracers, delegate(bool x)
			{
				CheatToggles.colorBasedTracers = x;
			}), UILibrary.UI.Toggle(" Distance Based", () => CheatToggles.distanceBasedTracers, delegate(bool x)
			{
				CheatToggles.distanceBasedTracers = x;
			})),
			UILibrary.UI.Submenu("Minimap", false, null, UILibrary.UI.Toggle(" Crewmates", () => CheatToggles.mapCrew, delegate(bool x)
			{
				CheatToggles.mapCrew = x;
			}), UILibrary.UI.Toggle(" Impostors", () => CheatToggles.mapImps, delegate(bool x)
			{
				CheatToggles.mapImps = x;
			}), UILibrary.UI.Toggle(" Ghosts", () => CheatToggles.mapGhosts, delegate(bool x)
			{
				CheatToggles.mapGhosts = x;
			}), UILibrary.UI.Toggle(" Color Based", () => CheatToggles.colorBasedMap, delegate(bool x)
			{
				CheatToggles.colorBasedMap = x;
			}))
		}));
		groups.Add(UILibrary.UI.Group("Role Cheats", "shield", isExpanded: false, new List<CreateList> { UILibrary.UI.Toggle(" Set Fake Role", () => CheatToggles.changeRole, delegate(bool x)
		{
			CheatToggles.changeRole = x;
		}) }, new List<SubmenuInfo>
		{
			UILibrary.UI.Submenu("Impostor", false, null, UILibrary.UI.Toggle(" Allow Tasks", () => CheatToggles.impostorTasks, delegate(bool x)
			{
				CheatToggles.impostorTasks = x;
			}), UILibrary.UI.Toggle(" Kill Reach", () => CheatToggles.killReach, delegate(bool x)
			{
				CheatToggles.killReach = x;
			}), UILibrary.UI.Toggle(" Kill Anyone", () => CheatToggles.killAnyone, delegate(bool x)
			{
				CheatToggles.killAnyone = x;
			}), UILibrary.UI.Toggle(" No Kill Cooldown", () => CheatToggles.zeroKillCd, delegate(bool x)
			{
				CheatToggles.zeroKillCd = x;
			}), UILibrary.UI.Toggle(" Auto Kill Nearby", () => CheatToggles.autoKillNearby, delegate(bool x)
			{
				CheatToggles.autoKillNearby = x;
			})),
			UILibrary.UI.Submenu("Shapeshifter", false, null, UILibrary.UI.Toggle(" No Ss Animation", () => CheatToggles.noShapeshiftAnim, delegate(bool x)
			{
				CheatToggles.noShapeshiftAnim = x;
			}), UILibrary.UI.Toggle(" Endless Ss Duration", () => CheatToggles.endlessSsDuration, delegate(bool x)
			{
				CheatToggles.endlessSsDuration = x;
			}), UILibrary.UI.Toggle(" Shapeshift All", () => CheatToggles.shapeshiftAll, delegate(bool x)
			{
				CheatToggles.shapeshiftAll = x;
			}), UILibrary.UI.Toggle(" Reset Shapeshifts", () => CheatToggles.resetShapeshift, delegate(bool x)
			{
				CheatToggles.resetShapeshift = x;
			})),
			UILibrary.UI.Submenu("Phantom", false, null, UILibrary.UI.Toggle(" No Vanish Cooldown", () => CheatToggles.noVanishCooldown, delegate(bool x)
			{
				CheatToggles.noVanishCooldown = x;
			}), UILibrary.UI.Toggle(" Endless Vanish Duration", () => CheatToggles.endlessVanishDuration, delegate(bool x)
			{
				CheatToggles.endlessVanishDuration = x;
			}), UILibrary.UI.Toggle(" Kill While Vanished", () => CheatToggles.killVanished, delegate(bool x)
			{
				CheatToggles.killVanished = x;
			})),
			UILibrary.UI.Submenu("Viper", false, null, UILibrary.UI.Toggle(" Instant Dissolve", () => CheatToggles.instantDissolve, delegate(bool x)
			{
				CheatToggles.instantDissolve = x;
			})),
			UILibrary.UI.Submenu("Crewmate", false, null),
			UILibrary.UI.Submenu("Tracker", false, null, UILibrary.UI.Toggle(" Endless Tracking", () => CheatToggles.endlessTracking, delegate(bool x)
			{
				CheatToggles.endlessTracking = x;
			}), UILibrary.UI.Toggle(" No Track Delay", () => CheatToggles.noTrackingDelay, delegate(bool x)
			{
				CheatToggles.noTrackingDelay = x;
			}), UILibrary.UI.Toggle(" No Track Cooldown", () => CheatToggles.noTrackingCooldown, delegate(bool x)
			{
				CheatToggles.noTrackingCooldown = x;
			}), UILibrary.UI.Toggle(" Track Reach", () => CheatToggles.trackReach, delegate(bool x)
			{
				CheatToggles.trackReach = x;
			})),
			UILibrary.UI.Submenu("Engineer", false, null, UILibrary.UI.Toggle(" Endless Vent Time", () => CheatToggles.endlessVentTime, delegate(bool x)
			{
				CheatToggles.endlessVentTime = x;
			}), UILibrary.UI.Toggle(" No Vent Cooldown", () => CheatToggles.noVentCooldown, delegate(bool x)
			{
				CheatToggles.noVentCooldown = x;
			})),
			UILibrary.UI.Submenu("Scientist", false, null, UILibrary.UI.Toggle(" Endless Battery", () => CheatToggles.endlessBattery, delegate(bool x)
			{
				CheatToggles.endlessBattery = x;
			}), UILibrary.UI.Toggle(" No Vitals Cooldown", () => CheatToggles.noVitalsCooldown, delegate(bool x)
			{
				CheatToggles.noVitalsCooldown = x;
			})),
			UILibrary.UI.Submenu("Detective", false, null, UILibrary.UI.Toggle(" Interrogate Reach", () => CheatToggles.interrogateReach, delegate(bool x)
			{
				CheatToggles.interrogateReach = x;
			})),
			UILibrary.UI.Submenu("Judge", false, null, UILibrary.UI.Toggle(" Unlock Overrule", () => CheatToggles.judgeOverrule, delegate(bool x)
			{
				CheatToggles.judgeOverrule = x;
			}))
		}));
		groups.Add(UILibrary.UI.Group("Ship", "map", isExpanded: false, new List<CreateList>
		{
			UILibrary.UI.Button(" Call Meeting", delegate
			{
				ZenithXCheats.callMeeting();
			}),
			UILibrary.UI.Button(" Destroy Game", delegate
			{
				ZenithXCheats.DestroyInGameCheat();
			}),
			UILibrary.UI.Toggle(" Close Meeting", () => CheatToggles.closeMeeting, delegate(bool x)
			{
				CheatToggles.closeMeeting = x;
			}),
			UILibrary.UI.Toggle(" Report Body", () => CheatToggles.reportBody, delegate(bool x)
			{
				CheatToggles.reportBody = x;
			}),
			UILibrary.UI.Toggle(" Auto Keypad Code", () => CheatToggles.autoKeypadCode, delegate(bool x)
			{
				CheatToggles.autoKeypadCode = x;
			}),
			UILibrary.UI.Toggle(" Auto Open Doors on Use", () => CheatToggles.autoOpenDoorsOnUse, delegate(bool x)
			{
				CheatToggles.autoOpenDoorsOnUse = x;
			}),
			UILibrary.UI.Toggle(" Unfixable Lights", () => CheatToggles.unfixableLights, delegate(bool x)
			{
				CheatToggles.unfixableLights = x;
			}),
			UILibrary.UI.Toggle(" Change Map to Sabotage Map", () => CheatToggles.changeMapToSabotage, delegate(bool x)
			{
				CheatToggles.changeMapToSabotage = x;
			}),
			UILibrary.UI.Toggle(" Crash Server on Game Start", () => CheatToggles.attemptToCrashLobby, delegate(bool x)
			{
				CheatToggles.attemptToCrashLobby = x;
			}),
			UILibrary.UI.Toggle(" Block Sabotages", () => CheatToggles.BlockSabotages, delegate(bool x)
			{
				CheatToggles.BlockSabotages = x;
			}),
			UILibrary.UI.Toggle(" Tasks Arrows", () => CheatToggles.taskArrows, delegate(bool x)
			{
				CheatToggles.taskArrows = x;
			}),
			UILibrary.UI.Toggle(" Memeify", () => CheatToggles.memeify, delegate(bool x)
			{
				CheatToggles.memeify = x;
			})
		}, new List<SubmenuInfo>
		{
			UILibrary.UI.Submenu("Sabotages", false, null, UILibrary.UI.Button(" Sabotage All", delegate
			{
				ZenithXCheats.SabotageAll();
			}), UILibrary.UI.Button(" Repair All", delegate
			{
				ZenithXCheats.RepairAll();
			}), UILibrary.UI.Toggle(" Reactor", () => CheatToggles.reactorSab, delegate(bool x)
			{
				CheatToggles.reactorSab = x;
			}), UILibrary.UI.Toggle(" Oxygen", () => CheatToggles.oxygenSab, delegate(bool x)
			{
				CheatToggles.oxygenSab = x;
			}), UILibrary.UI.Toggle(" Lights", () => CheatToggles.elecSab, delegate(bool x)
			{
				CheatToggles.elecSab = x;
			}), UILibrary.UI.Toggle(" Comms", () => CheatToggles.commsSab, delegate(bool x)
			{
				CheatToggles.commsSab = x;
			}), UILibrary.UI.Toggle(" MushroomMixup", () => CheatToggles.mushSab, delegate(bool x)
			{
				CheatToggles.mushSab = x;
			})),
			UILibrary.UI.Submenu("Vents", false, null, UILibrary.UI.Toggle(" Unlock Vents", () => CheatToggles.useVents, delegate(bool x)
			{
				CheatToggles.useVents = x;
			}), UILibrary.UI.Toggle(" Kick All From Vents", () => CheatToggles.kickVents, delegate(bool x)
			{
				CheatToggles.kickVents = x;
			}), UILibrary.UI.Toggle(" Walk In Vents", () => CheatToggles.walkVent, delegate(bool x)
			{
				CheatToggles.walkVent = x;
			}), UILibrary.UI.Button(" Teleport All", delegate
			{
				ZenithXCheats.TeleportAllToVent();
			}), UILibrary.UI.Toggle(" Spam Tp All", () => CheatToggles.spamTpAll, delegate(bool x)
			{
				CheatToggles.spamTpAll = x;
			}), UILibrary.UI.Toggle(" Spam Tp Imps", () => CheatToggles.spamTpImps, delegate(bool x)
			{
				CheatToggles.spamTpImps = x;
			}), UILibrary.UI.Toggle(" Disable Vents", () => CheatToggles.DisableVents, delegate(bool x)
			{
				CheatToggles.DisableVents = x;
			}))
		}));
		groups.Add(UILibrary.UI.Group("Chat", "message", isExpanded: false, new List<CreateList>
		{
			UILibrary.UI.Toggle(" Enable Chat Always", () => CheatToggles.alwaysChat, delegate(bool x)
			{
				CheatToggles.alwaysChat = x;
			}),
			UILibrary.UI.Toggle(" Chat Mimic", () => CheatToggles.chatMimic, delegate(bool x)
			{
				CheatToggles.chatMimic = x;
			}),
			UILibrary.UI.Toggle(" Chat Bypasses", () => CheatToggles.chatJailbreak, delegate(bool x)
			{
				CheatToggles.chatJailbreak = x;
			})
		}, new List<SubmenuInfo>()));
		groups.Add(UILibrary.UI.Group("Host Powers", "crown", isExpanded: false, new List<CreateList>
		{
			UILibrary.UI.Button(" Force Start", delegate
			{
				ZenithXCheats.forceStartGameCheat();
			}),
			UILibrary.UI.Button(" Increase Lobby Timer", delegate
			{
				ZenithXCheats.extendLobbyTimer();
			}),
			UILibrary.UI.Button(" Skip Meeting", delegate
			{
				ZenithXCheats.skipMeetingCheat();
			}),
			UILibrary.UI.Button(" Force Imposter Role", delegate
			{
				ZenithXCheats.requestImpostorRole();
			}),
			UILibrary.UI.Button(" Revive All Players", delegate
			{
				ZenithXCheats.reviveAllCheat();
			}),
			UILibrary.UI.Button(" Crash Server", delegate
			{
				ZenithXCheats.crashEveryone();
			}),
			UILibrary.UI.Button(" Reset Kill Cooldown (All)", delegate
			{
				ZenithXCheats.removeKillCDForEveryone();
			}),
			UILibrary.UI.Button(" Force Meeting Random", delegate
			{
				ZenithXCheats.ForceMeetingRandom();
			}),
			UILibrary.UI.Button(" Force Aum Rpc For Everyone", delegate
			{
				ZenithXCheats.ForceAumRpcCheat();
			}),
			UILibrary.UI.Button(" Assign Clean Filter Task", delegate
			{
				ZenithXCheats.assignCleanFilterTask();
			}),
			UILibrary.UI.Toggle(" Force Start Button On Click", () => CheatToggles.forceStartOnClickStart, delegate(bool x)
			{
				CheatToggles.forceStartOnClickStart = x;
			}),
			UILibrary.UI.Toggle(" Revive Player", () => CheatToggles.revivePlayer, delegate(bool x)
			{
				CheatToggles.revivePlayer = x;
			}),
			UILibrary.UI.Toggle(" Eject Player", () => CheatToggles.ejectPlayer, delegate(bool x)
			{
				CheatToggles.ejectPlayer = x;
			}),
			UILibrary.UI.Toggle(" No Game Setup Limits", () => CheatToggles.noOptionsLimits, delegate(bool x)
			{
				CheatToggles.noOptionsLimits = x;
			}),
			UILibrary.UI.Toggle(" No Game End", () => CheatToggles.noGameEnd, delegate(bool x)
			{
				CheatToggles.noGameEnd = x;
			}),
			UILibrary.UI.Toggle(" Custom Seekers", () => CheatToggles.customSeekers, delegate(bool x)
			{
				CheatToggles.customSeekers = x;
			})
		}, new List<SubmenuInfo> { UILibrary.UI.Submenu("Extra", false, null, UILibrary.UI.Toggle(" Kill Anyone", () => CheatToggles.killAnyone, delegate(bool x)
		{
			CheatToggles.killAnyone = x;
		}), UILibrary.UI.Toggle(" Kill Targeted Player", () => CheatToggles.killPlayer, delegate(bool x)
		{
			CheatToggles.killPlayer = x;
		}), UILibrary.UI.Toggle(" Telekill Targeted Player", () => CheatToggles.telekillPlayer, delegate(bool x)
		{
			CheatToggles.telekillPlayer = x;
		}), UILibrary.UI.Button(" Kill All Crewmates", delegate
		{
			ZenithXCheats.killAllCrewCheat();
		}), UILibrary.UI.Button(" Kill All Impostors", delegate
		{
			ZenithXCheats.killAllImpsCheat();
		}), UILibrary.UI.Button(" Kill Everyone", delegate
		{
			ZenithXCheats.killAllCheat();
		}), UILibrary.UI.Toggle(" Kill Other Impostors", () => CheatToggles.impostorHack, delegate(bool x)
		{
			CheatToggles.impostorHack = x;
		}), UILibrary.UI.Toggle(" Evil Vote", () => CheatToggles.evilVote, delegate(bool x)
		{
			CheatToggles.evilVote = x;
		}), UILibrary.UI.Toggle(" Vote Immune", () => CheatToggles.voteImmune, delegate(bool x)
		{
			CheatToggles.voteImmune = x;
		})) }));
		groups.Add(UILibrary.UI.Group("Cosmetics & Outfits", "sparkles", isExpanded: false, new List<CreateList>
		{
			UILibrary.UI.Toggle(" Free Cosmetics Unlocker", () => CheatToggles.freeCosmetics, delegate(bool x)
			{
				CheatToggles.freeCosmetics = x;
			}),
			UILibrary.UI.Toggle(" Allow Duplicate Color", () => CheatToggles.allowDuplicateColor, delegate(bool x)
			{
				CheatToggles.allowDuplicateColor = x;
			}),
			UILibrary.UI.Button(" Change Everyones Color To Yours", delegate
			{
				ZenithXCheats.changeEveryonesColorToYours();
			}),
			UILibrary.UI.Button(" Set Your Color To Preferred", delegate
			{
				ZenithXCheats.setMyColorToPreferred();
			}),
			UILibrary.UI.Toggle(" Disco Color", () => CheatToggles.discoColor, delegate(bool x)
			{
				CheatToggles.discoColor = x;
			}),
			UILibrary.UI.Toggle(" Shuffle My Outfit", () => CheatToggles.shuffleOutfit, delegate(bool x)
			{
				CheatToggles.shuffleOutfit = x;
			}),
			UILibrary.UI.Toggle(" Shuffle All Outfits", () => CheatToggles.shuffleAllOutfits, delegate(bool x)
			{
				CheatToggles.shuffleAllOutfits = x;
			}),
			UILibrary.UI.Button(" Copy Player Outfit", delegate
			{
				ZenithXCheats.copyOutfitCheat();
			}),
			UILibrary.UI.Toggle(" Reset My Outfit", () => CheatToggles.resetOutfit, delegate(bool x)
			{
				CheatToggles.resetOutfit = x;
			})
		}, new List<SubmenuInfo>()));
		groups.Add(UILibrary.UI.Group(" Animations", "play", isExpanded: false, new List<CreateList>
		{
			UILibrary.UI.Toggle(" Shields", () => CheatToggles.animShields, delegate(bool x)
			{
				CheatToggles.animShields = x;
			}),
			UILibrary.UI.Toggle(" Asteroids", () => CheatToggles.animAsteroids, delegate(bool x)
			{
				CheatToggles.animAsteroids = x;
			}),
			UILibrary.UI.Toggle(" Empty Garbage", () => CheatToggles.animEmptyGarbage, delegate(bool x)
			{
				CheatToggles.animEmptyGarbage = x;
			}),
			UILibrary.UI.Toggle(" Medbay Scan", () => CheatToggles.animScan, delegate(bool x)
			{
				CheatToggles.animScan = x;
			}),
			UILibrary.UI.Toggle(" Fake Cams In Use", () => CheatToggles.animCamsInUse, delegate(bool x)
			{
				CheatToggles.animCamsInUse = x;
			}),
			UILibrary.UI.Toggle(" Freeze", () => CheatToggles.freezeAnimations, delegate(bool x)
			{
				CheatToggles.freezeAnimations = x;
			}),
			UILibrary.UI.Toggle(" Pet", () => CheatToggles.animPet, delegate(bool x)
			{
				CheatToggles.animPet = x;
			}),
			UILibrary.UI.Toggle(" No Shhh Anim", () => CheatToggles.noShhScreenAnimation, delegate(bool x)
			{
				CheatToggles.noShhScreenAnimation = x;
			}),
			UILibrary.UI.Toggle(" No Kill Anim", () => CheatToggles.noKillAnimation, delegate(bool x)
			{
				CheatToggles.noKillAnimation = x;
			}),
			UILibrary.UI.Toggle(" No Seeker Anim", () => CheatToggles.noSeekerAnimation, delegate(bool x)
			{
				CheatToggles.noSeekerAnimation = x;
			})
		}, new List<SubmenuInfo>()));
		groups.Add(UILibrary.UI.Group("Account & Client Utility", "settings", isExpanded: false, new List<CreateList>
		{
			UILibrary.UI.Toggle(" Avoid Bans/Penalties", () => CheatToggles.avoidBans, delegate(bool x)
			{
				CheatToggles.avoidBans = x;
			}),
			UILibrary.UI.Toggle(" Copy Lobby Code On Disconnect", () => CheatToggles.copyLobbyCodeOnDisconnect, delegate(bool x)
			{
				CheatToggles.copyLobbyCodeOnDisconnect = x;
			}),
			UILibrary.UI.Toggle(" Spoof April Fools Date", () => CheatToggles.spoofAprilFoolsDate, delegate(bool x)
			{
				CheatToggles.spoofAprilFoolsDate = x;
			}),
			UILibrary.UI.Toggle(" Unlock Extra Mod Features", () => CheatToggles.unlockFeatures, delegate(bool x)
			{
				CheatToggles.unlockFeatures = x;
			}),
			UILibrary.UI.Toggle(" Debug Mode", () => CheatToggles.debugMode, delegate(bool x)
			{
				CheatToggles.debugMode = x;
			}),
			UILibrary.UI.Toggle(" Save Spoof Data", () => CheatToggles.saveSpoofData, delegate(bool x)
			{
				CheatToggles.saveSpoofData = x;
			}),
			UILibrary.UI.Toggle(" Kick Player (Client)", () => CheatToggles.kickPlayer, delegate(bool x)
			{
				CheatToggles.kickPlayer = x;
			}),
			UILibrary.UI.Toggle(" Zero Eject Crewmate Button CD", () => CheatToggles.zeroEjectCrewmateButtonCD, delegate(bool x)
			{
				CheatToggles.zeroEjectCrewmateButtonCD = x;
			})
		}, new List<SubmenuInfo>()));
		groups.Add(UILibrary.UI.Group("Settings & UI", "sliders", isExpanded: false, new List<CreateList>
		{
			UILibrary.UI.Toggle(" Open Menu Config File", () => true, delegate
			{
				Utils.OpenConfigFile();
			}),
			UILibrary.UI.Toggle(" Automatically Save Settings", () => SaveSettings.AutoSaveEnabled, delegate(bool x)
			{
				SaveSettings.SetAutoSave(x);
			}),
			UILibrary.UI.Button(" Save Configuration", SaveSettings.PromptSaveConfiguration),
			UILibrary.UI.Button(" Load Configuration", SaveSettings.PromptLoadConfiguration),
			UILibrary.UI.Button(" Delete Configuration", SaveSettings.PromptDeleteConfiguration),
			UILibrary.UI.Button(" Open Configurations Folder", SaveSettings.OpenConfigurationsFolder),
			UILibrary.UI.Toggle(" Menu RGB Mode", () => CheatToggles.RGBMode, delegate(bool x)
			{
				CheatToggles.RGBMode = x;
			}),
			UILibrary.UI.Toggle(" Teleport Mod Menu To Mouse", () => CheatToggles.teleportMenuToMouse, delegate(bool x)
			{
				CheatToggles.teleportMenuToMouse = x;
			}),
			UILibrary.UI.Toggle(" Enable Animations", () => _enableAnimations, delegate(bool x)
			{
				_enableAnimations = x;
			}),
			UILibrary.UI.Toggle(" Enable Shadows", () => _enableShadows, delegate(bool x)
			{
				_enableShadows = x;
			}),
			UILibrary.UI.Toggle(" Enable Glow Effects", () => _enableGlowEffects, delegate(bool x)
			{
				_enableGlowEffects = x;
			}),
			UILibrary.UI.Toggle(" Enable Rounded Corners", () => _enableRoundedCorners, delegate(bool x)
			{
				_enableRoundedCorners = x;
			}),
			UILibrary.UI.Button(" Reset Theme Colors", ResetThemeColors)
		}, new List<SubmenuInfo>
		{
			UILibrary.UI.Submenu("UI", false, null, UILibrary.UI.Toggle(" Console UI", () => CheatToggles.showConsoleMenu, delegate(bool x)
			{
				CheatToggles.showConsoleMenu = x;
			}), UILibrary.UI.Toggle(" Tasks UI", () => CheatToggles.showTasksMenu, delegate(bool x)
			{
				CheatToggles.showTasksMenu = x;
			}), UILibrary.UI.Toggle(" Doors UI", () => CheatToggles.showDoorsMenu, delegate(bool x)
			{
				CheatToggles.showDoorsMenu = x;
			}), UILibrary.UI.Toggle(" Protect UI", () => CheatToggles.showProtectMenu, delegate(bool x)
			{
				CheatToggles.showProtectMenu = x;
			}), UILibrary.UI.Toggle(" Role UI", () => CheatToggles.showRolesMenu, delegate(bool x)
			{
				CheatToggles.showRolesMenu = x;
			})),
			UILibrary.UI.Submenu("Color Themes", false, null, UILibrary.UI.Toggle(" Use Custom Color", () => _useCustomColor, delegate(bool x)
			{
				_useCustomColor = x;
			}), UILibrary.UI.Slider(" Custom Hue", () => _customHue, delegate(float x)
			{
				_customHue = x;
			}, 0f, 1f), UILibrary.UI.Slider(" Custom Saturation", () => _customSaturation, delegate(float x)
			{
				_customSaturation = x;
			}, 0f, 1f), UILibrary.UI.Slider(" Custom Brightness", () => _customBrightness, delegate(float x)
			{
				_customBrightness = x;
			}, 0f, 1f)),
			UILibrary.UI.Submenu("Notifications", false, null, UILibrary.UI.Toggle(" Notify On Player Death", () => CheatToggles.notifyOnDeath, delegate(bool x)
			{
				CheatToggles.notifyOnDeath = x;
			}), UILibrary.UI.Toggle(" Notify On Player Disconnect", () => CheatToggles.notifyOnDisconnect, delegate(bool x)
			{
				CheatToggles.notifyOnDisconnect = x;
			}), UILibrary.UI.Toggle(" Notify On Vent Usage", () => CheatToggles.notifyOnVent, delegate(bool x)
			{
				CheatToggles.notifyOnVent = x;
			}))
		}));
		_windowHeight = 650f;
	}

	private void SetEditMode(int mode)
	{
		_editMode = mode;
		switch (mode)
		{
		case 0:
			_editVideoPath = "";
			_editAudioPath = "";
			if ((Object)(object)_editAudioSource != (Object)null && _editAudioSource.isPlaying)
			{
				_editAudioSource.Stop();
			}
			if ((Object)(object)_editVideoPlayer != (Object)null && _editVideoPlayer.isPlaying)
			{
				_editVideoPlayer.Stop();
			}
			break;
		case 1:
			_editVideoPath = "C:/users/leuar/downloads/Its 2022.mp4";
			_editAudioPath = "C:/users/leuar/downloads/Its 2022.mp3";
			LoadAndPlayAudio(_editAudioPath);
			LoadAndPlayVideo(_editVideoPath);
			break;
		case 2:
			_editVideoPath = "C:/users/leuar/downloads/independence.mp4";
			_editAudioPath = "C:/users/leuar/downloads/independence.mp3";
			LoadAndPlayAudio(_editAudioPath);
			LoadAndPlayVideo(_editVideoPath);
			break;
		}
	}

	private void LoadAndPlayVideo(string path)
	{
		if (!((Object)(object)_editVideoPlayer == (Object)null))
		{
			if (File.Exists(path))
			{
				_editVideoPlayer.url = "file:///" + path;
				_editVideoPlayer.Play();
			}
			else
			{
				ZenithX.Warning("[MenuUI] Video file not found: " + path);
			}
		}
	}

	private void LoadAndPlayAudio(string path)
	{
		if (!((Object)(object)_editAudioSource == (Object)null))
		{
			if (File.Exists(path))
			{
				_currentAudioPath = path;
			}
			else
			{
				ZenithX.Warning("[MenuUI] Audio file not found: " + path);
			}
		}
	}

	[HideFromIl2Cpp]
	private IEnumerator LoadAudioCoroutine()
	{
		yield return null;
	}

	private void OnDestroy()
	{
		if ((Object)(object)_videoRenderTexture != (Object)null)
		{
			_videoRenderTexture.Release();
			Object.Destroy((Object)(object)_videoRenderTexture);
		}
		if ((Object)(object)_editVideoPlayer != (Object)null)
		{
			Object.Destroy((Object)(object)_editVideoPlayer);
		}
		if ((Object)(object)_editAudioSource != (Object)null)
		{
			Object.Destroy((Object)(object)_editAudioSource);
		}
	}

	private static Color A(Color c, float a)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		return new Color(c.r, c.g, c.b, a);
	}

	private static float SmoothEase(float t)
	{
		t = Mathf.Clamp01(t);
		return t * t * (3f - 2f * t);
	}

	private static void ResetThemeColors()
	{
		_customHue = 0.6f;
		_customSaturation = 0.8f;
		_customBrightness = 0.9f;
		_useCustomColor = false;
		AlertUI.Success("Theme colors reset to default!");
	}

	private static Color GetCurrentAccent()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (_useCustomColor)
		{
			return Color.HSVToRGB(_customHue, _customSaturation, _customBrightness);
		}
		return UIAccent;
	}

	private GUIContent GetContent(string text)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		if (!CachedContents.TryGetValue(text, out var value))
		{
			value = new GUIContent(text);
			CachedContents[text] = value;
		}
		return value;
	}

	private void UpdateAnimations()
	{
		if (!_enableAnimations)
		{
			_menuAnim = (isGUIActive ? 1f : 0f);
			for (int i = 0; i < groups.Count; i++)
			{
				_groupAnim[i] = (groups[i].isExpanded ? 1f : 0f);
			}
			return;
		}
		float unscaledDeltaTime = Time.unscaledDeltaTime;
		if (!isGUIActive && _menuAnim <= 0.01f)
		{
			bool flag = true;
			for (int j = 0; j < groups.Count; j++)
			{
				float num = (groups[j].isExpanded ? 1f : 0f);
				if (!_groupAnim.TryGetValue(j, out var value) || Mathf.Abs(value - num) > 0.01f)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				return;
			}
		}
		float num2 = (isGUIActive ? 1f : 0f);
		_menuAnim = Mathf.SmoothDamp(_menuAnim, num2, ref _menuVel, 0.08f, 1000f, unscaledDeltaTime);
		for (int k = 0; k < groups.Count; k++)
		{
			if (!_groupAnim.ContainsKey(k))
			{
				_groupAnim[k] = (groups[k].isExpanded ? 1f : 0f);
			}
			if (!_groupVel.ContainsKey(k))
			{
				_groupVel[k] = 0f;
			}
			float num3 = (groups[k].isExpanded ? 1f : 0f);
			float num4 = _groupAnim[k];
			float value2 = _groupVel[k];
			num4 = Mathf.SmoothDamp(num4, num3, ref value2, 0.1f, 1000f, unscaledDeltaTime);
			_groupAnim[k] = num4;
			_groupVel[k] = value2;
		}
	}

	private void SyncFavoritesGroup()
	{
		if (groups == null)
		{
			return;
		}
		for (int i = 0; i < groups.Count; i++)
		{
			if (string.Equals(groups[i].name, favoritesGroup.name, StringComparison.Ordinal))
			{
				GroupInfo value = groups[i];
				value.items = favoritesGroup.items;
				groups[i] = value;
				break;
			}
		}
	}

	private void Update()
	{
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			SaveSettings.Update();
			if (ZenithX.menuKeybind != null && LastMenuKeybind != ZenithX.menuKeybind.Value)
			{
				LastMenuKeybind = ZenithX.menuKeybind.Value;
				CachedMenuKey = Utils.stringToKeycode(LastMenuKeybind);
			}
			if (ZenithX.toggleZoom != null && _lastToggleZoomKeybind != ZenithX.toggleZoom.Value)
			{
				_lastToggleZoomKeybind = ZenithX.toggleZoom.Value;
				_cachedToggleZoomKey = Utils.stringToKeycode(_lastToggleZoomKeybind);
			}
			if (Input.GetKeyDown(CachedMenuKey))
			{
				isGUIActive = !isGUIActive;
				if (isGUIActive && !_groupsInitialized)
				{
					InitializeGroups();
				}
				if (CheatToggles.teleportMenuToMouse)
				{
					Vector2 val = Vector2.op_Implicit(Input.mousePosition);
					((Rect)(ref WindowRect)).position = new Vector2(val.x, (float)Screen.height - val.y);
				}
			}
			if (!isGUIActive)
			{
				_menuAnim = 0f;
				return;
			}
			UpdateAnimations();
			if (!Utils.isPlayer)
			{
				return;
			}
			if (Input.GetKeyDown((KeyCode)282))
			{
				PlayerControl localPlayer = PlayerControl.LocalPlayer;
				if (localPlayer != null)
				{
					PetBehaviour pet = localPlayer.GetPet();
					if (pet != null)
					{
						pet.StartPetAnim();
					}
				}
			}
			if (Input.GetKeyDown((KeyCode)286))
			{
				MinigamePatches.CompleteCurrentTask();
			}
			if (Input.GetKeyDown(_cachedToggleZoomKey))
			{
				CheatToggles.zoomOut = !CheatToggles.zoomOut;
			}
			if (!Utils.isHost)
			{
				CheatToggles.voteImmune = false;
				CheatToggles.impostorHack = false;
				CheatToggles.evilVote = false;
			}
			if (!Utils.isShip)
			{
				CheatToggles.unfixableLights = false;
				CheatToggles.reportBody = false;
				CheatToggles.closeMeeting = false;
				CheatToggles.reactorSab = false;
				CheatToggles.oxygenSab = false;
				CheatToggles.commsSab = false;
				CheatToggles.elecSab = false;
				CheatToggles.mushSab = false;
			}
			if (CheatToggles.RGBMode)
			{
				hue += Time.unscaledDeltaTime * 0.3f;
				if (hue > 1f)
				{
					hue -= 1f;
				}
			}
			OverloadHandler.Run();
			MenuLock.Update();
		}
		catch (Exception value)
		{
			ZenithX.Warning($"[MenuUI] Update Error: {value}");
		}
	}

	[HideFromIl2Cpp]
	private void CheckForFavoriteKey(CreateList element, Rect itemRect)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Invalid comparison between Unknown and I4
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Invalid comparison between Unknown and I4
		if (element != null && ContainsMouse(itemRect) && (int)Event.current.type == 4 && (int)Event.current.keyCode == 102)
		{
			ToggleFavorite(element);
			Event.current.Use();
		}
	}

	private static string GetFavoriteItemKey(CreateList element)
	{
		if (element == null)
		{
			return string.Empty;
		}
		return (element.GetType().Name + "|" + (element.label ?? string.Empty)).Trim();
	}

	[HideFromIl2Cpp]
	private bool IsFavorite(CreateList element)
	{
		if (element == null)
		{
			return false;
		}
		string favoriteItemKey = GetFavoriteItemKey(element);
		for (int i = 0; i < favoritesGroup.items.Count; i++)
		{
			if (string.Equals(GetFavoriteItemKey(favoritesGroup.items[i]), favoriteItemKey, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	[HideFromIl2Cpp]
	private void ToggleFavorite(CreateList originalElement)
	{
		if (originalElement == null)
		{
			return;
		}
		string favoriteItemKey = GetFavoriteItemKey(originalElement);
		int num = -1;
		for (int i = 0; i < favoritesGroup.items.Count; i++)
		{
			if (string.Equals(GetFavoriteItemKey(favoritesGroup.items[i]), favoriteItemKey, StringComparison.OrdinalIgnoreCase))
			{
				num = i;
				break;
			}
		}
		if (num >= 0)
		{
			favoritesGroup.items.RemoveAt(num);
			SyncFavoritesGroup();
			AlertUI.Success("Removed '" + originalElement.label + "' from favorites!");
			return;
		}
		CreateList createList = CloneFavoriteItem(originalElement);
		if (createList != null)
		{
			favoritesGroup.items.Add(createList);
			SyncFavoritesGroup();
			AlertUI.Success("Added '" + originalElement.label + "' to favorites!");
		}
	}

	private static CreateList CloneFavoriteItem(CreateList original)
	{
		if (original is ToggleInfo toggleInfo)
		{
			return new ToggleInfo(toggleInfo.label, toggleInfo.getState, toggleInfo.setState);
		}
		if (original is ButtonInfo buttonInfo)
		{
			return new ButtonInfo(buttonInfo.label, buttonInfo.action);
		}
		if (original is SliderInfo sliderInfo)
		{
			return new SliderInfo(sliderInfo.label, sliderInfo.getValue, sliderInfo.setValue, sliderInfo.minValue, sliderInfo.maxValue);
		}
		if (original is InputInfo inputInfo)
		{
			return new InputInfo(inputInfo.label, inputInfo.getValue, inputInfo.setValue);
		}
		if (original is KeybindInfo keybindInfo)
		{
			return new KeybindInfo(keybindInfo.label, keybindInfo.getValue, keybindInfo.setValue);
		}
		if (original is ColorPickerInfo colorPickerInfo)
		{
			return new ColorPickerInfo(colorPickerInfo.label, colorPickerInfo.getValue, colorPickerInfo.setValue);
		}
		return null;
	}

	private string GetGlyphForIcon(string iconKey)
	{
		if (string.IsNullOrEmpty(iconKey))
		{
			return "•";
		}
		return iconKey.ToLowerInvariant() switch
		{
			"user" => "◉", 
			"person" => "◉", 
			"eye" => "◌", 
			"shield" => "⬢", 
			"map" => "⌂", 
			"message" => "✉", 
			"chat" => "✉", 
			"crown" => "♛", 
			"sparkles" => "✦", 
			"star" => "✦", 
			"play" => "▶", 
			"settings" => "⚙", 
			"sliders" => "▤", 
			"stats" => "▣", 
			"list" => "☰", 
			"info" => "◍", 
			"host" => "♛", 
			"utility" => "⚙", 
			"favorites" => "★", 
			_ => "•", 
		};
	}

	private void DrawIconGlyph(Rect rect, string iconKey, Color tint)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		if (!string.IsNullOrEmpty(iconKey))
		{
			if (_iconStyle == null)
			{
				InitializeGUIStyles();
			}
			string glyphForIcon = GetGlyphForIcon(iconKey);
			_iconStyle.fontSize = Mathf.Max(10, Mathf.RoundToInt(Mathf.Min(((Rect)(ref rect)).width, ((Rect)(ref rect)).height) * 0.9f));
			_iconStyle.normal.textColor = tint;
			_iconStyle.alignment = (TextAnchor)4;
			_iconStyle.fontStyle = (FontStyle)1;
			SetGUIColor(tint);
			GUI.Label(rect, glyphForIcon, _iconStyle);
			SetGUIColor(TextDark);
		}
	}

	public void OnGUI()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Invalid comparison between Unknown and I4
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Invalid comparison between Unknown and I4
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Expected O, but got Unknown
		//IL_0097: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Invalid comparison between Unknown and I4
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Invalid comparison between Unknown and I4
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Invalid comparison between Unknown and I4
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Invalid comparison between Unknown and I4
		try
		{
			if (!isGUIActive)
			{
				return;
			}
			if (!MenuLock.IsUnlocked)
			{
				MenuLock.DrawLockScreen();
				return;
			}
			Event current = Event.current;
			if (current != null && ((int)current.type == 7 || (int)current.type == 8 || (int)current.type == 0 || (int)current.type == 1 || (int)current.type == 3 || (int)current.type == 4 || (int)current.type == 5))
			{
				if (_windowBg == null)
				{
					_windowBg = new GUIStyle
					{
						padding = new RectOffset(),
						border = new RectOffset()
					};
				}
				((Rect)(ref WindowRect)).width = _windowWidth;
				((Rect)(ref WindowRect)).height = _windowHeight;
				WindowRect = GUI.Window(0, WindowRect, WindowFunction.op_Implicit((Action<int>)WindowFunction), GUIContent.none, _windowBg);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(Object.op_Implicit("[MenuUI] OnGUI CRASH: " + ex));
			isGUIActive = false;
		}
	}

	private void InitializeGUIStyles()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_002a: Expected O, but got Unknown
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		if (!_stylesInitialized)
		{
			_windowBg = new GUIStyle
			{
				padding = new RectOffset(),
				border = new RectOffset()
			};
			_headerLabelStyle = CreateStyle(18, (FontStyle)1, (TextAnchor)3, UIText);
			_groupTitleStyle = CreateStyle(12, (FontStyle)1, (TextAnchor)3, UIText);
			_itemLabelStyle = CreateStyle(11, (FontStyle)0, (TextAnchor)3, UIText);
			_subItemLabelStyle = CreateStyle(10, (FontStyle)0, (TextAnchor)3, UIText);
			_buttonStyle = CreateStyle(11, (FontStyle)0, (TextAnchor)3, UIText);
			_checkmarkStyle = CreateStyle(11, (FontStyle)0, (TextAnchor)4, Color.white);
			_searchStyle = CreateStyle(11, (FontStyle)0, (TextAnchor)3, UIMuted);
			_versionStyle = CreateStyle(10, (FontStyle)0, (TextAnchor)5, UIMuted);
			_submenuIndicatorStyle = CreateStyle(9, (FontStyle)0, (TextAnchor)4, UIMuted);
			_iconStyle = CreateStyle(10, (FontStyle)1, (TextAnchor)4, TextDark);
			_sidebarGroupLabelStyle = CreateStyle(10, (FontStyle)1, (TextAnchor)3, TextDark);
			_groupLabelStyle = CreateStyle(10, (FontStyle)1, (TextAnchor)3, TextDark);
			_keybindValueStyle = CreateStyle(10, (FontStyle)1, (TextAnchor)4, UIAccent);
			_colorHexStyle = CreateStyle(9, (FontStyle)1, (TextAnchor)4, Color.white);
			_sliderLabelStyle = CreateStyle(10, (FontStyle)0, (TextAnchor)3, UIText);
			_sliderValueStyle = CreateStyle(10, (FontStyle)0, (TextAnchor)5, UIText);
			_smallBoldLabelStyle = CreateStyle(9, (FontStyle)1, (TextAnchor)3, UIText);
			_emptyStateStyle = CreateStyle(12, (FontStyle)2, (TextAnchor)4, UIMuted);
			_stylesInitialized = true;
		}
	}

	private static GUIStyle CreateStyle(int fontSize, FontStyle fontStyle, TextAnchor alignment, Color textColor)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		//IL_0037: Expected O, but got Unknown
		return new GUIStyle(GUI.skin.label)
		{
			fontSize = fontSize,
			fontStyle = fontStyle,
			alignment = alignment,
			normal = new GUIStyleState
			{
				textColor = textColor
			}
		};
	}

	private void UpdatePalette()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		Color currentAccent = GetCurrentAccent();
		if (!_paletteInitialized || !(currentAccent == _cachedAccent))
		{
			_cachedAccent = currentAccent;
			_palette = new ZPalette("custom", "Custom", new Color(0.08f, 0.1f, 0.14f, 0.65f), new Color(0.1f, 0.12f, 0.16f, 0.82f), currentAccent, UIAccentLight, new Color(0.12f, 0.14f, 0.18f, 0.92f), new Color(0.06f, 0.08f, 0.12f, 0.8f), UIText, UIMuted);
			_paletteInitialized = true;
		}
	}

	private void SetGUIColor(Color newColor)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (_currentColor != newColor)
		{
			GUI.color = newColor;
			_currentColor = newColor;
		}
	}

	private void UpdateSearchState()
	{
		if (_lastSearchQuery != searchQuery)
		{
			_lastSearchQuery = searchQuery;
			_searchDirty = true;
			_cachedSidebarHeight = -1f;
			_cachedContentHeight = -1f;
		}
	}

	private bool MatchesSearch(string text, string query)
	{
		if (!string.IsNullOrEmpty(query) && !string.IsNullOrEmpty(text))
		{
			return text.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
		}
		return false;
	}

	[HideFromIl2Cpp]
	private bool ItemMatchesSearch(CreateList item, string query)
	{
		if (string.IsNullOrEmpty(query) || item == null)
		{
			return true;
		}
		return MatchesSearch(item.label, query);
	}

	private bool SubmenuMatchesSearch(SubmenuInfo submenu, string query)
	{
		if (string.IsNullOrEmpty(query))
		{
			return true;
		}
		if (MatchesSearch(submenu.name, query))
		{
			return true;
		}
		if (submenu.items != null)
		{
			for (int i = 0; i < submenu.items.Count; i++)
			{
				if (ItemMatchesSearch(submenu.items[i], query))
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool GroupMatchesSearch(GroupInfo group, string query)
	{
		if (string.IsNullOrEmpty(query))
		{
			return true;
		}
		if (group.items != null)
		{
			for (int i = 0; i < group.items.Count; i++)
			{
				if (ItemMatchesSearch(group.items[i], query))
				{
					return true;
				}
			}
		}
		if (group.submenus != null)
		{
			for (int j = 0; j < group.submenus.Count; j++)
			{
				if (SubmenuMatchesSearch(group.submenus[j], query))
				{
					return true;
				}
			}
		}
		return false;
	}

	private void RefreshVisibleGroups()
	{
		_visibleGroupIds.Clear();
		if (string.IsNullOrEmpty(searchQuery))
		{
			for (int i = 0; i < groups.Count; i++)
			{
				_visibleGroupIds.Add(i);
			}
			return;
		}
		for (int j = 0; j < groups.Count; j++)
		{
			if (GroupMatchesSearch(groups[j], searchQuery))
			{
				_visibleGroupIds.Add(j);
			}
		}
	}

	private void ApplySearchExpansionState()
	{
		if (groups == null)
		{
			return;
		}
		for (int i = 0; i < groups.Count; i++)
		{
			GroupInfo groupInfo = groups[i];
			bool flag = GroupMatchesSearch(groupInfo, searchQuery);
			if (!string.IsNullOrEmpty(searchQuery) && flag)
			{
				groupInfo.isExpanded = true;
				groups[i] = groupInfo;
			}
			if (groupInfo.submenus != null)
			{
				for (int j = 0; j < groupInfo.submenus.Count; j++)
				{
					SubmenuInfo submenuInfo = groupInfo.submenus[j];
					submenuInfo.isExpanded = (string.IsNullOrEmpty(searchQuery) ? submenuInfo.isExpanded : SubmenuMatchesSearch(submenuInfo, searchQuery));
					groupInfo.submenus[j] = submenuInfo;
				}
				groups[i] = groupInfo;
			}
		}
	}

	private int GetSelectedGroupIndex()
	{
		if (groups == null)
		{
			return -1;
		}
		int num = -1;
		for (int i = 0; i < groups.Count; i++)
		{
			if (groups[i].isExpanded)
			{
				if (num >= 0)
				{
					GroupInfo value = groups[i];
					value.isExpanded = false;
					groups[i] = value;
				}
				else
				{
					num = i;
				}
			}
		}
		if (num >= 0)
		{
			return num;
		}
		if (!string.IsNullOrEmpty(searchQuery))
		{
			for (int j = 0; j < groups.Count; j++)
			{
				if (_visibleGroupIds.Contains(j))
				{
					return j;
				}
			}
		}
		for (int k = 0; k < groups.Count; k++)
		{
			if (_visibleGroupIds.Contains(k))
			{
				return k;
			}
		}
		return -1;
	}

	private float GetContentHeightForGroup(GroupInfo group)
	{
		if (_cachedContentHeight >= 0f && _cachedContentGroupId >= 0 && _cachedContentGroupId < groups.Count && string.IsNullOrEmpty(searchQuery))
		{
			return _cachedContentHeight;
		}
		float num = 54f;
		if (group.items != null)
		{
			num += (float)group.items.Count * 34f;
		}
		if (group.submenus != null)
		{
			for (int i = 0; i < group.submenus.Count; i++)
			{
				SubmenuInfo submenuInfo = group.submenus[i];
				num += 34f;
				if (submenuInfo.isExpanded && submenuInfo.items != null)
				{
					num += (float)submenuInfo.items.Count * 34f;
				}
			}
		}
		_cachedContentHeight = Mathf.Max(180f, num + 54f);
		return _cachedContentHeight;
	}

	private float GetSidebarHeight()
	{
		int count = groups.Count;
		if (_cachedSidebarHeight >= 0f && _cachedSidebarGroupCount == count && string.IsNullOrEmpty(searchQuery))
		{
			return _cachedSidebarHeight;
		}
		float num = 48f;
		for (int i = 0; i < groups.Count; i++)
		{
			if (string.IsNullOrEmpty(searchQuery) || _visibleGroupIds.Contains(i))
			{
				num += 38f;
			}
		}
		_cachedSidebarHeight = Mathf.Max(220f, num);
		_cachedSidebarGroupCount = count;
		return _cachedSidebarHeight;
	}

	[HideFromIl2Cpp]
	private void DrawGroupContentItem(Rect rect, CreateList item, Color accent, Vector2 mousePos)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Invalid comparison between Unknown and I4
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		if (item == null)
		{
			return;
		}
		CheckForFavoriteKey(item, rect);
		if (item is ToggleInfo toggleInfo)
		{
			bool flag = toggleInfo.getState();
			bool flag2 = ContainsMouse(rect, mousePos, 68f);
			Color val = (Color)(flag ? new Color(0.7f, 0.82f, 0.98f, 0.95f) : ButtonLight);
			Style.FillRounded(rect, flag2 ? ButtonHover : val, 7);
			Style.StrokeRounded(rect, A(accent, flag ? 0.32f : 0.14f), 7, 1);
			Rect r = default(Rect);
			((Rect)(ref r))._002Ector(((Rect)(ref rect)).x + 8f, ((Rect)(ref rect)).y + 6f, 16f, 16f);
			if (flag)
			{
				Style.FillRounded(r, new Color(0.5f, 0.7f, 0.98f, 0.95f), 4);
				Style.StrokeRounded(r, new Color(0.38f, 0.55f, 0.86f, 0.9f), 4, 1);
				SetGUIColor(Color.white);
				GUI.Label(new Rect(((Rect)(ref r)).x + 2f, ((Rect)(ref r)).y - 1f, 16f, 18f), "✓", _checkmarkStyle);
				SetGUIColor(TextDark);
			}
			else
			{
				Style.FillRounded(r, new Color(0.88f, 0.9f, 0.94f, 0.9f), 4);
				Style.StrokeRounded(r, A(accent, 0.12f), 4, 1);
			}
			GUI.Label(new Rect(((Rect)(ref rect)).x + 30f, ((Rect)(ref rect)).y + 5f, ((Rect)(ref rect)).width - 36f, 20f), toggleInfo.label, _itemLabelStyle);
			if ((int)Event.current.type == 0 && ContainsMouse(rect, mousePos, 68f))
			{
				toggleInfo.setState(!flag);
				Event.current.Use();
			}
		}
		else if (item is ButtonInfo buttonInfo)
		{
			bool flag3 = ContainsMouse(rect, mousePos, 68f);
			Color c = (Color)(flag3 ? new Color(0.88f, 0.92f, 1f, 1f) : ButtonLight);
			Style.FillRounded(rect, c, 7);
			Style.StrokeRounded(rect, A(accent, flag3 ? 0.45f : 0.14f), 7, 1);
			GUI.Label(new Rect(((Rect)(ref rect)).x + 10f, ((Rect)(ref rect)).y + 5f, ((Rect)(ref rect)).width - 20f, 20f), buttonInfo.label, _buttonStyle);
			if ((int)Event.current.type == 0 && flag3)
			{
				buttonInfo.action?.Invoke();
				Event.current.Use();
			}
		}
		else if (item is SliderInfo slider)
		{
			UILibrary.DrawSlider(rect, slider, accent, mousePos, _sliderLabelStyle, _sliderValueStyle, _enableRoundedCorners);
		}
		else if (item is InputInfo input)
		{
			UILibrary.DrawInput(rect, input, accent, mousePos, _sliderLabelStyle, _enableRoundedCorners);
		}
		else if (item is KeybindInfo keybind)
		{
			UILibrary.DrawKeybind(rect, keybind, accent, mousePos, _sliderLabelStyle, _keybindValueStyle, TextDark);
		}
		else if (item is ColorPickerInfo colorPicker)
		{
			UILibrary.DrawColorPicker(rect, colorPicker, accent, mousePos, _sliderLabelStyle, _colorHexStyle, TextDark, _enableRoundedCorners);
		}
	}

	private Vector2 GetScrollMousePosition(Rect scrollViewRect)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (Event.current == null)
		{
			return Vector2.zero;
		}
		return new Vector2(Event.current.mousePosition.x - ((Rect)(ref scrollViewRect)).x + _scrollPosition.x, Event.current.mousePosition.y - ((Rect)(ref scrollViewRect)).y + _scrollPosition.y);
	}

	private void DrawSubmenu(Rect rect, SubmenuInfo submenu, Color accent, Vector2 mousePos)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		bool isExpanded = submenu.isExpanded;
		bool num = ContainsMouse(rect, mousePos, 68f);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width, 30f);
		Color c = (num ? new Color(0.96f, 0.98f, 1f, 1f) : new Color(0.97f, 0.99f, 1f, 0.96f));
		if (isExpanded)
		{
			((Color)(ref c))._002Ector(0.94f, 0.97f, 1f, 0.94f);
		}
		Style.FillRounded(val, c, 8);
		Style.StrokeRounded(val, A(accent, isExpanded ? 0.45f : 0.14f), 8, 1);
		SetGUIColor(isExpanded ? TextDark : TextMutedDark);
		GUI.Label(new Rect(((Rect)(ref val)).x + 10f, ((Rect)(ref val)).y + 6f, 16f, 16f), isExpanded ? "▾" : "▸", _submenuIndicatorStyle);
		GUI.Label(new Rect(((Rect)(ref val)).x + 28f, ((Rect)(ref val)).y + 4f, ((Rect)(ref val)).width - 40f, 18f), submenu.name, _groupTitleStyle);
		SetGUIColor(TextDark);
		if ((int)Event.current.type == 0 && ContainsMouse(val, mousePos, 68f))
		{
			submenu.isExpanded = !submenu.isExpanded;
			_cachedContentHeight = -1f;
			_cachedSidebarHeight = -1f;
			for (int i = 0; i < groups.Count; i++)
			{
				GroupInfo value = groups[i];
				if (value.submenus == null)
				{
					continue;
				}
				for (int j = 0; j < value.submenus.Count; j++)
				{
					if (value.submenus[j].name == submenu.name)
					{
						value.submenus[j] = submenu;
						groups[i] = value;
						Event.current.Use();
						return;
					}
				}
			}
		}
		if (!isExpanded)
		{
			return;
		}
		float num2 = ((Rect)(ref rect)).y + 32f;
		if (submenu.items == null)
		{
			return;
		}
		Rect rect2 = default(Rect);
		for (int k = 0; k < submenu.items.Count; k++)
		{
			if (ItemMatchesSearch(submenu.items[k], searchQuery))
			{
				((Rect)(ref rect2))._002Ector(((Rect)(ref rect)).x + 8f, num2, ((Rect)(ref rect)).width - 16f, 30f);
				DrawGroupContentItem(rect2, submenu.items[k], accent, mousePos);
				num2 += 34f;
			}
		}
	}

	private void DrawEditMode(Rect windowRect, Color panelColor, Color textColor, Color accent)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_009b: Expected O, but got Unknown
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Expected O, but got Unknown
		//IL_01b5: Expected O, but got Unknown
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Expected O, but got Unknown
		//IL_02b6: Expected O, but got Unknown
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Expected O, but got Unknown
		//IL_0399: Expected O, but got Unknown
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_0480: Unknown result type (might be due to invalid IL or missing references)
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a1: Expected O, but got Unknown
		//IL_04a6: Expected O, but got Unknown
		//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0523: Unknown result type (might be due to invalid IL or missing references)
		//IL_0530: Unknown result type (might be due to invalid IL or missing references)
		//IL_053d: Unknown result type (might be due to invalid IL or missing references)
		//IL_054f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_050a: Unknown result type (might be due to invalid IL or missing references)
		//IL_051c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0554: Unknown result type (might be due to invalid IL or missing references)
		//IL_0556: Unknown result type (might be due to invalid IL or missing references)
		//IL_0558: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_0562: Unknown result type (might be due to invalid IL or missing references)
		//IL_0574: Unknown result type (might be due to invalid IL or missing references)
		//IL_058f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0600: Unknown result type (might be due to invalid IL or missing references)
		//IL_060f: Expected O, but got Unknown
		//IL_0614: Expected O, but got Unknown
		//IL_0619: Unknown result type (might be due to invalid IL or missing references)
		//IL_0627: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			float num = 48f;
			float num2 = ((Rect)(ref windowRect)).y + num + 16f;
			float num3 = ((Rect)(ref windowRect)).height - num - 32f;
			string text = ((_editMode == 1) ? "Edit 1 - Its 2022" : "Edit 2 - Independence");
			GUI.color = accent;
			GUI.Label(new Rect(((Rect)(ref windowRect)).x + 16f, num2, ((Rect)(ref windowRect)).width - 32f, 24f), text, new GUIStyle
			{
				fontSize = 16,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)3,
				normal = new GUIStyleState
				{
					textColor = accent
				}
			});
			Style.Fill(new Rect(((Rect)(ref windowRect)).x + 16f, num2 + 26f, ((Rect)(ref windowRect)).width - 32f, 2f), A(accent, 0.5f));
			num2 += 36f;
			float num4 = num3 * 0.6f;
			Rect r = default(Rect);
			((Rect)(ref r))._002Ector(((Rect)(ref windowRect)).x + 16f, num2, ((Rect)(ref windowRect)).width - 32f, num4);
			Style.FillRounded(r, A(panelColor, 0.85f), 12);
			Style.StrokeRounded(r, A(accent, 0.35f), 12, 1);
			GUI.color = textColor;
			GUI.Label(new Rect(((Rect)(ref r)).x + 16f, ((Rect)(ref r)).y + 16f, ((Rect)(ref r)).width - 32f, 24f), "Video Display", new GUIStyle
			{
				fontSize = 13,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)3,
				normal = new GUIStyleState
				{
					textColor = new Color(0.9f, 0.95f, 1f)
				}
			});
			if ((Object)(object)_videoRenderTexture != (Object)null && (Object)(object)_editVideoPlayer != (Object)null && _editVideoPlayer.isPlaying)
			{
				GUI.DrawTexture(new Rect(((Rect)(ref r)).x + 16f, ((Rect)(ref r)).y + 48f, ((Rect)(ref r)).width - 32f, ((Rect)(ref r)).height - 64f), (Texture)(object)_videoRenderTexture, (ScaleMode)2);
			}
			else
			{
				GUI.color = UIMuted;
				GUI.Label(new Rect(((Rect)(ref r)).x + 16f, ((Rect)(ref r)).y + 48f, ((Rect)(ref r)).width - 32f, ((Rect)(ref r)).height - 64f), "Playing: " + _editVideoPath + "\n\n[Video loading...]", new GUIStyle
				{
					fontSize = 11,
					alignment = (TextAnchor)3,
					wordWrap = true,
					normal = new GUIStyleState
					{
						textColor = UIMuted
					}
				});
			}
			num2 += num4 + 16f;
			float num5 = num3 - num4 - 32f;
			Rect r2 = default(Rect);
			((Rect)(ref r2))._002Ector(((Rect)(ref windowRect)).x + 16f, num2, ((Rect)(ref windowRect)).width - 32f, num5);
			Style.FillRounded(r2, A(panelColor, 0.85f), 12);
			Style.StrokeRounded(r2, A(accent, 0.35f), 12, 1);
			GUI.color = textColor;
			GUI.Label(new Rect(((Rect)(ref r2)).x + 16f, ((Rect)(ref r2)).y + 16f, ((Rect)(ref r2)).width - 32f, 24f), "Audio Controls", new GUIStyle
			{
				fontSize = 13,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)3,
				normal = new GUIStyleState
				{
					textColor = new Color(0.9f, 0.95f, 1f)
				}
			});
			string value = (((Object)(object)_editAudioSource != (Object)null && _editAudioSource.isPlaying) ? "Playing" : "Stopped");
			Color textColor2 = (GUI.color = (((Object)(object)_editAudioSource != (Object)null && _editAudioSource.isPlaying) ? accent : UIMuted));
			GUI.Label(new Rect(((Rect)(ref r2)).x + 16f, ((Rect)(ref r2)).y + 48f, ((Rect)(ref r2)).width - 32f, ((Rect)(ref r2)).height - 64f), $"Audio: {_editAudioPath}\nStatus: {value}\n\n[Audio playback controls would be displayed here]", new GUIStyle
			{
				fontSize = 11,
				alignment = (TextAnchor)3,
				wordWrap = true,
				normal = new GUIStyleState
				{
					textColor = textColor2
				}
			});
			Rect r3 = default(Rect);
			((Rect)(ref r3))._002Ector(((Rect)(ref windowRect)).x + ((Rect)(ref windowRect)).width - 100f, ((Rect)(ref windowRect)).y + 8f, 80f, 32f);
			bool flag = ((Rect)(ref r3)).Contains(Event.current.mousePosition);
			Color c = (flag ? new Color(accent.r * 0.3f, accent.g * 0.3f, accent.b * 0.3f, 0.9f) : new Color(accent.r * 0.2f, accent.g * 0.2f, accent.b * 0.2f, 0.8f));
			Style.FillRounded(r3, c, 6);
			Style.StrokeRounded(r3, A(accent, flag ? 0.4f : 0.25f), 6, 1);
			GUI.color = new Color(0.9f, 0.95f, 1f);
			GUI.Label(new Rect(((Rect)(ref r3)).x + 8f, ((Rect)(ref r3)).y + 6f, ((Rect)(ref r3)).width - 16f, 20f), "Back", new GUIStyle
			{
				fontSize = 12,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)4,
				normal = new GUIStyleState
				{
					textColor = new Color(0.9f, 0.95f, 1f)
				}
			});
			if ((int)Event.current.type == 0 && ((Rect)(ref r3)).Contains(Event.current.mousePosition))
			{
				_editMode = 0;
				if ((Object)(object)_editAudioSource != (Object)null && _editAudioSource.isPlaying)
				{
					_editAudioSource.Stop();
				}
				if ((Object)(object)_editVideoPlayer != (Object)null && _editVideoPlayer.isPlaying)
				{
					_editVideoPlayer.Stop();
				}
				Event.current.Use();
			}
		}
		catch (Exception ex)
		{
			ZenithX.Warning("[MenuUI] Legacy edit overlay failed: " + ex.Message);
			_editMode = 0;
		}
	}

	public void WindowFunction(int windowID)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Invalid comparison between Unknown and I4
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Invalid comparison between Unknown and I4
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_052b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0536: Unknown result type (might be due to invalid IL or missing references)
		//IL_053b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0557: Unknown result type (might be due to invalid IL or missing references)
		//IL_0576: Unknown result type (might be due to invalid IL or missing references)
		//IL_059a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a0: Invalid comparison between Unknown and I4
		//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05be: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c4: Invalid comparison between Unknown and I4
		//IL_08ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0951: Unknown result type (might be due to invalid IL or missing references)
		//IL_096a: Unknown result type (might be due to invalid IL or missing references)
		//IL_091a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0933: Unknown result type (might be due to invalid IL or missing references)
		//IL_06da: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_06df: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_070b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0707: Unknown result type (might be due to invalid IL or missing references)
		//IL_0713: Unknown result type (might be due to invalid IL or missing references)
		//IL_0715: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_09da: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_09fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bc2: Unknown result type (might be due to invalid IL or missing references)
		//IL_072b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a42: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a57: Unknown result type (might be due to invalid IL or missing references)
		//IL_076a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bec: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0786: Unknown result type (might be due to invalid IL or missing references)
		//IL_077f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0801: Unknown result type (might be due to invalid IL or missing references)
		//IL_0819: Unknown result type (might be due to invalid IL or missing references)
		//IL_0824: Unknown result type (might be due to invalid IL or missing references)
		//IL_082e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0830: Unknown result type (might be due to invalid IL or missing references)
		//IL_0abd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b65: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b69: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b6a: Unknown result type (might be due to invalid IL or missing references)
		_currentColor = Color.white;
		SetGUIColor(Color.white);
		UpdateSearchState();
		if (_searchDirty)
		{
			ApplySearchExpansionState();
			RefreshVisibleGroups();
			_searchDirty = false;
		}
		if (!_stylesInitialized)
		{
			InitializeGUIStyles();
		}
		UpdatePalette();
		if (Time.frameCount % 30 == 0)
		{
			SyncFavoritesGroup();
		}
		if (_editMode != 0)
		{
			if (File.Exists(_editVideoPath) || File.Exists(_editAudioPath))
			{
				DrawEditMode(new Rect(0f, 0f, _windowWidth, _windowHeight), UIPanel, UIText, _cachedAccent);
				return;
			}
			_editMode = 0;
		}
		Event current = Event.current;
		if (current != null)
		{
			Rect val = default(Rect);
			((Rect)(ref val))._002Ector(0f, 0f, ((Rect)(ref WindowRect)).width, 48f);
			Vector2 mousePosition = current.mousePosition;
			if ((int)current.type == 0 && ((Rect)(ref val)).Contains(mousePosition))
			{
				isDragging = true;
				current.Use();
			}
			else if ((int)current.type == 1 || (int)current.type == 21)
			{
				isDragging = false;
			}
		}
		Color cachedAccent = _cachedAccent;
		float windowWidth = _windowWidth;
		float windowHeight = _windowHeight;
		Rect r = new Rect(0f, 0f, windowWidth, windowHeight);
		if (!isDragging)
		{
			((Rect)(ref WindowRect)).width = _windowWidth;
			((Rect)(ref WindowRect)).height = _windowHeight;
		}
		if (_enableGlowEffects)
		{
			Style.FillRounded(new Rect(-12f, -12f, windowWidth + 24f, windowHeight + 24f), A(cachedAccent, 0.04f), 14);
		}
		if (_enableShadows)
		{
			Style.FillRounded(new Rect(8f, 8f, windowWidth, windowHeight), new Color(0f, 0f, 0f, 0.1f), 12);
		}
		Style.FillRounded(r, new Color(0.97f, 0.99f, 1f, 0.82f), 14);
		Style.StrokeRounded(r, A(cachedAccent, 0.55f), 14, 1);
		Style.FillRounded(new Rect(12f, 12f, windowWidth - 24f, windowHeight - 24f), new Color(0.99f, 1f, 1f, 0.9f), 12);
		Style.StrokeRounded(new Rect(12f, 12f, windowWidth - 24f, windowHeight - 24f), A(cachedAccent, 0.18f), 12, 1);
		Style.FillRounded(new Rect(0f, 0f, windowWidth, 48f), new Color(0.91f, 0.95f, 1f, 0.95f), 12);
		Style.Fill(new Rect(0f, 47f, windowWidth, 1f), A(cachedAccent, 0.42f));
		string text = "v" + ZenithX.ZenithXVersion;
		int playerCount = PlayerCountAPI.PlayerCount;
		SetGUIColor(TextDark);
		GUI.Label(new Rect(22f, 12f, windowWidth - 220f, 22f), "ZenithX", _headerLabelStyle);
		GUI.Label(new Rect(windowWidth - 170f, 9f, 150f, 16f), $"Players: {playerCount}", _versionStyle);
		GUI.Label(new Rect(windowWidth - 170f, 25f, 150f, 14f), text, _versionStyle);
		float num = 285f;
		float num2 = num + 8f;
		float num3 = windowWidth - num - 20f;
		Style.Fill(new Rect(num, 48f, 1f, windowHeight - 48f), A(cachedAccent, 0.14f));
		Style.FillRounded(new Rect(12f, 58f, num - 16f, windowHeight - 72f), new Color(0.98f, 0.99f, 1f, 0.72f), 10);
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(14f, 60f, num - 20f, windowHeight - 78f);
		float sidebarHeight = GetSidebarHeight();
		_sidebarScrollPosition = GUI.BeginScrollView(val2, _sidebarScrollPosition, new Rect(0f, 0f, num - 24f, sidebarHeight));
		Rect val3 = default(Rect);
		((Rect)(ref val3))._002Ector(10f, 8f, num - 42f, 28f);
		Style.FillRounded(val3, new Color(0.94f, 0.97f, 1f, 0.9f), 7);
		Style.StrokeRounded(val3, A(cachedAccent, 0.22f), 7, 1);
		SetGUIColor(TextMutedDark);
		GUI.Label(new Rect(((Rect)(ref val3)).x + 10f, ((Rect)(ref val3)).y + 4f, ((Rect)(ref val3)).width - 20f, 18f), string.IsNullOrEmpty(searchQuery) ? "Search..." : searchQuery, _searchStyle);
		SetGUIColor(TextDark);
		Vector2 mousePosition2 = current.mousePosition;
		Rect val4 = default(Rect);
		((Rect)(ref val4))._002Ector(num2, 60f, num3, windowHeight - 76f);
		Vector2 mousePos = default(Vector2);
		((Vector2)(ref mousePos))._002Ector(current.mousePosition.x - ((Rect)(ref val4)).x + _scrollPosition.x, current.mousePosition.y - ((Rect)(ref val4)).y + _scrollPosition.y);
		if ((int)current.type == 4 && ContainsMouse(val3, mousePosition2))
		{
			if ((int)current.keyCode == 8 && searchQuery.Length > 0)
			{
				searchQuery = searchQuery.Substring(0, searchQuery.Length - 1);
				_searchDirty = true;
				current.Use();
			}
			else if (current.character != 0 && !char.IsControl(current.character))
			{
				searchQuery += current.character;
				_searchDirty = true;
				current.Use();
			}
		}
		bool flag = !string.IsNullOrEmpty(searchQuery);
		int num4 = 48;
		Rect val5 = default(Rect);
		Color val7 = default(Color);
		for (int i = 0; i < groups.Count; i++)
		{
			if (flag && !_visibleGroupIds.Contains(i))
			{
				continue;
			}
			GroupInfo value = groups[i];
			((Rect)(ref val5))._002Ector(8f, (float)num4, num - 32f, 34f);
			Color val6 = (((Rect)(ref val5)).Contains(mousePosition2) ? new Color(0.96f, 0.98f, 1f, 1f) : new Color(0.97f, 0.98f, 1f, 0.95f));
			((Color)(ref val7))._002Ector(0.74f, 0.88f, 1f, 0.94f);
			Style.FillRounded(val5, value.isExpanded ? val7 : val6, 8);
			Style.StrokeRounded(val5, A(cachedAccent, value.isExpanded ? 0.4f : 0.14f), 8, 1);
			if (!string.IsNullOrEmpty(value.icon))
			{
				DrawIconGlyph(new Rect(((Rect)(ref val5)).x + 10f, ((Rect)(ref val5)).y + 7f, 18f, 18f), value.icon, value.isExpanded ? TextDark : TextMutedDark);
			}
			Color val8 = (Color)(value.isExpanded ? TextDark : new Color(0.22f, 0.28f, 0.35f, 1f));
			_sidebarGroupLabelStyle.normal.textColor = val8;
			SetGUIColor(val8);
			GUI.Label(new Rect(((Rect)(ref val5)).x + 32f, ((Rect)(ref val5)).y + 7f, ((Rect)(ref val5)).width - 48f, 18f), value.name, _sidebarGroupLabelStyle);
			SetGUIColor(TextDark);
			if ((int)current.type == 0 && ContainsMouse(val5, mousePosition2))
			{
				value.isExpanded = !value.isExpanded;
				if (value.isExpanded)
				{
					for (int j = 0; j < groups.Count; j++)
					{
						if (j != i)
						{
							GroupInfo value2 = groups[j];
							value2.isExpanded = false;
							groups[j] = value2;
						}
					}
				}
				groups[i] = value;
				_cachedSidebarHeight = -1f;
				_cachedContentHeight = -1f;
				current.Use();
			}
			num4 += 38;
		}
		SetGUIColor(TextDark);
		GUI.EndScrollView();
		int selectedGroupIndex = GetSelectedGroupIndex();
		if (_enableRoundedCorners)
		{
			Style.FillRounded(new Rect(num2, 60f, num3, windowHeight - 76f), new Color(0.96f, 0.98f, 1f, 0.7f), 10);
		}
		else
		{
			Style.Fill(new Rect(num2, 60f, num3, windowHeight - 76f), new Color(0.96f, 0.98f, 1f, 0.7f));
		}
		((Rect)(ref val4))._002Ector(num2, 60f, num3, windowHeight - 76f);
		if (selectedGroupIndex != _cachedContentGroupId)
		{
			_cachedContentGroupId = selectedGroupIndex;
			_cachedContentHeight = -1f;
		}
		float num5 = ((selectedGroupIndex >= 0 && selectedGroupIndex < groups.Count) ? GetContentHeightForGroup(groups[selectedGroupIndex]) : 180f);
		_scrollPosition = GUI.BeginScrollView(val4, _scrollPosition, new Rect(0f, 0f, num3 - 20f, num5));
		if (selectedGroupIndex >= 0 && selectedGroupIndex < groups.Count)
		{
			float num6 = 16f;
			float num7 = 12f;
			float num8 = num3 - 30f;
			GroupInfo groupInfo = groups[selectedGroupIndex];
			SetGUIColor(TextDark);
			GUI.Label(new Rect(num6, num7, num8, 22f), groupInfo.name, _groupTitleStyle);
			num7 += 30f;
			if (groupInfo.items != null)
			{
				Rect rect = default(Rect);
				for (int k = 0; k < groupInfo.items.Count; k++)
				{
					CreateList item = groupInfo.items[k];
					if (!flag || ItemMatchesSearch(item, searchQuery))
					{
						((Rect)(ref rect))._002Ector(num6, num7, num8, 30f);
						DrawGroupContentItem(rect, item, cachedAccent, mousePos);
						num7 += 34f;
					}
				}
			}
			if (groupInfo.submenus != null)
			{
				Rect rect2 = default(Rect);
				for (int l = 0; l < groupInfo.submenus.Count; l++)
				{
					SubmenuInfo submenu = groupInfo.submenus[l];
					if (!flag || SubmenuMatchesSearch(submenu, searchQuery))
					{
						float num9 = 34f + ((submenu.isExpanded && submenu.items != null) ? ((float)submenu.items.Count * 34f) : 0f);
						((Rect)(ref rect2))._002Ector(num6, num7, num8, num9);
						DrawSubmenu(rect2, submenu, cachedAccent, mousePos);
						num7 += num9 + 4f;
					}
				}
			}
			float num10 = 34f;
			GUI.Label(new Rect(num6, num7, num8, num10), "", GUIStyle.none);
			num7 += num10;
		}
		SetGUIColor(TextDark);
		GUI.EndScrollView();
		if (isDragging && current != null)
		{
			GUI.DragWindow(new Rect(0f, 0f, windowWidth, 48f));
		}
		SetGUIColor(Color.white);
	}

	private void DrawSlider(Rect rect, SliderInfo slider, Color accent, Vector2 mousePos, Color textColor)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		UILibrary.DrawSlider(rect, slider, accent, mousePos, _sliderLabelStyle, _sliderValueStyle, _enableRoundedCorners);
	}

	private void DrawInput(Rect rect, InputInfo input, Color accent, Vector2 mousePos, Color textColor)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		UILibrary.DrawInput(rect, input, accent, mousePos, _sliderLabelStyle, _enableRoundedCorners);
	}

	private void DrawKeybind(Rect rect, KeybindInfo keybind, Color accent, Vector2 mousePos, Color textColor)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		UILibrary.DrawKeybind(rect, keybind, accent, mousePos, _sliderLabelStyle, _keybindValueStyle, TextDark);
	}

	private void DrawColorPicker(Rect rect, ColorPickerInfo colorPicker, Color accent, Vector2 mousePos, Color textColor)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		UILibrary.DrawColorPicker(rect, colorPicker, accent, mousePos, _sliderLabelStyle, _colorHexStyle, TextDark, _enableRoundedCorners);
	}
}
