using System;
using System.Collections.Generic;
using AmongUs.InnerNet.GameDataMessages;
using InnerNet;
using UnityEngine;

namespace ZenithX;

public static class AntiCheat
{
	private static Dictionary<byte, DateTime> lastColorChangeTime = new Dictionary<byte, DateTime>();

	private static Dictionary<byte, int> colorChangeCount = new Dictionary<byte, int>();

	private static Dictionary<byte, Queue<float>> rpcTimestamps = new Dictionary<byte, Queue<float>>();

	private static Dictionary<byte, Vector2> lastValidPosition = new Dictionary<byte, Vector2>();

	private static Dictionary<byte, DateTime> lastKillTime = new Dictionary<byte, DateTime>();

	private static Dictionary<byte, int> killCount = new Dictionary<byte, int>();

	private static Dictionary<byte, DateTime> reportTimestamps = new Dictionary<byte, DateTime>();

	private static Dictionary<byte, int> reportCount = new Dictionary<byte, int>();

	private static Dictionary<byte, DateTime> voteTimestamps = new Dictionary<byte, DateTime>();

	private static Dictionary<byte, int> voteCount = new Dictionary<byte, int>();

	private static Dictionary<byte, int> currentLayer = new Dictionary<byte, int>();

	private static Dictionary<byte, int> noclipViolationCount = new Dictionary<byte, int>();

	private static readonly HashSet<byte> KnownCheatRPCs = new HashSet<byte>
	{
		69, 250, 164, 153, 200, 201, 150, 176, 202, 219,
		85, 121
	};

	public static Dictionary<byte, CheatAction> DetectedCheaters = new Dictionary<byte, CheatAction>();

	private static HashSet<string> KnownCheaters = new HashSet<string>();

	public static Dictionary<RpcCalls, RpcCheck> RpcHandlers = new Dictionary<RpcCalls, RpcCheck>
	{
		{
			(RpcCalls)0,
			new PlayAnimationCheck()
		},
		{
			(RpcCalls)1,
			new CompleteTaskCheck()
		},
		{
			(RpcCalls)4,
			new ExiledCheck()
		},
		{
			(RpcCalls)5,
			new CheckNameCheck()
		},
		{
			(RpcCalls)6,
			new SetNameCheck()
		},
		{
			(RpcCalls)8,
			new SetColorCheck()
		},
		{
			(RpcCalls)11,
			new ReportDeadBodyCheck()
		},
		{
			(RpcCalls)15,
			new SetScannerCheck()
		},
		{
			(RpcCalls)18,
			new SetStartCounterCheck()
		},
		{
			(RpcCalls)19,
			new EnterVentCheck()
		},
		{
			(RpcCalls)20,
			new ExitVentCheck()
		},
		{
			(RpcCalls)21,
			new SnapToCheck()
		},
		{
			(RpcCalls)26,
			new AddVoteCheck()
		},
		{
			(RpcCalls)27,
			new CloseDoorsCheck()
		},
		{
			(RpcCalls)31,
			new ClimbLadderCheck()
		},
		{
			(RpcCalls)32,
			new UsePlatformCheck()
		},
		{
			(RpcCalls)35,
			new UpdateSystemCheck()
		}
	};

	public static Dictionary<GameDataTypes, GameDataCheck> GameDataHandlers = new Dictionary<GameDataTypes, GameDataCheck> { 
	{
		(GameDataTypes)7,
		new ClientReadyCheck()
	} };

	public static void Flag(PlayerControl player, CheatAction action, string reason)
	{
		if (CheatToggles.AntiCheatEnabled && !((Object)(object)player == (Object)null) && !((Object)(object)player == (Object)(object)PlayerControl.LocalPlayer))
		{
			byte playerId = player.PlayerId;
			DetectedCheaters[playerId] = action;
			if (AntiCheatConfig.SendNotifications)
			{
				SendNotification(reason);
			}
			if (((InnerNetClient)AmongUsClient.Instance).AmHost)
			{
				Punish(player);
			}
		}
	}

