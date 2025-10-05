using UnityEngine;

public class VaporizerShoot : MonoBehaviour
{
    public GameObject projPrefab;

    public float projSpeed = 100f;
    public float shootDelay = 2f;
    public float maxTargetRange = 10000f;

    private bool readyToShoot = true;

    public void FireButtonPressed()
    {
        if (readyToShoot)
            ShootAtNearestMeteor();
        else
            Debug.Log("not ready to shoot");
    }

    private void ShootAtNearestMeteor()
    {
        GameObject nearestMeteor = FindNearestMeteor();
        if (nearestMeteor == null)
        {
            Debug.Log("no meteors nearby");
            return;
        }

        Vector3 direction = (nearestMeteor.transform.position - transform.position).normalized;

        GameObject proj = Instantiate(projPrefab, transform.position, Quaternion.LookRotation(direction));

        VapoizeProj projectileScript = proj.GetComponent<VapoizeProj>();
        if (projectileScript != null)
        {
            projectileScript.FireProjectile(gameObject, nearestMeteor, 0);
        }

        // Give it an initial push (optional if using homing)
        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direction * projSpeed;
        }

        Debug.Log($"fired at {nearestMeteor.name}");

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
