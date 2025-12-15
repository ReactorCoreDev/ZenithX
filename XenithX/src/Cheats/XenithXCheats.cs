using Sentry.Internal.Extensions;
using UnityEngine;
using HarmonyLib;
using System.Reflection;
using Il2CppSystem;
using UnityEngine.SceneManagement;
using System;
using Hazel;
using BepInEx.Unity.IL2CPP.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Threading.Tasks;
using InnerNet;
using PowerTools;
using AmongUs.GameOptions;
using Il2CppSystem.Collections.Generic;
using AmongUs.InnerNet.GameDataMessages;
using System.Runtime.CompilerServices;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Sentry.Internal;
using TMPro;

namespace ZenithX;

public static class ZenithXCheats
{
    private static float lastKillTime = 0f;
    public static bool autoKillNearbyEnabled = false;
    public static System.Collections.Generic.List<PlayerControl> autoKillNearbyKilledList = new System.Collections.Generic.List<PlayerControl>();
    public static void closeMeetingCheat()
    {
        if (CheatToggles.closeMeeting)
        {

            if (MeetingHud.Instance)
            { // Closes MeetingHud window if it's open

                // Destroy MeetingHud window gameobject
                AccessHelper.SetField(MeetingHud.Instance, "DespawnOnDestroy", false);
                UnityEngine.Object.Destroy(MeetingHud.Instance.gameObject);

                // Gameplay must be reenabled
                DestroyableSingleton<HudManager>.Instance.StartCoroutine((System.Collections.IEnumerator)DestroyableSingleton<HudManager>.Instance.CoFadeFullScreen(Color.black, Color.clear, 0.2f, false));
                PlayerControl.LocalPlayer.SetKillTimer(GameManager.Instance.LogicOptions.GetKillCooldown());
                ShipStatus.Instance.EmergencyCooldown = GameManager.Instance.LogicOptions.GetEmergencyCooldown();
                Camera.main.GetComponent<FollowerCamera>().Locked = false;
                DestroyableSingleton<HudManager>.Instance.SetHudActive(true);
                ControllerManager.Instance.CloseAndResetAll();

            }
            else if (ExileController.Instance != null)
            { // Ends exile cutscene if it's playing
                AccessHelper.Void(ExileController.Instance, "ReEnableGameplay");
                AccessHelper.Void(ExileController.Instance, "WrapUp");
            }

            CheatToggles.closeMeeting = false; // Button behaviour
        }
    }

    public static void skipMeetingCheat()
    {
        if (Utils.isMeeting)
        {
            MeetingHud.Instance.RpcVotingComplete(new Il2CppStructArray<MeetingHud.VoterState>(0L), null, true);
        }
    }

    public static void forceStartGameCheat()
    {
        if (Utils.isHost && Utils.isLobby)
        {
            AccessHelper.Void(AmongUsClient.Instance, "SendStartGame");
        }
    }

    public static void noKillCdCheat(PlayerControl playerControl)
    {
        if (CheatToggles.zeroKillCd && playerControl.killTimer > 0f)
        {
            playerControl.SetKillTimer(0f);
        }
    }

    public static void completeMyTasksCheat()
    {
        if (CheatToggles.completeMyTasks)
        {
            Utils.completeMyTasks();

            CheatToggles.completeMyTasks = false;
        }
    }

    public static void engineerCheats(EngineerRole engineerRole)
    {
        if (CheatToggles.endlessVentTime)
        {

            // Makes vent time so incredibly long (float.MaxValue) so that it never ends
            engineerRole.inVentTimeRemaining = float.MaxValue;

            // Vent time is reset to normal value after the cheat is disabled
        }
        else if (engineerRole.inVentTimeRemaining > engineerRole.GetCooldown())
        {

            engineerRole.inVentTimeRemaining = engineerRole.GetCooldown();

        }

        if (CheatToggles.noVentCooldown)
        {

            if (engineerRole.cooldownSecondsRemaining > 0f)
            {

                engineerRole.cooldownSecondsRemaining = 0f;
                AccessHelper.Void(DestroyableSingleton<HudManager>.Instance.AbilityButton, "ResetCoolDown");
                DestroyableSingleton<HudManager>.Instance.AbilityButton.SetCooldownFill(0f);

            }

        }
    }

