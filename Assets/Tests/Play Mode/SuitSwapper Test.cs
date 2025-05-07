using NUnit.Framework;
using UnityEngine;
using System.Collections;
using UnityEngine.TestTools;

public class SuitSwapperTests
{
    private GameObject swapperGO;
    private SuitSwapper swapper;
    private GameObject[] models;
    private GameObject cameraGO;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        // Create a mock camera and assign as MainCamera
        cameraGO = new GameObject("MainCamera");
        cameraGO.tag = "MainCamera";
        cameraGO.AddComponent<Camera>();

        // Create character models
        models = new GameObject[3];
        for (int i = 0; i < models.Length; i++)
        {
            models[i] = new GameObject($"Model_{i}");
        }

        // Create SuitSwapper GameObject and attach component
        swapperGO = new GameObject("Swapper");
        swapper = swapperGO.AddComponent<SuitSwapper>();
        swapper.characterModels = models;
        swapper.initialActiveIndex = 1;
        swapper.setCameraAsChild = true;

        yield return null; // Allow Start() to run
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        Object.Destroy(cameraGO);
        foreach (var model in models)
        {
            Object.Destroy(model);
        }
        Object.Destroy(swapperGO);
        yield return null;
    }

    [UnityTest]
    public IEnumerator Start_EnablesInitialModel_DisablesOthers()
    {
        yield return null; // Let Start() complete

        for (int i = 0; i < models.Length; i++)
        {
            bool shouldBeActive = (i == swapper.initialActiveIndex);
            Assert.AreEqual(shouldBeActive, models[i].activeSelf, $"Model {i} activation state incorrect.");
        }
    }

    [UnityTest]
    public IEnumerator SwapToNextModel_CyclesModelsAndMovesCamera()
    {
        yield return null; // Let Start() complete

        int nextIndex = (swapper.initialActiveIndex + 1) % models.Length;

        swapper.SwapToNextModel();

        yield return null;

        for (int i = 0; i < models.Length; i++)
        {
            bool shouldBeActive = (i == nextIndex);
            Assert.AreEqual(shouldBeActive, models[i].activeSelf, $"Model {i} should be {(shouldBeActive ? "active" : "inactive")}.");
        }

        Assert.AreEqual(models[nextIndex].transform, cameraGO.transform.parent, "Camera parent not updated correctly.");
    }

    [UnityTest]
    public IEnumerator SwapToNextModel_LoopsBackToFirstModel()
    {
        yield return null;

        for (int i = 0; i < models.Length; i++)
        {
            swapper.SwapToNextModel();
            yield return null;
        }

        // Should be back at initialActiveIndex
        int expectedIndex = (swapper.initialActiveIndex + models.Length) % models.Length;
        Assert.AreEqual(models[expectedIndex].transform, cameraGO.transform.parent);
    }
}
