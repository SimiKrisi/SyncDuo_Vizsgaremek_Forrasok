using UnityEngine;
using System.Collections.Generic;
using System.Linq;


[CreateAssetMenu(fileName = "NewLevelData", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    // A Labirintus mérete
    public int width = 10;
    public int height = 10;

    // A start pozíciók
    public Vector2Int startPosA = new Vector2Int(0, 0);
    public Vector2Int startPosB = new Vector2Int(0, 0);

    // Két labirintus adatai
    // A lista minden eleme egy CellType-ot tárol egy adott pozícióhoz

    [Header("Labirintus A (Felsõ)")]

    public List<int> mazeLayoutA = new List<int>();

    [Header("Labirintus B (Alsó)")]
  
    public List<int> mazeLayoutB = new List<int>();
       
    public Vector2Int finishPosA;
    public Vector2Int finishPosB;
}