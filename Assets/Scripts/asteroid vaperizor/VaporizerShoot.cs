using UnityEngine;

public class VaporizerShoot : MonoBehaviour
{
    public GameObject projPrefab;        

    [Header("Shooting Settings")]
    public float projSpeed = 100f;
    public float shootDelay = 2f;         
    public float maxTargetRange = 1000f; 

    private bool readyToShoot = true;

    private void Update()
    {
        //MyInput();
    }

    public void MyInput()
    {
        //bool shootButtonPressed = Input.GetKeyDown(KeyCode.Space); //placeholder

        //if (shootButtonPressed)
        //{
            ShootAtNearestMeteor();
        //}
    }

    private void ShootAtNearestMeteor()
    {
        GameObject nearestMeteor = FindNearestMeteor();
        if (nearestMeteor == null)
        {
            //Debug.Log("No meteors in range");
            return;
        }

        // Compute direction to meteor
        Vector3 direction = (nearestMeteor.transform.position - gameObject.transform.position).normalized;

        // Spawn projectile
        GameObject proj = Instantiate(projPrefab, gameObject.transform.position, Quaternion.LookRotation(direction));

        // Apply velocity
        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direction * projSpeed;
        }

        Debug.Log("fired at " + nearestMeteor.name);

        readyToShoot = false;
        Invoke(nameof(ResetShot), shootDelay);
    }

    private GameObject FindNearestMeteor()
    {
        GameObject[] meteors = GameObject.FindGameObjectsWithTag("Meteor");
        if (meteors.Length == 0) return null;

        GameObject nearest = null;
        float minDistanceSqr = Mathf.Infinity;
        Vector3 origin = transform.position;

        foreach (GameObject meteor in meteors)
        {
            if (meteor == null) continue;

            float distSqr = (meteor.transform.position - origin).sqrMagnitude;
            if (distSqr < minDistanceSqr && distSqr <= maxTargetRange * maxTargetRange)
            {
                nearest = meteor;
                minDistanceSqr = distSqr;
            }
        }

        return nearest;
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }
}
