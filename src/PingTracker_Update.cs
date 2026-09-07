using HarmonyLib;
using InnerNet;
using TMPro;
using UnityEngine;

namespace ZenithX;

[HarmonyPatch(typeof(PingTracker), "Update")]
public static class PingTracker_Update
{
	private static float FpsTimer;

	private static int FrameCount;

	private static string CachedFpsText = "FPS: N/A";

	public static void Postfix(PingTracker __instance)
	{
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		FrameCount++;
		FpsTimer += Time.unscaledDeltaTime;
		if (FpsTimer >= 0.2f)
		{
			CachedFpsText = Utils.getColoredFPSText(Mathf.RoundToInt((float)FrameCount / FpsTimer));
			FrameCount = 0;
			FpsTimer = 0f;
		}
		((TMP_Text)__instance.text).alignment = (TextAlignmentOptions)514;
		if (((InnerNetClient)AmongUsClient.Instance).IsGameStarted)
		{
			__instance.aspectPosition.DistanceFromEdge = new Vector3(-0.21f, 0.5f, 0f);
			((TMP_Text)__instance.text).text = "ZenithX by ReactorCoreDev - " + Utils.getColoredPingText(((InnerNetClient)AmongUsClient.Instance).Ping) + " - " + CachedFpsText;
		}
		else
		{
			((TMP_Text)__instance.text).text = "ZenithX by ReactorCoreDev\n" + Utils.getColoredPingText(((InnerNetClient)AmongUsClient.Instance).Ping) + "\n" + CachedFpsText;
		}
	}
}
