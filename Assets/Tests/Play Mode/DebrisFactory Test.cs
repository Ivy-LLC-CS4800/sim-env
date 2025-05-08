using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class DebrisFactoryTests
{
    private GameObject factoryObject;
    private DebrisFactory factory;

    private GameObject testWoodPrefab;
    private GameObject testMetalPrefab;
    private GameObject testOtherPrefab;
    private GameObject testConcretePrefab;

    [SetUp]
    public void SetUp()
    {
        factoryObject = new GameObject("Factory");
        factory = factoryObject.AddComponent<DebrisFactory>();

        // Create mock prefabs
        testWoodPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
        testMetalPrefab = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        testOtherPrefab = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        testConcretePrefab = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

        // Assign prefabs
        factory.woodDebrisPrefabs = new GameObject[] { testWoodPrefab };
        factory.metalDebrisPrefabs = new GameObject[] { testMetalPrefab };
        factory.otherDebrisPrefabs = new GameObject[] { testOtherPrefab };
        factory.concreteDebrisPrefabs = new GameObject[] { testConcretePrefab };
    }

    [UnityTest]
    public System.Collections.IEnumerator CreateDebris_CreatesCorrectPrefabInstance()
    {
        Vector3 position = new Vector3(1, 2, 3);
        Quaternion rotation = Quaternion.Euler(0, 90, 0);

        GameObject debris = factory.CreateDebris(DebrisType.Wood, position, rotation);
        yield return null;

        Assert.IsNotNull(debris);
        Assert.AreEqual(position, debris.transform.position);
        Assert.AreEqual(rotation.eulerAngles, debris.transform.rotation.eulerAngles);
    }

    [Test]
    public void GetPrefabsForType_ReturnsCorrectArray()
    {
        Assert.AreEqual(factory.woodDebrisPrefabs, factory.GetPrefabsForType(DebrisType.Wood));
        Assert.AreEqual(factory.metalDebrisPrefabs, factory.GetPrefabsForType(DebrisType.Metal));
        Assert.AreEqual(factory.otherDebrisPrefabs, factory.GetPrefabsForType(DebrisType.Other));
        Assert.AreEqual(factory.concreteDebrisPrefabs, factory.GetPrefabsForType(DebrisType.Concrete));
    }

    [UnityTest]
    public System.Collections.IEnumerator CreateDebris_ReturnsNullIfNoPrefabs()
    {
        factory.woodDebrisPrefabs = new GameObject[0];

        GameObject debris = factory.CreateDebris(DebrisType.Wood, Vector3.zero, Quaternion.identity);
        yield return null;

        Assert.IsNull(debris);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(factoryObject);
        Object.DestroyImmediate(testWoodPrefab);
        Object.DestroyImmediate(testMetalPrefab);
        Object.DestroyImmediate(testOtherPrefab);
        Object.DestroyImmediate(testConcretePrefab);
    }
}
