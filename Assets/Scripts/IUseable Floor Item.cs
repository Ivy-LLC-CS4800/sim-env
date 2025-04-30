using System.Diagnostics;
using System.Globalization;
using UnityEngine;

//<summary>
// Tracks the number of interactions with a specific gameObject
//</summary>
public class IUseableFloorItem : MonoBehaviour, IUseableFloor
{
    private int interactionCount = 0;

    //TODO: Increase the interaction count and notify object in handSlot
    //Parameters: gameObject in handSlot
    public void Use(GameObject source)
    {
        interactionCount++;
        UnityEngine.Debug.Log($"{gameObject.name} interacted with {source.name}, Count: {interactionCount}");
    }

    //TODO: Getter for InteractionCount for database purposes
    //Parameters: Initialized interactionCount
    public int getInteractionCount(){
        return interactionCount;
    }
}
