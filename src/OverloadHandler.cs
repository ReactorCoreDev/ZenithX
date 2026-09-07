using System;
using System.Collections.Generic;
using System.Linq;
using InnerNet;
using UnityEngine;

namespace ZenithX;

public static class OverloadHandler
{
	public enum TargetType
	{
		None,
		All,
		Custom,
		Host,
		Impostor,
		Crewmate
	}

	public static float cooldown = 1f;

	public static int strength = 5000;

	private static HashSet<int> _customTargets = new HashSet<int>();

	private static float _timer;

	private static float _attackLogTimer = 2f;

	private static Dictionary<int, int> _rpcCounters = new Dictionary<int, int>();

	private static int _nextTarget = int.MinValue;

	private static bool _hasRun;

	public static void Run()
	{
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		if (!CheatToggles.runOverload || OverloadUI.currentTargets.Count <= 0)
		{
			_timer = cooldown;
			_attackLogTimer = 2f;
			_nextTarget = int.MinValue;
			_hasRun = false;
			_rpcCounters.Clear();
			return;
		}
		_timer += Time.unscaledDeltaTime;
		_attackLogTimer += Time.unscaledDeltaTime;
		if (!(_timer >= cooldown))
		{
			return;
		}
		if (OverloadUI.maxPossibleTargets == OverloadUI.currentTargets.Count)
		{
			int num = -1;
			Utils.Overload(num, strength);
			_timer -= cooldown;
			if (!CheatToggles.olLogAttack)
			{
				return;
			}
			string value = ColorUtility.ToHtmlStringRGB(new Color(1f, 0.5f, 0f));
			if (!CheatToggles.olVerboseLogs)
			{
				_rpcCounters.TryAdd(num, 0);
				_rpcCounters.TryGetValue(num, out var value2);
				int value3 = value2 + strength;
				if (_attackLogTimer >= 2f)
				{
					OverloadUI.LogConsole($"> <b><color=#{value}>Broadcasted {value3} malformed RPCs to all players (ID : {num})</color></b>");
					_attackLogTimer -= 2f;
					_rpcCounters.Clear();
				}
				else
				{
					_rpcCounters[num] = value3;
				}
			}
			else
			{
				OverloadUI.LogConsole($"> <b><color=#{value}>Broadcasted {strength} malformed RPCs to all players (ID : {num})</color></b>");
			}
			return;
		}
		foreach (NetworkedPlayerInfo currentTarget in OverloadUI.currentTargets)
		{
			int clientId = currentTarget.ClientId;
			if (!_hasRun)
			{
				if (_nextTarget != int.MinValue && clientId != _nextTarget)
				{
					continue;
				}
				Utils.Overload(clientId, strength);
				_timer -= cooldown;
				if (CheatToggles.olLogAttack)
				{
					string value4 = ColorUtility.ToHtmlStringRGB(new Color(1f, 0.5f, 0f));
					if (!CheatToggles.olVerboseLogs)
					{
						_rpcCounters.TryAdd(clientId, 0);
						_rpcCounters.TryGetValue(clientId, out var value5);
						int value6 = value5 + strength;
						_rpcCounters[clientId] = value6;
					}
					else
					{
						OverloadUI.LogConsole($"> <b><color=#{value4}>Sent {strength} malformed RPCs to {currentTarget.DefaultOutfit.PlayerName} (ID : {clientId})</color></b>");
					}
				}
				_hasRun = true;
				continue;
			}
			_nextTarget = clientId;
			_hasRun = false;
			return;
		}
		_nextTarget = int.MinValue;
		_hasRun = false;
		if (!CheatToggles.olVerboseLogs)
		{
			if (!(_attackLogTimer >= 2f))
			{
				return;
			}
			string value7 = ColorUtility.ToHtmlStringRGB(new Color(1f, 0.5f, 0f));
			foreach (KeyValuePair<int, int> rpcCounter in _rpcCounters)
			{
				int clientId2 = rpcCounter.Key;
				int value8 = rpcCounter.Value;
				NetworkedPlayerInfo val = ((IEnumerable<NetworkedPlayerInfo>)OverloadUI.currentTargets).FirstOrDefault((Func<NetworkedPlayerInfo, bool>)((NetworkedPlayerInfo pd) => pd.ClientId == clientId2));
				if ((Object)(object)val != (Object)null)
				{
					OverloadUI.LogConsole($"> <b><color=#{value7}>Sent {value8} malformed RPCs to {val.DefaultOutfit.PlayerName} (ID : {clientId2})</color></b>");
				}
			}
			_attackLogTimer -= 2f;
			_rpcCounters.Clear();
		}
		else
		{
			_rpcCounters.Clear();
		}
	}

