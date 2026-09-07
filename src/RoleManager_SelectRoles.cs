using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AmongUs.GameOptions;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace ZenithX;

[HarmonyPatch(typeof(RoleManager), "SelectRoles")]
public static class RoleManager_SelectRoles
{
	public static bool Prefix()
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		if (!Utils.isHost || !CheatToggles.forcedRole.HasValue)
		{
			return true;
		}
		IGameOptions currentGameOptions = GameOptionsManager.Instance.CurrentGameOptions;
		List<PlayerControl> list = new List<PlayerControl>();
		Enumerator<PlayerControl> enumerator = PlayerControl.AllPlayerControls.GetEnumerator();
		while (enumerator.MoveNext())
		{
			PlayerControl current = enumerator.Current;
			list.Add(current);
		}
		HashSet<byte> hashSet = new HashSet<byte>();
		int count = list.Count;
		int adjustedNumImpostors = IGameOptionsExtensions.GetAdjustedNumImpostors(currentGameOptions, count);
		int i = 0;
		RoleTypes value = CheatToggles.forcedRole.Value;
		PlayerControl.LocalPlayer.RpcSetRole(value, false);
		hashSet.Add(PlayerControl.LocalPlayer.PlayerId);
		if (IsImpostorRole(value))
		{
			i++;
		}
		RoleTypes[] array = new RoleTypes[4];
		RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		RoleTypes[] array2 = (RoleTypes[])(object)array;
		foreach (RoleTypes val in array2)
		{
			int numPerGame = currentGameOptions.RoleOptions.GetNumPerGame(val);
			int chancePerGame = currentGameOptions.RoleOptions.GetChancePerGame(val);
			for (int k = 0; k < numPerGame; k++)
			{
				if (i >= adjustedNumImpostors)
				{
					break;
				}
				if (chancePerGame >= 100 || Random.Range(1, 101) <= chancePerGame)
				{
					PlayerControl randomUnassigned = GetRandomUnassigned(list, hashSet);
					if ((Object)(object)randomUnassigned == (Object)null)
					{
						break;
					}
					randomUnassigned.RpcSetRole(val, false);
					hashSet.Add(randomUnassigned.PlayerId);
					i++;
				}
			}
		}
		for (; i < adjustedNumImpostors; i++)
		{
			PlayerControl randomUnassigned2 = GetRandomUnassigned(list, hashSet);
			if ((Object)(object)randomUnassigned2 == (Object)null)
			{
				break;
			}
			randomUnassigned2.RpcSetRole((RoleTypes)1, false);
			hashSet.Add(randomUnassigned2.PlayerId);
		}
		RoleTypes[] array3 = new RoleTypes[7];
		RuntimeHelpers.InitializeArray(array3, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		array2 = (RoleTypes[])(object)array3;
		foreach (RoleTypes val2 in array2)
		{
			int numPerGame2 = currentGameOptions.RoleOptions.GetNumPerGame(val2);
			int chancePerGame2 = currentGameOptions.RoleOptions.GetChancePerGame(val2);
			for (int l = 0; l < numPerGame2; l++)
			{
				if (hashSet.Count >= count)
				{
					break;
				}
				if (chancePerGame2 >= 100 || Random.Range(1, 101) <= chancePerGame2)
				{
					PlayerControl randomUnassigned3 = GetRandomUnassigned(list, hashSet);
					if ((Object)(object)randomUnassigned3 == (Object)null)
					{
						break;
					}
					randomUnassigned3.RpcSetRole(val2, false);
					hashSet.Add(randomUnassigned3.PlayerId);
				}
			}
		}
		foreach (PlayerControl item in list)
		{
			if (!hashSet.Contains(item.PlayerId))
			{
				item.RpcSetRole((RoleTypes)0, false);
				hashSet.Add(item.PlayerId);
			}
		}
		return false;
	}

	private static bool IsImpostorRole(RoleTypes role)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Invalid comparison between Unknown and I4
		if ((int)role != 1 && (int)role != 5 && (int)role != 9)
		{
			return (int)role == 18;
		}
		return true;
	}

	private static PlayerControl GetRandomUnassigned(List<PlayerControl> allPlayers, HashSet<byte> assigned)
	{
		List<PlayerControl> list = new List<PlayerControl>();
		foreach (PlayerControl allPlayer in allPlayers)
		{
			if (!assigned.Contains(allPlayer.PlayerId))
			{
				list.Add(allPlayer);
			}
		}
		if (list.Count == 0)
		{
			return null;
		}
		return list[Random.Range(0, list.Count)];
	}
}
