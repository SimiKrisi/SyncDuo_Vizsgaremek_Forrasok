using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public static class AchievementRules
{
    public static bool IsConditionMet(int id, UserProfileData profile)
    {
        // Segédváltozók a számításigényesebb lekérdezésekhez (hogy ne számoljuk ki minden case-nél újra)
        int consecutiveDays = -1;
        int totalDeaths = -1;

        switch (id)
        {
            // ==========================================================
            // 1. NAPLÓZÁS SOROZATOK (Login Streak) - ID: 0-8
            // ==========================================================
            case 0: return profile.loginDates.Count >= 1; // First Step (Simán 1 belépés)

            // A többihez kiszámoljuk a sorozatot:
            case 1:  // 3 days
            case 2:  // 5 days
            case 3:  // 7 days
            case 4:  // 14 days
            case 5:  // 21 days
            case 6:  // 28 days
            case 7:  // 35 days
            case 8:  // 42 days
                if (consecutiveDays == -1) consecutiveDays = GetMaxConsecutiveDays(profile.loginDates);

                if (id == 1) return consecutiveDays >= 3;
                if (id == 2) return consecutiveDays >= 5;
                if (id == 3) return consecutiveDays >= 7;
                if (id == 4) return consecutiveDays >= 14;
                if (id == 5) return consecutiveDays >= 21;
                if (id == 6) return consecutiveDays >= 28;
                if (id == 7) return consecutiveDays >= 35;
                if (id == 8) return consecutiveDays >= 42;
                return false;

            // ==========================================================
            // 2. SZINTLÉPÉS (Reach Level X) - ID: 9-51
            // ==========================================================
            case 9: return profile.maxLevelReached >= 2;
            case 10: return profile.maxLevelReached >= 5;
            case 11: return profile.maxLevelReached >= 7;
            case 12: return profile.maxLevelReached >= 10;
            case 13: return profile.maxLevelReached >= 15;
            case 14: return profile.maxLevelReached >= 20;
            case 15: return profile.maxLevelReached >= 25;
            case 16: return profile.maxLevelReached >= 30;
            case 17: return profile.maxLevelReached >= 40;
            case 18: return profile.maxLevelReached >= 50;
            case 19: return profile.maxLevelReached >= 60;
            case 20: return profile.maxLevelReached >= 70;
            case 21: return profile.maxLevelReached >= 80;
            case 22: return profile.maxLevelReached >= 90;
            case 23: return profile.maxLevelReached >= 100;
            case 24: return profile.maxLevelReached >= 150;
            case 25: return profile.maxLevelReached >= 200;
            case 26: return profile.maxLevelReached >= 250;
            case 27: return profile.maxLevelReached >= 300;
            case 28: return profile.maxLevelReached >= 350;
            case 29: return profile.maxLevelReached >= 400;
            case 30: return profile.maxLevelReached >= 450;
            case 31: return profile.maxLevelReached >= 500;
            case 32: return profile.maxLevelReached >= 550;
            case 33: return profile.maxLevelReached >= 600;
            case 34: return profile.maxLevelReached >= 650;
            case 35: return profile.maxLevelReached >= 700;
            case 36: return profile.maxLevelReached >= 750;
            case 37: return profile.maxLevelReached >= 800;
            case 38: return profile.maxLevelReached >= 850;
            case 39: return profile.maxLevelReached >= 900;
            case 40: return profile.maxLevelReached >= 950;
            case 41: return profile.maxLevelReached >= 1000;
            case 42: return profile.maxLevelReached >= 1100;
            case 43: return profile.maxLevelReached >= 1200;
            case 44: return profile.maxLevelReached >= 1300;
            case 45: return profile.maxLevelReached >= 1400;
            case 46: return profile.maxLevelReached >= 1500;
            case 47: return profile.maxLevelReached >= 1600;
            case 48: return profile.maxLevelReached >= 1700;
            case 49: return profile.maxLevelReached >= 1800;
            case 50: return profile.maxLevelReached >= 1900;
            case 51: return profile.maxLevelReached >= 2000;

            // ==========================================================
            // 3. DAILY CHALLENGES (Beat X Daily) - ID: 52-64
            // ==========================================================
            case 52: return profile.dailyChallengesCompleted.Count() >= 1;
            case 53: return profile.dailyChallengesCompleted.Count() >= 3;
            case 54: return profile.dailyChallengesCompleted.Count() >= 5;
            case 55: return profile.dailyChallengesCompleted.Count() >= 7;
            case 56: return profile.dailyChallengesCompleted.Count() >= 10;
            case 57: return profile.dailyChallengesCompleted.Count() >= 15;
            case 58: return profile.dailyChallengesCompleted.Count() >= 20;
            case 59: return profile.dailyChallengesCompleted.Count() >= 25;
            case 60: return profile.dailyChallengesCompleted.Count() >= 30;
            case 61: return profile.dailyChallengesCompleted.Count() >= 40;
            case 62: return profile.dailyChallengesCompleted.Count() >= 50;
            case 63: return profile.dailyChallengesCompleted.Count() >= 75;
            case 64: return profile.dailyChallengesCompleted.Count() >= 100;

            // ==========================================================
            // 4. SPEEDRUNS (1 Minute Challenges) - ID: 65-71
            // ==========================================================
            case 65: return profile.oneMinChallengesCompleted.Count() >= 1;
            case 66: return profile.oneMinChallengesCompleted.Count() >= 3;
            case 67: return profile.oneMinChallengesCompleted.Count() >= 5;
            case 68: return profile.oneMinChallengesCompleted.Count() >= 10;
            case 69: return profile.oneMinChallengesCompleted.Count() >= 20;
            case 70: return profile.oneMinChallengesCompleted.Count() >= 50;
            case 71: return profile.oneMinChallengesCompleted.Count() >= 100;

            // ==========================================================
            // 5. HALÁLOZÁSOK (Dead by Red) - ID: 72-79
            // ==========================================================
            // Megjegyzés: Jelenleg minden halált számolunk (Died).
            // Ha késõbb lesz külön "Red Enemy", akkor a LevelAttempt-be
            // kell majd egy "CauseOfDeath" mezõ, és itt arra kell szûrni.
            case 72:
            case 73:
            case 74:
            case 75:
            case 76:
            case 77:
            case 78:
            case 79:
                if (totalDeaths == -1) totalDeaths = profile.levelHistory.Count(x => x.result == LevelResult.Died);

                if (id == 72) return totalDeaths >= 1;
                if (id == 73) return totalDeaths >= 5;
                if (id == 74) return totalDeaths >= 10;
                if (id == 75) return totalDeaths >= 50;
                if (id == 76) return totalDeaths >= 100;
                if (id == 77) return totalDeaths >= 200;
                if (id == 78) return totalDeaths >= 500;
                if (id == 79) return totalDeaths >= 1000;
                return false;

            default: return false;
        }
    }

    // --- SEGÉDFÜGGVÉNY A NAPOK SZÁMOLÁSÁHOZ (Változatlan) ---
    private static int GetMaxConsecutiveDays(List<string> dateStrings)
    {
        if (dateStrings == null || dateStrings.Count == 0) return 0;

        var sortedDates = dateStrings
            .Select(d => DateTime.Parse(d))
            .OrderBy(d => d)
            .Distinct()
            .ToList();

        if (sortedDates.Count == 0) return 0;

        int maxStreak = 1;
        int currentStreak = 1;

        for (int i = 1; i < sortedDates.Count; i++)
        {
            TimeSpan diff = sortedDates[i] - sortedDates[i - 1];

            if (diff.Days == 1)
            {
                currentStreak++;
            }
            else if (diff.Days > 1)
            {
                if (currentStreak > maxStreak) maxStreak = currentStreak;
                currentStreak = 1;
            }
        }
        if (currentStreak > maxStreak) maxStreak = currentStreak;

        return maxStreak;
    }
}