using System.Collections.Generic;

[System.Serializable]
public class AchievementData
{
    public int id;
    public string description;
    public int reward;

    // MÓDOSÍTÁS: bool helyett Enum!
    public AchievementState state = AchievementState.Locked;
}

// Mivel a JSON-ben egy "list" nevû tömbben vannak az adatok, kell egy keret (wrapper) osztály:
[System.Serializable]
public class AchievementListWrapper
{
    public System.Collections.Generic.List<AchievementData> list;
}