using JetBrains.Annotations;
using UnityEngine;

public class MeteorDamage : MonoBehaviour
{
    public double pop; //population will equal gameobjects population
    public GameObject meteor;

    private void Start()
    {
        RegionTemplate regTem = gameObject.GetComponent<RegionTemplate>();
        if (regTem != null)
        {
            pop = regTem.population;
            Debug.Log("Value: " + pop);
        }
    }

    private void Update()
    {
        meteor = GameObject.FindGameObjectWithTag("Meteor");
    }

    void takeDamage()
    {
        float metStats = meteor.GetComponent<MeteorStats>().meteorDamage;
        if(metStats == null) { Debug.Log("WARNIGN YOU FUCKED UP DIPSHIT "); }
        Debug.Log("pop: " + pop);
        pop -= (pop/metStats)*100; //takes away a percentage of the population OH THIS MATHS NEED TO BE FIXED WOW
        Debug.Log("new pop: "+pop);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Meteor")
        {
            Debug.Log("Collided");
            takeDamage();
            //show clipboard
        }
    }
}
