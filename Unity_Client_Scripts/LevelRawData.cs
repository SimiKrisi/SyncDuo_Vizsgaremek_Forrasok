using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelListWrapper
{
    public List<LevelRawData> boards; // A fő tömb a JSON-ben
}

[System.Serializable]
public class LevelRawData
{
    public string levelName; // pl. "Level_05"
    public int width;
    public int height;

    // Koordináták (Beágyazott objektumok a JSON-ben)
    public Vector2IntData startPosA;
    public Vector2IntData startPosB;

    public Vector2IntData finishPosA;
    public Vector2IntData finishPosB;

    // Ellenségek listája (Array of objects a JSON-ben)
    public List<Vector2IntData> enemyPositionsA;
    public List<Vector2IntData> enemyPositionsB;

    // A pálya rajza (Sima tömbök)
    public int[] mazeLayoutA;
    public int[] mazeLayoutB;
}

// Segédosztály az { "x": 1, "y": 1 } formátumhoz
[System.Serializable]
public struct Vector2IntData
{
    public int x;
    public int y;

    // Kényelmi funkció: Átalakítja Unity Vector2Int-té
    public Vector2Int ToVector2Int()
    {
        return new Vector2Int(x, y);
    }
}