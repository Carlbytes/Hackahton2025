using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class rotationTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float rotationalSpeed;
    public GameObject sunPivotObject;
    // Start is called before the first frame update
    void Start()
    {

        rotationalSpeed = Random.Range(-15.0f, -5.0f);

    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(sunPivotObject.transform.position, Vector3.up, rotationalSpeed * Time.deltaTime);

        transform.LookAt(sunPivotObject.transform.position);
    }


}
