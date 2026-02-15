using UnityEngine;

public class WorldForwardMoverSimple : MonoBehaviour
{
    [Header("Direction")]
    public Vector3 moveDirection = new Vector3(0, 0, 1);
    public Transform directionSource; // drag Airbus340 here (optional)

    [Header("Speed")]
    public float startSpeed = 300f;
    public float acceleration = 900f;
    public float maxSpeed = 3000f;

    float currentSpeed;

    void Start()
    {
        currentSpeed = startSpeed;
    }

    void Update()
    {
        float dt = Time.deltaTime;

        currentSpeed = Mathf.Min(maxSpeed, currentSpeed + acceleration * dt);

        Vector3 dir = moveDirection.normalized;

        if (directionSource != null)
        {
            dir = directionSource.TransformDirection(moveDirection);
            dir.y = 0f;              // keep it horizontal
            if (dir.sqrMagnitude > 0.0001f) dir.Normalize();
            else dir = Vector3.forward;
        }

        transform.position += dir * currentSpeed * dt;
    }
}
