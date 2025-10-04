using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class TempPlayInv : MonoBehaviour
{
    //temporary inventory system

    //public GameObject Player;
    //public bool hasUmbrella = false;
    //[SerializeField] Sprite umbrellaIcon;
    //[SerializeField] Image Hotbar1, Hotbar2, Hotbar3, Hotbar4;
    [SerializeField] InventorySystem inventorySystem;

    private void Update()
    {
        //if(hasUmbrella == true)
        //{

        //}
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "umbrellaPickup")
        {
            //hasUmbrella = true;
            InventoryItemData itemData = collision.gameObject.GetComponent<InventoryItemData>();

            if (collision != null)
            {
                //InventorySystem.Add(InventoryItemData itemData);
                Destroy(collision.gameObject);
            }
        }
    }
}
