using UnityEngine;
using System.Collections.Generic;
using System;

namespace ZenithX
{
    public class ConsoleUI : MonoBehaviour
    {
        public bool isVisible = false;
        private Vector2 scrollPosition = Vector2.zero;
        public static System.Collections.Generic.List<string> logEntries = new System.Collections.Generic.List<string>();
        private Rect windowRect = new Rect(320, 10, 500, 300); // Adjust size and position as needed
        private GUIStyle logStyle;
        private static readonly Color gradientTop = new Color(0.678f, 0.847f, 0.902f); // LightBlue (173, 216, 230)
        private static readonly Color gradientBottom = Color.blue;                      // Pure Blue (0, 0, 255)
  // LightBlue → Blue
    public static Color GradientColor
    {
        get { return Color.Lerp(gradientTop, gradientBottom, 0.5f); }
    }

    // Optional: get gradient color at position t (0 = LightBlue, 1 = Blue)
    public static Color GetGradientColor(float t)
    {
        return Color.Lerp(gradientTop, gradientBottom, Mathf.Clamp01(t));
    }
      private Texture2D MakeSolidTexture(int width, int height, Color color)
    {
        Texture2D tex = new Texture2D(width, height);
        Color[] colors = new Color[width * height];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = color;
        }
        tex.SetPixels(colors);
        tex.Apply();
        return tex;
    }

        public static bool TryParseHtmlString(string hex, out Color uiColor)
        {
            uiColor = Color.white;

            if (string.IsNullOrEmpty(hex))
                return false;

            try
            {
                if (hex.StartsWith("#"))
                    hex = hex.Substring(1);

                if (hex.Length != 6 && hex.Length != 8)
                    return false;

                byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                byte a = 255;

                if (hex.Length == 8)
                    a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);

                uiColor = new Color32(r, g, b, a);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Log(string message)
        {
            if (logEntries.Count >= 100) // Limit the number of logs to keep memory usage in check
            {
                logEntries.RemoveAt(0); // Remove the oldest log entry
            }

            logEntries.Add(message);

            // Scroll to the bottom
            scrollPosition.y = float.MaxValue;
        }

        private void OnGUI()
        {

            if (!isVisible) return;

            if (logStyle == null)
            {

                logStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 20,
                    richText = true // Essential for colored names
                };
            }
            
            Color configUIColor;

            if (TryParseHtmlString(ZenithX.menuHtmlColor.Value, out configUIColor))
            {
                GUI.backgroundColor = configUIColor;
            }

            GUI.backgroundColor = GradientColor;

            windowRect = GUI.Window(1, windowRect, (GUI.WindowFunction)ConsoleWindow, "ZenithX Console");
        }

        private void ConsoleWindow(int windowID)
        {
            GUILayout.BeginVertical();
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);

            foreach (var log in logEntries)
            {
                GUILayout.Label(log, logStyle); // Use the custom GUIStyle with the specified font size
            }

            if (GUILayout.Button("Clear Log")){
                logEntries.Clear();
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUI.DragWindow();
        }
    }
}