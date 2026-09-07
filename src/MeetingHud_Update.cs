using System.Collections.Generic;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Collections.Generic;
using InnerNet;
using UnityEngine;

namespace ZenithX;

[HarmonyPatch(typeof(MeetingHud), "Update")]
public static class MeetingHud_Update
{
	public static List<int> votedPlayers = new List<int>();

	public static void Prefix(MeetingHud __instance)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		if ((int)__instance.state >= 4)
		{
			return;
		}
		foreach (PlayerVoteArea item in (Il2CppArrayBase<PlayerVoteArea>)(object)__instance.playerStates)
		{
			if (!Object.op_Implicit((Object)(object)item))
			{
				continue;
			}
			NetworkedPlayerInfo playerById = GameData.Instance.GetPlayerById(PlayerId.op_Implicit(item.PlayerId));
			if (!((Object)(object)playerById != (Object)null) || playerById.Disconnected || PlayerId.op_Implicit(item.VotedForId) == PlayerVoteArea.HasNotVoted || PlayerId.op_Implicit(item.VotedForId) == PlayerVoteArea.MissedVote || PlayerId.op_Implicit(item.VotedForId) == PlayerVoteArea.DeadVote || votedPlayers.Contains(PlayerId.op_Implicit(item.PlayerId)))
			{
				continue;
			}
			votedPlayers.Add(PlayerId.op_Implicit(item.PlayerId));
			if (PlayerId.op_Implicit(item.VotedForId) != PlayerVoteArea.SkippedVote)
			{
				foreach (PlayerVoteArea item2 in (Il2CppArrayBase<PlayerVoteArea>)(object)__instance.playerStates)
				{
					if (PlayerId.op_Implicit(item2.PlayerId) == PlayerId.op_Implicit(item.VotedForId))
					{
						__instance.BloopAVoteIcon(playerById, 0, ((Component)item2).transform);
						break;
					}
				}
			}
			else if (Object.op_Implicit((Object)(object)__instance.SkippedVoting))
			{
				__instance.BloopAVoteIcon(playerById, 0, __instance.SkippedVoting.transform);
			}
		}
		foreach (PlayerVoteArea item3 in (Il2CppArrayBase<PlayerVoteArea>)(object)__instance.playerStates)
		{
			if (!Object.op_Implicit((Object)(object)item3))
			{
				continue;
			}
			VoteSpreader component = ((Component)((Component)item3).transform).GetComponent<VoteSpreader>();
			if (Object.op_Implicit((Object)(object)component))
			{
				Enumerator<SpriteRenderer> enumerator3 = component.Votes.GetEnumerator();
				while (enumerator3.MoveNext())
				{
					((Component)enumerator3.Current).gameObject.SetActive(CheatToggles.revealVotes);
				}
			}
		}
		if (Object.op_Implicit((Object)(object)__instance.SkippedVoting))
		{
			__instance.SkippedVoting.SetActive(CheatToggles.revealVotes);
		}
	}

	public static void Postfix(MeetingHud __instance)
	{
		ZenithXESP.MeetingNametags(__instance);
		PlayerControl.LocalPlayer.onLadder = false;
	}
}
