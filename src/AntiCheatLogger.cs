using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZenithX;

public static class AntiCheatLogger
{
	public class LogEntry
	{
		public DateTime Timestamp { get; set; }

		public string Level { get; set; }

		public string Message { get; set; }

		public byte? PlayerId { get; set; }

		public string PlayerName { get; set; }

		public CheatAction? Action { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder3 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(3, 1, stringBuilder2);
			handler.AppendLiteral("[");
			handler.AppendFormatted(Timestamp, "yyyy-MM-dd HH:mm:ss.fff");
			handler.AppendLiteral("] ");
			stringBuilder3.Append(ref handler);
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder4 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(3, 1, stringBuilder2);
			handler.AppendLiteral("[");
			handler.AppendFormatted(Level);
			handler.AppendLiteral("] ");
			stringBuilder4.Append(ref handler);
			if (PlayerId.HasValue)
			{
				stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder5 = stringBuilder2;
				handler = new StringBuilder.AppendInterpolatedStringHandler(13, 2, stringBuilder2);
				handler.AppendLiteral("[Player ");
				handler.AppendFormatted(PlayerId.Value);
				handler.AppendLiteral(" (");
				handler.AppendFormatted(PlayerName);
				handler.AppendLiteral(")] ");
				stringBuilder5.Append(ref handler);
			}
			if (Action.HasValue)
			{
				stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder6 = stringBuilder2;
				handler = new StringBuilder.AppendInterpolatedStringHandler(3, 1, stringBuilder2);
				handler.AppendLiteral("[");
				handler.AppendFormatted(Action.Value);
				handler.AppendLiteral("] ");
				stringBuilder6.Append(ref handler);
			}
			stringBuilder.Append(Message);
			return stringBuilder.ToString();
		}
	}

	private static readonly object logLock = new object();

	private static readonly Queue<LogEntry> logQueue = new Queue<LogEntry>();

	private static bool isInitialized = false;

	public static void Initialize()
	{
		if (!isInitialized)
		{
			isInitialized = true;
			ZenithX.Log("ZenithX Anti-Cheat Logger initialized");
		}
	}

	public static void LogCheatDetection(PlayerControl player, CheatAction action, string additionalInfo = "")
	{
		if (!isInitialized)
		{
			Initialize();
		}
		LogEntry obj = new LogEntry
		{
			Timestamp = DateTime.Now,
			Level = "DETECTION",
			PlayerId = ((player != null) ? new byte?(player.PlayerId) : ((byte?)null))
		};
		object obj2;
		if (player == null)
		{
			obj2 = null;
		}
		else
		{
			NetworkedPlayerInfo data = player.Data;
			obj2 = ((data != null) ? data.PlayerName : null);
		}
		if (obj2 == null)
		{
			obj2 = "Unknown";
		}
		obj.PlayerName = (string)obj2;
		obj.Action = action;
		obj.Message = (string.IsNullOrEmpty(additionalInfo) ? "Cheat detected" : additionalInfo);
		EnqueueLog(obj);
	}

	public static void LogWarning(string message)
	{
		if (!isInitialized)
		{
			Initialize();
		}
		EnqueueLog(new LogEntry
		{
			Timestamp = DateTime.Now,
			Level = "WARNING",
			Message = message
		});
	}

	public static void LogInfo(string message)
	{
		if (!isInitialized)
		{
			Initialize();
		}
		EnqueueLog(new LogEntry
		{
			Timestamp = DateTime.Now,
			Level = "INFO",
			Message = message
		});
	}

	public static void LogError(string message, Exception ex = null)
	{
		if (!isInitialized)
		{
			Initialize();
		}
		EnqueueLog(new LogEntry
		{
			Timestamp = DateTime.Now,
			Level = "ERROR",
			Message = ((ex != null) ? (message + ": " + ex.Message) : message)
		});
	}

	public static void LogDebug(string message)
	{
		if (!isInitialized)
		{
			Initialize();
		}
		EnqueueLog(new LogEntry
		{
			Timestamp = DateTime.Now,
			Level = "DEBUG",
			Message = message
		});
	}

	private static void EnqueueLog(LogEntry entry)
	{
		lock (logLock)
		{
			logQueue.Enqueue(entry);
			while (logQueue.Count > 100)
			{
				WriteLog(logQueue.Dequeue());
			}
			WriteLog(entry);
		}
	}

	private static void WriteLog(LogEntry entry)
	{
		ZenithX.Log(entry.ToString());
	}

	public static void Flush()
	{
		lock (logLock)
		{
			while (logQueue.Count > 0)
			{
				WriteLog(logQueue.Dequeue());
			}
		}
	}

	public static List<LogEntry> GetRecentLogs(int count = 100)
	{
		lock (logLock)
		{
			return new List<LogEntry>(logQueue).Take(count).ToList();
		}
	}
}
