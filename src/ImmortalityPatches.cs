using HarmonyLib;
using UnityEngine;

namespace ZenithX;

public static class ImmortalityPatches
{
	[HarmonyPatch(typeof(VentilationSystem), "Update")]
	public static class BlockSendingUpdates
	{
		private static bool Prefix(Operation op, int ventId)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Invalid comparison between Unknown and I4
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Invalid comparison between Unknown and I4
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Invalid comparison between Unknown and I4
			if (ventId != 50 && CheatToggles.Immortality && ((int)op == 2 || (int)op == 3 || (int)op == 4))
			{
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(GameManager), "StartGame")]
	public static class OnGameStart
	{
		private static void Postfix()
		{
			if (CheatToggles.Immortality)
			{
				VentilationSystem.Update((Operation)2, 50);
			}
		}
	}

	[HarmonyPatch(typeof(PlayerControl), "MurderPlayer")]
	public static class OnMurder
	{
		private static void Postfix(PlayerControl __instance, PlayerControl target)
		{
			if (CheatToggles.Immortality && (Object)(object)target == (Object)(object)PlayerControl.LocalPlayer)
			{
				AlertUI.Warning(__instance.Data.PlayerName + " attempted to kill you!");
			}
		}
	}

	[HarmonyPatch(typeof(MeetingHud), "Close")]
	public static class OnMeetingEnd
	{
		private static void Postfix()
		{
			if (!CheatToggles.Immortality)
			{
				return;
			}
			PlayerControl localPlayer = PlayerControl.LocalPlayer;
			if (localPlayer != null)
			{
				NetworkedPlayerInfo data = localPlayer.Data;
				if (((data != null) ? new bool?(data.IsDead) : ((bool?)null)) == true)
				{
					return;
				}
			}
			VentilationSystem.Update((Operation)2, 50);
		}
	}

	public const int CustomVentId = 50;
}
