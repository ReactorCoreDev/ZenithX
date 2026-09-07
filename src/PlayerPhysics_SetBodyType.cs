using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(PlayerPhysics), "SetBodyType")]
public static class PlayerPhysics_SetBodyType
{
	public static void Prefix(ref PlayerBodyTypes bodyType)
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected I4, but got Unknown
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		if (!(CheatToggles.bodyType == "Default"))
		{
			bodyType = (PlayerBodyTypes)(int)(CheatToggles.bodyType switch
			{
				"Horse" => 1, 
				"Seeker" => 2, 
				"Long" => 3, 
				"LongHorse" => 4, 
				"Classic" => 5, 
				"Normal" => 0, 
				_ => bodyType, 
			});
		}
	}
}
