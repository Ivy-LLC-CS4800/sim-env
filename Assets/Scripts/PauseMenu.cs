using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI; // Required for Button

public class PauseMenu : MonoBehaviour
{
    public GameObject canvasObject; // Reference to the Canvas GameObject
    private bool isCanvasActive = false;
    public Button resumeButton; // Reference to the Resume button
    public Button exitButton;   // Reference to the Exit button
    public SceneLoader sceneLoader; // Reference to the SceneLoader script
    public MoveController moveController;
    public FPSController fpsController;

    void Start()
    {
        Debug.Log("PauseMenu started");
        canvasObject.SetActive(false); // Hide at start
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (moveController == null)
        {
            moveController = GetComponent<MoveController>();
        }

        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(ResumeGame);
        }
        else
        {
            Debug.LogError("Resume button not assigned in the inspector!");
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(ExitGame);
        }
        else
        {
            Debug.LogError("Exit button not assigned in the inspector!");
        }
    }

    void Update()
    {
        if (Keyboard.current.digit8Key.wasPressedThisFrame)
        {
            ToggleCanvas();
        }  
         
    }

    void ToggleCanvas()
    {
        Debug.Log(isCanvasActive);
        isCanvasActive = !isCanvasActive;
        canvasObject.SetActive(isCanvasActive);
        Debug.Log("right before: " + isCanvasActive);
        if (isCanvasActive)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            moveController.enabled = false; // Disable player movement
            fpsController.enabled = false; // Disable FPS controller
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked; // You might want to control this in your player script
            Cursor.visible = false;
            moveController.enabled = true; // Re-enable player movement
            fpsController.enabled = true; // Re-enable FPS controller
        }
    }

    void CloseCanvas()
    {
        if (isCanvasActive)
        {
            isCanvasActive = !isCanvasActive;
            canvasObject.SetActive(isCanvasActive);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            moveController.enabled = true;
            fpsController.enabled = true;
        }
    }

    public void ResumeGame()
    {
        CloseCanvas(); // This will hide the canvas and resume the game
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        sceneLoader.LoadReportScene();
#endif
    }
}
