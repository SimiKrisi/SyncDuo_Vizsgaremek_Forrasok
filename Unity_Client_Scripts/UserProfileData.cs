using System;
using System.Collections.Generic;

// Daily challenge rekord (dátum + idõ)
[System.Serializable]
public class DailyChallengeRecord
{
    public string date; // éééé-hh-nn formátum
    public float completionTime; // másodpercben

    public DailyChallengeRecord() { }

    public DailyChallengeRecord(string date, float time)
    {
        this.date = date;
        this.completionTime = time;
    }
}

// 1 Minute Challenge rekord (dátum + teljesített pályák száma)
[System.Serializable]
public class OneMinChallengeRecord
{
   
    public int levelsCompleted; // hány pályát teljesített

    public OneMinChallengeRecord() { }

    public OneMinChallengeRecord( int levels)
    {

        this.levelsCompleted = levels;
    }
}

[System.Serializable]
public class UserProfileData
{
    public string userId;
    public string userName;
    public int coins;
    public int maxLevelReached;
    public string language = "en";
    public List<string> loginDates = new List<string>();
    public List<LevelAttempt> levelHistory = new List<LevelAttempt>();
    public List<DailyChallengeRecord> dailyChallengesCompleted = new List<DailyChallengeRecord>();
    public List<OneMinChallengeRecord> oneMinChallengesCompleted = new List<OneMinChallengeRecord>();

    /// <summary>
    /// Üres konstruktor a JSON deserializációhoz.
    /// Inicializálja a listákat hogy ne legyenek null-ok.
    /// </summary>
    public UserProfileData()
    {
        loginDates = new List<string>();
        levelHistory = new List<LevelAttempt>();
        dailyChallengesCompleted = new List<DailyChallengeRecord>();
        oneMinChallengesCompleted = new List<OneMinChallengeRecord>();
    }

    /// <summary>
    /// Konstruktor új felhasználóhoz alapértékekkel.
    /// </summary>
    public UserProfileData(string id, string name)
    {
        this.userId = id;
        this.userName = name;

        this.maxLevelReached = 0;
        this.language = "en";
        this.coins = 1000;

        this.loginDates = new List<string>();
        this.levelHistory = new List<LevelAttempt>();
        this.dailyChallengesCompleted = new List<DailyChallengeRecord>();
        this.oneMinChallengesCompleted = new List<OneMinChallengeRecord>();
    }
}