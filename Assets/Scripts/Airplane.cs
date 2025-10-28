using UnityEngine;

public class Airplane : MonoBehaviour
{
    public bool autoStart = true;
    public float delay = 1.5f;
    public KeyCode manualKey = KeyCode.Space;

    //nose up
    public float maxPitch = 20f;      // how high to tilt the nose
    public float pitchSpeed = 25f;    // deg/sec

    public float targetAltitude = 8f;
    public float climbSpeed = 2f;

    //during take off
    public bool keepSmallBob = true;
    public float bobAmp = 0.2f;
    public float bobFreq = 1.2f;

    //leveling off time
    public float maxTime = 10f;       // seconds before leveling out

    float startY;
    float timer;
    bool started;
    bool leveled;
    float currentPitch;

    void Start()
    {
        startY = transform.position.y;
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (!started)
        {
            if ((autoStart && timer >= delay) || (!autoStart && Input.GetKeyDown(manualKey)))
                started = true;
            else
                return;
        }

        if (started && !leveled) // taking off
        {
            // tilt nose UP 
            currentPitch = Mathf.MoveTowards(currentPitch, -maxPitch, pitchSpeed * Time.deltaTime);

            // climb upward
            float targetY = startY + targetAltitude;
            float newY = Mathf.MoveTowards(transform.position.y, targetY, climbSpeed * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);

            // after maxTime, start leveling
            if (timer >= delay + maxTime)
                leveled = true;
        }
        
        else
        {
            currentPitch = Mathf.MoveTowards(currentPitch, 0f, pitchSpeed * Time.deltaTime);

            if (keepSmallBob)
            {
                float y = startY + targetAltitude +
                          Mathf.Sin(Time.time * Mathf.PI * 2f * bobFreq) * bobAmp;
                transform.position = new Vector3(transform.position.x, y, transform.position.z);
            }
        }

        transform.rotation = Quaternion.Euler(currentPitch, 0f, 0f);
    }
}