    public static void shapeshifterCheats(ShapeshifterRole shapeshifterRole)
    {
        if (CheatToggles.endlessSsDuration)
        {

            // Makes shapeshift duration so incredibly long (float.MaxValue) so that it never ends
            shapeshifterRole.durationSecondsRemaining = float.MaxValue;

            // Shapeshift duration is reset to normal value after the cheat is disabled
        }
        else if (shapeshifterRole.durationSecondsRemaining > 5f)
        {
            shapeshifterRole.durationSecondsRemaining = 5f;
        }
    }

    public static void scientistCheats(ScientistRole scientistRole)
    {
        if (CheatToggles.noVitalsCooldown)
        {

            scientistRole.currentCooldown = 0f;
        }

        if (CheatToggles.endlessBattery)
        {

            // Makes vitals battery so incredibly long (float.MaxValue) so that it never ends
            scientistRole.currentCharge = float.MaxValue;

            // Battery charge is reset to normal value after the cheat is disabled
        }
        else if (scientistRole.currentCharge > scientistRole.RoleCooldownValue)
        {

            scientistRole.currentCharge = scientistRole.RoleCooldownValue;

        }
    }

    public static void trackerCheats(TrackerRole trackerRole)
    {
        if (CheatToggles.noTrackingCooldown)
        {

            trackerRole.cooldownSecondsRemaining = 0f;
            trackerRole.delaySecondsRemaining = 0f;

            AccessHelper.Void(DestroyableSingleton<HudManager>.Instance.AbilityButton, "ResetCoolDown");
            DestroyableSingleton<HudManager>.Instance.AbilityButton.SetCooldownFill(0f);
        }

        if (CheatToggles.noTrackingDelay)
        {
            MapBehaviour.Instance.trackedPointDelayTime = 5f;
        }

        if (CheatToggles.endlessTracking)
        {

            // Makes vitals battery so incredibly long (float.MaxValue) so that it never ends
            trackerRole.durationSecondsRemaining = float.MaxValue;

            // Battery charge is reset to normal value after the cheat is disabled
        }

        else if (trackerRole.durationSecondsRemaining > 5f)
        {

            trackerRole.durationSecondsRemaining = 5f;
        }
    }
    public static void phantomCheats(PhantomRole phantomRole)
    {
        return;
    }

    public static void useVentCheat(HudManager hudManager)
    {
        // try-catch to prevent errors when role is null
        try
        {

            // Engineers & Impostors don't need this cheat so it is disabled for them
            // Ghost venting causes issues so it is also disabled

            if (!PlayerControl.LocalPlayer.Data.Role.CanVent && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                hudManager.ImpostorVentButton.gameObject.SetActive(CheatToggles.useVents);
            }

        }
        catch { }
    }

    public static void fungleSabotageCheat(FungleShipStatus shipStatus)
    {
        var currentMapID = Utils.getCurrentMapID();

        ZenithXSabotageSystem.HandleSpores(shipStatus, currentMapID);
    }

    public static void sabotageCheat(ShipStatus shipStatus)
    {
        var currentMapID = Utils.getCurrentMapID();

        // Handle all sabotage systems
        ZenithXSabotageSystem.HandleReactor(shipStatus, currentMapID);
        ZenithXSabotageSystem.HandleOxygen(shipStatus, currentMapID);
        ZenithXSabotageSystem.HandleComms(shipStatus, currentMapID);
        ZenithXSabotageSystem.HandleElectrical(shipStatus, currentMapID);
        ZenithXSabotageSystem.HandleMushMix(shipStatus, currentMapID);
        ZenithXSabotageSystem.HandleDoors(shipStatus);
    }
    public static void walkInVentCheat()
    {
        try
        {

            if (CheatToggles.walkVent)
            {
                PlayerControl.LocalPlayer.inVent = false;
                PlayerControl.LocalPlayer.moveable = true;
            }

        }
        catch { }
    }

    public static void kickVentsCheat()
    {
        if (CheatToggles.kickVents)
        {

            foreach (var vent in ShipStatus.Instance.AllVents)
            {

                VentilationSystem.Update(VentilationSystem.Operation.BootImpostors, vent.Id);

            }

            CheatToggles.kickVents = false; // Button behaviour
        }
    }

