using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeLaneController : MonoBehaviour
{
    [Header("Move This (IMPORTANT)")]
    public Transform moveTarget;

    [Header("Lanes")]
    public float laneOffset = 2.5f;
    public int minLane = -1;
    public int maxLane = 1;

    [Header("Movement")]
    public float lateralSmoothTime = 0.08f;

    [Header("Swipe")]
    public float minSwipePixels = 80f;
    public float maxSwipeTime = 0.35f;

    [Header("Debug")]
    public bool debugLogs = true;

    int lane = 0;
    float baseLocalX;
    float velX;

    Vector2 startPos;
    float startTime;
    bool swiping;

    Transform T => moveTarget != null ? moveTarget : transform;

    void Start()
    {
        baseLocalX = T.localPosition.x;
    }

    void Update()
    {
        HandleSwipeInput();
    }

    void LateUpdate()
    {
        // moves ONLY local X (lane switching)
        Vector3 lp = T.localPosition;
        float targetX = baseLocalX + lane * laneOffset;
        lp.x = Mathf.SmoothDamp(lp.x, targetX, ref velX, lateralSmoothTime);
        T.localPosition = lp;
    }

    void HandleSwipeInput()
    {
        // touch
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(t.fingerId))
                return;

            if (t.phase == TouchPhase.Began)
            {
                swiping = true;
                startPos = t.position;
                startTime = Time.time;
            }
            else if (swiping && (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled))
            {
                EvaluateSwipe(t.position);
                swiping = false;
            }
            return;
        }

        // mouse 
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            swiping = true;
            startPos = Input.mousePosition;
            startTime = Time.time;
        }
        else if (swiping && Input.GetMouseButtonUp(0))
        {
            EvaluateSwipe((Vector2)Input.mousePosition);
            swiping = false;
        }
    }

    void EvaluateSwipe(Vector2 endPos)
    {
        float dt = Time.time - startTime;
        if (dt > maxSwipeTime) return;

        Vector2 delta = endPos - startPos;

        if (Mathf.Abs(delta.x) < minSwipePixels) return;
        if (Mathf.Abs(delta.x) < Mathf.Abs(delta.y)) return;

        if (delta.x > 0) MoveRight();
        else MoveLeft();
    }

    public void MoveLeft()
    {
        int old = lane;
        lane = Mathf.Clamp(lane - 1, minLane, maxLane);
        if (debugLogs && lane != old) Debug.Log("Lane LEFT -> " + lane);
    }

    public void MoveRight()
    {
        int old = lane;
        lane = Mathf.Clamp(lane + 1, minLane, maxLane);
        if (debugLogs && lane != old) Debug.Log("Lane RIGHT -> " + lane);
    }
}
