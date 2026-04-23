using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Ellenség viselkedését irányító osztály BFS alapú útvonalkeresõvel.
/// </summary>
public class EnemyController : MonoBehaviour
{
    #region Constants
    private const float GRID_OFFSET = 0.5f;
    private const float MOVEMENT_THRESHOLD = 0.01f;
    private const float RAYCAST_DISTANCE = 1.0f;
    private const float INITIALIZATION_DELAY = 0.5f;
    private const float GIZMO_TARGET_RADIUS = 0.3f;
    private const float GIZMO_AGGRO_RADIUS = 0.2f;
    private const float GIZMO_AGGRO_HEIGHT = 0.5f;
    private const int BFS_SAFETY_LIMIT = 1000;
    private const string PLAYER_TAG = "Player";
    #endregion

    #region Inspector Fields
    [Header("Beállítások")]
    public float detectionRange = 5.0f;
    public float speed = 2.0f;
    public LayerMask obstacleLayer;

    [Header("Referencia")]
    public Transform targetPlayer;
    #endregion

    #region State
    private Vector3 currentTargetPosition;
    private bool isMoving = false;
    private bool hasAggro = false;
    private bool canSeePlayer = false;
    #endregion

    #region Pathfinding Directions
    private static readonly Vector2Int[] CardinalDirections = 
    {
        Vector2Int.right,
        Vector2Int.left,
        Vector2Int.up,
        Vector2Int.down
    };
    #endregion

    #region Unity Lifecycle
    void Start()
    {
        SnapToGrid();
        canSeePlayer = false;
        StartCoroutine(InitializeAfterDelay());
    }

