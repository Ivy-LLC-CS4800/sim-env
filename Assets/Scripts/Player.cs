using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
//<summary>
//Gives the player the ability to detect gameObjects they can use, pickup, and drop
//</summary>
public class Player : MonoBehaviour
{
    [SerializeField] private LayerMask pickableLayerMask;
    [SerializeField] private LayerMask useableLayerMask;
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private GameObject pickUpUI;
    [SerializeField] private GameObject useableUI;
    [SerializeField] [Min(1)] private float hitRange = 3;
    [SerializeField] private Transform pickUpParent;
    [SerializeField] private GameObject inHandItem;
    [SerializeField] private InputActionReference interactionInput, dropInput, useInput;
    private RaycastHit pickableHit;
    private RaycastHit useableHit;
    private Collider lastHit = null;

    //TODO: Assigns keybinds to specific functions
    //Parameters: Scene change and or application start
    private void Start()
    {
         if (interactionInput?.action != null)
        interactionInput.action.performed += PickUp;

        if (dropInput?.action != null)
        dropInput.action.performed += Drop;

        if (useInput?.action != null)
        useInput.action.performed += Use;
    }

    //TODO: gameObject in handSlot interacts with gameObjects on the floor, gameObject interacted with keeps track of number of times it was interacted with
    //Parameters: Keyboard input, raycast detecting gameObject, gameObject in handSlot, gameObject useableFloor component, useable layer, use function
    private void Use(InputAction.CallbackContext obj){
        if(useableHit.collider != null){
            if(inHandItem != null){
                IUseableFloor usable = useableHit.collider.GetComponent<IUseableFloor>();
                if (usable != null){
                    usable.Use(inHandItem);
                } 
                else{
                    Debug.Log("Hit object not implementing IUseableFloor");
                }
            }
            else{
                Debug.Log("No held item");
            }
        }
        else {
            Debug.Log("No useable objects");
        }
    }

    //TODO: Drops gameObject in handSlot
    //Parameters: Keyboard input, gameObject in handSlot, gameObject rigid body component
    private void Drop(InputAction.CallbackContext obj){
        if (inHandItem != null){
            inHandItem.transform.SetParent(null);
            Rigidbody rb = inHandItem.GetComponent<Rigidbody>();
            if (rb != null){
                rb.isKinematic = false;
            }
            inHandItem = null;
        }
    }

    //TODO: Picks up a gameObject into the handSlot
    //Parameters: Keyboard input, raycast detects gameObject, empty in handSlot, gameObject IPickable component
    private void PickUp(InputAction.CallbackContext obj){
        if(pickableHit.collider != null && inHandItem == null){
            IPickable pickableItem = pickableHit.collider.GetComponent<IPickable>();
            if (pickableItem != null){
                inHandItem = pickableItem.PickUp();
                inHandItem.transform.SetParent(pickUpParent.transform, pickableItem.KeepWorldPosition);
            }
        }
    }

    //TODO: Keeps raycast infront of main camera, enable/disable pickUp UI, enable/disable useable UI, enable/disable outline script
    //Parameters: Raycast initilized, inHand slot empty/full for UI, gameObject outline component
    private void Update(){

        //This raycast is for useable objects
        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out useableHit, hitRange, useableLayerMask)){
            Debug.DrawRay(playerCameraTransform.position, playerCameraTransform.forward * hitRange, Color.blue);
            if(inHandItem != null){
                useableUI.SetActive(true);
                return;
            }
        } 
        else{
            useableUI.SetActive(false);
        }

        //This raycast is for pickUp objects
        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out pickableHit, hitRange, pickableLayerMask)){
            Debug.DrawRay(playerCameraTransform.position, playerCameraTransform.forward * hitRange, Color.red);
            pickUpUI.SetActive(true);
        
            //This block checks the last detected item to disable the outline script if the item is in the handSlot.
            //First outline items when there is no last detected gameObject
            var script = pickableHit.collider.GetComponent<Outline>();
            if(script != null){
                if(lastHit != null){
                    var lastScript = lastHit.GetComponent<Outline>();
                    if(lastScript != null){
                        lastScript.enabled = false;
                    }
                }
                script.enabled = true;
                lastHit = pickableHit.collider;
            } 
        }
        //Then disable outline script when the gameObject is the same as the last detected gameObject
        else{
            pickUpUI.SetActive(false);
            if(lastHit != null){
                var lastScript = lastHit.GetComponent<Outline>();
                if(lastScript != null){
                    lastScript.enabled = false;
                }
                lastHit = null;
            }
        }
        if (inHandItem != null){
            return;
        }
    }
}
