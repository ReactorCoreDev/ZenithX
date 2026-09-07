using System;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Events;

namespace ZenithX;

[HarmonyPatch(typeof(HudManager), "Start")]
public static class HudManager_Start
{
	public static void Postfix(HudManager __instance)
	{
		if ((Object)(object)__instance == (Object)null || (Object)(object)__instance.MapButton == (Object)null)
		{
			return;
		}
		((UnityEventBase)__instance.MapButton.OnClick).RemoveAllListeners();
		if (!((Behaviour)__instance.MapButton).isActiveAndEnabled)
		{
			((Component)__instance.MapButton).gameObject.SetActive(true);
		}
		((UnityEvent)__instance.MapButton.OnClick).AddListener(UnityAction.op_Implicit((Action)delegate
		{
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Expected O, but got Unknown
			//IL_0062: Expected O, but got Unknown
			if (!((Object)(object)PlayerControl.LocalPlayer == (Object)null))
			{
				bool isImpostor = PlayerControl.LocalPlayer.Data.Role.IsImpostor;
				if (CheatToggles.changeMapToSabotage && !isImpostor)
				{
					__instance.ToggleMapVisible(new MapOptions
					{
						Mode = (Modes)3
					});
				}
				else
				{
					__instance.ToggleMapVisible(new MapOptions
					{
						Mode = (Modes)((!isImpostor) ? 1 : 3)
					});
				}
			}
		}));
		GameStartManager gameStartManager = DestroyableSingleton<GameStartManager>.Instance;
		if (!((Object)(object)gameStartManager != (Object)null) || !((Object)(object)gameStartManager.StartButton != (Object)null))
		{
			return;
		}
		((UnityEventBase)gameStartManager.StartButton.OnClick).RemoveAllListeners();
		((UnityEvent)gameStartManager.StartButton.OnClick).AddListener(UnityAction.op_Implicit((Action)delegate
		{
			if (CheatToggles.forceStartOnClickStart)
			{
				gameStartManager.countDownTimer = 0f;
				gameStartManager.startState = (StartingStates)1;
				gameStartManager.BeginGame();
			}
			else
			{
				gameStartManager.BeginGame();
			}
		}));
	}
}
