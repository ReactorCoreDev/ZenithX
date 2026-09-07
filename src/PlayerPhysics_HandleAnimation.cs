using HarmonyLib;
using InnerNet;

namespace ZenithX;

[HarmonyPatch(typeof(PlayerPhysics), "HandleAnimation")]
public static class PlayerPhysics_HandleAnimation
{
	public static bool Prefix(PlayerPhysics __instance)
	{
		if (CheatToggles.freezeAnimations && ((InnerNetObject)__instance).AmOwner)
		{
			__instance.ResetAnimState();
			return false;
		}
		return true;
	}
}
