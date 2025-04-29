using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private LayerMask pickableLayerMask;
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private GameObject pickUpUI;
    [SerializeField] [Min(1)] private float hitRange = 3;
    [SerializeField] private Transform pickUpParent;
    [SerializeField] private GameObject inHandItem;
    [SerializeField] private InputActionReference interactionInput, dropInput, useInput;
    private RaycastHit hit;


    private void Start()
    {
        //Starting causes interaction system to assign functions
        interactionInput.action.performed += PickUp;
        dropInput.action.performed += Drop;
        useInput.action.performed += Use;
    }

    //Funtion meant to interaction one object in hand with one in sim
    private void Use(InputAction.CallbackContext obj){
            IUsable usable = hit.collider.GetComponent<IUsable>();
            //If the object has a Use() function, calls it
            //Currently does nothing 
            if (usable != null){
                usable.Use(this.gameObject);
            }
    }

    //Function to drop any item currently in inHandItem slot, does nothing
    // if empty
    private void Drop(InputAction.CallbackContext obj){
        if (inHandItem != null){
            //Drops object at inHandItem slot position, maybe put it slightly in front of character
            inHandItem.transform.SetParent(null);
            inHandItem = null;
            //If object has a rigid body, make the object be affected by physics once dropped
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb != null){
                rb.isKinematic = false;
            }
        }
    }

    //Function to pick up an object in the interactable layer
    //Object must include script IPickupableItem
    private void PickUp(InputAction.CallbackContext obj){
        //Players need to be actively looking at object and have their hands slot free to pick up
        if(hit.collider != null && inHandItem == null){
            IPickable pickableItem = hit.collider.GetComponent<IPickable>();
            if (pickableItem != null){
                //Places object in hand slot & sets slot to parent so object moves with player
                inHandItem = pickableItem.PickUp();
                inHandItem.transform.SetParent(pickUpParent.transform, pickableItem.KeepWorldPosition);
            }
        }
    }

    //Function to detect if the camera is looking at an interactable object
    //Uses raycast to check every second
    //IF PickUpUI NOT FILLED RAYCAST DOES NOT UPDATE
    private void Update(){
        //Line below shows the raycast line but can be removed without issue
        Debug.DrawRay(playerCameraTransform.position, playerCameraTransform.forward * hitRange, Color.red);
        //Shows the pickUpUI if the raycast detect something
        if (hit.collider != null){
            pickUpUI.SetActive(false);
        }
        //Updates inHandItem slot for other functions for pickup and drop
        if (inHandItem != null){
            return;
        }
        //Keeps the raycast in front of the camera
        if (Physics.Raycast(
            playerCameraTransform.position, 
            playerCameraTransform.forward, 
            out hit, 
            hitRange, 
            pickableLayerMask))
        {
            pickUpUI.SetActive(true);
        }
    }
}