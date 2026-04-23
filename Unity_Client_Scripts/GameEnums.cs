using System;
using System.Collections.Generic;

// --- 1. AZ ÁLLAPOTOK (ENUM) ---
public enum AchievementState
{
    Locked = 0,    // Még nincs meg (Lakat)
    Unlocked = 1,  // Megvan, de még nem vette át a jutalmat (Gomb aktív!)
    Collected = 2  // Átvette a jutalmat (Pipa, inaktív)
}
public enum ShopItemState { 
    Unpurchased =0, // Még nincs meg (Gomb aktív!)
    Purchased=1, // Megvásárolta, de még nem használja (Gomb aktív, más szín)
    Applied=2 // Használja (Gomb inaktív, zöldes szín)
}

// --- 2. PÁLYA EREDMÉNYEK (STATISZTIKA) ---
public enum LevelResult
{
    Died,     // Meghalt
    GiveUp,   // Feladta
    Win       // Nyert
}

[System.Serializable]
public class LevelAttempt
{
    public int levelIndex;      // Melyik pálya?
    public float duration;      // Hány másodpercig tartott?
    public LevelResult result;  // Mi lett a vége?
    public string timestamp;    // Mikor történt? (Dátum)

    public LevelAttempt(int lvl, float time, LevelResult res)
    {
        levelIndex = lvl;
        duration = time;
        result = res;
        timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
