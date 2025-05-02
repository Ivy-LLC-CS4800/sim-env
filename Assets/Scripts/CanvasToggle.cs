using UnityEngine;
using UnityEngine.InputSystem;

public class UIController : MonoBehaviour
{
    public GameObject taskCanvas;
    private bool isCanvasActive = false;

    void Start()
    {
        taskCanvas.SetActive(false); // Hide at start
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            ToggleTaskCanvas();
        }
    }

    void ToggleTaskCanvas()
    {
        isCanvasActive = !isCanvasActive;
        taskCanvas.SetActive(isCanvasActive);

        if (isCanvasActive)
        {
            // Unlock cursor when UI is open
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Optionally disable player input here
            // e.g., PlayerRoot.GetComponent<PlayerController>().enabled = false;
        }
        else
        {
            // Lock cursor when UI is closed
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Optionally re-enable player input
            // e.g., PlayerRoot.GetComponent<PlayerController>().enabled = true;
        }
    }
}
