using UnityEngine;

// Controlls the visiblity of iCommand
public class iCommandHelper : MonoBehaviour {
    // Variables
    [SerializeField] private Canvas iCommand; // UI Canvas
    private bool isVisible = false; // track visibility

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        // Check if iCommand exists and set it immediately to invisible
        if (iCommand != null) {
            iCommand.enabled = false;
        }//end if
    }//end Start()

    // Update is called once per frame
    void Update() {
        ShowHideICommand();
    }//end Update()

    void ShowHideICommand() {
        if (Input.GetKeyDown(KeyCode.I)) {
            isVisible = !isVisible; // Switch visibility
            if (iCommand != null) {
                iCommand.enabled = isVisible;
            }//end if
        }//end if
    }//end ShowHideICommand()
}//end iCommandHelper
