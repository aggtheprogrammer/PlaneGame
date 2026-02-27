using UnityEngine;

public class LaneDebugMove : MonoBehaviour
{
    public float step = 400f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Vector3 p = transform.localPosition;
            p.x -= step;
            transform.localPosition = p;
            Debug.Log("A pressed -> " + transform.localPosition);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            Vector3 p = transform.localPosition;
            p.x += step;
            transform.localPosition = p;
            Debug.Log("D pressed -> " + transform.localPosition);
        }
    }
}