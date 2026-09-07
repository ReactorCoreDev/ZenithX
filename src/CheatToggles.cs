using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using AmongUs.GameOptions;
using UnityEngine;

namespace ZenithX;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct CheatToggles
{
	public static bool forceRole;

	public static RoleTypes? forcedRole;

	public static bool noClip;

	public static bool speedBoost;

	public static bool teleportPlayer;

	public static bool teleportCursor;

	public static bool reportBody;

	public static bool killPlayer;

	public static bool telekillPlayer;

	public static bool murderPlayer;

	public static bool murderAll;

	public static bool forceAumRpcForEveryone;

	public static bool invertControls;

	public static bool changeRole;

	public static bool zeroKillCd;

	public static bool completeMyTasks;

	public static bool killReach;

	public static bool killAnyone;

	public static bool endlessSsDuration;

	public static bool endlessBattery;

	public static bool endlessTracking;

	public static bool noTrackingCooldown;

	public static bool noTrackingDelay;

	public static bool noVitalsCooldown;

	public static bool noVentCooldown;

	public static bool endlessVentTime;

	public static bool endlessVanish;

	public static bool killVanished;

	public static bool noShapeshiftAnim;

	public static bool showPlayerInfo;

	public static bool fullBright;

	public static bool seeGhosts;

	public static bool seeRoles;

	public static bool seeDisguises;

	public static bool revealVotes;

	public static bool moreLobbyInfo;

	public static bool spectate;

	public static bool zoomOut;

	public static bool freecam;

	public static bool mapCrew;

	public static bool mapImps;

	public static bool mapGhosts;

	public static bool colorBasedMap;

	public static bool tracersImps;

	public static bool tracersCrew;

	public static bool tracersGhosts;

	public static bool tracersBodies;

	public static bool colorBasedTracers;

	public static bool distanceBasedTracers;

	public static bool alwaysChat;

	public static bool chatJailbreak;

	public static bool closeMeeting;

	public static bool doorsSab;

	public static bool unfixableLights;

	public static bool commsSab;

	public static bool elecSab;

	public static bool reactorSab;

	public static bool oxygenSab;

	public static bool mushSab;

	public static bool mushSpore;

	public static bool spamCloseAllDoors;

	public static bool autoOpenDoorsOnUse;

	public static bool useVents;

	public static bool walkVent;

	public static bool kickVents;

	public static bool impostorHack;

	public static bool evilVote;

	public static bool voteImmune;

	public static bool unlockFeatures;

	public static bool freeCosmetics;

	public static bool avoidBans;

	public static bool noOptionsLimits;

	public static bool RGBMode;

	public static bool revive;

	public static bool noAbilityCD;

	public static bool EnableCheatDetection;

	public static bool AntiCheatEnabled;

	public static bool AutoKickCheaters;

	public static bool LogCheatAttempts;

	public static bool fakeScan;

	public static bool fakeTrash;

	public static bool autoKillNearby;

	public static bool changeMapToSabotage;

	public static bool spamOpenAllDoors;

	public static bool closeAllDoors;

	public static bool openAllDoors;

	public static bool ejectPlayer;

	public static bool notifyOnDeath;

	public static bool notifyOnDisconnect;

	public static bool notifyOnVent;

	public static bool showNotificationLog;

	public static bool logTasks;

	public static bool logGameState;

	public static bool incognitoMode;

	public static bool freezeAnimations;

	public static bool spamChat;

	public static bool chatMimic;

	public static bool resetOutfit;

	public static bool shuffleAllOutfits;

	public static bool shuffleOutfit;

	public static bool shapeshiftAll;

	public static bool resetShapeshift;

	public static bool animShields;

	public static bool animAsteroids;

	public static bool animEmptyGarbage;

	public static bool animScan;

	public static bool animCamsInUse;

	public static bool saveSpoofData;

	public static bool noCooldowns;

	public static bool kickPlayer;

	public static bool noGameEnd;

	public static bool impostorTasks;

	public static bool copyLobbyCodeOnDisconnect;

	public static bool spoofAprilFoolsDate;

	public static bool trackReach;

	public static bool interrogateReach;

	public static bool infiniteInterrogates;

	public static bool showProtectMenu;

	public static bool showRolesMenu;

	public static bool showConsoleMenu;

	public static bool showDoorsMenu;

	public static bool showTasksMenu;

	public static bool animPet;

	public static bool teleportMenuToMouse;

	public static bool forceStartOnClickStart;

	public static bool logVents;

	public static bool noVanishCooldown;

	public static bool revivePlayer;

	public static bool ContinuousKillingAll;

	public static bool shapeshiftCheat;

