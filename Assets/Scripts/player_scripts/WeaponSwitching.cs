 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{
    public int selectedWeapon = 0;
    public GameObject hotbar1, hotbar2, hotbar3, hotbar4;
    void Start()
    {
        hotbar1.SetActive(true); hotbar2.SetActive(false); hotbar3.SetActive(false); hotbar4.SetActive(false);
        SelectWeapon();
    }

    
    void Update()
    {
        int previousSelectedWeapon = selectedWeapon;

        //change weapons with scroll wheel
        if(Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if(selectedWeapon >= transform.childCount - 1)
                selectedWeapon = 0;
            else
                selectedWeapon++;   
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (selectedWeapon >= transform.childCount - 1)
                selectedWeapon = 0;
            else
                selectedWeapon--;
        }

        //change weapon using number keys
        if (Input.GetKeyDown(KeyCode.Alpha1)){ 
            selectedWeapon = 0; hotbar1.SetActive(true); hotbar2.SetActive(false); hotbar3.SetActive(false); hotbar4.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 2){
            selectedWeapon = 1; hotbar1.SetActive(false); hotbar2.SetActive(true); hotbar3.SetActive(false); hotbar4.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && transform.childCount >= 3){
            selectedWeapon = 2; hotbar1.SetActive(false); hotbar2.SetActive(false); hotbar3.SetActive(true); hotbar4.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && transform.childCount >= 4){
            selectedWeapon = 3; hotbar1.SetActive(false); hotbar2.SetActive(false); hotbar3.SetActive(false); hotbar4.SetActive(true);
        }

        if (previousSelectedWeapon != selectedWeapon)
        {
            SelectWeapon();
        }
    }

    void SelectWeapon()
    {
        int i = 0;
        foreach (Transform weapon in transform)
        {
            if(i == selectedWeapon)
                weapon.gameObject.SetActive(true);
            else
                weapon.gameObject.SetActive(false);
            i++;
        }
    }
}
