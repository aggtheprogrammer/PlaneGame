using UnityEngine;

public class Airplane : MonoBehaviour
{
    public EnvironmentMover envMover;

    //take off 
    private const float takeoffDelay = 1.0f;    // nose-up begins at 1 s
    private const float maxPitch = 25f;
    private const float climbSpeed = 3.2f;
    private const float pitchSpeed = 14f;
    private const float targetAltitude = 18f;
    private const float maxTime = 13.5f;        

    private Vector3 startPos;
    private float timer;
    private bool begunPitch;
    private bool leveled;
    private float currentPitch;

    void Start()
    {
        startPos = transform.position;
        timer = 0f;
        Time.timeScale = 1f;
        begunPitch = false;
        leveled = false;
    }

    void Update()
    {
        timer += Time.deltaTime;

        // trigger nose up exactly at 1 s
        if (!begunPitch && timer >= takeoffDelay)
        {
            begunPitch = true;
            if (envMover != null)
                envMover.StartDescending();
        }

        if (begunPitch && !leveled)
        {
            // tilt nose up smoothly 
            float t = Mathf.Clamp01((timer - takeoffDelay) / maxTime);
            currentPitch = Mathf.Lerp(0f, maxPitch, t * t);

            // climb upward smoothly
            float targetY = startPos.y + targetAltitude;
            float newY = Mathf.MoveTowards(transform.position.y, targetY, climbSpeed * Time.deltaTime);
            transform.position = new Vector3(startPos.x, newY, startPos.z);

            // after maxTime, begin leveling off
            if (timer >= takeoffDelay + maxTime)
                leveled = true;
        }
        else if (leveled)
        {
            // return nose to level
            currentPitch = Mathf.MoveTowards(currentPitch, 0f, pitchSpeed * Time.deltaTime);
        }

        // apply rotation 
        transform.rotation = Quaternion.Euler(-currentPitch, transform.rotation.eulerAngles.y, 0f);
    }
}