    public static void killAllCheat()
    {

            if (Utils.isLobby)
            {

                HudManager.Instance.Notifier.AddDisconnectMessage("Killing in lobby disabled for being too buggy");

            }
            else
            {

                // Kill all players by sending a successful MurderPlayer RPC call
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    Utils.murderPlayer(player, MurderResultFlags.Succeeded);
                }

            }
    }

    public static void killAllCrewCheat()
    {
            if (Utils.isLobby)
            {

                HudManager.Instance.Notifier.AddDisconnectMessage("Killing in lobby disabled for being too buggy");

            }
            else
            {

                // Kill all players by sending a successful MurderPlayer RPC call
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Data.Role.TeamType == RoleTeamTypes.Crewmate)
                    {
                        Utils.murderPlayer(player, MurderResultFlags.Succeeded);
                    }
                }

            }
    }

    public static void killAllImpsCheat()
    {
        if (Utils.isLobby)
        {
            HudManager.Instance.Notifier.AddDisconnectMessage("Killing in lobby is too buggy");
        }
        else
        {
            // Kill all players by sending a successful MurderPlayer RPC call
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Data.Role.TeamType == RoleTeamTypes.Impostor)
                {
                    Utils.murderPlayer(player, MurderResultFlags.Succeeded);
                }
            }
        }
    }

    static int ctrlRightClickDelay = 0;

    static int GetFps()
    {
        return (int)Mathf.Round(1f / Time.deltaTime);
    }

    public static void teleportCursorCheat()
    {
        if (CheatToggles.teleportCursor)
        {
Vector3 mouse = Input.mousePosition;
        Vector2 target = new Vector2(mouse.x, mouse.y);

        bool isValid = target.x != 0f && target.y != 0f;

        // Right-click (single TP)
        if (isValid && Input.GetMouseButtonDown(1))
        {
            Vector2 world = Camera.main.ScreenToWorldPoint((target));
            
            PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(world);
        }

        // CTRL + Hold Right-click (continuous TP with delay)
        else if (isValid && Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButton(1))
        {
            if (ctrlRightClickDelay <= 0)
            {
                Vector3 mouse2 = Input.mousePosition;
                Vector2 target2 = new Vector2(mouse2.x, mouse2.y);
                Vector2 world = Camera.main.ScreenToWorldPoint(target2);
                
                PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(world);

                ctrlRightClickDelay = (int)(0.1f * GetFps());
            }
            else ctrlRightClickDelay--;
        }
        }
    }

    public static void noClipCheat()
    {
        try
        {
            PlayerControl.LocalPlayer.Collider.enabled = !(CheatToggles.noClip || PlayerControl.LocalPlayer.onLadder);
        }
        catch { }
    }

    public static void speedBoostCheat()
    {
        float speed = 2f;
        float ghostSpeed = 2.5f;

        float speedMultiplier = 2.0f;

        try
        {
            // If the speedBoost cheat is enabled, the default speed is multiplied by the speed multiplier
            // Otherwise the default speed is used by itself

            float newSpeed = CheatToggles.speedBoost ? speed * speedMultiplier : speed;

            float newGhostSpeed = CheatToggles.speedBoost ? ghostSpeed * speedMultiplier : ghostSpeed;

            PlayerControl.LocalPlayer.MyPhysics.Speed = newSpeed;
            PlayerControl.LocalPlayer.MyPhysics.GhostSpeed = newGhostSpeed;
        }
        catch {}
    }

    public static void reviveCheat()
    {
        if (!CheatToggles.revive)
        {
            return;
        }
        Utils.revivePlayer(PlayerControl.LocalPlayer);
        CheatToggles.revive = false;
    }

    public static void noAbilityCDCheat()
    {
        if (!CheatToggles.noAbilityCD) return;

        var localPlayer = PlayerControl.LocalPlayer;
        if (localPlayer == null || GameData.Instance == null) return;
        if (AmongUsClient.Instance == null || (!AmongUsClient.Instance.IsGameStarted && !Utils.isLobby)) return;

        var roleBehaviour = localPlayer.Data?.Role;
        var roleType = roleBehaviour?.Role ?? AmongUs.GameOptions.RoleTypes.Crewmate;
        var options = GameOptionsManager.Instance?.CurrentGameOptions;
        if (options == null) return;

        if (AmongUs.GameOptions.GameModes.HideNSeek == GameOptionsManager.Instance.CurrentGameOptions.GameMode)
        {
            options.SetFloat(AmongUs.GameOptions.FloatOptionNames.KillCooldown, 0.0042f);
        }
        else
        {
            switch (roleType)
            {
                case AmongUs.GameOptions.RoleTypes.Engineer:
                    if (roleBehaviour is EngineerRole)
                    {
                        options.SetFloat(AmongUs.GameOptions.FloatOptionNames.EngineerCooldown, 0.01f);
                        options.SetFloat(AmongUs.GameOptions.FloatOptionNames.EngineerInVentMaxTime, 30f);
                    }
                    break;

                case AmongUs.GameOptions.RoleTypes.Scientist:
                    if (roleBehaviour is ScientistRole)
                    {
                        options.SetFloat(AmongUs.GameOptions.FloatOptionNames.ScientistCooldown, 0.01f);
                        options.SetFloat(AmongUs.GameOptions.FloatOptionNames.ScientistBatteryCharge, int.MaxValue);
                    }
                    break;

                case AmongUs.GameOptions.RoleTypes.Tracker:
                    if (roleBehaviour is TrackerRole)
                    {
                        options.SetFloat(AmongUs.GameOptions.FloatOptionNames.TrackerCooldown, 0.01f);
                        options.SetFloat(AmongUs.GameOptions.FloatOptionNames.TrackerDelay, 0.01f);
                        options.SetFloat(AmongUs.GameOptions.FloatOptionNames.TrackerDuration, int.MaxValue);
                    }
                    break;

                case AmongUs.GameOptions.RoleTypes.Noisemaker:
                    if (roleBehaviour is NoisemakerRole)
                    {
                        options.SetFloat(AmongUs.GameOptions.FloatOptionNames.NoisemakerAlertDuration, 100f);
                    }
                    break;

                case AmongUs.GameOptions.RoleTypes.Detective:
                    if (roleBehaviour is DetectiveRole)
                        options.SetFloat(AmongUs.GameOptions.FloatOptionNames.DetectiveSuspectLimit, 15f);
                    break;

                case AmongUs.GameOptions.RoleTypes.GuardianAngel:
                    if (roleBehaviour is GuardianAngelRole)
                        options.SetFloat(AmongUs.GameOptions.FloatOptionNames.GuardianAngelCooldown, 0.01f);
                    break;

                case AmongUs.GameOptions.RoleTypes.Shapeshifter:
                    if (roleBehaviour is ShapeshifterRole)
                        options.SetFloat(AmongUs.GameOptions.FloatOptionNames.KillCooldown, 0.0042f);
                        localPlayer.SetKillTimer(0f);
                        options.SetFloat(AmongUs.GameOptions.FloatOptionNames.ShapeshifterCooldown, 0.01f);
                        options.SetFloat(AmongUs.GameOptions.FloatOptionNames.ShapeshifterDuration, int.MaxValue);
                    break;

                case AmongUs.GameOptions.RoleTypes.Phantom:
                    if (roleBehaviour is PhantomRole)
                        options.SetFloat(AmongUs.GameOptions.FloatOptionNames.KillCooldown, 0.0042f);
                        localPlayer.SetKillTimer(0f);
                        options.SetFloat(AmongUs.GameOptions.FloatOptionNames.PhantomDuration, int.MaxValue);
                        options.SetFloat(AmongUs.GameOptions.FloatOptionNames.PhantomCooldown, 0.01f);
                    break;

                case AmongUs.GameOptions.RoleTypes.Viper:
                    if (roleBehaviour is ViperRole)
                        options.SetFloat(AmongUs.GameOptions.FloatOptionNames.KillCooldown, 0.0042f);
                        localPlayer.SetKillTimer(0f);
                        options.SetFloat(AmongUs.GameOptions.FloatOptionNames.ViperDissolveTime, 0.01f);
                    break;
            }

            // Global kill / ability cooldowns
            AccessHelper.Void(DestroyableSingleton<HudManager>.Instance.AbilityButton, "ResetCoolDown");

            if (localPlayer.RemainingEmergencies < int.MaxValue)
                localPlayer.RemainingEmergencies = int.MaxValue;
        }
    }

    public static void murderAllCheat()
    {
        if (CheatToggles.murderAll)
        {
            // Kill all players by sending a successful MurderPlayer RPC call
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!player == PlayerControl.LocalPlayer)
                {
                    Utils.murderPlayer(player, MurderResultFlags.Succeeded);
                }
            }

            CheatToggles.murderAll = false;
        }
    }

    public static void panicMode() // panic mode function
    {
        CheatToggles.panicMode();
        ModManager.Instance.ModStamp.enabled = false;
    }

    private static bool _hasUsedScanCheatBefore;

    private static void ForceSetScanner(PlayerControl player, bool toggle)
    {
        var count = ++player.scannerCount;
        player.SetScanner(toggle, count);
        RpcSetScannerMessage rpcMessage = new(player.NetId, toggle, count);
        AmongUsClient.Instance.LateBroadcastReliableMessage(Unsafe.As<IGameDataMessage>(rpcMessage));
    }

