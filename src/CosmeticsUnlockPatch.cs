using AmongUs.Data;
using AmongUs.Data.Player;
using HarmonyLib;
using Il2CppSystem;

namespace ZenithX;

[HarmonyPatch(typeof(HatManager), "Initialize")]
public static class CosmeticsUnlockPatch
{
	public static void Postfix(HatManager __instance)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		CosmeticsUnlocker.unlockCosmetics(__instance);
		PlayerData player = DataManager.Player;
		if (player != null)
		{
			PlayerStoreData store = player.Store;
			if (store != null)
			{
				store.LastBundlesViewDate = DateTime.Now;
				store.LastHatsViewDate = DateTime.Now;
				store.LastOutfitsViewDate = DateTime.Now;
				store.LastVisorsViewDate = DateTime.Now;
				store.LastPetsViewDate = DateTime.Now;
				store.LastNameplatesViewDate = DateTime.Now;
				store.LastCosmicubeViewDate = DateTime.Now;
			}
			((AbstractSaveData)player).Save();
		}
	}
}
