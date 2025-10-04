using UnityEngine;
using UnityEngine.UI;

public class MeteorStats : MonoBehaviour
{
    public float meteorSpeed, meteorDamage, meteorSize;

    public Slider meteorSpeedSlider, meteorSizeSlider;

    void selectStats()
    {

    }

    private void Update()
    {
        //will change to user input
        meteorSpeed = meteorSpeedSlider.value;
        meteorSize = meteorSizeSlider.value;

        meteorDamage = ((meteorSize + meteorSpeed) / 110) * 100.0f;
        //meteorDamage = (Random.Range(1, 10) * meteorSize * (meteorSpeed / 2));
    }
}