public static void ScanCheat()
    {
        if (CheatToggles.animScan && !_hasUsedScanCheatBefore)
        {
            ForceSetScanner(PlayerControl.LocalPlayer, true);
            _hasUsedScanCheatBefore = true;
        }
        else if (!CheatToggles.animScan && _hasUsedScanCheatBefore)
        {
            ForceSetScanner(PlayerControl.LocalPlayer, false);
            _hasUsedScanCheatBefore = false;
        }
    }

    private static void ForcePlayAnimation(byte animationType)
    {
        // PlayerControl.LocalPlayer.RpcPlayAnimation(1); wouldn't work if visual tasks are turned off
        // The below way makes sure it works regardless of visual task settings

        PlayerControl.LocalPlayer.PlayAnimation(animationType);
        RpcPlayAnimationMessage rpcMessage = new(PlayerControl.LocalPlayer.NetId, animationType);
        AmongUsClient.Instance.LateBroadcastUnreliableMessage(Unsafe.As<IGameDataMessage>(rpcMessage));
    }

    private static bool _hasUsedCamsCheatBefore;

    public static void AnimationCheat()
    {
        var map = (MapNames)Utils.getCurrentMapID();

        if (CheatToggles.animShields)
        {
            if (map is MapNames.Skeld or MapNames.Dleks)
            {
                ForcePlayAnimation((byte)TaskTypes.PrimeShields);
            }
            CheatToggles.animShields = false;
        }
        if (CheatToggles.animAsteroids)
        {
            if (map is MapNames.Skeld or MapNames.Dleks or MapNames.Polus)
            {
                ForcePlayAnimation((byte)TaskTypes.ClearAsteroids);
            }
            else
            {
                CheatToggles.animAsteroids = false;
            }
        }
        if (CheatToggles.animEmptyGarbage)
        {
            if (map is MapNames.Skeld or MapNames.Dleks)
            {
                ForcePlayAnimation((byte)TaskTypes.EmptyGarbage);
            }
            CheatToggles.animEmptyGarbage = false;
        }

        if (CheatToggles.animCamsInUse && !_hasUsedCamsCheatBefore)
        {
            // There is no cameras on Mira HQ and Fungle
            if (map is MapNames.MiraHQ or MapNames.Fungle)
            {
                CheatToggles.animCamsInUse = false;
            }
            else
            {
                // ShipStatus.Instance.UpdateSystem(SystemTypes.Security, PlayerControl.LocalPlayer, (byte)(CheatToggles.animCamsInUse ? 1 : 0));
                ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Security, 1);
                _hasUsedCamsCheatBefore = true;
            }
        }
        else if (!CheatToggles.animCamsInUse && _hasUsedCamsCheatBefore)
        {
            // Turn off cams if the cheat was used before and is now disabled
            ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Security, 0);
            _hasUsedCamsCheatBefore = false;
        }

    }

