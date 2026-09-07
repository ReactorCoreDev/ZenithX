using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace ZenithX;

public static class MenuLock
{
	private static string _passwordInput = "";

	private static bool _showError = false;

	private static float _errorTimer = 0f;

	private static bool _isFocused = false;

	private static float _cursorBlinkTimer = 0f;

	private static bool _showCursor = true;

	public static bool IsUnlocked = true;

	private static readonly string DefaultPassword = "zenith";

	public static void Update()
	{
		if (_showError)
		{
			_errorTimer -= Time.deltaTime;
			if (_errorTimer <= 0f)
			{
				_showError = false;
			}
		}
		_cursorBlinkTimer += Time.deltaTime;
		if (_cursorBlinkTimer >= 0.5f)
		{
			_showCursor = !_showCursor;
			_cursorBlinkTimer = 0f;
		}
	}

	public static void DrawLockScreen()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Expected O, but got Unknown
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Expected O, but got Unknown
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Expected O, but got Unknown
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04af: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d5: Expected O, but got Unknown
		//IL_0507: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Invalid comparison between Unknown and I4
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Invalid comparison between Unknown and I4
		//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_0538: Unknown result type (might be due to invalid IL or missing references)
		//IL_0540: Unknown result type (might be due to invalid IL or missing references)
		//IL_0547: Unknown result type (might be due to invalid IL or missing references)
		//IL_054e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0563: Unknown result type (might be due to invalid IL or missing references)
		//IL_056f: Expected O, but got Unknown
		//IL_059b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Invalid comparison between Unknown and I4
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Invalid comparison between Unknown and I4
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Invalid comparison between Unknown and I4
		GUI.color = new Color(0f, 0f, 0f, 0.85f);
		GUI.Box(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), "");
		GUI.color = Color.white;
		float num = 460f;
		float num2 = 360f;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(((float)Screen.width - num) / 2f, ((float)Screen.height - num2) / 2f, num, num2);
		GUI.backgroundColor = new Color(0.08f, 0.04f, 0.05f, 0.96f);
		GUI.Box(val, "", GUI.skin.box);
		GUIStyle val2 = new GUIStyle(GUI.skin.label)
		{
			fontSize = 34,
			fontStyle = (FontStyle)1,
			alignment = (TextAnchor)4
		};
		val2.normal.textColor = new Color(1f, 0f, 0.5f, 1f);
		GUIStyle val3 = val2;
		GUI.Label(new Rect(((Rect)(ref val)).x + 20f, ((Rect)(ref val)).y + 30f, ((Rect)(ref val)).width - 40f, 55f), "Menu Locked", val3);
		GUIStyle val4 = new GUIStyle(GUI.skin.label)
		{
			fontSize = 14,
			alignment = (TextAnchor)4
		};
		val4.normal.textColor = new Color(0.8f, 0.7f, 0.7f);
		GUIStyle val5 = val4;
		GUI.Label(new Rect(((Rect)(ref val)).x + 20f, ((Rect)(ref val)).y + 95f, ((Rect)(ref val)).width - 40f, 25f), "Enter password to unlock the menu", val5);
		Rect val6 = default(Rect);
		((Rect)(ref val6))._002Ector(((Rect)(ref val)).x + 50f, ((Rect)(ref val)).y + 140f, ((Rect)(ref val)).width - 100f, 45f);
		GUI.backgroundColor = new Color(0.12f, 0.05f, 0.06f, 1f);
		GUI.Box(val6, "");
		if ((int)Event.current.type == 0 && ((Rect)(ref val6)).Contains(Event.current.mousePosition))
		{
			_isFocused = true;
			Event.current.Use();
		}
		else if ((int)Event.current.type == 0 && !((Rect)(ref val6)).Contains(Event.current.mousePosition))
		{
			_isFocused = false;
		}
		if (_isFocused)
		{
			GUI.backgroundColor = new Color(0.6f, 0.18f, 0.18f, 1f);
			GUI.Box(new Rect(((Rect)(ref val6)).x - 2f, ((Rect)(ref val6)).y - 2f, ((Rect)(ref val6)).width + 4f, ((Rect)(ref val6)).height + 4f), "");
		}
		string text = _passwordInput;
		if (string.IsNullOrEmpty(text))
		{
			text = "Enter password...";
		}
		if (_isFocused && _showCursor)
		{
			text += "|";
		}
		GUIStyle val7 = new GUIStyle(GUI.skin.label)
		{
			fontSize = 18,
			fontStyle = (FontStyle)1,
			alignment = (TextAnchor)3
		};
		val7.normal.textColor = Color.white;
		GUIStyle val8 = val7;
		GUI.Label(new Rect(((Rect)(ref val6)).x + 12f, ((Rect)(ref val6)).y + 8f, ((Rect)(ref val6)).width - 22f, ((Rect)(ref val6)).height - 16f), text, val8);
		if (_isFocused && (int)Event.current.type == 4)
		{
			if ((int)Event.current.keyCode == 13 || (int)Event.current.keyCode == 271)
			{
				CheckPassword();
				Event.current.Use();
			}
			else if ((int)Event.current.keyCode == 8)
			{
				if (_passwordInput.Length > 0)
				{
					_passwordInput = _passwordInput.Substring(0, _passwordInput.Length - 1);
				}
				Event.current.Use();
			}
			else if ((int)Event.current.keyCode == 27)
			{
				_passwordInput = string.Empty;
				_isFocused = false;
				Event.current.Use();
			}
			else if (!char.IsControl(Event.current.character))
			{
				char character = Event.current.character;
				if (((character >= 'a' && character <= 'z') || (character >= 'A' && character <= 'Z') || (character >= '0' && character <= '9')) && _passwordInput.Length < 30)
				{
					_passwordInput += character;
				}
				Event.current.Use();
			}
		}
		GUIStyle val9 = new GUIStyle(GUI.skin.button)
		{
			fontSize = 18,
			fontStyle = (FontStyle)1
		};
		val9.normal.textColor = Color.white;
		GUIStyle val10 = val9;
		if (GUI.Button(new Rect(((Rect)(ref val)).x + ((Rect)(ref val)).width / 2f - 80f, ((Rect)(ref val)).y + 205f, 160f, 45f), "Unlock", val10))
		{
			CheckPassword();
		}
		if (_showError)
		{
			GUIStyle val11 = new GUIStyle(GUI.skin.label)
			{
				fontSize = 12,
				alignment = (TextAnchor)4,
				fontStyle = (FontStyle)1
			};
			val11.normal.textColor = new Color(1f, 0.4f, 0.4f);
			GUIStyle val12 = val11;
			GUI.Label(new Rect(((Rect)(ref val)).x + 20f, ((Rect)(ref val)).y + 330f, ((Rect)(ref val)).width - 40f, 25f), "Incorrect password!", val12);
		}
		GUI.backgroundColor = Color.white;
		GUI.color = Color.white;
	}

	private static void CheckPassword()
	{
		string text = HashPassword(_passwordInput);
		string text2 = HashPassword(DefaultPassword);
		if (text == text2)
		{
			IsUnlocked = true;
			_passwordInput = string.Empty;
			_showError = false;
			_isFocused = false;
		}
		else
		{
			_showError = true;
			_errorTimer = 3f;
			_passwordInput = string.Empty;
		}
	}

	private static string HashPassword(string password)
	{
		if (string.IsNullOrEmpty(password))
		{
			return string.Empty;
		}
		using SHA256 sHA = SHA256.Create();
		byte[] bytes = Encoding.UTF8.GetBytes(password);
		return Convert.ToHexString(sHA.ComputeHash(bytes));
	}
}
