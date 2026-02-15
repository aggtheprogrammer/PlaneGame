using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeLaneController : MonoBehaviour
{
    [Header("Move This")]
    public Transform moveTarget;

    [Header("Lanes")]
    public float laneOffset = 400f;   // keep 400 if your scene scale needs it
    public int minLane = -1;
    public int maxLane = 1;

    [Header("Movement")]
    public float lateralSmoothTime = 0.08f;

    [Header("Swipe")]
    public float minSwipePixels = 80f;
    public float maxSwipeTime = 0.35f;

    [Header("Debug")]
    public bool debugLogs = true;

    [Header("Lane Direction")]
    public Transform forwardReference; // drag a RUNWAY forward marker / airport forward object here
    public bool invertLanes = false;

    [Header("Rotation Lock (optional)")]
    public Transform rotationTarget;   // drag Airbus340 here
    public bool lockYawRoll = true;

    // state
    int lane = 0;
    Vector2 startPos;
    float startTime;
    bool swiping;

    float baseYaw;
    float baseRoll;

    Vector3 basePos;
    Vector3 lateralVel;

    [Header("Bank (tilt)")]
    public float maxBankAngle = 12f;      // how much it tilts (degrees)
    public float bankSmoothTime = 0.15f;   // how smooth the tilt is
    public float bankFromError = 0.03f;   // how strong the tilt reacts to lane error

    float bankVel;
    float currentBank;


    void Start()
    {
        if (moveTarget == null) moveTarget = transform;

        // lane 0 reference position
        basePos = moveTarget.position;

        if (rotationTarget == null)
            rotationTarget = transform;

        baseYaw = rotationTarget.localEulerAngles.y;
        baseRoll = rotationTarget.localEulerAngles.z;
    }

    void Update()
    {
        HandleSwipeInput();
    }

   void LateUpdate()
    {
        if (moveTarget == null) return;

        // Forward direction of the runway/world
        Vector3 forward = forwardReference != null ? forwardReference.forward : Vector3.forward;
        forward.y = 0f;
        if (forward.sqrMagnitude < 0.0001f) forward = Vector3.forward;
        forward.Normalize();

        // Sideways direction = perpendicular to forward (force perfectly horizontal)
        Vector3 right = Vector3.Cross(forward, Vector3.up);

        right.y = 0f;
        right.Normalize();

        if (invertLanes) 
            right = -right;

        // Target lane position (lock Y to the ORIGINAL lane root height)
        Vector3 targetPos = basePos + right * (lane * laneOffset);
        targetPos.y = basePos.y;

        // Smooth move
        Vector3 newPos = Vector3.SmoothDamp(moveTarget.position, targetPos, ref lateralVel, lateralSmoothTime);

        // Hard lock Y + kill any Y velocity accumulation
        newPos.y = basePos.y;
        lateralVel.y = 0f;

        moveTarget.position = newPos;

        // ----- BANK (tilt) based on lane target (stable) -----
        float desiredBank = -lane * maxBankAngle; // flip sign if it feels reversed
        currentBank = Mathf.SmoothDampAngle(currentBank, desiredBank, ref bankVel, bankSmoothTime);

        // When close enough, return bank to 0 smoothly
        float distToTarget = Vector3.Distance(moveTarget.position, targetPos);

        if (distToTarget < 5f)
        currentBank = Mathf.SmoothDampAngle(currentBank, 0f, ref bankVel, bankSmoothTime);

        // Apply bank ONLY to the visual (rotationTarget)
        if (rotationTarget != null)
        {
            Vector3 e = rotationTarget.localEulerAngles;
            rotationTarget.localEulerAngles = new Vector3(e.x, baseYaw, baseRoll + currentBank);
        }
    }

    void HandleSwipeInput()
    {
        // touch
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            // if UI blocks you, keep commented while testing:
            // if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(t.fingerId)) return;

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
        // if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

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

    // Call this after Start button if your objects teleport on start
    public void RecenterLanes()
    {
        if (moveTarget == null) moveTarget = transform;
        basePos = moveTarget.position;
        lane = 0;
    }
}
