using UnityEngine;

public class CloudRecycler : MonoBehaviour
{
    public Transform cloudA;
    public Transform cloudB;

    public float speed = 60f;       
    public float tileLength = 500f;  
    public float recycleZ = -500f;   

    public Transform directionSource; 

    void Update()
    {
        Vector3 move = -directionSource.forward * speed * Time.deltaTime;

        cloudA.position += move;
        cloudB.position += move;

        if (cloudA.position.z < recycleZ)
            cloudA.position = new Vector3(cloudA.position.x, cloudA.position.y, cloudB.position.z + tileLength);

        if (cloudB.position.z < recycleZ)
            cloudB.position = new Vector3(cloudB.position.x, cloudB.position.y, cloudA.position.z + tileLength);
    }
}