using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//<summary>
// Interface for gameObjects that can be pickedUp, shared required functions
//</summary>
public interface IPickable
{
    bool KeepWorldPosition { get; }

    GameObject PickUp();
}