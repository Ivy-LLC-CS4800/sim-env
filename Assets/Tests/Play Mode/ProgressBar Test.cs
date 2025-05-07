using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;

public class ProgressBarTests
{
    private GameObject testObject;
    private ProgressBar progressBar;
    private Slider slider;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        // Create GameObject and add components
        testObject = new GameObject("ProgressBarTestObject");
        slider = testObject.AddComponent<Slider>();
        progressBar = testObject.AddComponent<ProgressBar>();

        // Wait one frame to simulate Start() behavior
        yield return null;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        Object.Destroy(testObject);
        yield return null;
    }

    [UnityTest]
    public IEnumerator SetProgress_FillsSliderToTargetValue()
    {
        float targetValue = 0.8f;

        progressBar.SetProgress(targetValue);

        // Wait for the duration of the animation plus a little buffer
        yield return new WaitForSeconds(progressBar.duration + 0.1f);

        Assert.AreEqual(targetValue, slider.value, 0.01f); // Allow small margin for floating point
    }

    [Test]
    public void ProgressBar_Start_SetsSliderInitialValues()
    {
        progressBar = testObject.AddComponent<ProgressBar>();
        slider = testObject.GetComponent<Slider>();

        // Manually call Start() since it's not called in EditMode unit test
        typeof(ProgressBar).GetMethod("Start", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .Invoke(progressBar, null);

        Assert.AreEqual(0f, slider.value);
        Assert.IsFalse(slider.interactable);
    }
}
