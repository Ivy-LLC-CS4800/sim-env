using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PickableItemTests
{
    private GameObject pickableGO;
    private PickableItem pickableItem;

    /// <summary>
    /// Creates a gameObject with the PickableItem component
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        pickableGO = new GameObject("PickableItem");
        pickableItem = pickableGO.AddComponent<PickableItem>();
    }

    /// <summary>
    /// Removes the gameObject to reset test environment
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(pickableGO);
    }

    //Test: If rigidbody component exists, gameObjects moves to correct place and kinematic is enabled
    //Predicted: gameObject moved to hand gameObject with appropriate rotation
    //Checked: PickUp()
    [UnityTest]
    public IEnumerator PickUp_WithRigidbody_SetsKinematicAndTransforms()
    {
        var rb = pickableGO.AddComponent<Rigidbody>();

        // Call Awake() manually in tests
        pickableItem.Invoke("Awake", 0f);
        yield return null;

        var returnedGO = pickableItem.PickUp();

        Assert.AreEqual(pickableGO, returnedGO);
        Assert.IsTrue(rb.isKinematic);
        Assert.AreEqual(Vector3.zero, pickableGO.transform.localPosition);
        Assert.AreEqual(Quaternion.Euler(270f, 0f, 340f), pickableGO.transform.localRotation);
        Assert.AreEqual(Vector3.one, pickableGO.transform.localScale);
    }

    //Test: If no rigidbody component exists, only move gameObject to correct place
    //Predicted: gameObject will be placed in hand slot with rotation, no change in kinematics
    //Checked: getComponent<Rigidbody>(), PickUp()
    [UnityTest]
    public IEnumerator PickUp_WithoutRigidbody_SkipsKinematicAndSetsTransforms()
    {
        // Ensure no Rigidbody
        var rb = pickableGO.GetComponent<Rigidbody>();
        if (rb != null) Object.DestroyImmediate(rb);

        pickableItem.Invoke("Awake", 0f);
        yield return null;

        var returnedGO = pickableItem.PickUp();

        Assert.AreEqual(pickableGO, returnedGO);
        Assert.AreEqual(Vector3.zero, pickableGO.transform.localPosition);
        Assert.AreEqual(Quaternion.Euler(270f, 0f, 340f), pickableGO.transform.localRotation);
        Assert.AreEqual(Vector3.one, pickableGO.transform.localScale);
    }
}
