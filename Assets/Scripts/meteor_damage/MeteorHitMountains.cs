using UnityEngine;

public class MeteorHitMountains : MonoBehaviour
{
    public Transform target;
    public float hitRange = 25.0f;
    public float damageReduce = 0;
    public GameObject meteor;
    void Start()
    {
        meteor = GameObject.FindWithTag("Meteor");
    }

    void Update()
    {
        if (target && Vector3.Distance(transform.position, target.position) <= hitRange)
        {
            damageReduce = gameObject.GetComponent<HitMountains>().mountainReduceDamage;
        }

    }
}
