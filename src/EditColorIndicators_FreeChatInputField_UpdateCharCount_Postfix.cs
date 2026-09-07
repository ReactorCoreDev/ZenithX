using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ZenithX;

[HarmonyPatch(typeof(FreeChatInputField), "UpdateCharCount")]
public static class EditColorIndicators_FreeChatInputField_UpdateCharCount_Postfix
{
	public static void Postfix(FreeChatInputField __instance)
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		int length = __instance.textArea.text.Length;
		((TMP_Text)__instance.charCountText).SetText($"{length}/{__instance.textArea.characterLimit}", true);
		TextMeshPro charCountText = __instance.charCountText;
		Color color = (Color)((length < 1610612735) ? Color.black : ((length >= int.MaxValue) ? Color.red : new Color(1f, 1f, 0f, 1f)));
		((Graphic)charCountText).color = color;
	}
}
