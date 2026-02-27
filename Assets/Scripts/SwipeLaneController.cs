using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeLaneController : MonoBehaviour
{
    [Header("Move This")]
    public Transform moveTarget;

    [Header("Reference Direction")]
    public Transform forwardReference;  
    public bool invertLanes = false;

    [Header("Lanes")]
    public float laneOffset = 400f;
    public int minLane = -1;
    public int maxLane = 1;

    [Header("Movement")]
    public float lateralSmoothTime = 0.08f;

    [Header("Drag")]
    public float dragSensitivity = 1.0f; 
    public bool snapToLaneOnRelease = true; // if false, will keep the free position instead of snapping to nearest lane on drag end

    [Header("Debug")]
    public bool debugLogs = true;

    [Header("Phase Management")]
    public bool acceptSwipeInput = true;

    int lane = 0;

    Vector3 baseWorldPos;
    Vector3 lateralVel;

    bool dragging = false;
    Vector2 dragStartScreenPos;
    float dragStartOffset;
    float currentOffset;

    void Start()
    {
        if (moveTarget == null) moveTarget = transform;

        baseWorldPos = moveTarget.position;
        currentOffset = 0f;

        if (debugLogs)
            Debug.Log("DragLaneController initialized. MoveTarget: " + moveTarget.name);
    }

    void Update()
    {
        HandleDragInput();
        HandleKeyboardInput();
    }

    void LateUpdate()
    {
        if (moveTarget == null) return;

        Vector3 right = GetLaneRight();

        Vector3 target = baseWorldPos + right * currentOffset;
        target.y = moveTarget.position.y;

        moveTarget.position = Vector3.SmoothDamp(
            moveTarget.position,
            target,
            ref lateralVel,
            lateralSmoothTime
        );
    }

    Vector3 GetLaneRight()
    {
        Vector3 right;

        if (forwardReference != null)
        {
            Vector3 fwd = Vector3.ProjectOnPlane(forwardReference.forward, Vector3.up).normalized;
            if (fwd.sqrMagnitude < 0.0001f) fwd = Vector3.forward;

            right = Vector3.Cross(Vector3.up, fwd).normalized;
        }
        else
        {
            right = Vector3.right;
        }

        if (invertLanes) right = -right;

        return right;
    }

    void HandleDragInput()
    {
        if (!acceptSwipeInput) return;

        // Ignore touches over UI
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began)
            {
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(t.fingerId))
                    return;

                dragging = true;
                dragStartScreenPos = t.position;
                dragStartOffset = currentOffset;

                if (debugLogs) Debug.Log("Touch drag started at: " + dragStartScreenPos);
            }
            else if (dragging && (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary))
            {
                UpdateDrag(t.position);
            }
            else if (dragging && (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled))
            {
                EndDrag();
            }

            return;
        }

        // Mouse
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            dragging = true;
            dragStartScreenPos = Input.mousePosition;
            dragStartOffset = currentOffset;

            if (debugLogs) Debug.Log("Mouse drag started at: " + dragStartScreenPos);
        }
        else if (dragging && Input.GetMouseButton(0))
        {
            UpdateDrag((Vector2)Input.mousePosition);
        }
        else if (dragging && Input.GetMouseButtonUp(0))
        {
            EndDrag();
        }
    }

    void UpdateDrag(Vector2 currentScreenPos)
    {
        float deltaX = currentScreenPos.x - dragStartScreenPos.x;

        // Convert screen drag into world lane offset
        float newOffset = dragStartOffset + (deltaX * dragSensitivity);

        float minOffset = minLane * laneOffset;
        float maxOffset = maxLane * laneOffset;

        currentOffset = Mathf.Clamp(newOffset, minOffset, maxOffset);

        if (debugLogs)
            Debug.Log("Dragging. deltaX=" + deltaX + " currentOffset=" + currentOffset);
    }

    void EndDrag()
    {
        dragging = false;

        if (snapToLaneOnRelease)
        {
            lane = Mathf.RoundToInt(currentOffset / laneOffset);
            lane = Mathf.Clamp(lane, minLane, maxLane);
            currentOffset = lane * laneOffset;

            if (debugLogs)
                Debug.Log("Drag ended. Snapped to lane: " + lane);
        }
        else
        {
            lane = Mathf.RoundToInt(currentOffset / laneOffset);
            lane = Mathf.Clamp(lane, minLane, maxLane);

            if (debugLogs)
                Debug.Log("Drag ended. Free position kept. Approx lane: " + lane);
        }

        lateralVel = Vector3.zero;
    }

    void HandleKeyboardInput()
    {
        if (!acceptSwipeInput) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            MoveLeft();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            MoveRight();
        }
    }

    public void MoveLeft()
    {
        lane = Mathf.Clamp(lane - 1, minLane, maxLane);
        currentOffset = lane * laneOffset;
        lateralVel = Vector3.zero;

        if (debugLogs)
            Debug.Log("MoveLeft -> lane " + lane);
    }

    public void MoveRight()
    {
        lane = Mathf.Clamp(lane + 1, minLane, maxLane);
        currentOffset = lane * laneOffset;
        lateralVel = Vector3.zero;

        if (debugLogs)
            Debug.Log("MoveRight -> lane " + lane);
    }

    public void RecenterLanes()
    {
        if (moveTarget == null) moveTarget = transform;

        baseWorldPos = moveTarget.position;
        lane = 0;
        currentOffset = 0f;
        lateralVel = Vector3.zero;

        if (debugLogs)
            Debug.Log("RecenterLanes called. New baseWorldPos = " + baseWorldPos);
    }

    public void SetSwipeInputEnabled(bool enabled)
    {
        acceptSwipeInput = enabled;
    }
}