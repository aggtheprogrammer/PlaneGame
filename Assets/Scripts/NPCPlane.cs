using UnityEngine;

public class NPCPlane : MonoBehaviour
{
    public float speed = 5f;
    public float MoveDuration = 10f;
    private float moveTimer;

    void Update()
    {
        moveTimer += Time.deltaTime;
        if (moveTimer < MoveDuration)
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
    }
}
