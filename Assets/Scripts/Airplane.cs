using UnityEngine;

public class Airplane : MonoBehaviour
{
    public bool autoStart = true;
    public float delay = 1.5f;
    public KeyCode manualKey = KeyCode.Space;

    //take off
    public float maxPitch = 20f;        // nose tilt degrees
    public float pitchSpeed = 25f;      // deg/sec
    public float targetAltitude = 8f;   
    public float climbSpeed = 2f;      

    //level flight and bob motion
    public float maxTime = 6;         // seconds before leveling
    public bool keepSmallBob = true;
    public float bobAmp = 0.2f;
    public float bobFreq = 1.2f;


    private Vector3 startPos;           // stores the full starting position (x,y,z)
    private float timer;
    private bool started;
    private bool leveled;
    private float currentPitch;

    void Start()
    {
        startPos = transform.position;  // keep full 3D start position
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Wait for auto or manual start
        if (!started)
        {
            if ((autoStart && timer >= delay) || (!autoStart && Input.GetKeyDown(manualKey)))
                started = true;
            else
                return;
        }

        if (started && !leveled)
        {
            // tilt nose UP 
            currentPitch = Mathf.MoveTowards(currentPitch, maxPitch, pitchSpeed * Time.deltaTime);

            // climb upward smoothly relative to start Y
            float targetY = startPos.y + targetAltitude;
            float newY = Mathf.MoveTowards(transform.position.y, targetY, climbSpeed * Time.deltaTime);

            transform.position = new Vector3(startPos.x, newY, startPos.z);

            // after maxTime, begin leveling off
            if (timer >= delay + maxTime)
                leveled = true;
        }
        else
        {
            // return nose to level
            currentPitch = Mathf.MoveTowards(currentPitch, 0f, pitchSpeed * Time.deltaTime);

            // optional small floating motion once leveled
            if (keepSmallBob)
            {
                float y = startPos.y + targetAltitude +
                          Mathf.Sin(Time.time * Mathf.PI * 2f * bobFreq) * bobAmp;
                transform.position = new Vector3(startPos.x, y, startPos.z);
            }
        }

        transform.rotation = Quaternion.Euler(-currentPitch, transform.rotation.eulerAngles.y, 0f);
    }
}
