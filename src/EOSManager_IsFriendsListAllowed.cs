using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(EOSManager), "IsFriendsListAllowed")]
public static class EOSManager_IsFriendsListAllowed
{
	public static void Postfix(ref bool __result)
	{
		if (CheatToggles.unlockFeatures)
		{
			__result = true;
		}
	}
}
