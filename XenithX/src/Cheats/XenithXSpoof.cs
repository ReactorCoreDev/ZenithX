using AmongUs.Data;

namespace ZenithX;
public static class ZenithXSpoof
{
    public static uint parsedLevel;

    public static void spoofLevel()
    {
        // Ensure game is in a valid state
            if (AmongUsClient.Instance == null || AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started)
            {
                // Don't log warning repeatedly to avoid spam
                return;
            }

            // Check if player data is available
            if (DataManager.Player == null || DataManager.Player.Stats == null)
            {
                return;
            }
        // Parse Spoofing.Level config entry and turn it into a uint
        if (!string.IsNullOrEmpty(ZenithX.spoofLevel.Value) && 
            uint.TryParse(ZenithX.spoofLevel.Value, out parsedLevel) &&
            parsedLevel != DataManager.Player.Stats.Level)
        {

            // Temporarily save the spoofed level using DataManager
            DataManager.Player.Stats.Level = parsedLevel - 1;
            DataManager.Player.Save();
        }
    }

    public static string spoofFriendCode()
    {
        string friendCode = ZenithX.guestFriendCode.Value;
        if (string.IsNullOrWhiteSpace(friendCode))
        {
            friendCode = DestroyableSingleton<AccountManager>.Instance.GetRandomName();
        }
        return friendCode;
    }

    public static void spoofPlatform(PlatformSpecificData platformSpecificData)
    {
        if (platformSpecificData == null) return; // prevent NullReferenceException

        Platforms? platformType;

        // Parse Spoofing.Platform config entry and save it as the spoofed platform type
        if (Utils.stringToPlatformType(ZenithX.spoofPlatform.Value, out platformType))
        {
            platformSpecificData.Platform = (Platforms)platformType;
        }
    }
}