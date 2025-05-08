using UnityEngine;

public class MockPickable : MonoBehaviour, IPickable
{
    public GameObject PickUp()
    {
        return gameObject;
    }

    public bool KeepWorldPosition => false;
}

public class MockUseableFloor : MonoBehaviour, IUseableFloor
{
    public int CallCount { get; private set; }
    public GameObject LastUsedItem { get; private set; }

    public void Use(GameObject item)
    {
        CallCount++;
        LastUsedItem = item;
    }

    public void ResetCallCount()
    {
        CallCount = 0;
        LastUsedItem = null;
    }
}