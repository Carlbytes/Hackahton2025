using UnityEngine;

public class MeteorDamage : MonoBehaviour
{
    public double pop; //population will equal gameobjects population
    public MeteorStats metStats;

    private void Start()
    {
        RegionTemplate regTem = gameObject.GetComponent<RegionTemplate>();
        if (regTem != null)
        {
            pop = regTem.population;
            Debug.Log("Value: " + pop);
        }
    }
    void takeDamage()
    {
        pop -= (pop/100)*metStats.meteorDamage; //takes away a percentage of the population
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Meteor")
        {
            takeDamage();
            //show clipboard
        }
    }
}
