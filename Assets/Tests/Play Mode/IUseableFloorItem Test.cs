using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class IUseableFloorItemTests
{
    private GameObject itemGO;
    private IUseableFloorItem useableItem;
    private GameObject heldItem;

    /// <summary>
    /// Creates a useableFloorItem gameObject with an IUseableFloorItem, Creates a heldItem gameObject
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        itemGO = new GameObject("UseableFloorItem");
        useableItem = itemGO.AddComponent<IUseableFloorItem>();

        heldItem = new GameObject("HeldItem");
    }

    /// <summary>
    /// Removes UseableFloorItem and heldItem to reset test environment
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(itemGO);
        Object.DestroyImmediate(heldItem);
    }

    //Test: gameObject accurately tracks number of interactions
    //Predicted: [0,1,2] starts at 0 and each respective use increases predicted value by one
    //Checked: getInteractionCount(), Use()
    [UnityTest]
    public IEnumerator Use_IncrementsInteractionCount()
    {
        Assert.AreEqual(0, useableItem.getInteractionCount());

        useableItem.Use(heldItem);
        yield return null;

        Assert.AreEqual(1, useableItem.getInteractionCount());

        useableItem.Use(heldItem);
        yield return null;

        Assert.AreEqual(2, useableItem.getInteractionCount());
    }

    //Test: gameObject accurately logs which gameObject interacted with it and the number of interactions
    //Predicted: UseableFloorItem, HeldItem
    //Checked: Use()
    [UnityTest]
    public IEnumerator Use_LogsInteractionMessage()
    {
        LogAssert.Expect(LogType.Log, $"{itemGO.name} interacted with {heldItem.name}, Count: 1");

        useableItem.Use(heldItem);
        yield return null;
    }
}
