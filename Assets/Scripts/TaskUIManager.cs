using System.Collections.Generic;
using UnityEngine;

public class TaskUIManager : MonoBehaviour
{
    public GameObject taskEntryPrefab;
    public Transform taskContainer; // Where the current task appears
    public DatabaseManager dbManager;

    private List<DatabaseManager.TaskData> tasks;
    private int currentTaskIndex = 0;

    void Start()
    {
        tasks = dbManager.GetAllTasks();
        ShowCurrentTask();
    }

    private void ShowCurrentTask()
    {
        // Clear existing task UI
        foreach (Transform child in taskContainer)
        {
            Destroy(child.gameObject);
        }

        if (currentTaskIndex >= tasks.Count)
        {
            Debug.Log("All tasks completed!");
            return;
        }

        // Instantiate current task
        var taskData = tasks[currentTaskIndex];
        GameObject taskObj = Instantiate(taskEntryPrefab, taskContainer);
        TaskEntryUI taskUI = taskObj.GetComponent<TaskEntryUI>();
        taskUI.Initialize(taskData, dbManager, OnTaskCompleted);
    }

    private void OnTaskCompleted()
    {
        currentTaskIndex++;
        ShowCurrentTask();
    }
}
