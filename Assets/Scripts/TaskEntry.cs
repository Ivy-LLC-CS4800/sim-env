using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TaskEntryUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI taskNameText;
    public Toggle completionToggle;
    public Button showSubtasksButton;
    public Transform subtaskListParent;
    public GameObject subtaskEntryPrefab;

    private DatabaseManager.TaskData taskData;
    private DatabaseManager dbManager;

    private bool subtasksVisible = false;

    public void Initialize(DatabaseManager.TaskData task, DatabaseManager db)
    {
        taskData = task;
        dbManager = db;

        taskNameText.text = task.name;
        completionToggle.isOn = task.completion_status == "complete";
        completionToggle.onValueChanged.AddListener(OnToggleChanged);

        if (showSubtasksButton != null)
            showSubtasksButton.onClick.AddListener(ToggleSubtasks);

        // Optionally hide subtasks until toggled
        if (subtaskListParent != null)
            subtaskListParent.gameObject.SetActive(false);
    }

    private void OnToggleChanged(bool isComplete)
    {
        string newStatus = isComplete ? "complete" : "incomplete";
        dbManager.UpdateTaskCompletionStatus(taskData.id, newStatus);
    }
    
    private void ToggleSubtasks()
    {
        subtasksVisible = !subtasksVisible;

        if (subtaskListParent != null)
        {
            subtaskListParent.gameObject.SetActive(subtasksVisible);

            //if (subtasksVisible)
                //PopulateSubtasks();
        }
    }
    
    private void PopulateSubtasks()
    {
        // Clear any existing subtasks
        foreach (Transform child in subtaskListParent)
        {
            Destroy(child.gameObject);
        }

        List<DatabaseManager.SubtaskData> subtasks = dbManager.GetSubtasksForTask(taskData.id);

        foreach (var subtask in subtasks)
        {
            GameObject subtaskObj = Instantiate(subtaskEntryPrefab, subtaskListParent);
            TextMeshProUGUI subtaskText = subtaskObj.GetComponentInChildren<TextMeshProUGUI>();
            Toggle subtaskToggle = subtaskObj.GetComponentInChildren<Toggle>();

            if (subtaskText != null)
                subtaskText.text = subtask.name;

            if (subtaskToggle != null)
            {
                subtaskToggle.isOn = subtask.completion_status == "complete";
                int subtaskId = subtask.id; // Needed to avoid closure issues
                subtaskToggle.onValueChanged.AddListener(isOn =>
                {
                    string status = isOn ? "complete" : "incomplete";
                    dbManager.UpdateSubtaskCompletionStatus(subtaskId, status);
                });
            }
        }
    
    }
}
