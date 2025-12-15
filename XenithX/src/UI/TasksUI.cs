using System.Linq;
using UnityEngine;

namespace ZenithX;

public class TasksUI : MonoBehaviour
{
    public static bool isVisible = false;
    private Vector2 _scrollPosition = Vector2.zero;
    private Rect _windowRect = new(320, 10, 500, 300);
    private GUIStyle _playerHeaderStyle;
    private Il2CppSystem.Text.StringBuilder _tasksString = new();
    private readonly System.Collections.Generic.Dictionary<string, bool> _expandedPlayers = new();
    private static readonly Color gradientTop = new Color(0.678f, 0.847f, 0.902f); // LightBlue (173, 216, 230)
    private static readonly Color gradientBottom = Color.blue;                      // Pure Blue (0, 0, 255)
    public static Color GradientColor
    {
        get { return Color.Lerp(gradientTop, gradientBottom, 0.5f); }
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
    private void OnGUI()
    {
        if (!isVisible) return;

        _playerHeaderStyle ??= new GUIStyle(GUI.skin.button)
        {
            fontSize = 18,
            alignment = TextAnchor.MiddleLeft
        };

        if(TryParseHtmlString(ZenithX.menuHtmlColor.Value, out var configUIColor)){
            GUI.backgroundColor = configUIColor;
        }

        if (CheatToggles.init)
        {
            GUI.backgroundColor = GradientColor;
        }

        _windowRect = GUI.Window(3, _windowRect, (GUI.WindowFunction)TasksWindow, "Tasks");
    }

    private void TasksWindow(int windowID)
    {
        GUILayout.BeginVertical();
        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true);

        foreach (var pc in PlayerControl.AllPlayerControls)
        {
            if (!pc.Data || !pc.Data.Role) continue;

            GUILayout.BeginVertical();

            var nameKey = pc.name;
            _expandedPlayers.TryGetValue(nameKey, out var expanded);
            var arrow = expanded ? "\u25BC" : "\u25B6"; // ▼ or ▶

            var taskCount = pc.myTasks.Count;
            var completeCount = pc.myTasks.ToArray().Count(t => t.IsComplete);

            if (pc == PlayerControl.LocalPlayer && pc.Data.IsDead)
            {
                taskCount -= 1;
            }
            if (pc == PlayerControl.LocalPlayer && Utils.isAnySabotageActive)
            {
                taskCount -= 1;
            }
            if (pc == PlayerControl.LocalPlayer && pc.Data.Role.IsImpostor)
            {
                taskCount -= 1;
            }

            if (GUILayout.Button($"{arrow} [{completeCount}/{taskCount}] {nameKey}", _playerHeaderStyle))
            {
                _expandedPlayers[nameKey] = !expanded;
                expanded = !expanded;
            }

            if (expanded)
            {
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                foreach (var task in pc.myTasks)
                {
                    // Do some checks to not show texts: sabotage active, dead hint, impostor hint
                    if (task.TaskType is TaskTypes.ResetReactor or TaskTypes.RestoreOxy or TaskTypes.FixLights or TaskTypes.FixComms or TaskTypes.ResetSeismic or TaskTypes.StopCharles or TaskTypes.MushroomMixupSabotage) continue;

                    _tasksString.Clear();
                    task.AppendTaskText(_tasksString);
                    //_tasksString.Append($"Task Type: {task.TaskType.ToString()}");
                    var taskText = _tasksString.ToString();
                    if (taskText.Contains("You're dead") || taskText.Contains("Sabotage and kill")) continue;

                    GUILayout.BeginHorizontal();
                    GUILayout.Label(taskText.Replace("\n", "").Replace("</color>", "").Replace("<color=#00DD00FF>", "").Replace("<color=#FFFF00FF>", ""));
                    GUILayout.FlexibleSpace();

                    if (task.IsComplete)
                    {
                        GUILayout.Label("<color=#00ff00>✔ Complete</color>");
                    }
                    else
                    {
                        if (pc == PlayerControl.LocalPlayer)
                        {
                            if (GUILayout.Button("Complete"))
                            {
                                Utils.completeTask(task);
                            }
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }

        GUILayout.EndScrollView();

        if (GUILayout.Button("Complete My Tasks"))
        {
            CheatToggles.completeMyTasks = true;
        }

        GUILayout.EndVertical();

        GUI.DragWindow();
    }
}