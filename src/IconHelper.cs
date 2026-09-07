using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Il2CppSystem;
using UnityEngine;
using UnityEngine.Networking;

namespace ZenithX;

public static class IconHelper
{
	private static readonly Dictionary<string, string> IconPathCache = new Dictionary<string, string>();

	private static readonly Dictionary<string, Texture2D> IconTextureCache = new Dictionary<string, Texture2D>();

	private static readonly Dictionary<string, bool> FetchInProgress = new Dictionary<string, bool>();

	private const string LucideCDN = "https://unpkg.com/lucide@latest/icons/{0}.svg";

	private static readonly Dictionary<string, string> FallbackIconPaths = new Dictionary<string, string>
	{
		{ "alert-1", "M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0zM12 9v4M12 17h.01" },
		{ "user", "M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2M12 11a4 4 0 1 0 0-8 4 4 0 0 0 0 8" },
		{ "eye", "M2 12s3-7 10-7 10 7 10 7-3 7-10 7-10-7-10-7ZM12 12a3 3 0 1 0 0-6 3 3 0 0 0 0 6" },
		{ "shield", "M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10" },
		{ "map", "M3 7v6h6M21 17v-6h-6M14 7v6h6M3 17v-6h6" },
		{ "message", "M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z" },
		{ "settings", "M12.22 2h-.44a2 2 0 0 1-2 2v.18a2 2 0 0 1-1 1.73l-.43.25a2 2 0 0 1-2 0l-.15-.08a2 2 0 0 0-2.73.73l-.22.38a2 2 0 0 0 .73 2.73l.15.1a2 2 0 0 1 1 1.72v.51a2 2 0 0 1-1 1.74l-.15.09a2 2 0 0 0-.73 2.73l.22.38a2 2 0 0 0 2.73.73l.15-.08a2 2 0 0 1 2 0l.43.25a2 2 0 0 1 1 1.73V20a2 2 0 0 1 2 2h.44a2 2 0 0 1 2-2v-.18a2 2 0 0 1 1-1.73l.43-.25a2 2 0 0 1 2 0l.15.08a2 2 0 0 0 2.73-.73l.22-.39a2 2 0 0 0-.73-2.73l-.15-.09a2 2 0 0 1-1-1.74v-.47a2 2 0 0 1 1-1.74l.15-.09a2 2 0 0 0 .73-2.73l-.22-.39a2 2 0 0 0-2.73-.73l-.15.08a2 2 0 0 1-2 0l-.43-.25a2 2 0 0 1-1-1.73V4a2 2 0 0 1-2-2z" },
		{ "crown", "M2 4l3 12h14l3-12-6 7-4-7-4 7-6-7" },
		{ "sparkles", "M12 3v18M3 12h18M5.6 5.6l12.8 12.8M18.4 5.6L5.6 18.4" },
		{ "play", "M5 3l14 9-14 9V3z" },
		{ "sliders", "M4 21v-7M4 10V3M12 21v-9M12 8V3M20 21v-5M20 12V3M1 14h6M9 8h6M17 16h6" }
	};

	public static string GetIconPath(string iconName)
	{
		if (string.IsNullOrEmpty(iconName))
		{
			return "";
		}
		string text = Regex.Replace(iconName, "-\\d+$", "");
		if (IconPathCache.TryGetValue(text, out var value))
		{
			return value;
		}
		if (FallbackIconPaths.TryGetValue(text, out var value2))
		{
			IconPathCache[text] = value2;
			return value2;
		}
		if (FallbackIconPaths.TryGetValue(iconName, out value2))
		{
			IconPathCache[iconName] = value2;
			return value2;
		}
		if (!FetchInProgress.ContainsKey(text))
		{
			FetchInProgress[text] = true;
			MonoBehaviour val = Object.FindObjectOfType<MonoBehaviour>();
			if ((Object)(object)val != (Object)null)
			{
				val.StartCoroutine("FetchIconFromAPI", Object.op_Implicit(text));
			}
		}
		return "";
	}

	private static IEnumerator FetchIconFromAPI(string iconName)
	{
		string text = $"https://unpkg.com/lucide@latest/icons/{iconName}.svg";
		UnityWebRequest request = UnityWebRequest.Get(text);
		yield return request.SendWebRequest();
		if ((int)request.result == 1)
		{
			string value = ExtractPathFromSVG(request.downloadHandler.text);
			if (!string.IsNullOrEmpty(value))
			{
				IconPathCache[iconName] = value;
			}
		}
		FetchInProgress.Remove(iconName);
		request.Dispose();
	}

