using HarmonyLib;
using UnityEngine;

namespace ZenithX;

[HarmonyPatch(typeof(PlayerControl), "Shapeshift")]
public static class PlayerControl_Shapeshift
{
	public static void Postfix(PlayerControl __instance, PlayerControl targetPlayer, bool animate)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Invalid comparison between Unknown and I4
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		if (CheatToggles.notifyShapeshift && (int)__instance.CurrentOutfitType != 3)
		{
			NetworkedPlayerInfo data = targetPlayer.Data;
			PlainShipRoom roomFromPosition = Utils.getRoomFromPosition(__instance.GetTruePosition());
			string value = (((Object)(object)roomFromPosition != (Object)null) ? ((object)roomFromPosition.RoomId/*cast due to .constrained prefix*/).ToString() : "an unknown location");
			if (data.PlayerId == __instance.Data.PlayerId)
			{
				AlertUI.Notification($"<color=#{ColorUtility.ToHtmlStringRGB(GameData.Instance.GetPlayerById(__instance.PlayerId).Color)}>{GameData.Instance.GetPlayerById(__instance.PlayerId)._object.Data.PlayerName}</color> unshapeshifted in {value}", 5f);
			}
			else
			{
				AlertUI.Notification($"<color=#{ColorUtility.ToHtmlStringRGB(GameData.Instance.GetPlayerById(__instance.PlayerId).Color)}>{GameData.Instance.GetPlayerById(__instance.PlayerId)._object.Data.PlayerName}</color> shapeshifted into <color=#{ColorUtility.ToHtmlStringRGB(GameData.Instance.GetPlayerById(data.PlayerId).Color)}>{GameData.Instance.GetPlayerById(data.PlayerId)._object.Data.PlayerName}</color> in {value}", 5f);
			}
		}
	}
}
