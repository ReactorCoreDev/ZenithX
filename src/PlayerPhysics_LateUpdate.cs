using System;
using System.Collections.Generic;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using TMPro;
using UnityEngine;

namespace ZenithX;

[HarmonyPatch(typeof(PlayerPhysics), "LateUpdate")]
public static class PlayerPhysics_LateUpdate
{
	private static readonly Dictionary<byte, bool> WasInVent = new Dictionary<byte, bool>();

	private static readonly Dictionary<byte, Vector2> LastPositions = new Dictionary<byte, Vector2>();

	private static GameObject[] CachedBodies = Array.Empty<GameObject>();

	private static float BodyScanCooldown = 0f;

	public static void Postfix(PlayerPhysics __instance)
	{
		//IL_060d: Unknown result type (might be due to invalid IL or missing references)
		//IL_066c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0671: Unknown result type (might be due to invalid IL or missing references)
		//IL_0673: Unknown result type (might be due to invalid IL or missing references)
		//IL_0665: Unknown result type (might be due to invalid IL or missing references)
		//IL_0667: Unknown result type (might be due to invalid IL or missing references)
		//IL_0692: Unknown result type (might be due to invalid IL or missing references)
		//IL_0697: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if ((Object)(object)__instance == (Object)null)
			{
				return;
			}
			PlayerControl val = __instance.myPlayer ?? PlayerControl.LocalPlayer;
			if ((Object)(object)val == (Object)null || (Object)(object)val.Data == (Object)null)
			{
				return;
			}
			FunctionHelper.Catch(delegate
			{
				ZenithXESP.PlayerNametags(__instance);
			});
			FunctionHelper.Catch(delegate
			{
				ZenithXESP.seeGhostsCheat(__instance);
			});
			FunctionHelper.Catch(delegate
			{
				TracersHandler.DrawPlayerTracer(__instance);
			});
			FunctionHelper.Catch(ZenithXCheats.noClipCheat);
			FunctionHelper.Catch(ZenithXCheats.speedBoostCheat);
			FunctionHelper.Catch(ZenithXCheats.teleportCursorCheat);
			FunctionHelper.Catch(ZenithXCheats.completeMyTasksCheat);
			FunctionHelper.Catch(ZenithXCheats.reviveCheat);
			FunctionHelper.Catch(ZenithXCheats.noAbilityCDCheat);
			FunctionHelper.Catch(ZenithXCheats.murderAllCheat);
			FunctionHelper.Catch(ZenithXCheats.ScanCheat);
			FunctionHelper.Catch(ZenithXCheats.imposterAutoKillNearbyCheat);
			FunctionHelper.Catch(ZenithXCheats.AnimationCheat);
			FunctionHelper.Catch(ZenithXCheats.chatMimicCheat);
			FunctionHelper.Catch(ZenithXCheats.kickPlayerCheat);
			FunctionHelper.Catch(ZenithXCheats.resetOutfitCheat);
			FunctionHelper.Catch(ZenithXCheats.shuffleAllOutfitsCheat);
			FunctionHelper.Catch(ZenithXCheats.shuffleCheat);
			FunctionHelper.Catch(ZenithXCheats.ProtectCheat);
			FunctionHelper.Catch(ZenithXCheats.imposterCheat);
			FunctionHelper.Catch(ZenithXCheats.uncappedFPSCheat);
			FunctionHelper.Catch(ZenithXCheats.discoColorCheat);
			FunctionHelper.Catch(ZenithXCheats.SpamTpAllCheat);
			FunctionHelper.Catch(ZenithXCheats.SpamTpImpsCheat);
			FunctionHelper.Catch(ZenithXPPMCheats.spectatePPM);
			FunctionHelper.Catch(ZenithXPPMCheats.killPlayerPPM);
			FunctionHelper.Catch(ZenithXPPMCheats.telekillPlayerPPM);
			FunctionHelper.Catch(ZenithXPPMCheats.teleportPlayerPPM);
			FunctionHelper.Catch(ZenithXPPMCheats.changeRolePPM);
			FunctionHelper.Catch(ZenithXPPMCheats.ejectPlayerPPM);
			FunctionHelper.Catch(ZenithXPPMCheats.murderPlayerPPM);
			FunctionHelper.Catch(ZenithXPPMCheats.ForceRolePPM);
			FunctionHelper.Catch(ZenithXPPMCheats.revivePlayerPPM);
			FunctionHelper.Catch(ZenithXPPMCheats.shapeshiftCheatPPM);
			FunctionHelper.Catch(ZenithXPPMCheats.saveSpoofDataPPM);
			FunctionHelper.Catch(ZenithXPPMCheats.SetFakeAlivePPM);
			RPC_SpamTextPostfix.Update();
			if (!CheatToggles.spamChat)
			{
				RPC_SpamTextPostfix.spamText = null;
			}
			BodyScanCooldown += Time.deltaTime;
			if (BodyScanCooldown >= 0.25f)
			{
				CachedBodies = Il2CppArrayBase<GameObject>.op_Implicit((Il2CppArrayBase<GameObject>)(object)GameObject.FindGameObjectsWithTag("DeadBody"));
				BodyScanCooldown = 0f;
			}
			for (int num = 0; num < CachedBodies.Length; num++)
			{
				GameObject val2 = CachedBodies[num];
				if (!((Object)(object)val2 == (Object)null))
				{
					DeadBody component = val2.GetComponent<DeadBody>();
					if ((Object)(object)component != (Object)null && !component.Reported)
					{
						TracersHandler.DrawBodyTracer(component);
					}
				}
			}
			if (!val.Data.IsDead)
			{
				if (!val.inVent)
				{
					LastPositions[val.PlayerId] = val.GetTruePosition();
				}
				if (CheatToggles.notifyOnVent && Utils.isInGame)
				{
					byte playerId = val.PlayerId;
					bool inVent = val.inVent;
					if (WasInVent.TryGetValue(playerId, out var value) && value != inVent)
					{
						Vector2 value2;
						Vector2 position = ((!inVent || !LastPositions.TryGetValue(playerId, out value2)) ? val.GetTruePosition() : value2);
						PlainShipRoom roomFromPosition = Utils.getRoomFromPosition(position);
						NotificationHandler.HandleVent(val, inVent, ((Object)(object)roomFromPosition != (Object)null) ? ((object)roomFromPosition.RoomId/*cast due to .constrained prefix*/).ToString() : "Unknown");
					}
					WasInVent[playerId] = inVent;
				}
			}
			try
			{
				PlayerPhysics val3 = ((val != null) ? val.MyPhysics : null);
				if ((Object)(object)val3 != (Object)null)
				{
					if (CheatToggles.invertControls)
					{
						val3.Speed = 0f - Mathf.Abs(val3.Speed);
						val3.GhostSpeed = 0f - Mathf.Abs(val3.GhostSpeed);
					}
					else
					{
						val3.Speed = Mathf.Abs(val3.Speed);
						val3.GhostSpeed = Mathf.Abs(val3.GhostSpeed);
					}
				}
			}
			catch
			{
			}
			try
			{
				if (CheatToggles.nametagVision)
				{
					CosmeticsLayer cosmetics = val.cosmetics;
					object obj2;
					if (cosmetics == null)
					{
						obj2 = null;
					}
					else
					{
						TextMeshPro nameText = cosmetics.nameText;
						obj2 = ((nameText != null) ? ((Component)nameText).gameObject : null);
					}
					GameObject val4 = (GameObject)obj2;
					if ((Object)(object)val4 != (Object)null)
					{
						val4.SetActive(true);
					}
				}
			}
			catch
			{
			}
		}
		catch (Exception ex)
		{
			if (CheatToggles.debugMode)
			{
				ZenithX.Log("LateUpdate error: " + ex);
			}
		}
	}
}
