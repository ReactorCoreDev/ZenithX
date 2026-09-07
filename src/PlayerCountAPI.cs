using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ZenithX;

public static class PlayerCountAPI
{
	private static readonly string ObfuscatedUrl = "Mi4uKilgdXUgPzQzLjIiKTMuP3cqKDU+LzkuMzU0dC8qdCg7MzYtOyN0Oyoq";

	private static readonly HttpClient Client = new HttpClient
	{
		Timeout = TimeSpan.FromSeconds(10.0)
	};

	private static readonly string PlayerId = Guid.NewGuid().ToString("N");

	private static CancellationTokenSource _cts;

	private static int _closeSent;

	public static int PlayerCount { get; private set; }

	private static string GetApiUrl()
	{
		byte[] array = Convert.FromBase64String(ObfuscatedUrl);
		byte b = 90;
		for (int i = 0; i < array.Length; i++)
		{
			array[i] ^= b;
		}
		return Encoding.UTF8.GetString(array);
	}

	public static void Start()
	{
		if (_cts == null)
		{
			_closeSent = 0;
			_cts = new CancellationTokenSource();
			SendUpdate(_cts.Token);
			GetPlayersLoop(_cts.Token);
		}
	}

	public static void Stop()
	{
		if (_cts != null)
		{
			_cts.Cancel();
			_cts.Dispose();
			_cts = null;
		}
	}

	public static async Task Close()
	{
		if (Interlocked.Exchange(ref _closeSent, 1) != 0)
		{
			return;
		}
		try
		{
			using (await Client.GetAsync(GetApiUrl() + "/close?id=" + PlayerId))
			{
			}
		}
		catch
		{
		}
	}

	private static async Task SendUpdate(CancellationToken token)
	{
		_ = 1;
		try
		{
			using HttpResponseMessage response = await Client.GetAsync(GetApiUrl() + "/update?id=" + PlayerId, token);
			if (response.IsSuccessStatusCode && TryReadPlayerCount(await response.Content.ReadAsStringAsync(token), out var count))
			{
				PlayerCount = count;
			}
		}
		catch (OperationCanceledException)
		{
		}
		catch
		{
		}
	}

	private static async Task GetPlayersLoop(CancellationToken token)
	{
		string apiUrl = GetApiUrl();
		while (!token.IsCancellationRequested)
		{
			try
			{
				using HttpResponseMessage response = await Client.GetAsync(apiUrl + "/get-players", token);
				if (response.IsSuccessStatusCode && TryReadPlayerCount(await response.Content.ReadAsStringAsync(token), out var count))
				{
					PlayerCount = count;
				}
			}
			catch (OperationCanceledException)
			{
				break;
			}
			catch
			{
			}
			try
			{
				await Task.Delay(5000, token);
			}
			catch (OperationCanceledException)
			{
				break;
			}
		}
	}

	private static bool TryReadPlayerCount(string json, out int count)
	{
		count = 0;
		try
		{
			using JsonDocument jsonDocument = JsonDocument.Parse(json);
			JsonElement rootElement = jsonDocument.RootElement;
			if (rootElement.ValueKind == JsonValueKind.Number && rootElement.TryGetInt32(out count))
			{
				return true;
			}
			if (rootElement.ValueKind == JsonValueKind.Array)
			{
				count = rootElement.GetArrayLength();
				return true;
			}
			string[] array = new string[3] { "playerCount", "count", "players" };
			foreach (string propertyName in array)
			{
				if (rootElement.TryGetProperty(propertyName, out var value))
				{
					if (value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out count))
					{
						return true;
					}
					if (value.ValueKind == JsonValueKind.Array)
					{
						count = value.GetArrayLength();
						return true;
					}
				}
			}
		}
		catch
		{
		}
		return false;
	}
}
