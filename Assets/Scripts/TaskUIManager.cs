using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class TaskUIManager : MonoBehaviour
{
    public Transform taskListParent;            // Parent object (e.g., VerticalLayoutGroup)
    public GameObject taskEntryPrefab;          // Prefab for displaying a task
    public DatabaseManager dbManager;           // Reference to your DatabaseManager script

    void Start()
    {
        PopulateTaskUI();
    }

    public void PopulateTaskUI()
    {
        // Clear old entries
        foreach (Transform child in taskListParent)
        {
            Destroy(child.gameObject);
        }

        List<DatabaseManager.TaskData> tasks = dbManager.GetAllTasks();

        foreach (var task in tasks)
        {
            GameObject taskUI = Instantiate(taskEntryPrefab, taskListParent);
            TextMeshProUGUI textComponent = taskUI.GetComponentInChildren<TextMeshProUGUI>(); // Use Text if using Unity's Text UI

            if (textComponent != null)
            {
                Debug.Log($"Updating text for task: {task.name}");
                textComponent.text = $"{task.name} - {task.completion_status}";
            }
            else
            {
                Debug.LogWarning("TextMeshProUGUI component not found on taskEntryPrefab.");
            }
        }
    }
}