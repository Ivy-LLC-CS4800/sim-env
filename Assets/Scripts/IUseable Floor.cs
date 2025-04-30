using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//<summary>
// Interface for gameObjects that can be interacted with, shared required functions
//</summary>
public interface IUseableFloor
{
    void Use(GameObject source);
}
