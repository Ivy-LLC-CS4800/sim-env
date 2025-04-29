using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Report))]
public class ReportEditor : Editor
{
    // Flag to check if we've already updated the UI
    private bool hasUpdated = false;

    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        base.OnInspectorGUI();

        // Get the script reference
        Report reportScript = (Report)target;

        // Only update if not in play mode and only once during an editor refresh
        if (!Application.isPlaying && !hasUpdated)
        {
            reportScript.DisplayTitle(); // Update the title
            reportScript.DisplayScore(); // Update the score
            reportScript.DisplayTaskCompletion(); // Update the task completion

            // Set the flag to true to prevent infinite updates
            hasUpdated = true;

            // Manually trigger a refresh in the Inspector
            EditorUtility.SetDirty(reportScript); // Mark the script as dirty so it forces an update in the Inspector
        }
        else if (Application.isPlaying)
        {
            // Reset the flag when entering play mode to allow the editor script to work again after runtime
            hasUpdated = false;
        }
    }
}