	public static void Flag(string reason)
	{
		if (CheatToggles.AntiCheatEnabled && AntiCheatConfig.SendNotifications)
		{
			SendNotification(reason);
		}
	}

	private static void SendNotification(string reason)
	{
		AlertUI.Warning("<color=#8a2be2><b>ZenithX AntiCheat</b></color> " + reason);
	}

	private static void Punish(PlayerControl player)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Invalid comparison between Unknown and I4
		switch (AntiCheatConfig.Punishment)
		{
		case AntiCheatConfig.Punishments.Kick:
			((InnerNetClient)AmongUsClient.Instance).KickPlayer(((InnerNetObject)player).OwnerId, false);
			break;
		case AntiCheatConfig.Punishments.ErrorKick:
			if ((int)((InnerNetClient)AmongUsClient.Instance).GameState == 2)
			{
				((InnerNetClient)AmongUsClient.Instance).SendLateRejection(((InnerNetObject)player).OwnerId, (DisconnectReasons)215);
			}
			else
			{
				((InnerNetClient)AmongUsClient.Instance).KickPlayer(((InnerNetObject)player).OwnerId, false);
			}
			break;
		case AntiCheatConfig.Punishments.Ban:
			((InnerNetClient)AmongUsClient.Instance).KickPlayer(((InnerNetObject)player).OwnerId, true);
			break;
		}
	}

	public static void CheckColorChangeSpam(PlayerControl player)
	{
		if (!AntiCheatConfig.CheckColorChangeSpam || (Object)(object)player == (Object)null)
		{
			return;
		}
		byte playerId = player.PlayerId;
		DateTime now = DateTime.Now;
		if (!lastColorChangeTime.ContainsKey(playerId))
		{
			lastColorChangeTime[playerId] = now;
			colorChangeCount[playerId] = 1;
			return;
		}
		if ((float)(now - lastColorChangeTime[playerId]).TotalSeconds < AntiCheatConfig.ColorChangeCooldown)
		{
			colorChangeCount[playerId]++;
			if (colorChangeCount[playerId] > AntiCheatConfig.MaxColorChanges)
			{
				Flag(player, CheatAction.ColorChangeSpam, player.Data.PlayerName + " is spamming color changes");
			}
		}
		else
		{
			colorChangeCount[playerId] = 1;
		}
		lastColorChangeTime[playerId] = now;
	}

	public static void CheckRpcFlood(PlayerControl player)
	{
		if (AntiCheatConfig.CheckRpcFlood && !((Object)(object)player == (Object)null))
		{
			byte playerId = player.PlayerId;
			float time = Time.time;
			if (!rpcTimestamps.ContainsKey(playerId))
			{
				rpcTimestamps[playerId] = new Queue<float>();
			}
			Queue<float> queue = rpcTimestamps[playerId];
			while (queue.Count > 0 && time - queue.Peek() > AntiCheatConfig.RpcWindow)
			{
				queue.Dequeue();
			}
			queue.Enqueue(time);
			if (queue.Count > AntiCheatConfig.MaxRpcPerWindow)
			{
				Flag(player, CheatAction.RPCFlood, player.Data.PlayerName + " is flooding RPCs");
			}
		}
	}

	public static void CheckTeleportation(PlayerControl player, Vector2 newPosition)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		if (!AntiCheatConfig.CheckTeleportation || (Object)(object)player == (Object)null)
		{
			return;
		}
		byte playerId = player.PlayerId;
		if (player.inVent || Utils.GameLoaded)
		{
			return;
		}
		if (!lastValidPosition.ContainsKey(playerId))
		{
			lastValidPosition[playerId] = newPosition;
			return;
		}
		float num = Vector2.Distance(lastValidPosition[playerId], newPosition);
		if (num > AntiCheatConfig.TeleportDistanceThreshold && Game.IsInGame())
		{
			Flag(player, CheatAction.Teleportation, $"{player.Data.PlayerName} teleported {num:F1} units");
		}
		lastValidPosition[playerId] = newPosition;
	}

	public static void CheckMultipleKills(PlayerControl player)
	{
		if (!AntiCheatConfig.CheckMultipleKills || (Object)(object)player == (Object)null)
		{
			return;
		}
		byte playerId = player.PlayerId;
		DateTime now = DateTime.Now;
		if (!killCount.ContainsKey(playerId))
		{
			killCount[playerId] = 1;
			lastKillTime[playerId] = now;
			return;
		}
		killCount[playerId]++;
		if (killCount[playerId] > 2 && (now - lastKillTime[playerId]).TotalSeconds < 5.0)
		{
			Flag(player, CheatAction.MultipleKills, player.Data.PlayerName + " killed multiple people rapidly");
		}
		if ((now - lastKillTime[playerId]).TotalSeconds > 10.0)
		{
			killCount[playerId] = 1;
		}
		lastKillTime[playerId] = now;
	}

	public static void CheckReportAbuse(PlayerControl player)
	{
		if (!AntiCheatConfig.CheckReportAbuse || (Object)(object)player == (Object)null)
		{
			return;
		}
		byte playerId = player.PlayerId;
		DateTime now = DateTime.Now;
		if (!reportTimestamps.ContainsKey(playerId))
		{
			reportTimestamps[playerId] = now;
			reportCount[playerId] = 1;
			return;
		}
		if ((float)(now - reportTimestamps[playerId]).TotalSeconds < 5f)
		{
			reportCount[playerId]++;
			if (reportCount[playerId] > 3)
			{
				Flag(player, CheatAction.ReportAbuse, player.Data.PlayerName + " is spamming reports");
			}
		}
		else
		{
			reportCount[playerId] = 1;
		}
		reportTimestamps[playerId] = now;
	}

	public static void CheckVoteAbuse(PlayerControl player)
	{
		if (!AntiCheatConfig.CheckVoteAbuse || (Object)(object)player == (Object)null)
		{
			return;
		}
		byte playerId = player.PlayerId;
		DateTime now = DateTime.Now;
		if (!voteTimestamps.ContainsKey(playerId))
		{
			voteTimestamps[playerId] = now;
			voteCount[playerId] = 1;
			return;
		}
		if ((float)(now - voteTimestamps[playerId]).TotalSeconds < 2f)
		{
			voteCount[playerId]++;
			if (voteCount[playerId] > 5)
			{
				Flag(player, CheatAction.VoteAbuse, player.Data.PlayerName + " is spamming votes");
			}
		}
		else
		{
			voteCount[playerId] = 1;
		}
		voteTimestamps[playerId] = now;
	}

	public static void CheckGhostAbuse(PlayerControl player)
	{
		if (AntiCheatConfig.CheckGhostAbuse && !((Object)(object)player == (Object)null))
		{
			_ = player.PlayerId;
			NetworkedPlayerInfo playerData = Game.GetPlayerData(player);
			if (!((Object)(object)playerData == (Object)null) && !playerData.IsDead && ((Component)player).gameObject.layer == AntiCheatConfig.GhostLayer)
			{
				Flag(player, CheatAction.GhostAbuse, player.Data.PlayerName + " is on ghost layer while alive");
			}
		}
	}

	public static void CheckCheatClientRPC(PlayerControl player, byte callId)
	{
		if (AntiCheatConfig.DetectCheatClients && !((Object)(object)player == (Object)null) && !((Object)(object)player == (Object)(object)PlayerControl.LocalPlayer) && KnownCheatRPCs.Contains(callId))
		{
			string cheatName = GetCheatName(callId);
			Flag(player, CheatAction.ModDetectionRPC, player.Data.PlayerName + " detected using " + cheatName);
			if (AntiCheatConfig.PersistCheatData)
			{
				PersistCheaterData(player, cheatName);
			}
		}
	}

	private static string GetCheatName(byte callId)
	{
		switch (callId)
		{
		case 69:
		case 200:
			return "SickoMenu";
		case 250:
			return "KillNetwork";
		case 164:
		case 201:
			return "AmongUsMenu";
		case 150:
		case 153:
			return "BetterAmongUs";
		case 176:
			return "HostGuard";
		case 202:
			return "GoatNetClient";
		case 219:
			return "BanMod";
		case 85:
			return "AmongUsMenu";
		case 121:
			return "ChocooMenu";
		default:
			return "Unknown Cheat";
		}
	}

	private static void PersistCheaterData(PlayerControl player, string cheatName)
	{
		if (!((Object)(object)((player != null) ? player.Data : null) == (Object)null))
		{
			string item = player.Data.PlayerName + "_" + player.Data.FriendCode;
			if (!KnownCheaters.Contains(item))
			{
				KnownCheaters.Add(item);
				AntiCheatLogger.LogInfo($"Persisted cheater: {player.Data.PlayerName} ({cheatName})");
			}
		}
	}

	public static bool CheckGameStateRPC(PlayerControl player, byte callId)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Invalid comparison between Unknown and I4
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Invalid comparison between Unknown and I4
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Invalid comparison between Unknown and I4
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Invalid comparison between Unknown and I4
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Invalid comparison between Unknown and I4
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Expected I4, but got Unknown
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Invalid comparison between Unknown and I4
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Invalid comparison between Unknown and I4
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Expected I4, but got Unknown
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Invalid comparison between Unknown and I4
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		if (!AntiCheatConfig.Enabled || (Object)(object)player == (Object)null)
		{
			return true;
		}
		if ((Object)(object)player == (Object)(object)PlayerControl.LocalPlayer)
		{
			return true;
		}
		RpcCalls val = (RpcCalls)callId;
		if (AntiCheatConfig.CheckHostRPCs && !player.Data.IsDead && !((InnerNetClient)AmongUsClient.Instance).AmHost && (((int)val == 22 || (int)val == 29 || (int)val == 61) ? true : false))
		{
			Flag(player, CheatAction.InvalidRPC, $"{player.Data.PlayerName} sent host-only RPC {val}");
			return false;
		}
		if (AntiCheatConfig.CheckInGameSetRPCs && Game.IsInGame() && (((int)val == 8 || val - 39 <= 4) ? true : false))
		{
			Flag(player, CheatAction.AbnormalCosmetics, player.Data.PlayerName + " changed appearance during game");
			return false;
		}
		bool flag;
		if (AntiCheatConfig.CheckLobbyRPCs && Game.IsInLobby())
		{
			if ((int)val <= 4)
			{
				if ((int)val != 1 && (int)val != 4)
				{
					goto IL_01d7;
				}
			}
			else
			{
				switch (val - 11)
				{
				default:
					switch (val - 44)
					{
					case 0:
					case 1:
					case 2:
					case 3:
					case 4:
					case 8:
					case 11:
					case 12:
					case 18:
					case 19:
					case 20:
					case 21:
						break;
					default:
						goto IL_01d7;
					}
					break;
				case 0:
				case 1:
				case 3:
				case 5:
				case 8:
				case 9:
				case 11:
				case 13:
				case 14:
				case 16:
				case 20:
				case 21:
				case 23:
					break;
				case 2:
				case 4:
				case 6:
				case 7:
				case 10:
				case 12:
				case 15:
				case 17:
				case 18:
				case 19:
				case 22:
					goto IL_01d7;
				}
			}
			flag = true;
			goto IL_01d9;
		}
		goto IL_021d;
		IL_01d7:
		flag = false;
		goto IL_01d9;
		IL_01d9:
		if (flag)
		{
			Flag(player, CheatAction.InvalidRPC, $"{player.Data.PlayerName} sent game RPC in lobby: {val}");
			return false;
		}
		goto IL_021d;
		IL_021d:
		return true;
	}

	public static bool IsTrustedRPC(byte callId)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Invalid comparison between Unknown and I4
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Invalid comparison between Unknown and I4
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		foreach (RpcCalls value in Enum.GetValues(typeof(RpcCalls)))
		{
			if ((int)value == callId || (int)value == callId || (byte)value == callId)
			{
				return true;
			}
		}
		return false;
	}

	public static uint GetExpectedNetId(PlayerControl player)
	{
		if (!IsAnticheatPresent())
		{
			return ((InnerNetObject)player.Data).NetId;
		}
		return ((InnerNetObject)player).NetId;
	}

	public static bool IsAnticheatPresent()
	{
		if (Constants.IsVersionModded() || (Object)(object)PlayerControl.LocalPlayer == (Object)null || (Object)(object)PlayerControl.LocalPlayer.Data == (Object)null)
		{
			return false;
		}
		return ((InnerNetObject)PlayerControl.LocalPlayer.Data).OwnerId != ((InnerNetClient)AmongUsClient.Instance).HostId;
	}

	public static MapNames GetCurrentMap()
	{
		return (MapNames)Utils.GetCurrentMapID();
	}

	public static bool IsValidSabotageSystem(SystemTypes system)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Invalid comparison between Unknown and I4
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Invalid comparison between Unknown and I4
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Invalid comparison between Unknown and I4
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Invalid comparison between Unknown and I4
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Invalid comparison between Unknown and I4
		if ((int)system != 7 && (int)system != 8 && (int)system != 14 && (int)system != 3 && (int)system != 21 && (int)system != 58 && (int)system != 57)
		{
			return (int)system == 17;
		}
		return true;
	}

	public static bool IsValidPlatform(PlatformSpecificData platform)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected I4, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Invalid comparison between Unknown and I4
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Invalid comparison between Unknown and I4
		string platformName = platform.PlatformName;
		ulong xboxPlatformId = platform.XboxPlatformId;
		ulong psnPlatformId = platform.PsnPlatformId;
		Platforms platform2 = platform.Platform;
		switch (platform2 - 1)
		{
		default:
			if ((int)platform2 == 255)
			{
				return (int)((InnerNetClient)AmongUsClient.Instance).NetworkMode == 0;
			}
			return false;
		case 0:
		case 1:
		case 2:
		case 4:
		case 5:
		case 6:
			if (IsGenericPlatformName(platformName) && xboxPlatformId == 0L)
			{
				return psnPlatformId == 0;
			}
			return false;
		case 3:
			if (IsGenericPlatformName(platformName) && xboxPlatformId != 0L)
			{
				return psnPlatformId == 0;
			}
			return false;
		case 8:
			if (!IsGenericPlatformName(platformName) && platformName.Length >= 3 && platformName.Length <= 16 && xboxPlatformId != 0L)
			{
				return psnPlatformId == 0;
			}
			return false;
		case 9:
			if (!IsGenericPlatformName(platformName) && xboxPlatformId == 0L)
			{
				return psnPlatformId != 0;
			}
			return false;
		case 7:
			if (!IsGenericPlatformName(platformName) && xboxPlatformId == 0L)
			{
				return psnPlatformId == 0;
			}
			return false;
		}
	}

	public static bool IsGenericPlatformName(string platformName)
	{
		return platformName == "TESTNAME";
	}

	public static void CheckPlatformSpoof(PlayerControl player)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if (!AntiCheatConfig.CheckSpoofedPlatforms || (Object)(object)player == (Object)null)
		{
			return;
		}
		ClientData clientFromCharacter = ((InnerNetClient)AmongUsClient.Instance).GetClientFromCharacter(player);
		if (clientFromCharacter != null)
		{
			PlatformSpecificData platformData = clientFromCharacter.PlatformData;
			if (!IsValidPlatform(platformData))
			{
				Flag(player, CheatAction.PlatformSpoofing, $"{clientFromCharacter.PlayerName} spoofed platform identity. Platform: {platformData.Platform}, name: {platformData.PlatformName}, XUID: {platformData.XboxPlatformId}, PSID: {platformData.PsnPlatformId}");
			}
		}
	}
}
