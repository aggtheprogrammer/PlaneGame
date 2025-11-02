using UnityEngine;

public class NPCPlane : MonoBehaviour
{
    public float speed = 5f;
    public float MoveDuration = 10f; // duration to move in 10 seconds
    private float moveTimer;

    void Update()
    {
        moveTimer += Time.deltaTime;
        if (moveTimer < MoveDuration) // move for the specified duration
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
    }
}
