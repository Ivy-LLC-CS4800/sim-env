using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//<summary>
// Enables gameObjects to be picked up and placed in handSlot with proper position and rotation.
//</summary>
public class PickableItem : MonoBehaviour, IPickable
{
    [field: SerializeField] public bool KeepWorldPosition { get; private set; }

    private Rigidbody rb;

    //TODO: detect the rigid body component of gameObjects (defaults as null)
    //Parameters: Begin the program
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    //TODO: Changes the gameObjects positon and rotation to fit in handSlot
    //Parameters: gameObject pickableItem component
    public GameObject PickUp()
    {
        if (rb != null) //protective statement in case gameObject has no rigid body
        {
            rb.isKinematic = true;
        }
        transform.localPosition = Vector3.zero; //Vector3 passes 3d directions
        transform.localRotation = Quaternion.Euler(270f, 0f, 340f);
        transform.localScale = Vector3.one;
        return this.gameObject;
    }
}