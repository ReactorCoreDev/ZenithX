using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(ImpostorRole), "IsValidTarget")]
public static class ImpostorRole_IsValidTarget
{
	public static void Postfix(NetworkedPlayerInfo target, ref bool __result)
	{
		if (CheatToggles.killAnyone)
		{
			__result = Utils.isValidTarget(target);
		}
	}
}
