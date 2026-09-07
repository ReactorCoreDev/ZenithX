using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZenithX;

public class AlertUI : MonoBehaviour
{
	[Serializable]
	public enum AlertType
	{
		Notification,
		Success,
		Warning,
		Error
	}

	private class GuiNotification
	{
		public string text;

		public Color color;

		public float timer;

		public float maxTime;

		public float posY;

		public float alpha;

		public float glowTime;

		public AlertType type;
	}

	private static bool showPopup = false;

	private static string popupText = "";

	private static Action popupOkAction;

	private static bool showPopupButtons = false;

	private static string popupButtonsText = "";

	private static Action popupYesAction;

	private static Action popupNoAction;

	private static bool showPopupInput = false;

	private static string popupInputText = "";

	private static string popupInputMessage = "";

	private static string popupInputSubmitLabel = "Save";

	private static Action<string> popupInputAction;

	private static List<GuiNotification> notifications = new List<GuiNotification>();

	public static void Notification(string text, float? cooldown = null)
	{
		ZenithXSoundManager.PlaySound("Notification", 1f);
		ShowNotification(text, AlertType.Notification, cooldown ?? 5f);
	}

	public static void Success(string text, float? cooldown = null)
	{
		ZenithXSoundManager.PlaySound("Success", 1f);
		ShowNotification(text, AlertType.Success, cooldown ?? 5f);
	}

	public static void Warning(string text, float? cooldown = null)
	{
		ZenithXSoundManager.PlaySound("Warning", 0.5f);
		ShowNotification(text, AlertType.Warning, cooldown ?? 5f);
	}

	public static void Error(string text, float? cooldown = null)
	{
		ZenithXSoundManager.PlaySound("Error", 1f);
		ShowNotification(text, AlertType.Error, cooldown ?? 5f);
	}

	public static void ShowPopup(string text, Action onOk = null)
	{
		showPopup = true;
		popupText = text;
		popupOkAction = onOk;
		ZenithXSoundManager.PlaySound("Notification", 1f);
		ShowNotification(text, AlertType.Notification);
	}

	public static void ShowPopupButtons(string text, Action onYes = null, Action onNo = null)
	{
		showPopupButtons = true;
		popupButtonsText = text;
		popupYesAction = onYes;
		popupNoAction = onNo;
		ZenithXSoundManager.PlaySound("Notification", 1f);
		ShowNotification(text, AlertType.Warning, 3f);
	}

	public static void ShowPopupInput(string text, string initialValue, Action<string> onSubmit, string submitLabel = "Save")
	{
		showPopupInput = true;
		popupInputMessage = text;
		popupInputText = initialValue ?? "";
		popupInputSubmitLabel = submitLabel;
		popupInputAction = onSubmit;
		ZenithXSoundManager.PlaySound("Notification", 1f);
	}

