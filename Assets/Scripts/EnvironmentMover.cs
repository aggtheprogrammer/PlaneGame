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

    private float currentFallSpeed = 0f;
    private bool descending = false;
    private float descendTimer = 0f;

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
        // accelerate movement along runway (X)
        if (speed < maxSpeed)
            speed += acceleration * Time.deltaTime;

        // move runway backwards along X
        transform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);

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
