using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// A játékos mozgását és célba érkezés érzékelését kezelõ osztály.
/// Swipe alapú input rendszert használ, grid-alapú mozgással.
/// </summary>
public class PlayerController : MonoBehaviour
{
    #region Konstansok

    private const float PositionTolerance = 0.01f;
    private const int Player1Id = 1;
    private const int Player2Id = 2;

    #endregion

    #region Inspector Mezõk

    [Header("Input Beállítások")]
    public float minSwipeDistance = 100f;

    [Header("Mozgás Beállítások")]
    public float moveDistance = 1f;
    public float moveSpeed = 5f;

    [Header("Collision Beállítások")]
    public LayerMask wallMask;

    [Header("Játékos Azonosító")]
    public int playerId;


    #endregion

    #region Privát Mezõk - Input

    private Vector2 swipeStartPosition;
    

    #endregion

    #region Privát Mezõk - Mozgás

    private Vector3 targetPosition;
    private bool isMoving = false;

    #endregion

    #region Privát Mezõk - Finish Detekció

    private bool hasReachedFinish = false;
    private bool isInFinishTriggerArea = false;
    private Vector2Int finishGridPosition;

    #endregion

    #region Unity Életciklus Metódusok

    /// <summary>
    /// Inicializálja a játékos kezdõ pozícióját.
    /// </summary>
    private void Start()
    {
        //Debug.Log("PlayerController: Start() lefutott");
        targetPosition = transform.position;
    }

    /// <summary>
    /// Kezeli az input-ot és a mozgást minden frame-ben.
    /// </summary>
    private void Update()
    {
        ProcessInput();
        UpdateMovement();
        
    }

    #endregion

    #region Publikus Metódusok - Konfiguráció

    /// <summary>
    /// Beállítja a célpont grid pozícióját.
    /// </summary>
    public void SetFinishPosition(Vector2Int gridPos)
    {
        finishGridPosition = gridPos;
    }

    /// <summary>
    /// Visszaállítja a játékos állapotát.
    /// </summary>
    public void ResetPlayerState()
    {
        hasReachedFinish = false;
        isInFinishTriggerArea = false;
    }

    #endregion

    #region Input Kezelés
        
    /// <summary>
    /// Feldolgozza a swipe input-ot.
    /// </summary>
    private void ProcessInput()
    {
        //Debug.Log("PlayerController: ProcessInput() lefutott");
        Pointer pointer = Pointer.current;
        if (pointer == null)
            return;

        if (pointer.press.isPressed)
        {
            
            HandlePointerPress(pointer);
        }
        else
        {
            
            ResetSwipeStart();
        }
    }

    /// <summary>
    /// Kezeli a pointer lenyomását és a swipe észlelését.
    /// </summary>
    private void HandlePointerPress(Pointer pointer)
    {
        if (swipeStartPosition == Vector2.zero)
        {
            swipeStartPosition = pointer.position.ReadValue();
        }

        Vector2 currentPosition = pointer.position.ReadValue();
        Vector2 swipeDelta = currentPosition - swipeStartPosition;

        if (swipeDelta.magnitude >= minSwipeDistance)
        {
            
            ProcessSwipe(currentPosition);
            HideTutorialIfNeeded();
            ResetSwipeStart();
        }
    }

    /// <summary>
    /// Feldolgozza a swipe mozdulatot és meghatározza az irányt.
    /// </summary>
    private void ProcessSwipe(Vector2 endPosition)
    {
        if (isMoving)
            return;

        Vector2 swipeDirection = (endPosition - swipeStartPosition).normalized;
        Vector3 moveDirection = DetermineSwipeDirection(swipeDirection);

        if (moveDirection != Vector3.zero)
        {
            TrySetTarget(moveDirection);
        }
    }

    /// <summary>
    /// Meghatározza a swipe irányát (fel/le/bal/jobb).
    /// </summary>
    private Vector3 DetermineSwipeDirection(Vector2 swipe)
    {
        bool isHorizontal = Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y);

