using UnityEngine;

public class EnvironmentMover : MonoBehaviour
{
    public Transform airplane;            // drag the Airbus here

    public float descendDelay = 8f;       // start descending BEFORE nose-up (plane’s noseUpDelay = 10)
    private float timer = 0f;
    private bool descending = false;

    // backward movement
    public float speed = 10f;
    public float acceleration = 3f;
    public float maxSpeed = 60f;

    //downward movement
    public float fallSpeed = 0f;
    public float fallAcceleration = 3f;   
    public float maxFallSpeed = 25f;      
    public float verticalOffset = 4f;     // how far below the plane the environment should sit

    void Update()
    {
        timer += Time.deltaTime;

        // always move backwards
        if (speed < maxSpeed)
            speed += acceleration * Time.deltaTime;

        transform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);

        // trigger descent a bit earlier (8s)
        if (!descending && timer >= descendDelay)
            descending = true;

        // follow plane’s height 
        if (descending && airplane != null)
        {
            float targetY = airplane.position.y - verticalOffset;
            Vector3 pos = transform.position;

            // move fast toward target Y
            pos.y = Mathf.MoveTowards(pos.y, targetY, maxFallSpeed * Time.deltaTime);
            transform.position = pos;
        }
    }
    public void StartDescending()
{
    descending = true;
}
}
