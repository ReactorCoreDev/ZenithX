// In PlayerStatsData.cs
using System;
using System.Collections.Generic;
using AmongUs.Data;
using AmongUs.Data.Player;

namespace ZenithX;

public static class PlayerStatsHandler
{
    public static List<string> stats = new List<string>
    {
        "GamesStarted", "GamesFinished", "GamesAsImpostor", "GamesAsCrewmate",
        "BodiesReported", "EmergenciesCalled", "SabotagesFixed", "TasksCompleted",
        "AllTasksCompleted", "CrewmateStreak", "ImpostorKills", "TimesMurdered",
        "TimesEjected", "TimesPettedPet", "HideAndSeek_GamesCrewmateSurvived",
        "HideAndSeek_TimesVented", "HideAndSeek_ImpostorKills", "HideAndSeek_FastestCrewmateWin",
        "HideAndSeek_FastestImpostorWin", "Map_Skeld_Wins", "Map_Polus_Wins",
        "Map_MiraHQ_Wins", "Map_Airship_Wins", "Map_Fungle_Wins", "Role_Crewmate_Wins",
        "Role_Impostor_Wins", "Role_Engineer_Wins", "Role_Engineer_Vents",
        "Role_GuardianAngel_Wins", "Role_GuardianAngel_CrewmatesProtected",
        "Role_Scientist_Wins", "Role_Scientist_ChargesGained", "Role_Shapeshifter_Wins",
        "Role_Shapeshifter_ShiftedKills", "Role_Noisemaker_Wins", "Role_Phantom_Wins",
        "Role_Tracker_Wins", "GameResult_CrewmatesByVote_Wins", "GameResult_CrewmatesByVote_Losses",
        "GameResult_CrewmatesByVote_Draws", "GameResult_CrewmatesByTask_Wins",
        "GameResult_CrewmatesByTask_Losses", "GameResult_CrewmatesByTask_Draws",
        "GameResult_ImpostorsByVote_Wins", "GameResult_ImpostorsByVote_Losses",
        "GameResult_ImpostorsByVote_Draws", "GameResult_ImpostorsByKill_Wins",
        "GameResult_ImpostorsByKill_Losses", "GameResult_ImpostorsByKill_Draws",
        "GameResult_ImpostorsBySabotage_Wins", "GameResult_ImpostorsBySabotage_Losses",
        "GameResult_ImpostorsBySabotage_Draws", "GameResult_ImpostorDisconnect_Wins",
        "GameResult_ImpostorDisconnect_Losses", "GameResult_ImpostorDisconnect_Draws",
        "GameResult_CrewmateDisconnect_Wins", "GameResult_CrewmateDisconnect_Losses",
        "GameResult_CrewmateDisconnect_Draws", "GameResult_HideAndSeek_CrewmatesByTimer_Wins",
        "GameResult_HideAndSeek_CrewmatesByTimer_Losses", "GameResult_HideAndSeek_CrewmatesByTimer_Draws",
        "GameResult_HideAndSeek_ImpostorsByKills_Wins", "GameResult_HideAndSeek_ImpostorsByKills_Losses",
        "GameResult_HideAndSeek_ImpostorsByKills_Draws", "Role_Detective_CrewmatesQuestioned",
        "Role_Detective_Wins"
    };
    public static List<string> formattedList = new List<string>
    {
        "Games Started", "Games Finished", "Games As Impostor", "Games As Crewmate",
        "Bodies Reported", "Emergencies Called", "Sabotages Fixed", "Tasks Completed",
        "All Tasks Completed", "Crewmate Streak", "Impostor Kills", "Times Murdered",
        "Times Ejected", "Times Petted Pet", "Hide And Seek Survived As Crewmate",
        "Hide And Seek Times Vented", "Hide And Seek Impostor Kills", "Hide And Seek Fastest Crewmate Win",
        "Hide And Seek Fastest Impostor Win", "Skeld Wins", "Polus Wins",
        "MiraHQWins", "AirshipWins", "Fungle Wins", "Crewmate Wins",
        "Impostor Wins", "Engineer Wins", "Engineer Vents",
        "Guardian Angel Wins", "Guardian Angel Crewmates Protected",
        "Scientist Wins", "Scientist Charges Gained", "Shapeshifter Wins",
        "Shapeshifter Shifted Kills", "Noisemaker Wins", "Phantom Wins",
        "Tracker Wins", "Crewmates By Vote Wins", "Crewmates By Vote Losses",
        "Crewmates By Vote Draws", "Crewmates By Task Wins",
        "Crewmates By Task Losses", "Crewmates By Task Draws",
        "Impostors By Vote Wins", "Impostors By Vote Losses",
        "Impostors By Vote Draws", "Impostors By Kill Wins",
        "Impostors By Kill Losses", "Impostors By Kill Draws",
        "Impostors By Sabotage Wins", "Impostors By Sabotage Losses",
        "Impostors By Sabotage Draws", "Impostor Disconnect Wins",
        "Impostor Disconnect Losses", "Impostor Disconnect Draws",
        "Crewmate Disconnect Wins", "Crewmate Disconnect Losses",
        "Crewmate Disconnect Draws", "Hide And Seek Crewmates By Timer Wins",
        "Hide And Seek Crewmates By Timer Losses", "Hide And Seek Crewmates By Timer Draws",
        "Hide And Seek Impostors By Kills Wins", "Hide And Seek Impostors By Kills Losses",
        "Hide And Seek Impostors By Kills Draws", "Detective Crewmates Questioned",
        "Detective Wins"
    };