        if (isHorizontal)
        {
            return swipe.x > 0 ? Vector3.right : Vector3.left;
        }
        else
        {
            return swipe.y > 0 ? Vector3.up : Vector3.down;
        }
    }

    /// <summary>
    /// Elrejti a tutorial panelt ha szükséges.
    /// </summary>
    private void HideTutorialIfNeeded()
    {
        if (GameSceneUIController.Instance != null)
        {
            GameSceneUIController.Instance.HideTutorial();
        }
    }

    /// <summary>
    /// Visszaállítja a swipe kezdõpontját.
    /// </summary>
    private void ResetSwipeStart()
    {
        swipeStartPosition = Vector2.zero;
    }

    #endregion

    #region Mozgás Kezelés

    /// <summary>
    /// Megpróbálja beállítani a cél pozíciót, ha nincs fal az útban.
    /// </summary>
    private void TrySetTarget(Vector3 direction)
    {
        if (IsGameFinished())
        {
            
            return;
        }

        if (IsWallBlocking(direction))
        {

            return;
        }
        
        SetTargetPosition(direction);
        PlayStepSFX();
        ResetFinishTriggerFlag();
    }

    /// <summary>
    /// Ellenõrzi, hogy a játék véget ért-e.
    /// </summary>
    private bool IsGameFinished()
    {
        return FinishManager.Instance != null && FinishManager.Instance.gameFinished;
    }

    /// <summary>
    /// Ellenõrzi, hogy van-e fal a megadott irányban.
    /// </summary>
    private bool IsWallBlocking(Vector3 direction)
    {
        Vector2 origin = transform.position;
        Vector2 dir2D = direction;

        RaycastHit2D wallHit = Physics2D.Raycast(origin, dir2D, moveDistance, wallMask);
        return wallHit.collider != null;
    }

    /// <summary>
    /// Beállítja a cél pozíciót és elindítja a mozgást.
    /// </summary>
    private void SetTargetPosition(Vector3 direction)
    {
        targetPosition = transform.position + direction * moveDistance;
        isMoving = true;
    }

    /// <summary>
    /// Nullázza a finish trigger flag-et mozgás indításakor.
    /// </summary>
    private void ResetFinishTriggerFlag()
    {
        isInFinishTriggerArea = false;
    }

    /// <summary>
    /// Frissíti a játékos pozícióját a cél felé.
    /// </summary>
    private void UpdateMovement()
    {
        
        if (!isMoving)
        {
            
            return;
        }
        
        MoveTowardsTarget();

        if (HasReachedTarget())
        {
            SnapToTarget();
            StopMovement();
            CheckFinishAfterStopping();
        }
    }

    /// <summary>
    /// Mozgatja a játékost a cél felé.
    /// </summary>
    private void MoveTowardsTarget()
    {

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Ellenõrzi, hogy elérte-e a játékos a célpontot.
    /// </summary>
    private bool HasReachedTarget()
    {
        return Vector3.Distance(transform.position, targetPosition) < PositionTolerance;
    }

    /// <summary>
    /// Pontosan a célpontra állítja a játékost.
    /// </summary>
    private void SnapToTarget()
    {
        transform.position = targetPosition;
    }

    /// <summary>
    /// Leállítja a mozgást.
    /// </summary>
    private void StopMovement()
    {
        isMoving = false;
    }

    #endregion

    #region Hangeffekt Lejátszás

    /// <summary>
    /// Lejátssza a lépés hangeffektet új swipe esetén.
    /// </summary>
    private void PlayStepSFX()
    {
        if (AudioManager.Instance != null)
        {
            //AudioManager.Instance.PlayStepSFX();
        }
    }

    #endregion

    #region Finish Detekció

    /// <summary>
    /// Ellenõrzi megállás után, hogy a játékos célba ért-e.
    /// </summary>
    private void CheckFinishAfterStopping()
    {
        if (!isInFinishTriggerArea || hasReachedFinish)
            return;

        Vector2Int currentGridPosition = GetCurrentGridPosition();

        if (IsOnFinishTile(currentGridPosition))
        {
            NotifyFinishReached();
        }
        else
        {
            LogFinishPositionMismatch(currentGridPosition);
        }
    }

    /// <summary>
    /// Visszaadja a játékos jelenlegi grid pozícióját.
    /// </summary>
    private Vector2Int GetCurrentGridPosition()
    {
        return new Vector2Int(
            Mathf.FloorToInt(transform.position.x),
            Mathf.FloorToInt(transform.position.y)
        );
    }

    /// <summary>
    /// Ellenõrzi, hogy a játékos a finish tile-on van-e.
    /// </summary>
    private bool IsOnFinishTile(Vector2Int currentPosition)
    {
        return currentPosition == finishGridPosition;
    }

    /// <summary>
    /// Értesíti a FinishManager-t a célba érkezésrõl.
    /// </summary>
    private void NotifyFinishReached()
    {
        hasReachedFinish = true;

        if (FinishManager.Instance != null)
        {
            FinishManager.Instance.PlayerReachedFinish(playerId);
        }
    }

    /// <summary>
    /// Debug log a finish pozíció eltérésrõl.
    /// </summary>
    private void LogFinishPositionMismatch(Vector2Int currentPosition)
    {
        UnityEngine.Debug.Log($"Player {playerId} trigger-ben van, de nem a cél grid pozíción. " +
                             $"Aktuális: {currentPosition}, Cél: {finishGridPosition}");
    }

    #endregion

    #region Trigger Kezelés

    /// <summary>
    /// Kezeli a finish trigger területén való tartózkodást.
    /// </summary>
    private void OnTriggerStay2D(Collider2D other)
    {
        if (IsPlayerInCorrectFinishArea(other))
        {
            isInFinishTriggerArea = true;

            if (!isMoving)
            {
                CheckFinishAfterStopping();
            }
        }
    }

    /// <summary>
    /// Kezeli a finish trigger területérõl való kilépést.
    /// </summary>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (IsPlayerInCorrectFinishArea(other))
        {
            ResetFinishState();
        }
    }

    /// <summary>
    /// Ellenõrzi, hogy a játékos a saját finish területén van-e.
    /// </summary>
    private bool IsPlayerInCorrectFinishArea(Collider2D trigger)
    {
        int finishALayer = LayerMask.NameToLayer("Maze_A_Finish");
        int finishBLayer = LayerMask.NameToLayer("Maze_B_Finish");
        int triggerLayer = trigger.gameObject.layer;

        return (playerId == Player1Id && triggerLayer == finishALayer) ||
               (playerId == Player2Id && triggerLayer == finishBLayer);
    }

    /// <summary>
    /// Visszaállítja a finish állapotot amikor elhagyja a területet.
    /// </summary>
    private void ResetFinishState()
    {
        isInFinishTriggerArea = false;
        hasReachedFinish = false;

        if (FinishManager.Instance != null)
        {
            FinishManager.Instance.ResetFinishState();
        }
    }

    #endregion
}