using Epic.OnlineServices.KWS;
using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(AccountManager), "UpdateKidAccountDisplay")]
public static class AccountManager_KidAccountPatch
{
	public static bool Prefix(AccountManager __instance)
	{
		if (CheatToggles.unlockFeatures)
		{
			__instance.freeChatAllowed = (KWSPermissionStatus)0;
			__instance.customDisplayName = (KWSPermissionStatus)0;
			__instance.friendsListAllowed = (KWSPermissionStatus)0;
		}
		return false;
	}
}
