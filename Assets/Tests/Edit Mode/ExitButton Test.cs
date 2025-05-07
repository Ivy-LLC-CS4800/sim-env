using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ExitButtonScriptTests
{
    private GameObject exitButtonGO;
    private ExitButtonScript exitButton;

    /// <summary>
    /// Creates a gameObject with the ExitButtonScript component
    /// </summary>
    [SetUp]
    public void Setup()
    {
        // Create a new GameObject and attach the ExitButtonScript
        exitButtonGO = new GameObject("ExitButton");
        exitButton = exitButtonGO.AddComponent<ExitButtonScript>();
    }

    /// <summary>
    /// Checks if application is still running, not ended prematurely, before destroying gameObject
    /// </summary>
    [TearDown]
    public void Teardown()
    {
        if (Application.isPlaying)
            Object.Destroy(exitButtonGO);
        else
            Object.DestroyImmediate(exitButtonGO);
    }

    //Test: When Exit button is pressed, script properly logs when button is pushed
    //Predicted: Exit log
    //Checked: ExitGame()
    [UnityTest]
    public IEnumerator ExitGame_LogsExitMessage()
    {
        LogAssert.Expect(LogType.Log, "Exit button pressed");

        // Act
        exitButton.ExitGame();

        // Wait a frame for the log to appear
        yield return null;
    }
}
