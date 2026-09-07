using System;
using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(EOSManager), "Update")]
public static class Passive_SpoofedFriendCodePostfix
{
	public static string defaultFC;

	public static void Postfix(EOSManager __instance)
	{
		if (CheatToggles.incognitoMode)
		{
			if (defaultFC == null)
			{
				defaultFC = __instance.FriendCode;
			}
			string text = DestroyableSingleton<AccountManager>.Instance.GetRandomName().ToLower();
			string text2 = new Random().Next(1000, 10000).ToString();
			__instance.FriendCode = text + "#" + text2;
		}
		else if (ZenithX.guestFriendCode.Value != "" && ZenithX.guestFriendCode.Value != __instance.FriendCode)
		{
			__instance.FriendCode = ZenithX.guestFriendCode.Value;
		}
		else if (defaultFC != null)
		{
			__instance.FriendCode = defaultFC;
			defaultFC = null;
		}
	}
}
