using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZenithX;

public static class RainbowNameHandler
{
	public static float rainbowHue;

	private static readonly HashSet<PlayerControl> RainbowPlayers = new HashSet<PlayerControl>();

	public static void AddRainbow(PlayerControl player)
	{
		if (!((Object)(object)player == (Object)null))
		{
			RainbowPlayers.Add(player);
		}
	}

	public static void RemoveRainbow(PlayerControl player)
	{
		if (!((Object)(object)player == (Object)null))
		{
			RainbowPlayers.Remove(player);
		}
	}

	public static void Clear()
	{
		RainbowPlayers.Clear();
	}

	public static void UpdateRainbowChatBubbles()
	{
		_ = ZenithX.DevMode;
	}

	public static void Update()
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if (!ZenithX.DevMode || (Object)(object)PlayerControl.LocalPlayer == (Object)null)
		{
			return;
		}
		rainbowHue += Time.deltaTime * 0.25f;
		if (rainbowHue > 1f)
		{
			rainbowHue -= 1f;
		}
		string value = ColorUtility.ToHtmlStringRGB(Color.HSVToRGB(rainbowHue, 1f, 1f));
		foreach (PlayerControl item in RainbowPlayers.Where((PlayerControl p) => (Object)(object)p != (Object)null && (Object)(object)p.Data != (Object)null).ToList())
		{
			if (!((Object)(object)item == (Object)null) && !((Object)(object)item.Data == (Object)null))
			{
				item.SetName($"<color=#{value}>{item.Data.PlayerName}</color>");
			}
		}
		if ((Object)(object)PlayerControl.LocalPlayer.Data != (Object)null && RainbowPlayers.Contains(PlayerControl.LocalPlayer))
		{
			PlayerControl.LocalPlayer.SetName($"<color=#{value}>{PlayerControl.LocalPlayer.Data.PlayerName}</color>");
		}
		UpdateRainbowChatBubbles();
	}
}
