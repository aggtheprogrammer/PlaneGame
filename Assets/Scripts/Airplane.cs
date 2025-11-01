using UnityEngine;

public class Airplane : MonoBehaviour
{
    public EnvironmentMover envMover;

    // take off
    private const float takeoffDelay = 0.01f;    // nose up begins at 0.01 s
    private const float maxPitch = 25f;
    private const float climbSpeed = 3f;
    private const float pitchSpeed = 14f;
    private const float targetAltitude = 18f;
    private const float maxTime = 20f;

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
    begunPitch = true;
    leveled = false;

    // trigger pitch immediately
    begunPitch = true;
    if (envMover == null)
        envMover = FindObjectOfType<EnvironmentMover>();
    if (envMover != null)
        envMover.StartDescending();
}

    void Update()
    {
        timer += Time.deltaTime;

        // trigger nose up exactly at 1 s
        if (!begunPitch && timer >= takeoffDelay)
        {
            begunPitch = true;

            if (envMover != null)
            {
                envMover.StartDescending();
                Debug.Log("✅ Airplane triggered environment descent!");
            }
            else
            {
                Debug.LogWarning("⚠️ No EnvironmentMover found!");
            }
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
        transform.rotation = Quaternion.Euler(-currentPitch, transform.rotation.eulerAngles.y, 0f);
    }

    // tell EnvironmentMover when nose up started
    public bool IsPitchingUp()
    {
        return begunPitch && !leveled;
    }
}
