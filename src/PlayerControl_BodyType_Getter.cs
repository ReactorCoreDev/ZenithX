using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(/*Could not decode attribute arguments.*/)]
public static class PlayerControl_BodyType_Getter
{
	public static bool Prefix(ref PlayerBodyTypes __result)
	{
		if (CheatToggles.bodyType == "Default")
		{
			return true;
		}
		switch (CheatToggles.bodyType)
		{
		case "Horse":
			__result = (PlayerBodyTypes)1;
			return false;
		case "Seeker":
			__result = (PlayerBodyTypes)2;
			return false;
		case "Long":
			__result = (PlayerBodyTypes)3;
			return false;
		case "LongHorse":
			__result = (PlayerBodyTypes)4;
			return false;
		case "Classic":
			__result = (PlayerBodyTypes)5;
			return false;
		case "Normal":
			__result = (PlayerBodyTypes)0;
			return false;
		default:
			return true;
		}
	}
}
