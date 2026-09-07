using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZenithX;

public static class PlayerBehaviorAnalyzer
{
	private static Dictionary<byte, Queue<Vector2>> movementHistory = new Dictionary<byte, Queue<Vector2>>();

	private static Dictionary<byte, Queue<float>> movementTimestamps = new Dictionary<byte, Queue<float>>();

	private const int MOVEMENT_HISTORY_SIZE = 30;

	private static Dictionary<byte, Queue<DateTime>> killTimestamps = new Dictionary<byte, Queue<DateTime>>();

	private const int KILL_HISTORY_SIZE = 5;

	private static Dictionary<byte, Queue<DateTime>> taskTimestamps = new Dictionary<byte, Queue<DateTime>>();

	private const int TASK_HISTORY_SIZE = 15;

	private static Dictionary<byte, Queue<DateTime>> ventTimestamps = new Dictionary<byte, Queue<DateTime>>();

	private const int VENT_HISTORY_SIZE = 10;

	private static Dictionary<byte, Queue<byte>> voteHistory = new Dictionary<byte, Queue<byte>>();

	private const int VOTE_HISTORY_SIZE = 5;

	public static void TrackMovement(PlayerControl player)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)player == (Object)null))
		{
			byte playerId = player.PlayerId;
			if (!movementHistory.ContainsKey(playerId))
			{
				movementHistory[playerId] = new Queue<Vector2>();
				movementTimestamps[playerId] = new Queue<float>();
			}
			Vector3 position = ((Component)player).transform.position;
			float time = Time.time;
			movementHistory[playerId].Enqueue(Vector2.op_Implicit(position));
			movementTimestamps[playerId].Enqueue(time);
			while (movementHistory[playerId].Count > 30)
			{
				movementHistory[playerId].Dequeue();
				movementTimestamps[playerId].Dequeue();
			}
		}
	}

	public static bool DetectBotlikeMovement(PlayerControl player)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)player == (Object)null)
		{
			return false;
		}
		byte playerId = player.PlayerId;
		if (!movementHistory.ContainsKey(playerId) || movementHistory[playerId].Count < 20)
		{
			return false;
		}
		Vector2[] array = movementHistory[playerId].ToArray();
		movementTimestamps[playerId].ToArray();
		int num = 0;
		for (int i = 2; i < array.Length; i++)
		{
			Vector2 val = array[i - 1] - array[i - 2];
			Vector2 val2 = array[i] - array[i - 1];
			if (((Vector2)(ref val)).magnitude > 0.01f && ((Vector2)(ref val2)).magnitude > 0.01f && Vector2.Angle(val, val2) < 5f)
			{
				num++;
			}
		}
		return (float)num > (float)array.Length * 0.8f;
	}

	public static bool DetectInstantMovement(PlayerControl player)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)player == (Object)null)
		{
			return false;
		}
		byte playerId = player.PlayerId;
		if (!movementTimestamps.ContainsKey(playerId) || movementTimestamps[playerId].Count < 2)
		{
			return false;
		}
		float[] array = movementTimestamps[playerId].ToArray();
		Vector2[] array2 = movementHistory[playerId].ToArray();
		for (int i = 1; i < array.Length; i++)
		{
			float num = array[i] - array[i - 1];
			float num2 = Vector2.Distance(array2[i], array2[i - 1]);
			if (num < 0.05f && num2 > 5f)
			{
				return true;
			}
		}
		return false;
	}

	public static void TrackKill(PlayerControl player)
	{
		if (!((Object)(object)player == (Object)null))
		{
			byte playerId = player.PlayerId;
			if (!killTimestamps.ContainsKey(playerId))
			{
				killTimestamps[playerId] = new Queue<DateTime>();
			}
			killTimestamps[playerId].Enqueue(DateTime.Now);
			while (killTimestamps[playerId].Count > 5)
			{
				killTimestamps[playerId].Dequeue();
			}
		}
	}

	public static bool DetectKillPattern(PlayerControl player)
	{
		if ((Object)(object)player == (Object)null)
		{
			return false;
		}
		byte playerId = player.PlayerId;
		if (!killTimestamps.ContainsKey(playerId) || killTimestamps[playerId].Count < 3)
		{
			return false;
		}
		DateTime[] array = killTimestamps[playerId].ToArray();
		List<double> list = new List<double>();
		for (int i = 1; i < array.Length; i++)
		{
			list.Add((array[i] - array[i - 1]).TotalSeconds);
		}
		if (list.Count < 2)
		{
			return false;
		}
		double mean = list.Average();
		if (Math.Sqrt(list.Sum((double x) => Math.Pow(x - mean, 2.0)) / (double)list.Count) < 2.0)
		{
			return mean > 10.0;
		}
		return false;
	}

	public static void TrackTaskCompletion(PlayerControl player)
	{
		if (!((Object)(object)player == (Object)null))
		{
			byte playerId = player.PlayerId;
			if (!taskTimestamps.ContainsKey(playerId))
			{
				taskTimestamps[playerId] = new Queue<DateTime>();
			}
			taskTimestamps[playerId].Enqueue(DateTime.Now);
			while (taskTimestamps[playerId].Count > 15)
			{
				taskTimestamps[playerId].Dequeue();
			}
		}
	}

	public static bool DetectTaskBot(PlayerControl player)
	{
		if ((Object)(object)player == (Object)null)
		{
			return false;
		}
		byte playerId = player.PlayerId;
		if (!taskTimestamps.ContainsKey(playerId) || taskTimestamps[playerId].Count < 10)
		{
			return false;
		}
		DateTime[] array = taskTimestamps[playerId].ToArray();
		List<double> list = new List<double>();
		for (int i = 1; i < array.Length; i++)
		{
			list.Add((array[i] - array[i - 1]).TotalSeconds);
		}
		if (list.Count < 5)
		{
			return false;
		}
		double mean = list.Average();
		if (Math.Sqrt(list.Sum((double x) => Math.Pow(x - mean, 2.0)) / (double)list.Count) < 1.0)
		{
			return mean > 0.5;
		}
		return false;
	}

	public static void TrackVentUsage(PlayerControl player)
	{
		if (!((Object)(object)player == (Object)null))
		{
			byte playerId = player.PlayerId;
			if (!ventTimestamps.ContainsKey(playerId))
			{
				ventTimestamps[playerId] = new Queue<DateTime>();
			}
			ventTimestamps[playerId].Enqueue(DateTime.Now);
			while (ventTimestamps[playerId].Count > 10)
			{
				ventTimestamps[playerId].Dequeue();
			}
		}
	}

	public static bool DetectVentAbuse(PlayerControl player)
	{
		if ((Object)(object)player == (Object)null)
		{
			return false;
		}
		byte playerId = player.PlayerId;
		if (!ventTimestamps.ContainsKey(playerId) || ventTimestamps[playerId].Count < 5)
		{
			return false;
		}
		DateTime[] array = ventTimestamps[playerId].ToArray();
		for (int i = 1; i < array.Length; i++)
		{
			if ((array[i] - array[i - 1]).TotalSeconds < 2.0)
			{
				return true;
			}
		}
		return false;
	}

	public static void TrackVote(PlayerControl player, byte votedFor)
	{
		if (!((Object)(object)player == (Object)null))
		{
			byte playerId = player.PlayerId;
			if (!voteHistory.ContainsKey(playerId))
			{
				voteHistory[playerId] = new Queue<byte>();
			}
			voteHistory[playerId].Enqueue(votedFor);
			while (voteHistory[playerId].Count > 5)
			{
				voteHistory[playerId].Dequeue();
			}
		}
	}

	public static bool DetectVoteBot(PlayerControl player)
	{
		if ((Object)(object)player == (Object)null)
		{
			return false;
		}
		byte playerId = player.PlayerId;
		if (!voteHistory.ContainsKey(playerId) || voteHistory[playerId].Count < 5)
		{
			return false;
		}
		byte[] array = voteHistory[playerId].ToArray();
		byte b = array[0];
		byte[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			if (array2[i] != b)
			{
				return false;
			}
		}
		return true;
	}

	public static void ResetPlayerData(byte playerId)
	{
		if (movementHistory.ContainsKey(playerId))
		{
			movementHistory[playerId].Clear();
			movementTimestamps[playerId].Clear();
		}
		if (killTimestamps.ContainsKey(playerId))
		{
			killTimestamps[playerId].Clear();
		}
		if (taskTimestamps.ContainsKey(playerId))
		{
			taskTimestamps[playerId].Clear();
		}
		if (ventTimestamps.ContainsKey(playerId))
		{
			ventTimestamps[playerId].Clear();
		}
		if (voteHistory.ContainsKey(playerId))
		{
			voteHistory[playerId].Clear();
		}
	}

	public static void ResetAllData()
	{
		movementHistory.Clear();
		movementTimestamps.Clear();
		killTimestamps.Clear();
		taskTimestamps.Clear();
		ventTimestamps.Clear();
		voteHistory.Clear();
	}
}
