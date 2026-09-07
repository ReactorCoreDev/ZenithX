using UnityEngine;

namespace ZenithX;

public static class Drawing
{
	public static void DrawLine(Vector2 start, Vector2 end, Color color, float width)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = end - start;
		float num = 57.29578f * Mathf.Atan2(val.y, val.x);
		GUIUtility.RotateAroundPivot(num, start);
		GUI.DrawTexture(new Rect(start.x, start.y - width / 2f, ((Vector2)(ref val)).magnitude, width), (Texture)(object)CreateTexture(color), (ScaleMode)0);
		GUIUtility.RotateAroundPivot(0f - num, start);
	}

	private static Texture2D CreateTexture(Color color)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		Texture2D val = new Texture2D(1, 1);
		val.SetPixel(0, 0, color);
		val.Apply();
		return val;
	}
}
