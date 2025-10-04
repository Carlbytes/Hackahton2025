using UnityEngine;

public class MeteroMovement : MonoBehaviour
{
    public float speed = 3;
    public float startX, startY, startZ;
    public float targetX, targetY, targetZ;

    void Start()
    {
        startX = Random.Range(100.0f, 0.0f);
        startY = Random.Range(100.0f, 0.0f);
        startZ = Random.Range(100.0f, 0.0f);

        gameObject.transform.position = new Vector3(startX, startY, startZ);

        targetX = Random.Range(0.0f, -100.0f);
        targetY = Random.Range(0.0f, -100.0f);
        targetZ = Random.Range(0.0f, -100.0f);
    }

    void Update()
    {
        
    }

    private void DestroyAndCreate()
    {
        Destroy(gameObject);
        Instantiate(gameObject);
    }
}
