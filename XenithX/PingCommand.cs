[HarmonyPatch(typeof(ChatController), nameof(ChatController.SendChat))]
public static class ChatHistory_ChatController_SendChat_Prefix
{
    public static readonly List<string> ChatHistory = new List<string>();
    private static Il2CppSystem.DateTime LastPingCommand = Il2CppSystem.DateTime.MinValue;
    private const float CommandCooldown = 5f;
    private static readonly Dictionary<byte, Il2CppSystem.DateTime> lastPingTimes = new Dictionary<byte, Il2CppSystem.DateTime>();

    public static bool Prefix(ChatController __instance)
    {
        string text = __instance.freeChatField.textArea.text;

        // Add to chat history if not duplicate
        if (ChatHistory.LastOrDefault() != text)
            ChatHistory.Add(text);
        ChatJailbreak_ChatController_Update_Postfix.CurrentHistorySelection = ChatHistory.Count;

        // Skip if empty or quick chat is visible
        if (string.IsNullOrWhiteSpace(text) || __instance.quickChatField.Visible)
            return true;

        string[] args = text.Split(' ');

        switch (args[0].ToLower())
        {
            case "/ping":
                __instance.freeChatField.Clear();
                __instance.timeSinceLastMessage = 3f;
                HandlePingCommand();
                return false;

            case "/say":
                if (args.Length > 1)
                {
                    string msg = string.Join(" ", args.Skip(1));
                    CustomMessage.SendMessageToAll(msg);
                    __instance.freeChatField.Clear();
                    __instance.timeSinceLastMessage = 3f;
                    return false;
                }
                break;

            case "/pm":
                if (args.Length > 2)
                {
                    string targetName = args[1];
                    string msg = string.Join(" ", args.Skip(2));
                    var target = PlayerControl.AllPlayerControls
                        .ToArray()
                        .FirstOrDefault(p => p.Data.PlayerName.Equals(targetName, System.StringComparison.OrdinalIgnoreCase));
                    if (target != null)
                        CustomMessage.SendPrivateMessage(msg, target);

                    __instance.freeChatField.Clear();
                    __instance.timeSinceLastMessage = 3f;
                    return false;
                }
                break;
        }

        return true;
    }

    private static void HandlePingCommand()
    {
        if (PlayerControl.LocalPlayer == null || AmongUsClient.Instance == null || !AmongUsClient.Instance.AmConnected)
        {
            CustomMessage.SendMessageToAll("<color=#ff0000>Error: You are not in a lobby.</color>");
            return;
        }

        if ((Il2CppSystem.DateTime.UtcNow - LastPingCommand).TotalSeconds < CommandCooldown)
        {
            int wait = (int)Il2CppSystem.Math.Ceiling(CommandCooldown - (Il2CppSystem.DateTime.UtcNow - LastPingCommand).TotalSeconds);
            CustomMessage.SendMessageToAll($"<color=#ffff00>Wait {wait}s before using /ping again.</color>");
            return;
        }

        LastPingCommand = Il2CppSystem.DateTime.UtcNow;

        var builder = new System.Text.StringBuilder();
        builder.AppendLine("<b><color=#00ffff>Ping Information:</color></b>");
        foreach (var player in PlayerControl.AllPlayerControls.ToArray().OrderBy(p => p.PlayerId))
        {
            int ping = player.AmOwner ? AmongUsClient.Instance.Ping : SimulatePing(player.PlayerId);
            string pingColor = ping < 100 ? "00ff00" : (ping < 200 ? "ffff00" : "ff0000");
            builder.AppendLine($"{player.Data.PlayerName}: <color=#{pingColor}>{ping}ms</color>");
        }

        CustomMessage.SendMessageToAll(builder.ToString());
    }

    private static int SimulatePing(byte playerId)
    {
        if (!lastPingTimes.TryGetValue(playerId, out var last))
        {
            lastPingTimes[playerId] = Il2CppSystem.DateTime.UtcNow;
            return -1;
        }

        int simulated = (int)(Il2CppSystem.DateTime.UtcNow - last).TotalMilliseconds;
        lastPingTimes[playerId] = Il2CppSystem.DateTime.UtcNow;
        return simulated;
    }
}