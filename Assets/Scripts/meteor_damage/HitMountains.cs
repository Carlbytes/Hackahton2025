using UnityEngine;

public class HitMountains : MonoBehaviour
{
    public double mountainSize; //km^2
    public float mountainReduceDamage; //% of damage mitigated
    public HitMountains(double mountainSize)
    {
        this.mountainSize = mountainSize;
    }

    public HitMountains(float mountainReduceDamage)
    {
        this.mountainReduceDamage = mountainReduceDamage;
    }
}
