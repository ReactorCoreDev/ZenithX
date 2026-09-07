using HarmonyLib;
using Il2CppSystem.Collections.Generic;

namespace ZenithX;

[HarmonyPatch(typeof(DetectiveRole), "AddVictimToNotes")]
public class DetectiveRole_AddVictimToNotes
{
	private static bool Prefix(DetectiveRole __instance, NetworkedPlayerInfo playerInfo)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		if (!CheatToggles.infiniteInterrogates)
		{
			return true;
		}
		if (__instance.notesPageInfos == null)
		{
			__instance.notesPageInfos = new List<DetectiveNotesPageInfo>();
		}
		__instance.notesPageInfos.Add(new DetectiveNotesPageInfo(playerInfo));
		return false;
	}
}