	public static void AddCustomTarget(NetworkedPlayerInfo playerData)
	{
		int clientId = playerData.ClientId;
		_customTargets.Add(clientId);
	}

	public static void RemoveCustomTarget(NetworkedPlayerInfo playerData)
	{
		int clientId = playerData.ClientId;
		_customTargets.Remove(clientId);
	}

	public static bool IsCustomTarget(NetworkedPlayerInfo playerData)
	{
		return _customTargets.Contains(playerData.ClientId);
	}

	public unsafe static (HashSet<TargetType> targetTypes, bool isTarget) GetTarget(NetworkedPlayerInfo playerData)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		HashSet<TargetType> hashSet = new HashSet<TargetType>();
		if (CheatToggles.overloadAll)
		{
			hashSet.Add(TargetType.All);
			flag = true;
		}
		if (CheatToggles.overloadHost && ((InnerNetClient)AmongUsClient.Instance).HostId == playerData.ClientId)
		{
			hashSet.Add(TargetType.Host);
			flag = true;
		}
		if ((Object)(object)playerData.Role != (Object)null)
		{
			RoleTeamTypes teamType = playerData.Role.TeamType;
			if (CheatToggles.overloadCrew)
			{
				object obj = (object)(RoleTeamTypes)0;
				if (((object)(*(RoleTeamTypes*)(&teamType))/*cast due to .constrained prefix*/).Equals(obj))
				{
					hashSet.Add(TargetType.Crewmate);
					flag = true;
				}
			}
			if (CheatToggles.overloadImps)
			{
				object obj2 = (object)(RoleTeamTypes)1;
				if (((object)(*(RoleTeamTypes*)(&teamType))/*cast due to .constrained prefix*/).Equals(obj2))
				{
					hashSet.Add(TargetType.Impostor);
					flag = true;
				}
			}
		}
		if (IsCustomTarget(playerData))
		{
			hashSet.Add(TargetType.Custom);
			flag = true;
		}
		if (!flag)
		{
			hashSet.Add(TargetType.None);
		}
		return (targetTypes: hashSet, isTarget: flag);
	}

	public static void ClearCustomTargets()
	{
		_customTargets.Clear();
	}

	public static void PopulateCustomTargets(PlayerControl[] players, TargetType targetType)
	{
		int num = players.Length;
		for (int i = 0; i < num; i++)
		{
			NetworkedPlayerInfo data = players[i].Data;
			(HashSet<TargetType>, bool) target = GetTarget(data);
			if (target.Item2 && !IsCustomTarget(data) && target.Item1.Contains(targetType))
			{
				AddCustomTarget(data);
			}
		}
	}

	public static (int strength, float cooldown) CalculateAdaptedValues()
	{
		int num = ((OverloadUI.maxPossibleTargets == OverloadUI.currentTargets.Count) ? 1 : Math.Max(1, OverloadUI.currentTargets.Count));
		float item = 10f / (float)num;
		int num2 = Math.Max(1, Utils.GetPing() / 100);
		int num3 = 100000;
		return (strength: Math.Max(1, num3 / num2 / num), cooldown: item);
	}
}
