using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// A pályák generálásáért, kamera beállításáért és az 1 perces kihívás kezeléséért felelős osztály.
/// </summary>
public class LevelGenerator : MonoBehaviour
{
    #region Konstansok

    private const float CellCenterOffset = 0.5f;
    private const float CameraOffsetY = 0.5f;
    private const float CameraZDepth = -10f;
    private const float BoundaryInsetX = 0.5f;
    private const float BoundaryInsetY = 0.5f;
    private const float ChallengeDuration = 60f;
    private const int MinimumCameraSize = 4;
    private const int RandomSelectionMaxAttempts = 100;
    private const int InvalidLevelIndex = -1;

    #endregion

    #region Inspector Mezők

    [Header("JSON Beállítások")]
    public string jsonFileName = "levels";

    [Header("Intro Beállítások")]
    public float introDuration = 2.0f;
    public float introDelay = 1.0f;

    [Header("Tilemaps")]
    public Tilemap mazeATilemap;
    public Tilemap mazeBTilemap;
    public Tilemap finishATilemap;
    public Tilemap finishBTilemap;

    [Header("Tile Asset-ek")]
    public TileBase floorTile;
    public TileBase wallTile;
    public TileBase goalTile;

    [Header("Játék Objektumok")]
    public Transform playerA;
    public Transform playerB;
    public Camera camA;
    public Camera camB;
    public GameObject tutorialPanel;
    public GameObject enemyPrefab;

    #endregion

    #region Privát Mezők - Pálya Adatok

    private LevelListWrapper levelWrapper;
    private LevelRawData currentLevel;

    #endregion

    #region Privát Mezők - Kamera Állapot

    private bool shouldFollowPlayers = false;
    private float currentOrthoSize;
    private bool isIntroPlaying = false;
    private float maxComfortableSize = 12f;

    #endregion

    #region Privát Mezők - 1 Perces Kihívás

    private float remainingTime;
    private int completedLevelsCount = 0;
    private bool isTimerRunning = false;
    private int lastLevelIndex = InvalidLevelIndex;

    #endregion

    #region Unity Életciklus Metódusok

    

    /// <summary>
    /// Inicializálja a pályát, kamerákat és elindítja az 1 perces kihívást ha szükséges.
    /// </summary>
    void Start()
    {
    
        EnsureGameplayTimeRunning();

        int levelIndex = SetGamemode();
        InitializeScreenLimits();
        LoadLevelsFromJson();

        if (levelWrapper != null && levelWrapper.boards != null)
        {
            levelIndex = HandleLevelIndexSelection(levelIndex);
            currentLevel = levelWrapper.boards[levelIndex];
        }

        InitializeManagers();
        GenerateLevel();
        StartChallengeIfNeeded();
        SetupTutorialIfNeeded(levelIndex);
    }

    /// <summary>
    /// Gondoskodik róla, hogy a játékmenet időskálája aktív legyen.
    /// </summary>
    private void EnsureGameplayTimeRunning()
    {
        if (Time.timeScale <= 0f)
        {
            Debug.LogWarning("⏱️ Time.timeScale 0 volt pályabetöltéskor, visszaállítva 1-re.");
            Time.timeScale = 1f;
        }
    }

    /// <summary>
    /// Frissíti az 1 perces kihívás időzítőjét minden frame-ben.
    /// </summary>
    void Update()
    {
        if (isTimerRunning && LevelContext.Gamemode == "1minchallenge")
        {
            UpdateChallengeTimer();
        }
    }

    /// <summary>
    /// Frissíti a kamerák pozícióját a játékosok követéséhez.
    /// </summary>
    void LateUpdate()
    {
        if (ShouldUpdateCameraPositions())
        {
            UpdateCameraPositions();
        }
    }

    #endregion

    #region Inicializálás

