using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Collections.Generic;

namespace ZenithX;

public static class ShipTrollPatches
{
	[HarmonyPatch(typeof(VentilationSystem), "Deserialize")]
	public static class DisableVents
	{
		private static void Postfix(VentilationSystem __instance)
		{
			if (!CheatToggles.DisableVents || __instance.PlayersInsideVents.Count >= PlayerControl.AllPlayerControls.Count)
			{
				return;
			}
			Enumerator<byte, byte> enumerator = __instance.PlayersInsideVents.Values.GetEnumerator();
			while (enumerator.MoveNext())
			{
				byte current = enumerator.Current;
				if (current < ((Il2CppArrayBase<Vent>)(object)ShipStatus.Instance.AllVents).Count)
				{
					VentilationSystem.Update((Operation)0, (int)current);
				}
			}
		}
	}

	[HarmonyPatch(typeof(SabotageSystemType), "Deserialize")]
	public static class BlockSabotages
	{
		private static void Postfix(SabotageSystemType __instance)
		{
			if (CheatToggles.BlockSabotages && !(__instance.Timer > 0.1f))
			{
				ShipStatus.Instance.RpcUpdateSystem((SystemTypes)17, byte.MaxValue);
			}
		}
	}
}
