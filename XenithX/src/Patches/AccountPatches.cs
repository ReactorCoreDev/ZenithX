using AmongUs.Data.Player;
using HarmonyLib;
using AmongUs.Data;
using System;

namespace ZenithX;

[HarmonyPatch(typeof(EOSManager), nameof(EOSManager.StartInitialLoginFlow))]
public static class EOSManager_StartInitialLoginFlow
{
    public static bool Prefix(EOSManager __instance)
    {
        if (__instance == null) return true;

        try
        {
            __instance.DeleteDeviceID(new Action(__instance.EndMergeGuestAccountFlow));

            if (!ZenithX.guestMode.Value) return true;

            __instance.StartTempAccountFlow();
            __instance.CloseStartupWaitScreen();

            ZenithX.Log("[ZenithX] Guest login flow started successfully.");
        }
        catch (Exception ex)
        {
            ZenithX.Log($"[ZenithX] EOSManager_StartInitialLoginFlow Prefix error: {ex}");
            return true;
        }

        return false;
    }
}

[HarmonyPatch(typeof(EOSManager))]
public static class EOSManagerFeatureUnlocks
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(EOSManager.IsFreechatAllowed))]
    [HarmonyPatch(nameof(EOSManager.IsFriendsListAllowed))]
    [HarmonyPatch(nameof(EOSManager.IsMinorOrWaiting))]
    public static void UnlockFeatures(ref bool __result)
    {
        if (CheatToggles.unlockFeatures)
        {
            __result = true;
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(EOSManager.IsAllowedOnline))]
    public static void ForceOnline(ref bool canOnline)
    {
        if (CheatToggles.unlockFeatures) canOnline = true;
    }
}

[HarmonyPatch(typeof(FullAccount), nameof(FullAccount.CanSetCustomName))]
public static class FullAccount_CustomName
{
    public static void Prefix(ref bool canSetName)
    {
        if (CheatToggles.unlockFeatures) canSetName = true;
    }
}

[HarmonyPatch(typeof(AccountManager), nameof(AccountManager.CanPlayOnline))]
public static class AccountManager_OnlinePatch
{
    public static void Postfix(ref bool __result)
    {
        if (CheatToggles.unlockFeatures) __result = true;
    }
}

[HarmonyPatch(typeof(InnerNet.InnerNetClient), nameof(InnerNet.InnerNetClient.JoinGame))]
public static class InnerNetClient_JoinGame
{
    public static void Prefix()
    {
        if (CheatToggles.unlockFeatures)
            DataManager.Player.Account.LoginStatus = EOSManager.AccountLoginStatus.LoggedIn;
    }
}

[HarmonyPatch(typeof(PlayerBanData))]
public static class PlayerBanData_Patches
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(PlayerBanData.BanPoints), MethodType.Setter)]
    public static bool ResetBanPoints(PlayerBanData __instance, ref float value)
    {
        if (CheatToggles.avoidBans)
        {
            value = 0f;
            return false;
        }
        return true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(PlayerBanData.IsBanned), MethodType.Getter)]
    public static void DisableBans(ref bool __result)
    {
        if (CheatToggles.avoidBans) __result = false;
    }
}

public enum KWSPermissionStatus__Enum
{
    Granted = 0,
    Rejected = 1,
    Pending = 2
}

[HarmonyPatch(typeof(AccountManager), nameof(AccountManager.UpdateKidAccountDisplay))]
public static class AccountManager_KidAccountPatch
{
    public static bool Prefix(AccountManager __instance)
    {
        if (CheatToggles.unlockFeatures)
        {
            __instance.freeChatAllowed = (Epic.OnlineServices.KWS.KWSPermissionStatus)KWSPermissionStatus__Enum.Granted;
            __instance.customDisplayName = (Epic.OnlineServices.KWS.KWSPermissionStatus)KWSPermissionStatus__Enum.Granted;
            __instance.friendsListAllowed = (Epic.OnlineServices.KWS.KWSPermissionStatus)KWSPermissionStatus__Enum.Granted;
        }
        return false;
    }
}