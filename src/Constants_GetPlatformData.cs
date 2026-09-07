using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(Constants), "GetPlatformData")]
public static class Constants_GetPlatformData
{
	public static void Postfix(ref PlatformSpecificData __result)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		if (Utils.stringToPlatformType(ZenithX.spoofPlatform.Value, out var platform))
		{
			__result = new PlatformSpecificData
			{
				Platform = platform.Value,
				PlatformName = Constants.GetPlatformName()
			};
		}
	}
}
