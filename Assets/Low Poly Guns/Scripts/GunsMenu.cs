using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GunsMenu : MonoBehaviour
{
    //public GameObject Buttons;
    public GameObject[] Weapons;
    int currentWeapon = 0;
    void Start()
    {
        Weapons[0].SetActive(true);
    }

    public void NextGun()
    {
        Weapons[currentWeapon].SetActive(false);
        currentWeapon++;
        if (currentWeapon >= Weapons.Length)
            currentWeapon = 0;
        Weapons[currentWeapon].SetActive(true);
    }
    public void PreviousGun()
    {
        Weapons[currentWeapon].SetActive(false);
        currentWeapon--;
        if (currentWeapon < 0)
            currentWeapon = Weapons.Length - 1;
        Weapons[currentWeapon].SetActive(true);
    }

    public void ChangeWeapon()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            Weapons[currentWeapon] = Weapons[0];
            Debug.Log("weapon 1");
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            Weapons[currentWeapon] = Weapons[1];
            Debug.Log(Weapons[currentWeapon].name);
        }
        if(Input.GetKey(KeyCode.Alpha3))
        {
            Weapons[currentWeapon] = Weapons[2];
            Debug.Log(Weapons[currentWeapon].name);
        }
        if (Input.GetKey(KeyCode.Alpha4))
        {
            Weapons[currentWeapon] = Weapons[3];
            Debug.Log(Weapons[currentWeapon].name);
        }
    }

    private void Update()
    {
        //if ((Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)))
        //{
        //    Buttons.SetActive(false);
        //}
        //else if(Input.touchCount == 0 && !Input.GetMouseButton(0))
        //{
        //    Buttons.SetActive(true);
        //}
    }
}
