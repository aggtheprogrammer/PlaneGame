using UnityEngine;

public class EnvironmentMover : MonoBehaviour
{
    public Transform airplane;          
    public Airplane airplaneScript;     

    // movement along runway X
    public float speed = 10f;
    public float acceleration = 3f;
    public float maxSpeed = 60f;

    // downward movement Y
    public float fallAcceleration = 15f;
    public float maxFallSpeed = 40f;
    public float fallStartDelay = 0f;
    public float startSpeedOnStart = 60f; 
    public Vector3 moveDirection = new Vector3(0f, 0f, -1f); // world -Z (towards camera usually)
    public Transform directionSource; 


    private float currentFallSpeed = 0f;
    private bool descending = false;
    private float descendTimer = 0f;
    private bool running = false;

    public void StartMoving()
    {
        running = true;
        speed = startSpeedOnStart;   // instant "perfect" speed
    }



    void Start()
    {
        // if not manually assigned, try to auto-detect
        if (airplane == null)
        {
            Airplane foundPlane = FindObjectOfType<Airplane>();
            if (foundPlane != null)
            {
                airplane = foundPlane.transform;
                airplaneScript = foundPlane;
            }
        }
    }

    void Update()
    {
        if (!running)
        return;

        // accelerate movement along runway (X)
        if (speed < maxSpeed)
            speed += acceleration * Time.deltaTime;

        // move world backwards opposite of the airplane forward (on XZ plane only)
        Vector3 dir = (directionSource != null) ? directionSource.forward : Vector3.forward;
        dir.y = 0f; // IMPORTANT: no up/down
        dir = dir.normalized;

        transform.position += -dir * speed * Time.deltaTime;

        // check for airplane pitch-up if linked
        if (airplaneScript != null && airplaneScript.IsPitchingUp())
            descending = true;

        // handle descent
        if (descending)
        {
            descendTimer += Time.deltaTime;

            if (descendTimer >= fallStartDelay)
            {
                if (currentFallSpeed < maxFallSpeed)
                    currentFallSpeed += fallAcceleration * Time.deltaTime;

                // move downward along world Y axis
                transform.Translate(Vector3.down * currentFallSpeed * Time.deltaTime, Space.World);
            }
        }
    }

    public void StartDescending()
    {
        descending = true;
    }
}
