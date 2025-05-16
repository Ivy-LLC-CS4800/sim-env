using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject canvasObject;
    public Button resumeButton;
    public Button exitButton;
    public SceneLoader sceneLoader;

    private bool isPaused = false;
    private bool allowToggle = true;

    void Start()
    {
        SetPaused(false);

        if (resumeButton != null)
            resumeButton.onClick.AddListener(OnResumeButton);
        else
            Debug.LogError("Resume button not assigned!");

        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitButton);
        else
            Debug.LogError("Exit button not assigned!");
    }

    void Update()
    {
        // Handle Tab key input even when time is paused
        if (Input.GetKeyDown(KeyCode.Tab) && allowToggle)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void PauseGame()
    {
        isPaused = true;
        canvasObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
        allowToggle = false; // prevent double-trigger
    }

    void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        canvasObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        allowToggle = true;
    }

    void OnResumeButton()
    {
        ResumeGame();
    }

    void OnExitButton()
    {
        Time.timeScale = 1f;
        if (sceneLoader != null)
        {
            sceneLoader.LoadReportScene();
        }
        else
        {
            Debug.LogError("SceneLoader not assigned!");
        }
    }

    void SetPaused(bool pause)
    {
        isPaused = pause;
        canvasObject.SetActive(pause);
        Cursor.lockState = pause ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = pause;
        Time.timeScale = pause ? 0f : 1f;
    }
}
