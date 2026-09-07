using AmongUs.GameOptions;
using HarmonyLib;

namespace ZenithX;

[HarmonyPatch(typeof(NumberOption), "Increase")]
public static class NumberOption_Increase
{
	public static bool Prefix(NumberOption __instance)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Invalid comparison between Unknown and I4
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Invalid comparison between Unknown and I4
		if (!CheatToggles.noOptionsLimits)
		{
			return true;
		}
		IGameOptions currentGameOptions = GameOptionsManager.Instance.CurrentGameOptions;
		float num = default(float);
		currentGameOptions.TryGetFloat((FloatOptionNames)2, ref num);
		int num2 = default(int);
		currentGameOptions.TryGetInt((Int32OptionNames)1, ref num2);
		StringNames title = ((OptionBehaviour)__instance).Title;
		if ((int)title != 133)
		{
			if ((int)title == 137)
			{
				currentGameOptions.SetFloat((FloatOptionNames)2, num + __instance.Increment);
			}
			else
			{
				__instance.Value += __instance.Increment;
				__instance.UpdateValue();
				((OptionBehaviour)__instance).OnValueChanged.Invoke((OptionBehaviour)(object)__instance);
				__instance.AdjustButtonsActiveState();
			}
		}
		else
		{
			currentGameOptions.SetInt((Int32OptionNames)1, num2 + (int)__instance.Increment);
		}
		return false;
	}
}
