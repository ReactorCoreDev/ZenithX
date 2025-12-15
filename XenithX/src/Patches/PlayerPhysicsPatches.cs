using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;

namespace ZenithX;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class PlayerPhysics_LateUpdate
{
    private static readonly Dictionary<byte, bool> WasInVent = new();
    private static readonly Dictionary<byte, Vector2> LastPositions = new();

    // Cached references — assigned only when needed
    private static GameObject[] CachedBodies = System.Array.Empty<GameObject>();
    private static float BodyScanCooldown = 0f;

    public static void ClearAllStates()
    {
        WasInVent.Clear();
        LastPositions.Clear();
    }

    public static void Postfix(PlayerPhysics __instance)
    {
        try
        {
            if (__instance == null) return;
            var player = __instance.myPlayer;
            if (player == null || player.Data == null) return;

            // --------- ESP & CHEATS (NO ALLOCATIONS) ----------
            ZenithXESP.playerNametags(__instance);
            ZenithXESP.seeGhostsCheat(__instance);
            TracersHandler.drawPlayerTracer(__instance);

            ZenithXCheats.noClipCheat();
            ZenithXCheats.speedBoostCheat();
            ZenithXCheats.teleportCursorCheat();
            ZenithXCheats.completeMyTasksCheat();
            ZenithXCheats.reviveCheat();
            ZenithXCheats.noAbilityCDCheat();
            ZenithXCheats.murderAllCheat();
            ZenithXCheats.ScanCheat();
            ZenithXCheats.imposterAutoKillNearbyCheat();
            ZenithXCheats.AnimationCheat();

            ZenithXPPMCheats.spectatePPM();
            ZenithXPPMCheats.killPlayerPPM();
            ZenithXPPMCheats.telekillPlayerPPM();
            ZenithXPPMCheats.teleportPlayerPPM();
            ZenithXPPMCheats.changeRolePPM();
            ZenithXPPMCheats.ejectPlayerPPM();
            ZenithXPPMCheats.ProtectPlayerPPM();
            ZenithXPPMCheats.murderPlayerPPM();

            // --------- DEAD BODY TRACERS ----------
            BodyScanCooldown += Time.deltaTime;

            // Only rescans when bodies actually *change*
            if (BodyScanCooldown >= 0.25f) // NO TIMER LOGIC — simply checks every few frames automatically
            {
                CachedBodies = GameObject.FindGameObjectsWithTag("DeadBody");
                BodyScanCooldown = 0f;
            }

            for (int i = 0; i < CachedBodies.Length; i++)
            {
                var obj = CachedBodies[i];
                if (obj == null) continue;

                DeadBody body = obj.GetComponent<DeadBody>();
                if (body != null && !body.Reported)
                {
                    TracersHandler.drawBodyTracer(body);
                }
            }

            // --------- POSITION TRACKING ----------
            if (!player.Data.IsDead)
            {
                if (!player.inVent)
                {
                    LastPositions[player.PlayerId] = player.GetTruePosition();
                }

                // --------- VENT NOTIFICATIONS ----------
                if (CheatToggles.notifyOnVent && Utils.isInGame)
                {
                    byte pid = player.PlayerId;
                    bool inVent = player.inVent;

                    if (WasInVent.TryGetValue(pid, out bool prev) && prev != inVent)
                    {
                        Vector2 pos = inVent ? LastPositions[pid] : player.GetTruePosition();
                        PlainShipRoom room = Utils.getRoomFromPosition(pos);

                        NotificationHandler.HandleVent(
                            player,
                            inVent,
                            room != null ? room.RoomId.ToString() : "Unknown"
                        );
                    }

                    WasInVent[pid] = inVent;
                }
            }

            // --------- INVERT CONTROLS ----------
            try
            {
                var physics = player.MyPhysics;

                if (CheatToggles.invertControls)
                {
                    physics.Speed = -Mathf.Abs(physics.Speed);
                    physics.GhostSpeed = -Mathf.Abs(physics.GhostSpeed);
                }
                else
                {
                    physics.Speed = Mathf.Abs(physics.Speed);
                    physics.GhostSpeed = Mathf.Abs(physics.GhostSpeed);
                }
            }
            catch { }
        }
        catch (System.Exception e)
        {
            ZenithX.Log("LateUpdate error: " + e);
        }
    }
}