    /// <summary>
    /// Beállítja a JSON fájlnevet és visszaadja a kezdő pálya indexét a játékmód alapján.
    /// </summary>
    private int SetGamemode()
    {
        string gamemode = LevelContext.Gamemode;

        if (gamemode == "normal")
        {
            jsonFileName = "levels";
            return LevelContext.CurrentLevelIndex;
        }
        else if (gamemode == "dailychallenge")
        {
            jsonFileName = "dailychallenges";
            return LevelContext.DailyChallengeIndex;
        }
        else if (gamemode == "1minchallenge")
        {
            jsonFileName = "1minchallenges";
            return InvalidLevelIndex; // Jelzi, hogy random választás szükséges
        }

        return 0;
    }

    /// <summary>
    /// Inicializálja a képernyő méretétől függő kamera limiteket.
    /// </summary>
    private void InitializeScreenLimits()
    {
        float referenceHeight = 2400f;
        float referenceSize = 12f;
        float ratio = (float)Screen.height / referenceHeight;
        maxComfortableSize = Mathf.Max(8f, referenceSize * ratio);
    }

    /// <summary>
    /// Kezeli a pálya index kiválasztását (random választás 1minchallenge esetén).
    /// </summary>
    private int HandleLevelIndexSelection(int levelIndex)
    {
        if (levelIndex == InvalidLevelIndex)
        {
            levelIndex = GetRandomLevelIndex(levelWrapper.boards.Count);
            lastLevelIndex = levelIndex;
            Debug.Log($"🎲 1min Challenge - Random pálya kiválasztva: index {levelIndex}");
        }

        if (levelIndex < 0 || levelIndex >= levelWrapper.boards.Count)
        {
            levelIndex = 0;
        }

        return levelIndex;
    }

    /// <summary>
    /// Inicializálja a manager példányokat.
    /// </summary>
    private void InitializeManagers()
    {
        if (FinishManager.Instance != null)
        {
            FinishManager.Instance.ResetFinishState();
        }
    }

    /// <summary>
    /// Elindítja az 1 perces kihívást ha szükséges.
    /// </summary>
    private void StartChallengeIfNeeded()
    {
        if (LevelContext.Gamemode == "1minchallenge")
        {
            remainingTime = ChallengeDuration;
            completedLevelsCount = 0;
            isTimerRunning = true;
            Debug.Log("⏱️ 1 perces kihívás indítva!");
        }
    }

    /// <summary>
    /// Beállítja a tutorialt ha az első pályán vagyunk normal módban.
    /// </summary>
    private void SetupTutorialIfNeeded(int levelIndex)
    {
        if (tutorialPanel == null) return;

        if (levelIndex == 0 && LevelContext.Gamemode == "normal")
        {
            tutorialPanel.SetActive(true);
        }
        else
        {
            tutorialPanel.SetActive(false);
        }
    }

    #endregion

    #region JSON Betöltés

    /// <summary>
    /// Betölti a pályák adatait a megadott JSON fájlból.
    /// </summary>
    private void LoadLevelsFromJson()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(jsonFileName);

        if (jsonFile == null)
        {
            Debug.LogError($"CRITICAL: Nem található a '{jsonFileName}.json' a Resources mappában!");
            return;
        }

        levelWrapper = JsonUtility.FromJson<LevelListWrapper>(jsonFile.text);

