using AmongUs.GameOptions;
using HarmonyLib;
using InnerNet;
using TMPro;

namespace ZenithX;

[HarmonyPatch(typeof(GameContainer), "SetupGameInfo")]
public static class MoreLobbyInfo_GameContainer_SetupGameInfo_Postfix
{
	public static void Postfix(GameContainer __instance)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		if (CheatToggles.moreLobbyInfo)
		{
			string trueHostName = __instance.gameListing.TrueHostName;
			uint iP = __instance.gameListing.IP;
			string iPString = __instance.gameListing.IPString;
			QuickChatModes quickChat = __instance.gameListing.QuickChat;
			byte mapId = __instance.gameListing.MapId;
			uint language = __instance.gameListing.Language;
			IGameOptions options = __instance.gameListing.Options;
			int gameId = __instance.gameListing.GameId;
			int age = __instance.gameListing.Age;
			string value = $"Age: {age / 60}:{((age % 60 < 10) ? "0" : "")}{age % 60}";
			string value2 = Utils.PlatformTypeToString(__instance.gameListing.Platform);
			if (CheatToggles.debugMode)
			{
				ZenithX.Log($"All information for lobby by {trueHostName}: {iP}, {iPString}, {gameId}, {quickChat}, {gameId}, {mapId}, {language}, {options}, {age}");
			}
			((TMP_Text)__instance.capacity).text = $"<size=40%>{"<#0000>000000000000000</color>"}\n{trueHostName}\n{((TMP_Text)__instance.capacity).text}\n<#fb0>{GameCode.IntToGameName(gameId)}</color>\n<#b0f>{value2}</color>\n{value}\n{"<#0000>000000000000000</color>"}</size>";
		}
	}
}
