using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;

namespace ZenithX;

public static class NotificationHandler
{
	private static void AddLog(string message)
	{
		ConsoleUI.logEntries.Add(message);
		AlertUI.Notification(message, 10f);
	}

	private static (string realName, string displayName, bool isDisguised) GetPlayerIdentity(PlayerControl player)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)player == (Object)null || (Object)(object)player.Data == (Object)null)
		{
			return (realName: "", displayName: "", isDisguised: false);
		}
		string item = $"<color=#{ColorUtility.ToHtmlStringRGB(player.Data.Color)}>{player.Data.PlayerName}</color>";
		string item2 = $"<color=#{ColorUtility.ToHtmlStringRGB(Color32.op_Implicit(((Il2CppArrayBase<Color32>)(object)Palette.PlayerColors)[player.CurrentOutfit.ColorId]))}>{player.CurrentOutfit.PlayerName}</color>";
		bool item3 = player.CurrentOutfit.PlayerName != player.Data.PlayerName;
		return (realName: item, displayName: item2, isDisguised: item3);
	}

	public static void HandlePlayerKill(PlayerControl killer, PlayerControl victim)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		if (CheatToggles.notifyOnDeath && !((Object)(object)killer == (Object)null) && !((Object)(object)victim == (Object)null))
		{
			(string realName, string displayName, bool isDisguised) playerIdentity = GetPlayerIdentity(killer);
			string item = playerIdentity.realName;
			string item2 = playerIdentity.displayName;
			bool item3 = playerIdentity.isDisguised;
			string value = $"<color=#{ColorUtility.ToHtmlStringRGB(victim.Data.Color)}>{victim.CurrentOutfit.PlayerName}</color>";
			PlainShipRoom roomFromPosition = Utils.getRoomFromPosition(victim.GetTruePosition());
			string value2 = (((Object)(object)roomFromPosition != (Object)null) ? ((object)roomFromPosition.RoomId/*cast due to .constrained prefix*/).ToString() : "an unknown location");
			string message = ((!item3) ? $"{item} killed {value} in {value2}." : $"{item} (as {item2}) killed {value} in {value2}.");
			AddLog(message);
		}
	}

	public static void HandleGuardianAngelSave(PlayerControl killer, PlayerControl target)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		if (CheatToggles.notifyOnDeath && !((Object)(object)killer == (Object)null) && !((Object)(object)target == (Object)null))
		{
			(string realName, string displayName, bool isDisguised) playerIdentity = GetPlayerIdentity(killer);
			string item = playerIdentity.realName;
			string item2 = playerIdentity.displayName;
			bool item3 = playerIdentity.isDisguised;
			string value = $"<color=#{ColorUtility.ToHtmlStringRGB(target.Data.Color)}>{target.CurrentOutfit.PlayerName}</color>";
			PlainShipRoom roomFromPosition = Utils.getRoomFromPosition(target.GetTruePosition());
			string value2 = (((Object)(object)roomFromPosition != (Object)null) ? ((object)roomFromPosition.RoomId/*cast due to .constrained prefix*/).ToString() : "an unknown location");
			string message = ((!item3) ? $"{item} tried to kill {value} in {value2}. (Saved)" : $"{item} (as {item2}) tried to kill {value} in {value2}. (Saved)");
			AddLog(message);
		}
	}

	public static void HandleVent(PlayerControl player, bool entered, string roomName)
	{
		if (CheatToggles.notifyOnVent && !((Object)(object)player == (Object)null))
		{
			(string realName, string displayName, bool isDisguised) playerIdentity = GetPlayerIdentity(player);
			string item = playerIdentity.realName;
			string item2 = playerIdentity.displayName;
			bool item3 = playerIdentity.isDisguised;
			string value = (entered ? "entered" : "exited");
			string message = ((!item3) ? $"{item} has {value} a vent in {roomName}." : $"{item} (as {item2}) has {value} a vent in {roomName}.");
			AddLog(message);
		}
	}

	public static void HandlePlayerDisconnect(NetworkedPlayerInfo player)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (CheatToggles.notifyOnDisconnect && !((Object)(object)player == (Object)null))
		{
			AddLog($"<color=#{ColorUtility.ToHtmlStringRGB(player.Color)}>{player.PlayerName}</color>" + " has disconnected.");
		}
	}
}