	public static bool nametagVision;

	public static bool autoKeypadCode;

	public static bool setFakeAlive;

	public static bool debugMode;

	public static bool zeroEjectCrewmateButtonCD;

	public static bool instantDissolve;

	public static bool uncappedFPS;

	public static bool discoColor;

	public static bool showTasksInMeetings;

	public static string bodyType;

	public static bool showProtectMenuSettings;

	public static bool olAutoStart;

	public static bool olAutoAdapt;

	public static bool olShowRpcTotal;

	public static bool olAutoStop;

	public static bool olLockTargets;

	public static bool olKillSwitch;

	public static bool olPlayerCooldown;

	public static bool olAutoClear;

	public static bool olLogStartStop;

	public static bool olLogAddRemove;

	public static bool olLogDisconnect;

	public static bool olLogAttack;

	public static bool olVerboseLogs;

	public static bool runOverload;

	public static bool overloadAll;

	public static bool overloadHost;

	public static bool overloadCrew;

	public static bool overloadImps;

	public static bool overloadReset;

	public static bool attemptToCrashLobby;

	public static bool memeify;

	public static bool spamTpAll;

	public static bool spamTpImps;

	public static bool endlessVanishDuration;

	public static bool showLobbyTimer;

	public static bool ImmortalityEnabled;

	public static bool killOtherImpostors;

	public static bool DisableVents;

	public static bool BlockSabotages;

	public static bool customSeekers;

	public static bool taskArrows;

	public static bool allowDuplicateColor;

	public static bool applyingDuplicateColor;

	public static bool noShhScreenAnimation;

	public static bool noKillAnimation;

	public static bool noSeekerAnimation;

	public static bool notifyShapeshift;

	public static bool judgeOverrule;

	public static int destroyInGamePlayerId;

	public static int seekersCount;

	public static readonly Dictionary<string, KeyCode> Keybinds;

	public static readonly Dictionary<string, FieldInfo> ToggleFields;

	public static bool Immortality
	{
		get
		{
			return ImmortalityEnabled;
		}
		set
		{
			if (ImmortalityEnabled == value)
			{
				return;
			}
			if ((Object)(object)PlayerControl.LocalPlayer != (Object)null && !PlayerControl.LocalPlayer.inVent)
			{
				if (value)
				{
					VentilationSystem.Update((Operation)2, 50);
				}
				else
				{
					VentilationSystem.Update((Operation)3, 50);
				}
			}
			ImmortalityEnabled = value;
		}
	}

	static CheatToggles()
	{
		EnableCheatDetection = true;
		AntiCheatEnabled = false;
		AutoKickCheaters = false;
		LogCheatAttempts = true;
		debugMode = true;
		bodyType = "Default";
		destroyInGamePlayerId = -1;
		seekersCount = 3;
		Keybinds = new Dictionary<string, KeyCode>();
		ToggleFields = new Dictionary<string, FieldInfo>();
		FieldInfo[] fields = typeof(CheatToggles).GetFields(BindingFlags.Static | BindingFlags.Public);
		foreach (FieldInfo fieldInfo in fields)
		{
			if (!(fieldInfo.FieldType != typeof(bool)))
			{
				ToggleFields[fieldInfo.Name] = fieldInfo;
				Keybinds[fieldInfo.Name] = (KeyCode)0;
			}
		}
	}

	public static void DisablePPMCheats(string variableToKeep)
	{
		reportBody = !(variableToKeep != "reportBody") && reportBody;
		killPlayer = !(variableToKeep != "killPlayer") && killPlayer;
		telekillPlayer = !(variableToKeep != "telekillPlayer") && telekillPlayer;
		spectate = !(variableToKeep != "spectate") && spectate;
		changeRole = !(variableToKeep != "changeRole") && changeRole;
		teleportPlayer = !(variableToKeep != "teleportPlayer") && teleportPlayer;
		revivePlayer = !(variableToKeep != "revivePlayer") && revivePlayer;
		shapeshiftCheat = !(variableToKeep != "shapeshiftCheat") && shapeshiftCheat;
		saveSpoofData = !(variableToKeep != "saveSpoofData") && saveSpoofData;
		setFakeAlive = variableToKeep == "setFakeAlive" && setFakeAlive;
	}

	public static bool shouldPPMClose()
	{
		if (!setFakeAlive && !changeRole && !forceRole && !ejectPlayer && !reportBody && !telekillPlayer && !killPlayer && !spectate)
		{
			return !teleportPlayer;
		}
		return false;
	}

	public static void DisableAll()
	{
		foreach (FieldInfo value in ToggleFields.Values)
		{
			value.SetValue(null, false);
		}
	}
}
