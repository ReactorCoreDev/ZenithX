using AmongUs.GameOptions;
using HarmonyLib;
using System.Collections.Generic;

namespace ZenithX;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
public static class PlayerControl_FixedUpdate
{
    public static void Postfix(PlayerControl __instance)
    {
        if (__instance.AmOwner)
        {
            ZenithXCheats.noKillCdCheat(__instance);
        }
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdCheckMurder))]
public static class PlayerControl_CmdCheckMurder
{
    public static bool Prefix(PlayerControl __instance, PlayerControl target)
    {
        if (Utils.isLobby)
        {
            HudManager.Instance.Notifier.AddDisconnectMessage("Killing in lobby disabled for being too buggy");
            return false;
        }

        if (CheatToggles.killAnyone || CheatToggles.zeroKillCd || Utils.isVanished(__instance.Data) || Utils.isMeeting || 
            (ZenithXPPMCheats.oldRole != null && !Utils.getBehaviourByRoleType((RoleTypes)ZenithXPPMCheats.oldRole).IsImpostor))
        {
            if (!__instance.Data.Role.IsValidTarget(target.Data))
                return true;

            if (target.protectedByGuardianId > -1 && !CheatToggles.killAnyone)
                return true;

            Utils.murderPlayer(target, MurderResultFlags.Succeeded);
            return false;
        }

        return true;
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.TurnOnProtection))]
public static class PlayerControl_TurnOnProtection
{
    public static void Prefix(ref bool visible)
    {
        if (CheatToggles.seeGhosts)
            visible = true;
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdCheckShapeshift))]
public static class PlayerControl_CmdCheckShapeshift
{
    public static void Prefix(ref bool shouldAnimate)
    {
        if (shouldAnimate && CheatToggles.noShapeshiftAnim)
            shouldAnimate = false;
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdCheckRevertShapeshift))]
public static class PlayerControl_CmdCheckRevertShapeshift
{
    public static void Prefix(ref bool shouldAnimate)
    {
        if (shouldAnimate && CheatToggles.noShapeshiftAnim)
            shouldAnimate = false;
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSyncSettings))]
public static class PlayerControl_RpcSyncSettings
{
    public static bool Prefix()
    {
        return !CheatToggles.noOptionsLimits;
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
public static class PlayerControl_MurderPlayer_Patch
{
    private static readonly HashSet<byte> notifiedKilledVictims = new();

    public static void ClearNotifiedKilledVictims() => notifiedKilledVictims.Clear();

    public static void Prefix(PlayerControl __instance, PlayerControl target)
    {
        if (target == null) return;

        if (target.protectedByGuardianId != -1)
        {
            NotificationHandler.HandleGuardianAngelSave(__instance, target);
            return;
        }

        if (notifiedKilledVictims.Contains(target.PlayerId))
            return;

        NotificationHandler.HandlePlayerKill(__instance, target);
        notifiedKilledVictims.Add(target.PlayerId);
    }
}