namespace ZenithX;

public static class AntiCheatConfig
{
	public enum Punishments
	{
		None,
		Kick,
		ErrorKick,
		Ban
	}

	public static bool Enabled = true;

	public static bool SendNotifications = true;

	public static bool DiscardMaliciousRpc = true;

	public static Punishments Punishment = Punishments.None;

	public static float NotificationDuration = 10f;

	public static bool CheckPlayAnimation = true;

	public static bool CheckCompleteTask = true;

	public static bool CheckExiled = true;

	public static bool CheckName = true;

	public static bool CheckSetName = true;

	public static bool CheckSetColor = true;

	public static bool CheckReportDeadBody = true;

	public static bool CheckSetScanner = true;

	public static bool CheckSetStartCounter = true;

	public static bool CheckEnterVent = true;

	public static bool CheckExitVent = true;

	public static bool CheckSnapTo = true;

	public static bool CheckAddVote = true;

	public static bool CheckCloseDoors = true;

	public static bool CheckClimbLadder = true;

	public static bool CheckUsePlatform = true;

	public static bool CheckUpdateSystem = true;

	public static bool CheckClientReady = true;

	public static bool CheckSpoofedPlatforms = true;

	public static bool CheckSickoMenu = true;

	public static bool CheckAmongUsMenu = true;

	public static bool CheckBetterAmongUs = true;

	public static bool CheckKillNetwork = true;

	public static bool CheckHostGuard = true;

	public static bool CheckGoatNetClient = true;

	public static bool CheckBadWords = true;

	public static bool CheckChatSpam = true;

	public static bool CheckAFK = true;

	public static bool CheckSabotage = true;

	public static bool CheckEmergencies = true;

	public static bool CheckVotes = true;

	public static bool CheckRoles = true;

	public static bool CheckKillTimer = true;

	public static bool CheckColorChangeSpam = true;

	public static bool CheckTeleportation = true;

	public static bool CheckRpcFlood = true;

	public static bool CheckMultipleKills = true;

	public static bool CheckReportAbuse = true;

	public static bool CheckGhostAbuse = true;

	public static bool CheckVoteAbuse = true;

	public static float AFKThreshold = 60f;

	public static float SabotageCooldown = 30f;

	public static int MaxSabotagesPerGame = 5;

	public static float EmergencyCooldown = 30f;

	public static float MinKillInterval = 2f;

	public static float ColorChangeCooldown = 1f;

	public static int MaxColorChanges = 20;

	public static float TeleportDistanceThreshold = 15f;

	public static int MaxRpcPerWindow = 50;

	public static float RpcWindow = 1f;

	public static int MaxNoclipViolations = 3;

	public static int GhostLayer = 8;

	public static bool DetectCheatClients = true;

	public static bool DetectInvalidRpcs = true;

	public static bool PersistCheatData = true;

	public static bool CheckLobbyRPCs = true;

	public static bool CheckHostRPCs = true;

	public static bool CheckInGameSetRPCs = true;
}
