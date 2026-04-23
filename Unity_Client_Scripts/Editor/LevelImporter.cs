using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

public class LevelImporter : EditorWindow
{
    // Ez a menüpont jelenik meg felül a Unity menüben
    [MenuItem("Tools/Import Levels from JSON")]
    public static void ImportLevels()
    {
        // 1. A JSON file elérési útja (Projekt gyökérmappához képest, vagy Assets-en belül)
        string path = "Assets/Resources/levels.json"; // Ide tedd a JSON file-t!

        if (!File.Exists(path))
        {
            UnityEngine.Debug.LogError("Nem található a JSON file itt: " + path);
            return;
        }

        // 2. Beolvassuk a szöveget
        string jsonContent = File.ReadAllText(path);

        // 3. Deszerializáljuk (átalakítjuk C# objektummá)
        LevelListWrapper levelList = JsonUtility.FromJson<LevelListWrapper>(jsonContent);

        if (levelList == null || levelList.levels == null)
        {
            UnityEngine.Debug.LogError("Hiba a JSON feldolgozásakor. Ellenõrizd a formátumot!");
            return;
        }

        // 4. Mappa ellenõrzése, ahova menteni fogunk
        string savePath = "Assets/Resources/Levels";
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        // 5. Végigmegyünk a szinteken és létrehozzuk az Asseteket
        foreach (var rawLevel in levelList.levels)
        {
            CreateLevelAsset(rawLevel, savePath);
        }

        // Frissítjük az Editort, hogy látszódjanak az új file-ok
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        UnityEngine.Debug.Log($"Sikeres importálás! {levelList.levels.Count} szint létrehozva.");
    }

    private static void CreateLevelAsset(LevelJsonData data, string folderPath)
    {
        // Létrehozunk egy új LevelData példányt a memóriában
        LevelData asset = ScriptableObject.CreateInstance<LevelData>();

        // --- ADATOK MÁSOLÁSA ---
        // Itt töltjük fel az adatokat a JSON-ból az Asset-be.
        // Fontos: A LevelData változóneveinek egyeznie kell az itt írtakkal!

        asset.width = data.width;
        asset.height = data.height;

        asset.startPosA = data.startPosA;
        asset.startPosB = data.startPosB;

        // Tömbök másolása
        asset.mazeLayoutA = data.mazeLayoutA;
        // Ha a LevelData-ban List<int> van, akkor: new List<int>(data.mazeLayoutA);

        asset.mazeLayoutB = data.mazeLayoutB;

        asset.finishPosA = data.finishPosA;
        asset.finishPosB = data.finishPosB;

        // --- MENTÉS FILE-KÉNT ---
        string assetPath = $"{folderPath}/{data.levelName}.asset";

        // Ha már létezik ilyen nevû, töröljük vagy felülírjuk? 
        // Az AssetDatabase.CreateAsset hibát dobhat, ha már létezik, ezért érdemes ellenõrizni.
        LevelData existing = AssetDatabase.LoadAssetAtPath<LevelData>(assetPath);
        if (existing != null)
        {
            // Opcionális: Ha létezik, inkább felülírjuk az értékeit (EditorUtility.CopySerialized)
            // De most egyszerûség kedvéért töröljük és újat csinálunk, vagy felülírod manuálisan.
            AssetDatabase.DeleteAsset(assetPath);
        }

        AssetDatabase.CreateAsset(asset, assetPath);
    }
}

// --- SEGÉDOSZTÁLYOK A JSON BEOLVASÁSHOZ ---

[System.Serializable]
public class LevelListWrapper
{
    public List<LevelJsonData> levels;
}

[System.Serializable]
public class LevelJsonData
{
    public string levelName; // Hogy tudjuk, mi legyen a file neve
    public int width;
    public int height;
    public Vector2Int startPosA;
    public Vector2Int startPosB;
    public List<int> mazeLayoutA; // Vagy List<int>
    public List<int> mazeLayoutB;
    public Vector2Int finishPosA;
    public Vector2Int finishPosB;
}