	public static void ShowNotification(string text, AlertType type, float duration = 2.5f)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		Color color = (Color)(type switch
		{
			AlertType.Notification => new Color(0.9f, 0.9f, 0.9f), 
			AlertType.Success => Color.green, 
			AlertType.Warning => Color.yellow, 
			AlertType.Error => Color.red, 
			_ => new Color(0.9f, 0.9f, 0.9f), 
		});
		notifications.Add(new GuiNotification
		{
			text = text,
			color = color,
			timer = duration,
			maxTime = duration,
			posY = Screen.height + 60,
			alpha = 0f,
			glowTime = 0f,
			type = type
		});
	}

	private static string GetTranslatedText(GuiNotification notification)
	{
		return Localization.Translate(notification.text);
	}

	private static string GetTranslatedPopupText(string text)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			return text;
		}
		return Localization.Translate(text);
	}

	private static void DrawPopups()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		if (showPopup || showPopupButtons || showPopupInput)
		{
			GUI.backgroundColor = GUIStyles.GradientColor;
			GUI.Box(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), "");
		}
		if (showPopup)
		{
			Rect popupRect = new Rect((float)((Screen.width - 400) / 2), (float)((Screen.height - 200) / 2), 400f, 200f);
			GUI.backgroundColor = GUIStyles.GradientColor;
			GUI.ModalWindow(1001, popupRect, WindowFunction.op_Implicit((Action<int>)delegate
			{
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_004f: Unknown result type (might be due to invalid IL or missing references)
				string translatedPopupText = GetTranslatedPopupText(popupText);
				GUI.Label(new Rect(20f, 40f, ((Rect)(ref popupRect)).width - 40f, 60f), translatedPopupText, GUIStyles.buttonLabelStyle);
				if (GUIStyles.ManualButton(new Rect(160f, 130f, 80f, 35f), Localization.Translate("Ok"), GUIStyles.buttonLabelStyle))
				{
					popupOkAction?.Invoke();
					showPopup = false;
				}
			}), "");
		}
		if (showPopupButtons)
		{
			Rect popupRect2 = new Rect((float)((Screen.width - 440) / 2), (float)((Screen.height - 240) / 2), 440f, 240f);
			GUI.backgroundColor = GUIStyles.GradientColor;
			GUI.ModalWindow(1002, popupRect2, WindowFunction.op_Implicit((Action<int>)delegate
			{
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0059: Unknown result type (might be due to invalid IL or missing references)
				//IL_008b: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00db: Unknown result type (might be due to invalid IL or missing references)
				string translatedPopupText = GetTranslatedPopupText(popupButtonsText);
				GUI.Label(new Rect(20f, 40f, ((Rect)(ref popupRect2)).width - 40f, 80f), translatedPopupText, GUIStyles.buttonLabelStyle);
				GUI.backgroundColor = Color.green;
				if (GUIStyles.ManualButton(new Rect(40f, 150f, 90f, 40f), Localization.Translate("Yes"), GUIStyles.buttonLabelStyle))
				{
					popupYesAction?.Invoke();
					showPopupButtons = false;
				}
				GUI.backgroundColor = Color.red;
				if (GUIStyles.ManualButton(new Rect(310f, 150f, 90f, 40f), Localization.Translate("No"), GUIStyles.buttonLabelStyle))
				{
					popupNoAction?.Invoke();
					showPopupButtons = false;
				}
				GUI.backgroundColor = GUIStyles.GradientColor;
			}), "");
		}
		if (showPopupInput)
		{
			Rect popupRect3 = new Rect((float)((Screen.width - 440) / 2), (float)((Screen.height - 250) / 2), 440f, 250f);
			GUI.backgroundColor = GUIStyles.GradientColor;
			GUI.ModalWindow(1003, popupRect3, WindowFunction.op_Implicit((Action<int>)delegate
			{
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_005b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0070: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_009c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
				//IL_011c: Unknown result type (might be due to invalid IL or missing references)
				//IL_015b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0179: Unknown result type (might be due to invalid IL or missing references)
				GUI.Label(new Rect(20f, 35f, ((Rect)(ref popupRect3)).width - 40f, 55f), popupInputMessage, GUIStyles.buttonLabelStyle);
				Rect r = default(Rect);
				((Rect)(ref r))._002Ector(20f, 100f, ((Rect)(ref popupRect3)).width - 40f, 38f);
				Style.FillRounded(r, new Color(0.08f, 0.11f, 0.16f, 1f), 6);
				Style.StrokeRounded(r, new Color(0.45f, 0.75f, 1f, 0.9f), 6, 1);
				GUI.color = Color.white;
				GUI.Label(new Rect(((Rect)(ref r)).x + 8f, ((Rect)(ref r)).y + 4f, ((Rect)(ref r)).width - 16f, 30f), string.IsNullOrEmpty(popupInputText) ? "Type configuration name..." : popupInputText, GUIStyles.buttonLabelStyle);
				HandlePopupInputTyping();
				GUI.backgroundColor = Color.green;
				if (GUIStyles.ManualButton(new Rect(40f, 165f, 120f, 40f), Localization.Translate(popupInputSubmitLabel), GUIStyles.buttonLabelStyle))
				{
					Action<string> action = popupInputAction;
					string obj = popupInputText;
					showPopupInput = false;
					popupInputAction = null;
					action?.Invoke(obj);
				}
				GUI.backgroundColor = Color.red;
				if (GUIStyles.ManualButton(new Rect(280f, 165f, 120f, 40f), Localization.Translate("Cancel"), GUIStyles.buttonLabelStyle))
				{
					showPopupInput = false;
					popupInputAction = null;
				}
			}), "");
		}
		DrawNotifications();
	}

	private static void HandlePopupInputTyping()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Invalid comparison between Unknown and I4
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Invalid comparison between Unknown and I4
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Invalid comparison between Unknown and I4
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Invalid comparison between Unknown and I4
		Event current = Event.current;
		if (current == null || (int)current.type != 4)
		{
			return;
		}
		if ((int)current.keyCode == 8)
		{
			if (popupInputText.Length > 0)
			{
				popupInputText = popupInputText.Substring(0, popupInputText.Length - 1);
			}
			current.Use();
		}
		else if ((int)current.keyCode == 13 || (int)current.keyCode == 271)
		{
			SubmitPopupInput();
			current.Use();
		}
		else if (current.character != 0 && !char.IsControl(current.character))
		{
			popupInputText += current.character;
			current.Use();
		}
	}

	private static void SubmitPopupInput()
	{
		Action<string> action = popupInputAction;
		string obj = popupInputText;
		showPopupInput = false;
		popupInputAction = null;
		action?.Invoke(obj);
	}

	private static void DrawNotifications()
	{
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Expected O, but got Unknown
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		float num = 3f;
		float num2 = 44f;
		float num3 = 315f;
		float num4 = 8f;
		float num5 = Screen.height - 80;
		Rect val4 = default(Rect);
		for (int num6 = notifications.Count - 1; num6 >= 0; num6--)
		{
			GuiNotification guiNotification = notifications[num6];
			guiNotification.timer -= Time.unscaledDeltaTime;
			float num7 = num5 - (float)num6 * (num2 + num4);
			guiNotification.posY = Mathf.Lerp(guiNotification.posY, num7, 10f * Time.unscaledDeltaTime);
			if (guiNotification.timer > guiNotification.maxTime - 0.4f)
			{
				guiNotification.alpha = Mathf.Min(1f, guiNotification.alpha + Time.unscaledDeltaTime * 2.5f);
			}
			else if (guiNotification.timer < 0.5f)
			{
				guiNotification.alpha = Mathf.Max(0f, guiNotification.timer / 0.5f);
			}
			else
			{
				guiNotification.alpha = 1f;
			}
			if (guiNotification.timer <= 0f || guiNotification.alpha <= 0.01f)
			{
				notifications.RemoveAt(num6);
			}
			else
			{
				guiNotification.glowTime += Time.unscaledDeltaTime * 2f;
				float num8 = 0.26f + 0.16f * Mathf.Sin(guiNotification.glowTime);
				Color val = Color.Lerp(guiNotification.color, Color.white, num8);
				Rect val2 = new Rect(22f, guiNotification.posY - num, num3 + num * 2f, num2 + num * 2f);
				GUI.backgroundColor = Color.black;
				GUI.Box(val2, "");
				GUI.backgroundColor = val * new Color(1f, 1f, 1f, guiNotification.alpha);
				GUI.Box(new Rect(22f + num, guiNotification.posY, num3, num2), "");
				GUIStyle val3 = new GUIStyle(GUI.skin.label)
				{
					fontSize = 18,
					alignment = (TextAnchor)4,
					fontStyle = (FontStyle)1
				};
				val3.normal.textColor = new Color(0f, 0f, 0f, guiNotification.alpha);
				((Rect)(ref val4))._002Ector(22f + num, guiNotification.posY, num3, num2);
				string translatedText = GetTranslatedText(guiNotification);
				for (int i = -1; i <= 1; i++)
				{
					for (int j = -1; j <= 1; j++)
					{
						if (i != 0 || j != 0)
						{
							GUI.Label(new Rect(((Rect)(ref val4)).x + (float)i * 0.8f, ((Rect)(ref val4)).y + (float)j * 0.8f, ((Rect)(ref val4)).width, ((Rect)(ref val4)).height), translatedText, val3);
						}
					}
				}
				val3.normal.textColor = new Color(guiNotification.color.r, guiNotification.color.g, guiNotification.color.b, guiNotification.alpha);
				GUI.Label(val4, translatedText, val3);
			}
		}
	}

	public void OnGUI()
	{
		DrawPopups();
	}
}
