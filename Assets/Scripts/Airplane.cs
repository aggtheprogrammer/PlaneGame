using UnityEngine;

public class Airplane : MonoBehaviour
{
    public EnvironmentMover envMover;

    // take off
    public const float takeoffDelay = 0.01f;    
    public const float maxPitch = 25f;
    public const float climbSpeed = 20f; 
    public const float pitchSpeed = 14f;
    public const float targetAltitude = 10f; 
    public const float maxTime = 6f;
    public float holdDuration = 1.5f;

    public SwipeLaneController laneController;
    bool recentered;
    private Vector3 startPos;
    private float timer;
    private bool begunPitch;
    private bool leveled;
    private float currentPitch;

    private float lockedYaw;
    private float lockedRoll;


    void Start()
{
    startPos = transform.localPosition; // for Y reference only
    timer = 0f;
    Time.timeScale = 1f;
    leveled = false;

    Vector3 startEuler = transform.eulerAngles;
    lockedYaw = startEuler.y;
    lockedRoll = startEuler.z;


    // trigger pitch immediately
    if (envMover == null)
        envMover = FindObjectOfType<EnvironmentMover>();
    if (envMover != null)
        envMover.StartDescending();
    if (laneController == null)
        laneController = GetComponentInParent<SwipeLaneController>();

    Debug.Log("Airplane Start() ran!"); // for testing

}

    void Update()
    {
        timer += Time.deltaTime;

        // trigger nose up exactly at 1 s
        if (!begunPitch && timer >= takeoffDelay)
        {
            begunPitch = true;
        }

        if (begunPitch && !leveled)
        {
            // tilt nose up smoothly
            float t = Mathf.Clamp01((timer - takeoffDelay) / maxTime);
            currentPitch = Mathf.Lerp(0f, maxPitch, t);

            // climb upward smoothly
            float targetY = startPos.y + targetAltitude;
            float newY = Mathf.MoveTowards(transform.position.y, targetY, climbSpeed * Time.deltaTime); 
            Vector3 pos = transform.position;  // keep current X and Z (lane position)
            pos.y = newY;                       // only change height
            transform.position = pos;


            // after reaching max pitch, hold it for a few seconds before leveling off
            float holdDuration = 5f; // seconds to stay at this angle
            if (timer >= takeoffDelay + maxTime + holdDuration)
                leveled = true;

        }
        else if (leveled)
        {
            // return nose to level
            currentPitch = Mathf.MoveTowards(currentPitch, 0f, pitchSpeed * Time.deltaTime);
        }

        // apply rotation
        Vector3 e = transform.eulerAngles; // keep current yaw + roll
        transform.rotation = Quaternion.Euler(-currentPitch, lockedYaw, lockedRoll);



        //if (Time.frameCount % 60 == 0)
           // Debug.Log("Airplane Update running, y=" + transform.position.y); // for testing

        if (!recentered && transform.position.y >= (startPos.y + targetAltitude - 0.01f))
    {
        recentered = true;
        if (laneController != null)
            laneController.RecenterLanes();
    }

    }

    // tell EnvironmentMover when nose up started
    public bool IsPitchingUp()
    {
        return begunPitch && !leveled;
    }
}