	private static string ExtractPathFromSVG(string svgContent)
	{
		Match match = Regex.Match(svgContent, "<path[^>]*d=\"([^\"]*)\"");
		if (match.Success)
		{
			return match.Groups[1].Value;
		}
		return "";
	}

	public static bool HasIcon(string iconName)
	{
		if (!string.IsNullOrEmpty(iconName))
		{
			return !string.IsNullOrEmpty(GetIconPath(iconName));
		}
		return false;
	}

	public static void DrawIcon(Rect rect, string iconName, Color color, int size = 16)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		string iconPath = GetIconPath(iconName);
		if (!string.IsNullOrEmpty(iconPath))
		{
			GUI.color = color;
			DrawSVGPath(rect, iconPath, color, size);
			GUI.color = Color.white;
		}
	}

	private static void DrawSVGPath(Rect rect, string pathData, Color color, int size)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		Vector2[] array = ParseSVGPath(pathData, rect, size);
		if (array.Length < 2)
		{
			return;
		}
		GUI.color = color;
		for (int i = 0; i < array.Length - 1; i++)
		{
			Vector2 val = array[i];
			Vector2 val2 = array[i + 1];
			Vector2 val3 = val2 - val;
			Vector2 val4 = ((Vector2)(ref val3)).normalized;
			if (val4 == Vector2.zero)
			{
				val4 = Vector2.right;
			}
			Vector2 val5 = new Vector2(0f - val4.y, val4.x) * 1f;
			Vector2[] array2 = (Vector2[])(object)new Vector2[4]
			{
				val + val5,
				val - val5,
				val2 - val5,
				val2 + val5
			};
			float num = Mathf.Min(new float[4]
			{
				array2[0].x,
				array2[1].x,
				array2[2].x,
				array2[3].x
			});
			float num2 = Mathf.Min(new float[4]
			{
				array2[0].y,
				array2[1].y,
				array2[2].y,
				array2[3].y
			});
			float num3 = Mathf.Max(new float[4]
			{
				array2[0].x,
				array2[1].x,
				array2[2].x,
				array2[3].x
			});
			float num4 = Mathf.Max(new float[4]
			{
				array2[0].y,
				array2[1].y,
				array2[2].y,
				array2[3].y
			});
			Rect val6 = new Rect(num - ((Rect)(ref rect)).x, num2 - ((Rect)(ref rect)).y, num3 - num + 2f, num4 - num2 + 2f);
			GUI.color = color;
			GUI.DrawTexture(val6, (Texture)(object)Texture2D.whiteTexture, (ScaleMode)0);
		}
		GUI.color = Color.white;
	}

	private static Vector2[] ParseSVGPath(string pathData, Rect rect, int size)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_054b: Unknown result type (might be due to invalid IL or missing references)
		//IL_054c: Unknown result type (might be due to invalid IL or missing references)
		//IL_054e: Unknown result type (might be due to invalid IL or missing references)
		//IL_054f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0551: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_050c: Unknown result type (might be due to invalid IL or missing references)
		//IL_050d: Unknown result type (might be due to invalid IL or missing references)
		//IL_050f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0511: Unknown result type (might be due to invalid IL or missing references)
		//IL_0516: Unknown result type (might be due to invalid IL or missing references)
		//IL_0519: Unknown result type (might be due to invalid IL or missing references)
		//IL_051b: Unknown result type (might be due to invalid IL or missing references)
		//IL_051d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_0535: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		List<Vector2> list = new List<Vector2>();
		string[] array = Regex.Split(pathData, "(?=[MmLlHhVvZzCcQq])");
		Vector2 val = Vector2.zero;
		Vector2 val2 = Vector2.zero;
		_ = Vector2.zero;
		float num = Mathf.Min(((Rect)(ref rect)).width, ((Rect)(ref rect)).height) / 24f;
		Vector2 val3 = default(Vector2);
		((Vector2)(ref val3))._002Ector(((Rect)(ref rect)).x + ((Rect)(ref rect)).width / 2f, ((Rect)(ref rect)).y + ((Rect)(ref rect)).height / 2f);
		string[] array2 = array;
		Vector2 p2 = default(Vector2);
		Vector2 p3 = default(Vector2);
		Vector2 val6 = default(Vector2);
		Vector2 p = default(Vector2);
		Vector2 val4 = default(Vector2);
		foreach (string text in array2)
		{
			if (string.IsNullOrWhiteSpace(text))
			{
				continue;
			}
			char c = text[0];
			string[] array3 = Regex.Split(text.Substring(1).Trim(), "[\\s,]+");
			switch (c)
			{
			case 'M':
			case 'm':
			{
				for (int num4 = 0; num4 < array3.Length - 1; num4 += 2)
				{
					if (float.TryParse(array3[num4], out var result12) && float.TryParse(array3[num4 + 1], out var result13))
					{
						result12 *= num;
						result13 *= num;
						if (c == 'm')
						{
							val += new Vector2(result12, result13);
						}
						else
						{
							((Vector2)(ref val))._002Ector(result12, result13);
						}
						val2 = val;
						list.Add(val + val3);
					}
				}
				break;
			}
			case 'L':
			case 'l':
			{
				for (int num5 = 0; num5 < array3.Length - 1; num5 += 2)
				{
					if (float.TryParse(array3[num5], out var result15) && float.TryParse(array3[num5 + 1], out var result16))
					{
						result15 *= num;
						result16 *= num;
						if (c == 'l')
						{
							val += new Vector2(result15, result16);
						}
						else
						{
							((Vector2)(ref val))._002Ector(result15, result16);
						}
						list.Add(val + val3);
					}
				}
				break;
			}
			case 'H':
			case 'h':
			{
				string[] array4 = array3;
				for (int l = 0; l < array4.Length; l++)
				{
					if (float.TryParse(array4[l], out var result5))
					{
						result5 *= num;
						if (c == 'h')
						{
							val.x += result5;
						}
						else
						{
							val.x = result5;
						}
						list.Add(val + val3);
					}
				}
				break;
			}
			case 'V':
			case 'v':
			{
				string[] array4 = array3;
				for (int l = 0; l < array4.Length; l++)
				{
					if (float.TryParse(array4[l], out var result14))
					{
						result14 *= num;
						if (c == 'v')
						{
							val.y += result14;
						}
						else
						{
							val.y = result14;
						}
						list.Add(val + val3);
					}
				}
				break;
			}
			case 'C':
			case 'c':
			{
				for (int m = 0; m < array3.Length - 5; m += 6)
				{
					if (float.TryParse(array3[m], out var result6) && float.TryParse(array3[m + 1], out var result7) && float.TryParse(array3[m + 2], out var result8) && float.TryParse(array3[m + 3], out var result9) && float.TryParse(array3[m + 4], out var result10) && float.TryParse(array3[m + 5], out var result11))
					{
						result6 *= num;
						result7 *= num;
						result8 *= num;
						result9 *= num;
						result10 *= num;
						result11 *= num;
						if (c == 'c')
						{
							p2 = val + new Vector2(result6, result7);
							p3 = val + new Vector2(result8, result9);
							val6 = val + new Vector2(result10, result11);
						}
						else
						{
							((Vector2)(ref p2))._002Ector(result6, result7);
							((Vector2)(ref p3))._002Ector(result8, result9);
							((Vector2)(ref val6))._002Ector(result10, result11);
						}
						int num3 = 10;
						for (int n = 0; n <= num3; n++)
						{
							Vector2 val7 = CalculateBezierPoint((float)n / (float)num3, val, p2, p3, val6);
							list.Add(val7 + val3);
						}
						val = val6;
					}
				}
				break;
			}
			case 'Q':
			case 'q':
			{
				for (int j = 0; j < array3.Length - 3; j += 4)
				{
					if (float.TryParse(array3[j], out var result) && float.TryParse(array3[j + 1], out var result2) && float.TryParse(array3[j + 2], out var result3) && float.TryParse(array3[j + 3], out var result4))
					{
						result *= num;
						result2 *= num;
						result3 *= num;
						result4 *= num;
						if (c == 'q')
						{
							p = val + new Vector2(result, result2);
							val4 = val + new Vector2(result3, result4);
						}
						else
						{
							((Vector2)(ref p))._002Ector(result, result2);
							((Vector2)(ref val4))._002Ector(result3, result4);
						}
						int num2 = 10;
						for (int k = 0; k <= num2; k++)
						{
							Vector2 val5 = CalculateQuadraticBezierPoint((float)k / (float)num2, val, p, val4);
							list.Add(val5 + val3);
						}
						val = val4;
					}
				}
				break;
			}
			case 'Z':
			case 'z':
				val = val2;
				list.Add(val + val3);
				break;
			}
		}
		return list.ToArray();
	}

	private static Vector2 CalculateBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		float num = 1f - t;
		float num2 = t * t;
		float num3 = num * num;
		float num4 = num3 * num;
		float num5 = num2 * t;
		return num4 * p0 + 3f * num3 * t * p1 + 3f * num * num2 * p2 + num5 * p3;
	}

	private static Vector2 CalculateQuadraticBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		float num = 1f - t;
		float num2 = t * t;
		return num * num * p0 + 2f * num * t * p1 + num2 * p2;
	}
}
