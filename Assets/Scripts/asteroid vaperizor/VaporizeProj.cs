using System.Collections;
using UnityEngine;

public class VapoizeProj : MonoBehaviour
{
    public float lifetime = 45f;
    public float speed = 100f;
    public float turnSpeed = 5f;
    public AudioSource meteorExplodeAudio;

    private GameObject target;
    private bool fired = false;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        if (fired && target != null)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);

            rb.linearVelocity = transform.forward * speed;
        }
        else if (fired && target == null)
        {
            rb.linearVelocity = transform.forward * speed;
        }
    }

    public void FireProjectile(GameObject launcher, GameObject newTarget, int damage)
    {
        if (launcher && newTarget)
        {
            target = newTarget;
            fired = true;
            Debug.Log("locked on to " + newTarget.name);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Meteor"))
        {
            if (meteorExplodeAudio != null && meteorExplodeAudio.clip != null)
            {
                AudioSource.PlayClipAtPoint(meteorExplodeAudio.clip, transform.position);
            }

            Destroy(collision.gameObject);
            Destroy(gameObject, meteorExplodeAudio.clip.length);
            Debug.Log("destroyed meteor");
        }
    }
}
