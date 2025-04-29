using UnityEngine;

public class ExitButtonScript : MonoBehaviour
{
    public void ExitGame()
    {
        // Log a message to confirm button press in the editor (only works in the editor)
        Debug.Log("Exit button pressed");
        
        // If running in the editor, stop the play mode
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // Otherwise, quit the application
            Application.Quit();
        #endif
    }
}
