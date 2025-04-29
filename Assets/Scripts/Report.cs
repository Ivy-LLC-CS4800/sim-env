using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Data;
using Mono.Data.Sqlite;  // Make sure to add the necessary SQLite DLL for Unity.

public class Report : MonoBehaviour
{
    //public TextMeshProUGUI resultText; // Reference to the TextMeshPro GUI element to display result.
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI scoreText;

    public Slider[] taskSliders;  // Reference to sliders for each task in the Unity Inspector
    public TextMeshProUGUI[] taskLabels;  // Reference to task labels to show task names or percentages in the Unity Inspector


    private string connectionString = "URI=file:users.db"; // Adjust to SQLite DB path.

    void Start()
    {
        DisplayTitle();
        DisplayScore();
        DisplayTaskCompletion();
    }

    public void DisplayTitle()
    {
        string temp = Global.GlobalUser;
        titleText.text = $"Report for '{temp}'";

    }

    public void DisplayScore()
    {
        string temp = Global.GlobalUser;
        GetTotalScore(temp);
    }

    private void GetTotalScore(string userName)
    {
        string query = "SELECT total_score FROM users WHERE name = @name"; // SQL query to get the total_score for a given name.

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            
            using (var cmd = new SqliteCommand(query, connection))
            {
                // Add the parameter to avoid SQL injection attacks
                cmd.Parameters.AddWithValue("@name", userName);

                // Execute the query and get the result
                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    // Cast the result to a float (or int, depending on your database schema)
                    float totalScore = Convert.ToSingle(result);

                    // Display the result on the TextMeshPro UI
                    scoreText.text = $"Total Score for {userName}: {totalScore}%";
                }
                else
                {
                    // If no result is found for the given name
                    scoreText.text = $"User '{userName}' not found.";
                }
            }
        }
    }

    public void DisplayTaskCompletion()
{
    // Get the task name and completion percentage pair from DisplayTaskHelper
    Dictionary<string, int> taskData = DisplayTaskHelper();
    int taskIndex = 0;  // To iterate over the taskSliders and taskLabels arrays

    // Iterate through each task in the dictionary
    foreach (KeyValuePair<string, int> task in taskData)
    {
        // Update slider value for each task if taskSliders is not null
        if (taskIndex < taskSliders.Length)
        {
            // Update the slider (completion percentage as a fraction of 100)
            float completionPercentage = task.Value / 100f;

            // Call the ProgressBar script's SetProgress method to fill the slider smoothly
            if (taskSliders[taskIndex] != null)
            {
                // Ensure the slider has the ProgressBar script attached
                ProgressBar progressBarScript = taskSliders[taskIndex].GetComponent<ProgressBar>();
                if (progressBarScript != null)
                {
                    // Set the progress to the corresponding completion percentage (smooth fill)
                    progressBarScript.SetProgress(completionPercentage);
                }
            }

            // Optionally, update task labels with the completion percentage
            if (taskLabels.Length > taskIndex)
            {
                taskLabels[taskIndex].text = $"{task.Key}: {task.Value}%"; // Update the label with the percentage
            }
        }

        taskIndex++;  // Move to the next task
    }
}



    private Dictionary<string, int> DisplayTaskHelper()
    {
        Dictionary<string, int> taskResults = new Dictionary<string, int>();
        //List<string> temp = new List<string>();
        string queryTask = "SELECT id, name, subtasks FROM task"; // Query to get task data
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var cmd = new SqliteCommand(queryTask, connection))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int taskId = reader.GetInt32(0);  // task_id
                    string taskName = reader.GetString(1);  // task_name
                    int subtaskCount = reader.GetInt32(2); // number of subtasks

                    // Fetch subtasks for this task
                    float completionPercentage = GetSubtaskCompletion(taskId, subtaskCount);

                    // Round the completion percentage to an integer
                    int roundedCompletionPercentage = Mathf.RoundToInt(completionPercentage);

                    // Results
                    taskResults.Add(taskName, roundedCompletionPercentage);
                    //temp.Add($"{taskName}: {roundedCompletionPercentage}% Complete\n");
                }
            }
        }
        return taskResults;
    }

    private float GetSubtaskCompletion(int taskId, int subtaskCount)
    {
        int completedSubtasks = 0;
        float errorSubtasks = 0;

        string querySubtask = $"SELECT completion_status FROM subtask WHERE task_id = {taskId}";

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var cmd = new SqliteCommand(querySubtask, connection))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string status = reader.GetString(0).ToLower();  // Get completion status as a string
                    if (status == "completed" || status == "error")
                    {
                        completedSubtasks++;
                    }
                    if (status == "error")
                    {
                        errorSubtasks++;
                    }
                }
            }
        }

        // Calculate the completion percentage, accounting for "error" subtasks affecting score.
        float completionPercentage = (float)completedSubtasks / subtaskCount * 100;

        // Subtract 2 percent points for each error subtask
        completionPercentage -= errorSubtasks * 2;

        // Ensure the percentage doesn't go below 0
        return Mathf.Max(completionPercentage, 0);
    }
}
