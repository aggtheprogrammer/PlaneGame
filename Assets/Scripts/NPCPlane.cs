using UnityEngine;

public class NPCPlane : MonoBehaviour
{
    [Header("Runway Movement")]
    public float forwardSpeed = 15f;        // horizontal runway speed
    public float acceleration = 5f;         // acceleration while rolling
    public float maxForwardSpeed = 35f;
    public float moveDuration = 10f;   

    [Header("Takeoff Settings")]
    public float takeoffDelay = 3f;         // seconds before nose-up begins
    public float climbSpeed = 3f;           // vertical lift speed
    public float pitchSpeed = 20f;          // how fast nose tilts up
    public float maxPitch = 25f;            // maximum nose-up angle

    [Header("Lifetime")]
    public float flightDuration = 12f;      // how long until plane disappears

    private float timer = 0f;
    private bool takingOff = false;
    private bool airborne = false;
    private float currentPitch = 0f;
    private Vector3 startPos;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer < moveDuration)
        {
            transform.Translate(Vector3.right * forwardSpeed * Time.deltaTime);


            // accelerate while on ground
            if (!airborne)
            {
                forwardSpeed = Mathf.MoveTowards(forwardSpeed, maxForwardSpeed, acceleration * Time.deltaTime);
            }

            // always roll forward (local X direction)
            transform.Translate(Vector3.right * forwardSpeed * Time.deltaTime, Space.World);

            // start takeoff after delay
            if (!takingOff && timer >= takeoffDelay)
                takingOff = true;

            // nose-up phase
            if (takingOff && !airborne)
            {
                // smooth pitch up
                currentPitch = Mathf.MoveTowards(currentPitch, maxPitch, pitchSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Euler(-currentPitch, transform.rotation.eulerAngles.y, 0f);

                // start lifting off after some pitch is visible
                if (currentPitch >= maxPitch * 0.6f)
                    airborne = true;
            }

            // climb upward once airborne
            if (airborne)
            {
                transform.Translate(Vector3.up * climbSpeed * Time.deltaTime, Space.World);
            }
        }
        // remove plane after flightDuration
        else 
            {
            Destroy(gameObject);
            }
    }
}