    void Update()
    {
        if (!CanUpdate()) return;

        UpdateAggroState();

        if (!hasAggro) return;

        if (!isMoving)
        {
            FindPathWithBFS();
        }
        else
        {
            MoveToTarget();
        }
    }
    #endregion
    #region Effect Management
    ///<summary>
    ///Lejátsza az ellenség mérges effektjét
    /// </summary>
    private void PlayAgroEffect()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayEnemyGotAngrySFX();
        }
    }
    #endregion

    #region State Management
    /// <summary>
    /// Ellenõrzi, hogy fut-e az Update logika.
    /// </summary>
    private bool CanUpdate()
    {
        return targetPlayer != null && canSeePlayer;
    }

    /// <summary>
    /// Frissíti az aggro állapotot a játékos távolsága alapján.
    /// </summary>
    private void UpdateAggroState()
    {
        if (hasAggro) return;

        float distanceToPlayer = Vector3.Distance(transform.position, targetPlayer.position);

        if (distanceToPlayer <= detectionRange)
        {
            PlayAgroEffect();
            hasAggro = true;
        }
    }

    /// <summary>
    /// Inicializálás késleltetéssel.
    /// </summary>
    private IEnumerator InitializeAfterDelay()
    {
        yield return new WaitForSeconds(INITIALIZATION_DELAY);
        canSeePlayer = true;
    }
    #endregion

    #region Movement
    /// <summary>
    /// Pozíció igazítása a rács közepére.
    /// </summary>
    private void SnapToGrid()
    {
        float x = Mathf.Floor(transform.position.x) + GRID_OFFSET;
        float y = Mathf.Floor(transform.position.y) + GRID_OFFSET;
        transform.position = new Vector3(x, y, transform.position.z);
        currentTargetPosition = transform.position;
    }

    /// <summary>
    /// Mozgás a célpozíció felé.
    /// </summary>
    private void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(
            transform.position, 
            currentTargetPosition, 
            speed * Time.deltaTime
        );

        if (HasReachedTarget())
        {
            transform.position = currentTargetPosition;
            isMoving = false;
        }
    }

    /// <summary>
    /// Ellenõrzi, hogy elérte-e a célt.
    /// </summary>
    private bool HasReachedTarget()
    {
        return Vector3.Distance(transform.position, currentTargetPosition) < MOVEMENT_THRESHOLD;
    }
    #endregion

    #region Pathfinding
    /// <summary>
    /// BFS alapú útvonalkeresés és következõ lépés beállítása.
    /// </summary>
    private void FindPathWithBFS()
    {
        Vector2Int startNode = WorldToGrid(transform.position);
        Vector2Int goalNode = WorldToGrid(targetPlayer.position);

        if (startNode == goalNode) return;

        Dictionary<Vector2Int, Vector2Int> cameFrom = ExecuteBFS(startNode, goalNode);

        if (cameFrom != null && cameFrom.ContainsKey(goalNode))
        {
            Vector2Int firstStep = GetFirstStepFromPath(cameFrom, startNode, goalNode);
            SetNextMoveTarget(firstStep);
        }
    }

    /// <summary>
    /// BFS algoritmus futtatása.
    /// </summary>
    private Dictionary<Vector2Int, Vector2Int> ExecuteBFS(Vector2Int startNode, Vector2Int goalNode)
    {
        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        frontier.Enqueue(startNode);

        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>
        {
            [startNode] = startNode
        };

        int iterations = 0;

        while (frontier.Count > 0 && iterations < BFS_SAFETY_LIMIT)
        {
            iterations++;
            Vector2Int current = frontier.Dequeue();

            if (current == goalNode)
            {
                return cameFrom;
            }

            ExploreNeighbors(current, frontier, cameFrom);
        }

        return cameFrom;
    }

    /// <summary>
    /// Szomszédos mezõk feltárása.
    /// </summary>
    private void ExploreNeighbors(
        Vector2Int current, 
        Queue<Vector2Int> frontier, 
        Dictionary<Vector2Int, Vector2Int> cameFrom)
    {
        foreach (Vector2Int direction in CardinalDirections)
        {
            Vector2Int neighbor = current + direction;

            if (cameFrom.ContainsKey(neighbor)) continue;

            if (IsWalkable(current, direction))
            {
                frontier.Enqueue(neighbor);
                cameFrom[neighbor] = current;
            }
        }
    }

    /// <summary>
    /// Ellenõrzi, hogy az adott irányba járható-e a mezõ.
    /// </summary>
    private bool IsWalkable(Vector2Int gridPosition, Vector2Int direction)
    {
        Vector3 worldPosition = GridToWorld(gridPosition);
        Vector3 worldDirection = new Vector3(direction.x, direction.y, 0);

        RaycastHit2D hit = Physics2D.Raycast(
            worldPosition, 
            worldDirection, 
            RAYCAST_DISTANCE, 
            obstacleLayer
        );

        return hit.collider == null;
    }

    /// <summary>
    /// Meghatározza az elsõ lépést az útvonalból.
    /// </summary>
    private Vector2Int GetFirstStepFromPath(
        Dictionary<Vector2Int, Vector2Int> cameFrom, 
        Vector2Int startNode, 
        Vector2Int goalNode)
    {
        Vector2Int step = goalNode;

        while (cameFrom[step] != startNode)
        {
            step = cameFrom[step];
        }

        return step;
    }

    /// <summary>
    /// Beállítja a következõ lépés célpontját.
    /// </summary>
    private void SetNextMoveTarget(Vector2Int gridPosition)
    {
        currentTargetPosition = GridToWorld(gridPosition);
        isMoving = true;
    }
    #endregion

    #region Coordinate Conversion
    /// <summary>
    /// Világ koordináták rács koordinátákká alakítása.
    /// </summary>
    private Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        return new Vector2Int(
            Mathf.FloorToInt(worldPosition.x), 
            Mathf.FloorToInt(worldPosition.y)
        );
    }

    /// <summary>
    /// Rács koordináták világ koordinátákká alakítása.
    /// </summary>
    private Vector3 GridToWorld(Vector2Int gridPosition)
    {
        return new Vector3(
            gridPosition.x + GRID_OFFSET, 
            gridPosition.y + GRID_OFFSET, 
            0
        );
    }
    #endregion

    #region Collision
    /// <summary>
    /// Játékossal való ütközés kezelése.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsTargetPlayer(collision)) return;

        TriggerGameOver();
    }

    /// <summary>
    /// Ellenõrzi, hogy a célzott játékossal ütközött-e.
    /// </summary>
    private bool IsTargetPlayer(Collider2D collision)
    {
        if (!collision.CompareTag(PLAYER_TAG)) return false;

        if (collision.transform == targetPlayer)
        {
            return true;
        }

        #if UNITY_EDITOR || DEBUG
        UnityEngine.Debug.Log("Ütközés a másik játékossal - Figyelmen kívül hagyva.");
        #endif

        return false;
    }

    /// <summary>
    /// Game Over állapot aktiválása.
    /// </summary>
    private void TriggerGameOver()
    {
        #if UNITY_EDITOR || DEBUG
        UnityEngine.Debug.Log("GAME OVER - Elkaptalak!");
        #endif

        if (GameSceneUIController.Instance != null)
        {
            PlayDefeatSFX();
            GameSceneUIController.Instance.TriggerGameOver();
        }
    }
    #endregion
    #region Sfx Management
    private void PlayDefeatSFX()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDefeatSFX();
        }
    }
    #endregion
    #region Public API
    /// <summary>
    /// Leállítja az ellenséget és kikapcsolja az ütközését.
    /// </summary>
    public void StopEnemy()
    {
        enabled = false;
        isMoving = false;
        DisableCollider();
    }

    /// <summary>
    /// Kikapcsolja az ütközést.
    /// </summary>
    private void DisableCollider()
    {
        Collider2D collider = GetComponent<Collider2D>();

        if (collider != null)
        {
            collider.enabled = false;
        }
    }
    #endregion

    #region Debug Visualization
    private void OnDrawGizmos()
    {
        DrawMovementGizmo();
        DrawAggroGizmo();
    }

    /// <summary>
    /// Mozgás vizualizálása Gizmos-szal.
    /// </summary>
    private void DrawMovementGizmo()
    {
        if (!isMoving) return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, currentTargetPosition);
        Gizmos.DrawWireSphere(currentTargetPosition, GIZMO_TARGET_RADIUS);
    }

    /// <summary>
    /// Aggro állapot vizualizálása.
    /// </summary>
    private void DrawAggroGizmo()
    {
        if (!hasAggro) return;

        Gizmos.color = Color.yellow;
        Vector3 aggroPosition = transform.position + Vector3.up * GIZMO_AGGRO_HEIGHT;
        Gizmos.DrawWireSphere(aggroPosition, GIZMO_AGGRO_RADIUS);
    }
    #endregion
}