    public static void Increase(string statName)
    {
        if (!stats.Contains(statName))
        {
            ZenithX.Log($"[PlayerStatsHandler] Stat {statName} not found in stats list.");
            return;
        }

        try
        {
            // Convert string statName to StatID (assuming StatID is an enum or similar)
            if (Enum.TryParse<StatID>(statName, out StatID statId))
            {
                // Get the PlayerStatsData instance (assuming it's accessible, e.g., via singleton or injection)
                PlayerStatsData statsData = DataManager.Player.Stats; // Adjust based on actual access pattern

                if (statsData == null)
                {
                    ZenithX.Log("[PlayerStatsHandler] PlayerStatsData instance is null.");
                    return;
                }

                // Get current stat value
                uint currentValue = statsData.GetStat(statId);
                // Increment the stat
                statsData.IncrementStat(statId);
                // Save stats to EOS
                statsData.SaveStats();

                // Log the update (matching the log format from the original question)
                ZenithX.Log($"[PlayerStatsHandler] Stats updated: {statName} [from: {currentValue}] -> [to: {statsData.GetStat(statId)}]");
            }
            else
            {
                ZenithX.Log($"[PlayerStatsHandler] Failed to parse {statName} as StatID.");
            }
        }
        catch (Exception ex)
        {
            ZenithX.Log($"[PlayerStatsHandler] Error incrementing stat {statName}: {ex.Message}");
        }
    }

    public static void Decrease(string statName)
    {
        if (!stats.Contains(statName))
        {
            ZenithX.Log($"[PlayerStatsHandler] Stat {statName} not found in stats list.");
            return;
        }

        try
        {
            // Convert string statName to StatID (assuming StatID is an enum or similar)
            if (Enum.TryParse<StatID>(statName, out StatID statId))
            {
                // Get the PlayerStatsData instance (assuming it's accessible, e.g., via singleton or injection)
                PlayerStatsData statsData = DataManager.Player.Stats; // Adjust based on actual access pattern

                if (statsData == null)
                {
                    ZenithX.Log("[PlayerStatsHandler] PlayerStatsData instance is null.");
                    return;
                }

                // Get current stat value
                uint currentValue = statsData.GetStat(statId);
                // reset the stat by 1 the stat
                statsData.ResetStat(statId);
                // Save stats to EOS
                statsData.SaveStats();

                // Log the update (matching the log format from the original question)
                ZenithX.Log($"[PlayerStatsHandler] Stats updated: {statName} [from: {currentValue}] -> [to: {statsData.GetStat(statId)}]");
            }
            else
            {
                ZenithX.Log($"[PlayerStatsHandler] Failed to parse {statName} as StatID.");
            }
        }
        catch (Exception ex)
        {
            ZenithX.Log($"[PlayerStatsHandler] Error decreasing stat {statName}: {ex.Message}");
        }
    }
}