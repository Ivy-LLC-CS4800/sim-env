using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SubtaskEntryUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI subtaskText;   // The Text component to display subtask name
    public Toggle subtaskToggle;          // The Toggle component to mark the subtask as completed

    private int subtaskId;                // The unique ID for the subtask (for database reference)

    // Called to initialize and populate the subtask UI
    public void SetupSubtask(int subtaskId, string subtaskName, string completionStatus)
    {
        this.subtaskId = subtaskId;
        subtaskText.text = subtaskName;

        // Set the toggle based on completion status
        if (completionStatus == "Completed")
        {
            subtaskToggle.isOn = true;
        }
        else
        {
            subtaskToggle.isOn = false;
        }

        // Add listener for toggle state change
        subtaskToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    // Called when the subtask's completion toggle is changed
    private void OnToggleChanged(bool isCompleted)
    {
        // Update the completion status in the database
        UpdateSubtaskCompletionStatus(isCompleted ? "Completed" : "Incomplete");
    }

    // This method could be extended to update the subtask's status in the database
    private void UpdateSubtaskCompletionStatus(string newStatus)
    {
        // Example: Call a method from DatabaseManager to update subtask status in the database
        DatabaseManager.Instance.UpdateSubtaskCompletionStatus(subtaskId, newStatus);
    }

    // Optional: Cleanup listeners when object is destroyed
    private void OnDestroy()
    {
        subtaskToggle.onValueChanged.RemoveListener(OnToggleChanged);
    }
}
