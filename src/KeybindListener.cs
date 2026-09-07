using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZenithX;

public class KeybindListener : MonoBehaviour
{
	private void Update()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		if (DestroyableSingleton<HudManager>.InstanceExists && Object.op_Implicit((Object)(object)DestroyableSingleton<HudManager>.Instance.Chat) && DestroyableSingleton<HudManager>.Instance.Chat.IsOpenOrOpening)
		{
			return;
		}
		foreach (KeyValuePair<string, KeyCode> keybind in CheatToggles.Keybinds)
		{
			string key = keybind.Key;
			KeyCode value = keybind.Value;
			if ((int)value != 0 && Input.GetKeyDown(value) && CheatToggles.ToggleFields.TryGetValue(key, out var value2))
			{
				try
				{
					bool flag = (bool)value2.GetValue(null);
					value2.SetValue(null, !flag);
				}
				catch (Exception ex)
				{
					ZenithX.Error("Failed to toggle " + key + ": " + ex.Message);
				}
			}
		}
	}
}