        if (levelWrapper == null || levelWrapper.boards == null)
        {
            Debug.LogError("Hiba a JSON parszolása közben.");
        }
    }

    #endregion

    #region Pálya Generálás

    /// <summary>
    /// Generálja az aktuális pályát (tilemap-ek, játékosok, ellenségek, kamerák).
    /// </summary>
    public void GenerateLevel()
    {
        if (currentLevel == null) return;

        Debug.Log($"Generating Level: {currentLevel.levelName} ({currentLevel.width}x{currentLevel.height})");

        ClearTilemaps();
        BuildMazes();
        SpawnAllEnemies();
        PlaceFinishTiles();
        PositionPlayers();
        ReactivatePlayers();
        ConfigureFinishPositions();
        SetupCameras(currentLevel.width, currentLevel.height);
    }

    /// <summary>
    /// Törli az összes tilemap-et és a korábbi ellenségeket.
    /// </summary>
    private void ClearTilemaps()
    {
        mazeATilemap.ClearAllTiles();
        mazeBTilemap.ClearAllTiles();
        finishATilemap.ClearAllTiles();
        finishBTilemap.ClearAllTiles();

        DestroyChildren(mazeATilemap.transform);
        DestroyChildren(mazeBTilemap.transform);
    }

    /// <summary>
    /// Törli egy transform összes gyerek objektumát.
    /// </summary>
    private void DestroyChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Felépíti mindkét labirintust.
    /// </summary>
    private void BuildMazes()
    {
        BuildMaze(currentLevel.mazeLayoutA, currentLevel.width, currentLevel.height, mazeATilemap);
        BuildMaze(currentLevel.mazeLayoutB, currentLevel.width, currentLevel.height, mazeBTilemap);
    }

    /// <summary>
    /// Létrehozza az összes ellenséget mindkét labirintusban.
    /// </summary>
    private void SpawnAllEnemies()
    {
        SpawnEnemies(currentLevel.enemyPositionsA, mazeATilemap, playerA, "Maze_A_Enemy");
        SpawnEnemies(currentLevel.enemyPositionsB, mazeBTilemap, playerB, "Maze_B_Enemy");
    }

    /// <summary>
    /// Elhelyezi a célokat mindkét pályán.
    /// </summary>
    private void PlaceFinishTiles()
    {
        BuildFinishTiles(currentLevel.finishPosA.ToVector2Int(), finishATilemap);
        BuildFinishTiles(currentLevel.finishPosB.ToVector2Int(), finishBTilemap);
    }

    /// <summary>
    /// Pozicionálja a játékosokat a kezdőpozíciókra.
    /// </summary>
    private void PositionPlayers()
    {
        SetPlayerPosition(playerA, currentLevel.startPosA.ToVector2Int());
        SetPlayerPosition(playerB, currentLevel.startPosB.ToVector2Int());
    }

    /// <summary>
    /// Konfigurálja a finish pozíciókat a PlayerController-ekben.
    /// </summary>
    private void ConfigureFinishPositions()
    {
        SetPlayerFinishPositions(currentLevel.finishPosA.ToVector2Int(), currentLevel.finishPosB.ToVector2Int());
    }

    #endregion

    #region Tilemap Építés

    /// <summary>
    /// Felépít egy labirintust a megadott layout alapján.
    /// </summary>
    private void BuildMaze(int[] layout, int width, int height, Tilemap tilemap)
    {
        for (int i = 0; i < layout.Length; i++)
        {
            int tileValue = layout[i];
            Vector3Int tilePosition = CalculateTilePosition(i, width);

            PlaceTile(tileValue, tilePosition, tilemap);
        }
    }

    /// <summary>
    /// Kiszámítja egy tile pozícióját az index alapján.
    /// </summary>
    private Vector3Int CalculateTilePosition(int index, int width)
    {
        int x = index % width;
        int y = -(index / width); // Negatív Y, hogy lefelé épüljön
        return new Vector3Int(x, y, 0);
    }

    /// <summary>
    /// Elhelyez egy tile-t a megfelelő pozícióra.
    /// </summary>
    private void PlaceTile(int tileValue, Vector3Int position, Tilemap tilemap)
    {
        if (tileValue == 1)
        {
            tilemap.SetTile(position, wallTile);
        }
        else
        {
            tilemap.SetTile(position, floorTile);
        }
    }

    /// <summary>
    /// Elhelyezi a cél tile-t a megadott pozícióra.
    /// </summary>
    private void BuildFinishTiles(Vector2Int finishPosition, Tilemap tilemap)
    {
        tilemap.SetTile(new Vector3Int(finishPosition.x, -finishPosition.y, 0), goalTile);
    }

    #endregion

    #region Ellenség Kezelés

    /// <summary>
    /// Létrehozza az ellenségeket a megadott pozíciókon.
    /// </summary>
    private void SpawnEnemies(List<Vector2IntData> enemyPositions, Tilemap parentTilemap, Transform targetPlayer, string layerName)
    {
        if (enemyPositions == null) return;

        int layerID = LayerMask.NameToLayer(layerName);
        int wallLayerMask = 1 << parentTilemap.gameObject.layer;

        foreach (var posData in enemyPositions)
        {
            CreateEnemy(posData, parentTilemap, targetPlayer, layerID, wallLayerMask);
        }
    }

    /// <summary>
    /// Létrehoz egy ellenséget a megadott pozíción.
    /// </summary>
    private void CreateEnemy(Vector2IntData position, Tilemap parentTilemap, Transform targetPlayer, int layerID, int wallLayerMask)
    {
        Vector3 spawnPos = new Vector3(position.x + CellCenterOffset, -position.y + CellCenterOffset, 0);
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        SetLayerRecursively(enemy, layerID);
        ConfigureEnemyController(enemy, targetPlayer, wallLayerMask);
        enemy.transform.SetParent(parentTilemap.transform);
    }

    /// <summary>
    /// Konfigurálja az ellenség controller-ét.
    /// </summary>
    private void ConfigureEnemyController(GameObject enemy, Transform targetPlayer, int wallLayerMask)
    {
        var controller = enemy.GetComponent<EnemyController>();
        if (controller != null)
        {
            controller.targetPlayer = targetPlayer;
            controller.obstacleLayer = wallLayerMask;
        }
    }

    /// <summary>
    /// Rekurzívan beállítja a layer-t az objektumon és gyerekein.
    /// </summary>
    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    #endregion

    #region Játékos Pozicionálás

    /// <summary>
    /// Beállítja egy játékos pozícióját a grid koordináta alapján.
    /// </summary>
    private void SetPlayerPosition(Transform player, Vector2Int gridPos)
    {
        if (player != null)
        {
            player.position = new Vector3(gridPos.x + CellCenterOffset, -gridPos.y + CellCenterOffset, 0);
        }
    }

    /// <summary>
    /// Beállítja a finish pozíciókat a PlayerController-ekben.
    /// </summary>
    private void SetPlayerFinishPositions(Vector2Int finishPosA, Vector2Int finishPosB)
    {
        SetPlayerFinishPosition(playerA, finishPosA);
        SetPlayerFinishPosition(playerB, finishPosB);
    }

    /// <summary>
    /// Beállítja egy játékos finish pozícióját.
    /// </summary>
    private void SetPlayerFinishPosition(Transform player, Vector2Int finishPos)
    {
        if (player == null) return;

        PlayerController controller = player.GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.SetFinishPosition(new Vector2Int(finishPos.x, -finishPos.y));
        }
    }

    #endregion

    #region Kamera Beállítás

    /// <summary>
    /// Beállítja a kamerákat a pálya mérete alapján.
    /// </summary>
    private void SetupCameras(int width, int height)
    {
        if (camA == null || camB == null) return;

        bool fitsOnScreen = DoesFitOnScreen(width, height);
        float gameplayOrthoSize = CalculateGameplayOrthoSize(width, height, fitsOnScreen);
        Vector3 centerOfMap = CalculateMapCenter(width, height);

        if (!fitsOnScreen)
        {
            StartIntroSequence(gameplayOrthoSize, width, height, centerOfMap);
        }
        else
        {
            SetupStaticCameras(gameplayOrthoSize, centerOfMap);
        }
    }

    /// <summary>
    /// Meghatározza, hogy a pálya elfér-e a képernyőn.
    /// </summary>
    private bool DoesFitOnScreen(int width, int height)
    {
        return width <= maxComfortableSize && height <= maxComfortableSize;
    }

    /// <summary>
    /// Kiszámítja a játék kamera méretét.
    /// </summary>
    private float CalculateGameplayOrthoSize(int width, int height, bool fitsOnScreen)
    {
        if (fitsOnScreen)
        {
            return CalculateOrthoSizeForSmallMap(width, height);
        }
        else
        {
            return CalculateOrthoSizeForLargeMap(width, height);
        }
    }

    /// <summary>
    /// Kiszámítja a kamera méretet kis pályákhoz.
    /// </summary>
    private float CalculateOrthoSizeForSmallMap(int width, int height)
    {
        float maxDim = Mathf.Max(width, height);
        float orthoSize = (maxDim / 2f) - CellCenterOffset;
        return Mathf.Max(orthoSize, MinimumCameraSize);
    }

    /// <summary>
    /// Kiszámítja a kamera méretet nagy pályákhoz.
    /// </summary>
    private float CalculateOrthoSizeForLargeMap(int width, int height)
    {
        float minDim = Mathf.Min(width, height);
        float calculationBase = (minDim > maxComfortableSize) ? maxComfortableSize : minDim;
        return (calculationBase / 2f) - CellCenterOffset;
    }

    /// <summary>
    /// Kiszámítja a pálya középpontját.
    /// </summary>
    private Vector3 CalculateMapCenter(int width, int height)
    {
        return new Vector3(width / 2f, 0.5f - height / 2f, CameraZDepth);
    }

    /// <summary>
    /// Elindítja az intro szekvenciát nagy pályákhoz.
    /// </summary>
    private void StartIntroSequence(float gameplayOrthoSize, int width, int height, Vector3 centerOfMap)
    {
        float maxDimension = Mathf.Max(width, height);
        float overviewOrthoSize = maxDimension / 2f;
        StartCoroutine(PlayIntroSequence(gameplayOrthoSize, overviewOrthoSize, centerOfMap));
    }

    /// <summary>
    /// Beállítja a statikus kamerákat kis pályákhoz.
    /// </summary>
    private void SetupStaticCameras(float orthoSize, Vector3 centerPos)
    {
        shouldFollowPlayers = false;
        currentOrthoSize = orthoSize;
        SetCamera(camA, currentOrthoSize, centerPos);
        SetCamera(camB, currentOrthoSize, centerPos);
    }

    /// <summary>
    /// Játssza le az intro animációt (zoom-out majd zoom-in).
    /// </summary>
    private IEnumerator PlayIntroSequence(float targetSize, float startSize, Vector3 centerPos)
    {
        isIntroPlaying = true;
        shouldFollowPlayers = false;

        SetBothCameras(startSize, centerPos);
        yield return new WaitForSeconds(introDelay);

        yield return AnimateIntroZoom(targetSize, startSize, centerPos);

        CompleteIntroSequence(targetSize);
    }

    /// <summary>
    /// Beállítja mindkét kamerát ugyanarra a méretre és pozícióra.
    /// </summary>
    private void SetBothCameras(float size, Vector3 position)
    {
        SetCamera(camA, size, position);
        SetCamera(camB, size, position);
    }

    /// <summary>
    /// Animálja az intro zoom-ot.
    /// </summary>
    private IEnumerator AnimateIntroZoom(float targetSize, float startSize, Vector3 centerPos)
    {
        float elapsedTime = 0f;
        Vector3 targetPosA = GetClampedPosition(playerA.position, targetSize, camA.aspect, currentLevel.width, currentLevel.height);
        Vector3 targetPosB = GetClampedPosition(playerB.position, targetSize, camB.aspect, currentLevel.width, currentLevel.height);

        while (elapsedTime < introDuration)
        {
            float t = elapsedTime / introDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            InterpolateCameras(centerPos, targetPosA, targetPosB, startSize, targetSize, smoothT);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// Interpolálja a kamerák pozícióját és méretét.
    /// </summary>
    private void InterpolateCameras(Vector3 startPos, Vector3 targetPosA, Vector3 targetPosB, float startSize, float targetSize, float t)
    {
        float currentSize = Mathf.Lerp(startSize, targetSize, t);
        Vector3 currentPosA = Vector3.Lerp(startPos, targetPosA, t);
        Vector3 currentPosB = Vector3.Lerp(startPos, targetPosB, t);

        SetCamera(camA, currentSize, currentPosA);
        SetCamera(camB, currentSize, currentPosB);
    }

    /// <summary>
    /// Befejezi az intro szekvenciát.
    /// </summary>
    private void CompleteIntroSequence(float targetSize)
    {
        currentOrthoSize = targetSize;
        shouldFollowPlayers = true;
        isIntroPlaying = false;
    }

    /// <summary>
    /// Beállít egy kamerát a megadott méretre és pozícióra.
    /// </summary>
    private void SetCamera(Camera cam, float size, Vector3 pos)
    {
        cam.orthographicSize = size;
        cam.transform.position = new Vector3(pos.x, pos.y + CameraOffsetY, pos.z);
    }

    #endregion

    #region Kamera Követés

    /// <summary>
    /// Ellenőrzi, hogy frissíteni kell-e a kamera pozíciókat.
    /// </summary>
    private bool ShouldUpdateCameraPositions()
    {
        return currentLevel != null && shouldFollowPlayers && !isIntroPlaying;
    }

    /// <summary>
    /// Frissíti mindkét kamera pozícióját.
    /// </summary>
    private void UpdateCameraPositions()
    {
        UpdateSingleCamera(camA, playerA);
        UpdateSingleCamera(camB, playerB);
    }

    /// <summary>
    /// Frissít egy kamerát hogy kövesse a célpontját.
    /// </summary>
    private void UpdateSingleCamera(Camera cam, Transform target)
    {
        if (cam != null && target != null)
        {
            cam.transform.position = GetClampedPosition(
                target.position,
                currentOrthoSize,
                cam.aspect,
                currentLevel.width,
                currentLevel.height
            );
        }
    }

    /// <summary>
    /// Visszaad egy pozíciót ami a pálya határain belül van.
    /// </summary>
    private Vector3 GetClampedPosition(Vector3 targetPosition, float orthoSize, float aspect, int mapWidth, int mapHeight)
    {
        float vertExtent = orthoSize;
        float horzExtent = vertExtent * aspect;

        CalculateBoundaries(horzExtent, vertExtent, mapWidth, mapHeight, out float minX, out float maxX, out float minY, out float maxY);
        float clampedX = ClampValue(targetPosition.x, minX, maxX, mapWidth / 2f);
        float clampedY = ClampValue(targetPosition.y, minY, maxY, 0.5f - mapHeight / 2f);

        return new Vector3(clampedX, clampedY, CameraZDepth);
    }

    /// <summary>
    /// Kiszámítja a kamera határait.
    /// </summary>
    private void CalculateBoundaries(float horzExtent, float vertExtent, int mapWidth, int mapHeight, 
        out float minX, out float maxX, out float minY, out float maxY)
    {
        minX = horzExtent + BoundaryInsetX;
        maxX = mapWidth - horzExtent - BoundaryInsetX;
        maxY = -vertExtent - BoundaryInsetY;
        minY = -mapHeight + vertExtent + BoundaryInsetY;
    }

    /// <summary>
    /// Clampolja az értéket a min és max között, vagy visszaadja a fallback értéket.
    /// </summary>
    private float ClampValue(float value, float min, float max, float fallback)
    {
        if (min > max)
            return fallback;
        return Mathf.Clamp(value, min, max);
    }

    #endregion

    #region 1 Perces Kihívás

    /// <summary>
    /// Frissíti az 1 perces kihívás időzítőjét.
    /// </summary>
    private void UpdateChallengeTimer()
    {
        remainingTime -= Time.deltaTime;

        if (GameSceneUIController.Instance != null)
        {
            GameSceneUIController.Instance.UpdateTimer(remainingTime);
        }

        if (remainingTime <= 0f)
        {
            OnChallengeTimeExpired();
        }
    }

    /// <summary>
    /// Választ egy random pálya indexet (nem ugyanazt mint az előző).
    /// </summary>
    private int GetRandomLevelIndex(int levelCount)
    {
        if (levelCount <= 1)
            return 0;

        int newIndex;
        int attempts = 0;

        do
        {
            newIndex = UnityEngine.Random.Range(0, levelCount);
            attempts++;

            if (attempts > RandomSelectionMaxAttempts)
            {
                Debug.LogWarning("Random választás túl sok próbálkozás után megszakítva.");
                break;
            }
        }
        while (newIndex == lastLevelIndex && levelCount > 1);

        return newIndex;
    }

    /// <summary>
    /// Betölti a következő random pályát az 1 perces kihívásban.
    /// </summary>
    public void LoadNextRandomLevel()
    {
        if (!IsValidChallengeState())
            return;

        int randomIndex = GetRandomLevelIndex(levelWrapper.boards.Count);
        lastLevelIndex = randomIndex;
        currentLevel = levelWrapper.boards[randomIndex];

        Debug.Log($"🎯 Új pálya betöltve: {currentLevel.levelName} ({completedLevelsCount + 1}. pálya)");

        GenerateLevel();
    }

    /// <summary>
    /// Ellenőrzi hogy érvényes állapotban van-e az 1 perces kihívás.
    /// </summary>
    private bool IsValidChallengeState()
    {
        if (LevelContext.Gamemode != "1minchallenge" || !isTimerRunning)
            return false;

        if (levelWrapper == null || levelWrapper.boards == null || levelWrapper.boards.Count == 0)
        {
            Debug.LogError("Nincs pálya betöltve az 1 perces kihíváshoz!");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Kezeli a pálya teljesítését az 1 perces kihívásban.
    /// </summary>
    public void OnChallengeComplete()
    {
        if (LevelContext.Gamemode != "1minchallenge" || !isTimerRunning)
            return;

        completedLevelsCount++;
        Debug.Log($"✅ Pálya teljesítve! Összesen: {completedLevelsCount}");

        LoadNextRandomLevel();
    }

    /// <summary>
    /// Kezeli az idő lejártát az 1 perces kihívásban.
    /// </summary>
    private void OnChallengeTimeExpired()
    {
        isTimerRunning = false;

        Debug.Log($"⏰ 1 perc lejárt! Teljesített pályák: {completedLevelsCount}");

        SaveChallengeResults();
        ShowChallengeResults();
    }

    /// <summary>
    /// Menti az 1 perces kihívás eredményeit.
    /// </summary>
    private void SaveChallengeResults()
    {
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.RecordChallengeWin(true, 0f, completedLevelsCount);
        }
    }

    /// <summary>
    /// Megjeleníti az 1 perces kihívás eredményeit.
    /// </summary>
    private void ShowChallengeResults()
    {
        if (FinishManager.Instance != null)
        {
            FinishManager.Instance.Show1MinChallengeResults(completedLevelsCount);
        }
    }

    #endregion

    #region Játékos Állapotkezelés

    /// <summary>
    /// Visszaaktiválja a játékos kontrollereket (pl. korábbi GameOver után).
    /// </summary>
    private void ReactivatePlayers()
    {
        ReactivateSinglePlayer(playerA);
        ReactivateSinglePlayer(playerB);
    }

    /// <summary>
    /// Visszaaktivál egy játékost és lenullázza a mozgását.
    /// </summary>
    private void ReactivateSinglePlayer(Transform player)
    {
        if (player == null) return;

        PlayerController controller = player.GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.enabled = true;
            controller.ResetPlayerState();
        }

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    #endregion
}
