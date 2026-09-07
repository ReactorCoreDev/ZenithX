using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Collections.Generic;
using InnerNet;
using UnityEngine;

namespace ZenithX;

[HarmonyPatch(typeof(MeetingHud), "CheckForEndVoting")]
public static class MeetingHud_CheckForEndVoting
{
	public static bool Prefix(MeetingHud __instance)
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Invalid comparison between Unknown and I4
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		if (!CheatToggles.voteImmune)
		{
			return true;
		}
		if (!((IEnumerable<PlayerVoteArea>)__instance.playerStates).All((PlayerVoteArea ps) => ps.AmDead || ps.DidVote))
		{
			return true;
		}
		bool tie = default(bool);
		KeyValuePair<byte, int> max = Extensions.MaxPair(__instance.CalculateVotes(), ref tie);
		NetworkedPlayerInfo val = ((IEnumerable<NetworkedPlayerInfo>)GameData.Instance.AllPlayers.ToArray()).FirstOrDefault((Func<NetworkedPlayerInfo, bool>)((NetworkedPlayerInfo v) => !tie && v.PlayerId == max.Key));
		bool flag = false;
		ushort num = 0;
		JudgeOverrule val2 = default(JudgeOverrule);
		NetworkedPlayerInfo val3 = default(NetworkedPlayerInfo);
		NetworkedPlayerInfo val4 = default(NetworkedPlayerInfo);
		if (__instance.TryGetWinningOverrule(ref val2, ref val3, ref val4))
		{
			flag = true;
			num = val2.OverruleNonce;
			val = (((int)val4.Role.TeamType != 1) ? val3 : GameData.Instance.GetPlayerById(PlayerId.op_Implicit(val2.OverruledPlayerId)));
		}
		if ((Object)(object)val != (Object)null && (Object)(object)val == (Object)(object)PlayerControl.LocalPlayer.Data)
		{
			val = null;
		}
		VoterState[] array = (VoterState[])(object)new VoterState[((Il2CppArrayBase<PlayerVoteArea>)(object)__instance.playerStates).Length];
		for (int num2 = 0; num2 < ((Il2CppArrayBase<PlayerVoteArea>)(object)__instance.playerStates).Length; num2++)
		{
			PlayerVoteArea val5 = ((Il2CppArrayBase<PlayerVoteArea>)(object)__instance.playerStates)[num2];
			array[num2] = new VoterState
			{
				VoterId = PlayerId.op_Implicit(val5.PlayerId),
				VotedForId = PlayerId.op_Implicit(val5.VotedForId)
			};
		}
		__instance.RpcVotingComplete(Il2CppStructArray<VoterState>.op_Implicit(array), val, tie, flag, num);
		return false;
	}
}
