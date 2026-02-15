using UnityEngine;

public class SwipeWorldLaneController : MonoBehaviour
{
    [Header("Move This (WorldLateralRoot)")]
    public Transform worldLateralRoot;

    [Header("Lanes")]
    public float laneOffset = 400f;
    public int minLane = -1;
    public int maxLane = 1;

    [Header("Movement")]
    public float lateralSmoothTime = 0.08f;

    [Header("Swipe")]
    public float minSwipePixels = 80f;
    public float maxSwipeTime = 0.35f;

    [Header("Direction Reference")]
    // Assign Airbus340 here
    public Transform forwardReference;
    public bool invertLanes = false;

    [Header("Start Gate")]
    [SerializeField] private bool canSwipe = false;   // false until Start pressed

    [Header("Debug")]
    public bool debugLogs = true;

    int lane = 0;
    Vector2 startPos;
    float startTime;
    bool swiping;

    Vector3 baseLocalPos;
    Vector3 localVel;

    Transform R => worldLateralRoot != null ? worldLateralRoot : transform;

    void Awake()
    {
        if (worldLateralRoot == null) worldLateralRoot = transform;

        // auto-find plane if not assigned
        if (forwardReference == null)
        {
            Airplane p = FindObjectOfType<Airplane>();
            if (p != null) forwardReference = p.transform;
        }
    }

    void OnEnable()
    {
        baseLocalPos = R.localPosition;
        localVel = Vector3.zero;
        lane = Mathf.Clamp(lane, minLane, maxLane);
    }

    void Start()
    {
        baseLocalPos = R.localPosition;
    }

    public void SetSwipeEnabled(bool enabled)
    {
        canSwipe = enabled;
        if (debugLogs) Debug.Log("[Swipe] canSwipe = " + canSwipe);
    }

    void Update()
    {
        if (!canSwipe) return;
        HandleSwipe();
    }

    void LateUpdate()
    {
        // Lateral axis = right of plane heading (not fixed local X)
        Vector3 forwardW = forwardReference != null ? forwardReference.forward : Vector3.forward;
        forwardW.y = 0f;
        if (forwardW.sqrMagnitude < 0.0001f) forwardW = Vector3.forward;
        forwardW.Normalize();

        // correct right vector in Unity
        Vector3 rightW = Vector3.Cross(Vector3.up, forwardW);
        rightW.y = 0f;
        rightW.Normalize();

        Transform parent = R.parent;
        Vector3 rightLocal = parent != null ? parent.InverseTransformDirection(rightW) : rightW;
        rightLocal.y = 0f;
        if (rightLocal.sqrMagnitude < 0.0001f) rightLocal = Vector3.right;
        rightLocal.Normalize();

        // Move WORLD opposite to plane lane input
        float signedOffset = -lane * laneOffset;
        if (invertLanes) signedOffset = -signedOffset;

        Vector3 targetLocal = baseLocalPos + rightLocal * signedOffset;

        Vector3 newLocal = Vector3.SmoothDamp(R.localPosition, targetLocal, ref localVel, lateralSmoothTime);
        newLocal.y = baseLocalPos.y; // keep same height
        R.localPosition = newLocal;
    }

    void HandleSwipe()
    {
        // Touch
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began)
            {
                swiping = true;
                startPos = t.position;
                startTime = Time.unscaledTime;
                if (debugLogs) Debug.Log("[Swipe] Touch begin: " + startPos);
            }
            else if (swiping && (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled))
            {
                EvaluateSwipe(t.position);
                swiping = false;
            }
            return;
        }

        // Mouse (Editor)
        if (Input.GetMouseButtonDown(0))
        {
            swiping = true;
            startPos = Input.mousePosition;
            startTime = Time.unscaledTime;
            if (debugLogs) Debug.Log("[Swipe] Mouse begin: " + startPos);
        }
        else if (swiping && Input.GetMouseButtonUp(0))
        {
            EvaluateSwipe((Vector2)Input.mousePosition);
            swiping = false;
        }
    }

    void EvaluateSwipe(Vector2 endPos)
    {
        float dt = Time.unscaledTime - startTime;
        Vector2 delta = endPos - startPos;

        if (debugLogs) Debug.Log($"[Swipe] End. dt={dt:F3}s, delta=({delta.x:F1},{delta.y:F1})");

        if (dt > maxSwipeTime) return;
        if (Mathf.Abs(delta.x) < minSwipePixels) return;
        if (Mathf.Abs(delta.x) < Mathf.Abs(delta.y)) return;

        int oldLane = lane;
        if (delta.x > 0f) lane = Mathf.Clamp(lane + 1, minLane, maxLane);
        else lane = Mathf.Clamp(lane - 1, minLane, maxLane);

        if (debugLogs) Debug.Log($"[Swipe] Lane {oldLane} -> {lane}");
    }

    public void Recenter()
    {
        baseLocalPos = R.localPosition;
        lane = 0;
        localVel = Vector3.zero;
        if (debugLogs) Debug.Log("[Swipe] Recentered.");
    }
}