public static void imposterAutoKillNearbyCheat()
{
    if (!CheatToggles.autoKillNearby)
    {
        autoKillNearbyEnabled = false;
        return;
    }

    float autoKillDelay = 1f; // 1-second delay
    if (Time.time - lastKillTime < autoKillDelay) return;

    autoKillNearbyEnabled = true;
    float radius = 1.5f;

    PlayerControl closestPlayer = null;
    float minDistance = float.MaxValue;

    foreach (var player in PlayerControl.AllPlayerControls)
    {
        if (player != null && player != PlayerControl.LocalPlayer && !player.Data.IsDead &&
            player.Data.Role.TeamType == RoleTeamTypes.Crewmate &&
            PlayerControl.LocalPlayer.Data.Role.TeamType == RoleTeamTypes.Impostor)
        {
            float distance = Vector2.Distance(player.GetTruePosition(), PlayerControl.LocalPlayer.GetTruePosition());
            if (distance <= radius && distance < minDistance)
            {
                minDistance = distance;
                closestPlayer = player;
            }
        }
    }

   if (closestPlayer != null && !autoKillNearbyKilledList.Any<PlayerControl>(p => p != null && p.Data.PlayerId == closestPlayer.Data.PlayerId))
    {
        autoKillNearbyKilledList.Add(closestPlayer);
        Utils.murderPlayer(closestPlayer, MurderResultFlags.Succeeded);
        lastKillTime = Time.time;
        autoKillNearbyKilledList.Remove(closestPlayer);
        
        //CoroutineManager.CallDelayed(5f, () =>
        //{
            
        //});
    }
}

    public static void callMeeting()
    {
        var localPlayer = PlayerControl.LocalPlayer;
        if (localPlayer == null || GameData.Instance == null || AmongUsClient.Instance == null)
            return;

        // Only run if in an active game, not in lobby
        if (!AmongUsClient.Instance.IsGameStarted)
            return;

        var playerInfo = GameData.Instance.GetPlayerById(localPlayer.PlayerId);
        if (playerInfo == null)
            return;

        // Only call if alive and not currently in a meeting
        if (!localPlayer.Data.IsDead && !MeetingHud.Instance)
        {
            localPlayer.RpcStartMeeting(playerInfo);
        }
        // try{PlayerControl.LocalPlayer.CmdReportDeadBody(Meetings_DiePostfix.deadPlayer.Data);}catch{}
    }

