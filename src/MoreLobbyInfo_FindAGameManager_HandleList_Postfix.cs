using HarmonyLib;
using InnerNet;
using TMPro;

namespace ZenithX;

[HarmonyPatch(typeof(FindAGameManager), "HandleList")]
public static class MoreLobbyInfo_FindAGameManager_HandleList_Postfix
{
	public static void Postfix(TotalGameData totalGames, FindGamesListFilteredResponse response, FindAGameManager __instance)
	{
		if (CheatToggles.unlockFeatures)
		{
			((TMP_Text)__instance.TotalText).text = response.Metadata.AllGamesCount.ToString();
		}
	}
}
