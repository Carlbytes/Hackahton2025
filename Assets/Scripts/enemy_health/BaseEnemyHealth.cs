using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyHealth : MonoBehaviour
{
    public int health = 20;
    public void Hit(int hitAmount)
    {
        if (health >= 0)
        {
            health = health - hitAmount;
            Debug.Log("Health is now " + health + " for the " + gameObject.name);
        }
    }
}