public static void fakeTrashCheat()
{
    if (PlayerControl.LocalPlayer == null ||
        AmongUsClient.Instance == null ||
        !AmongUsClient.Instance.IsGameStarted ||
        ShipStatus.Instance == null)
        return;

    ShipStatus ship = ShipStatus.Instance;
    Transform hatchTransform = ship.transform.Find("HullItems/hatch0001");

    if (hatchTransform == null)
    {
        foreach (Transform child in ship.transform)
        {
            if ((child.name.Contains("hatch") || child.name.Contains("Hatch")) &&
                child.GetComponentInChildren<ParticleSystem>() != null)
            {
                hatchTransform = child;
                break;
            }
        }
    }

    if (hatchTransform == null) return;
    GameObject hatchObj = hatchTransform.gameObject;

    SpriteAnim spriteAnim = hatchObj.GetComponent<SpriteAnim>();
    if (spriteAnim != null)
    {
        spriteAnim.Paused = false;
        spriteAnim.Play(null, 1f);
    }

    ParticleSystem particles = hatchObj.GetComponentInChildren<ParticleSystem>();
    if (particles != null)
        particles.Emit(20);

    InnerNetObject netObj = hatchObj.GetComponent<InnerNetObject>();
    if (netObj == null) return;

    uint netId = netObj.NetId;

    foreach (var p in PlayerControl.AllPlayerControls)
    {
        var writer = AmongUsClient.Instance.StartRpcImmediately(
            PlayerControl.LocalPlayer.NetId,
            (byte)RpcCalls.PlayAnimation,
            SendOption.Reliable,
            p.OwnerId);

        writer.Write(netId);
        writer.Write((byte)8);

        AmongUsClient.Instance.FinishRpcImmediately(writer);
    }
}

        private static string GenerateFakeToken(byte playerId = 0)
        {
            long timestamp = System.DateTime.UtcNow.Ticks;
            string gameId = AmongUsClient.Instance?.GameId.ToString() ?? "0";
            string rawData = $"{playerId}-{timestamp}-{gameId}";
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(rawData));
                return System.Convert.ToBase64String(hashBytes);
            }
        }

            /// <summary>
            /// Force a imposter via RPC.
            /// </summary>
            public static void requestImpostorRole()
            {
                if (PlayerControl.LocalPlayer == null)
                {
                    return;
                }
                if (AmongUsClient.Instance == null || !Utils.isHost)
                {
                    HudManager.Instance.Notifier.AddDisconnectMessage("Force Imposter only host can use");
                    return;
                }
                try
                {
var local = PlayerControl.LocalPlayer;
        if (local == null) return;
        if (!Utils.isHost) return;

        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(
            local.NetId,
            (byte)RpcCalls.SetRole,
            SendOption.Reliable,
            -1
        );

        writer.Write(local.PlayerId);
        writer.Write((byte)RoleTypes.Viper);
        writer.Write(GenerateFakeToken(PlayerControl.LocalPlayer.PlayerId));

        AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error in RequestImpostorRole: {e}");
                }
            }

            public static void reviveAny()
            {
                if (!Utils.isHost)
                {
                    HudManager.Instance.Notifier.AddDisconnectMessage("Host required but fake revive possible");
                }
                
                // Player pick menu made for reviving any player by sending a successful revivePlayer RPC call
                PlayerPickMenu.openPlayerPickMenu(Utils.GetAllPlayerData(), (System.Action)(() =>
                {
                    if (PlayerPickMenu.targetPlayerData.Object.Data.IsDead)
                    {
                        Utils.revivePlayer(PlayerPickMenu.targetPlayerData.Object);
                    }
                }));
            }

            public static void reviveAll()
            {
                if (!Utils.isHost)
                {
                    HudManager.Instance.Notifier.AddDisconnectMessage("Host required for revive all");
                    return;
                }

                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    Utils.revivePlayer(player);
                }
            }

    public static void extendLobbyTimer()
    {
        var LobbyBehaviour = new LobbyBehaviour();
        LobbyBehaviour.RpcExtendLobbyTimer();
    }

    public static void assignCleanFilterTask()
    {
        var player = PlayerControl.LocalPlayer;
        if (player == null || !player.AmOwner) return;

        // Prevent duplicates
        foreach (var t in player.myTasks)
        {
            if (t.TaskType == TaskTypes.EmptyChute) return;
        }

        // Clone an existing task to use as a template
        if (player.myTasks.Count == 0) return; 

        PlayerTask template = player.myTasks[0]; // any existing task works
        PlayerTask newTask = UnityEngine.Object.Instantiate(template);
        newTask.TaskType = TaskTypes.EmptyChute;

        // Force Owner using reflection
        var ownerField = typeof(PlayerTask).GetField("Owner", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        ownerField.SetValue(newTask, player.PlayerId);

        // Add locally
        player.myTasks.Add(newTask);



player.AddSystemTask(SystemTypes.Storage);

var tasks = new Il2CppSystem.Collections.Generic.List<NetworkedPlayerInfo.TaskInfo>();

for (byte i = 0; i < player.myTasks.Count; i++)
{
    var t = player.myTasks[i];

    var info = new NetworkedPlayerInfo.TaskInfo
    {
        Id = i,
        TypeId = (byte)t.TaskType,
        Complete = t.IsComplete
    };

    tasks.Add(info);
}

player.SetTasks(tasks);

    }

public static void Memeify(string text1, string text2)
{
        GameObject mainCamera = Camera.main.gameObject;

        Transform hud = mainCamera.transform.Find("Hud");
        if (hud == null) return;

        Transform introCutscene = hud.Find("IntroCutscene");
        if (introCutscene == null) return;

        // Replace "TeamTitle_TMP" and "ImposterText_TMP" with their actual names if different
        var teamTitle = introCutscene.Find("TeamTitle_TMP")?.GetComponent<TMPro.TextMeshProUGUI>();
        var imposterText = introCutscene.Find("ImposterText_TMP")?.GetComponent<TMPro.TextMeshProUGUI>();

        if (teamTitle != null)
            teamTitle.text = text1;

        if (imposterText != null)
            imposterText.text = text2;
    //There are <color=#FF1919FF>0 People</color> who asked
}

    public static void SetCode()
    {
                GameObject mainCamera = Camera.main.gameObject;

  Transform keypadTransform = null;
for (int i = 0; i < mainCamera.transform.childCount; i++)
{
    var child = mainCamera.transform.GetChild(i);
    if (child != null && child.name != null && child.name.StartsWith("KeyPadDisarmMiniGame"))
    {
        keypadTransform = child;
        break;
    }
}

if (keypadTransform == null)
{
    ZenithX.Log("keypadTransform is invalid");
    return;
}

Transform numberTextTransform = null;
for (int i = 0; i < keypadTransform.childCount; i++)
{
    var child = keypadTransform.GetChild(i);
    if (child != null && child.name == "NumberText_TMP")
    {
        numberTextTransform = child;
        break;
    }
}

if (numberTextTransform == null)
{
    ZenithX.Log("numberTextTransform is invalid");
    return;
}

var numberText = numberTextTransform.GetComponent<TextMeshProUGUI>();
if (numberText == null)
{
    ZenithX.Log("numberText is invalid");
    return;
}

ZenithX.Log("Were applying the new text...");

KeypadGame keypadGame = new KeypadGame();

if (int.TryParse(numberText.text, out int number))
{
    keypadGame.number = number;
    ZenithX.Log("Applied!");
}

    }
}