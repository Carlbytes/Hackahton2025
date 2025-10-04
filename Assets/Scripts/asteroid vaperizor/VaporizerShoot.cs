using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VaporizerShoot : MonoBehaviour
{
    //gun stats
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;

    //bools
    bool shooting, readyToShoot, reloading;


    //reference
    public Camera fpsCam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;

    private void Awake()
    {
        readyToShoot = true;
    }

    private void Update()
    {

        MyInput();
    }

    private void MyInput()
    {
        if (1+1==2)//PLACEHOLDER IF STATMENT, NEEDS TO BE IF CERTAIN BUTTON PRESSED
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }
}
