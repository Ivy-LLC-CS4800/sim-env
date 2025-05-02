using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TaskEntryUI : MonoBehaviour
{
    public TextMeshProUGUI taskNameText;
    public Toggle completionToggle;
    public Transform subtaskListParent;
    public GameObject subtaskEntryPrefab;
    public Image taskNameBackground;
    public Image subtaskNameBackground;

    private DatabaseManager.TaskData taskData;
    private DatabaseManager dbManager;
    private System.Action onTaskCompleted;
    private VerticalLayoutGroup verticalLayoutGroup;


    public void Initialize(DatabaseManager.TaskData task, DatabaseManager db, System.Action onComplete)
{
    taskData = task;
    dbManager = db;
    onTaskCompleted = onComplete;

    taskNameText.text = task.name;
    taskNameText.enabled = true;
    completionToggle.isOn = task.completion_status == "complete";
    completionToggle.onValueChanged.AddListener(OnToggleChanged);

    if (taskNameBackground != null)
        {
            taskNameBackground.enabled = true;  // Ensure the background image is enabled
            float widthFactor = 1.2f; // 20% larger than the text
            float heightFactor = 1.2f; // 20% larger than the text
            taskNameBackground.rectTransform.sizeDelta = new Vector2(taskNameText.preferredWidth / widthFactor, taskNameText.preferredHeight / heightFactor);
        }

    verticalLayoutGroup = subtaskListParent.GetComponent<VerticalLayoutGroup>();
        if (verticalLayoutGroup != null)
        {
            verticalLayoutGroup.enabled = true; // Enable layout group to manage layout automatically
        }

    PopulateSubtasks();
}


    private void OnToggleChanged(bool isComplete)
    {
        string newStatus = isComplete ? "complete" : "incomplete";
        dbManager.UpdateTaskCompletionStatus(taskData.id, newStatus);

        if (isComplete)
        {
            onTaskCompleted?.Invoke();
        }
    }

    private void PopulateSubtasks()
{
    // Clear any existing subtasks
    List<Transform> children = new List<Transform>();
    foreach (Transform child in subtaskListParent)
    {
        children.Add(child);
    }
    foreach (Transform child in children)
    {
        Destroy(child.gameObject);
    }

    List<DatabaseManager.SubtaskData> subtasks = dbManager.GetSubtasksForTask(taskData.id);

    foreach (var subtask in subtasks)
    {
        GameObject subtaskObj = Instantiate(subtaskEntryPrefab, subtaskListParent);

        // Make sure entire object is active
        subtaskObj.SetActive(true);

        Image subtaskImage = subtaskObj.GetComponentInChildren<Image>(true);
        TextMeshProUGUI subtaskText = subtaskObj.GetComponentInChildren<TextMeshProUGUI>(true);
        Toggle subtaskToggle = subtaskObj.GetComponentInChildren<Toggle>(true);

        if (subtaskText != null)
        {
            subtaskText.gameObject.SetActive(true); // Ensure the text is enabled
            subtaskText.text = subtask.name;
            subtaskText.enabled = true;
        }

        if (subtaskImage != null)
            {
                subtaskImage.enabled = true; // Ensure the background image is enabled
                float widthFactor = 1.5f; // 20% larger than the text
                float heightFactor = 1.2f; // 20% larger than the text
                subtaskImage.rectTransform.sizeDelta = new Vector2(subtaskText.preferredWidth / widthFactor, subtaskText.preferredHeight / heightFactor);
            }

        if (subtaskToggle != null)
        {
            subtaskToggle.gameObject.SetActive(true);
            subtaskToggle.isOn = subtask.completion_status == "complete";
            int subtaskId = subtask.id;
            subtaskToggle.onValueChanged.AddListener(isOn =>
            {
                string status = isOn ? "complete" : "incomplete";
                dbManager.UpdateSubtaskCompletionStatus(subtaskId, status);
            });
        }
    }
}

}
