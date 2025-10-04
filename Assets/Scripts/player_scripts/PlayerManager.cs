using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public GameObject MainInventory;
    public Action interactwithObject;
    private IInteractable currentInteractable;
    void Start()
    {
        MainInventory.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            if (MainInventory.activeSelf == false) { MainInventory.SetActive(true); Cursor.visible = true; Cursor.lockState = CursorLockMode.None; }
            else if (MainInventory.activeInHierarchy) { MainInventory.SetActive(false); Cursor.visible = false; Cursor.lockState = CursorLockMode.Locked;}
        }
        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (context.action.name != "Interact") return;
        if (context.performed)
        {
            interactwithObject();
        }
    }
}
