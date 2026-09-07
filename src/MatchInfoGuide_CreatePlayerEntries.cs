using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ZenithX;

[HarmonyPatch(typeof(MatchInfoGuide), "CreatePlayerEntries")]
public static class MatchInfoGuide_CreatePlayerEntries
{
	private static Vector2 _anchoredPosition = new Vector2(0f, 0f);

	public static bool Prefix(MatchInfoGuide __instance)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		__instance.PlayerPool.ReclaimAll();
		int num = 51;
		Enumerator<NetworkedPlayerInfo> enumerator = GameData.Instance.AllPlayers.GetEnumerator();
		while (enumerator.MoveNext())
		{
			NetworkedPlayerInfo current = enumerator.Current;
			PlayerIdentifierButton component = ((Component)__instance.PlayerPool.Get<PoolableBehavior>()).GetComponent<PlayerIdentifierButton>();
			((Component)component).transform.localPosition = new Vector3(0f, 0f, -1f);
			component.Populate(current);
			((TMP_Text)component.NameText).text = Utils.getNameTag(current, current.PlayerName, isChat: false, isMatchInfo: true);
			if (_anchoredPosition == Vector2.zero)
			{
				_anchoredPosition = ((TMP_Text)component.NameText).rectTransform.anchoredPosition;
			}
			((TMP_Text)component.NameText).rectTransform.anchoredPosition = new Vector2(_anchoredPosition.x, _anchoredPosition.y + 0.05f);
			__instance.ControllerSelectable.Add((UiElement)(object)component.Button);
			component.SetTextStencil(num++);
		}
		return false;
	}
}
