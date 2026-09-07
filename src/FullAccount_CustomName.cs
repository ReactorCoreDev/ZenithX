using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(FullAccount), "CanSetCustomName")]
public static class FullAccount_CustomName
{
	public static void Prefix(ref bool canSetName)
	{
		if (CheatToggles.unlockFeatures)
		{
			canSetName = true;
		}
	}
}
