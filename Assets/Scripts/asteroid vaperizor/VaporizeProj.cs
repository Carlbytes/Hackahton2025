using UnityEngine;

public class VapoizeProj : MonoBehaviour
{
    public float lifetime = 10f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Meteor")
        {
            Destroy(gameObject);
            Destroy(collision.gameObject);
        }
    }
}
