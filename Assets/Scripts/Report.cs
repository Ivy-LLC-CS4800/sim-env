using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Data;
using Mono.Data.Sqlite;  // Make sure to add the necessary SQLite DLL for Unity.

/// <summary>
/// Handles calculation and display of the report screen to the users.
/// </summary>
public class Report : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI scoreText;

    public Image gradeImage;

    public Sprite gradeA;
    public Sprite gradeB;
    public Sprite gradeC;
    public Sprite gradeD;
    public Sprite gradeF;

    public Slider[] taskSliders;  // Reference to sliders for each task in the Unity Inspector
    public TextMeshProUGUI[] taskLabels;  // Reference to task labels to show task names or percentages in the Unity Inspector

    private string connectionString = "URI=file:" + Application.streamingAssetsPath + "/users.db";
    string temp = Global.Username;


    void Start()
    {
        DisplayTitle();
        UpdateTotalScoreFromTasks(); 
        DisplayScore();
        DisplayTaskCompletion();
        DisplayGrade();
    }

    /// <summary>
    /// Displays User's name as title
    /// </summary>
    public void DisplayTitle()
    {
        titleText.text = $"Report for '{temp}'";
    }

    /// <summary>
    /// Displays Total Score to user
    /// </summary>
    public void DisplayScore()
    {
        float score = GetTotalScore(temp);
        // Display the result on the TextMeshPro UI
        scoreText.text = $"Total Score for {temp}: {score}%";
    }

    /// <summary>
    /// Grabs user's score from database
    /// </summary>
    /// <param name="userName"></param>
    private float GetTotalScore(string userName)
    {
        string query = "SELECT total_score FROM users WHERE name = @name";
        using (var connection = new SqliteConnection(connectionString))
        {
            try
            {
                connection.Open();
                using (var cmd = new SqliteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@name", userName);
                    // Execute the query and get the result
                    object result = cmd.ExecuteScalar();

                    float totalScore = 0f;

                    if (result != null && result != DBNull.Value)
                    {
                        totalScore = Convert.ToSingle(result);
                        Debug.Log($"Total score for user '{userName}' found: {totalScore}%");
                        return totalScore;
                    }
                    else
                    {
                        Debug.LogWarning($"No score found for user '{userName}', defaulting to 0.");
                        return 0;
                    }
                    return totalScore;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in GetTotalScore: {ex.Message}\n{ex.StackTrace}");
                return 0;
            }
        }
    }



    /// <summary>
    /// Grabs data from database and displays task completion progress bars to user
    /// </summary>
    public void DisplayTaskCompletion()
    {
        // Get the task name and completion percentage pair from DisplayTaskHelper
        Dictionary<string, int> taskData = DisplayTaskHelper();
        int taskIndex = 0;  // To iterate over the taskSliders and taskLabels arrays

        if (taskData.Count > 0)
        {
            Debug.Log($"Found {taskData.Count} tasks.");
        }
        else
        {
            Debug.LogWarning("No tasks found in taskData.");
        }


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
                    ProgressBar progressBarScript = taskSliders[taskIndex].GetComponent<ProgressBar>();
                    if (progressBarScript != null)
                    {
                        progressBarScript.SetProgress(completionPercentage);
                    }
                    else
                    {
                        Debug.LogWarning($"ProgressBar script not found for slider {taskIndex}");
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

    /// <summary>
    /// Grabs all tasks and percentage of completion by user from database
    /// </summary>
    /// <returns>Returns a Dictionary with key: String and value: int of each task along with its percentage of completion</returns>
    private Dictionary<string, int> DisplayTaskHelper()
    {
        Dictionary<string, int> taskResults = new Dictionary<string, int>();
        string queryTask = @"
            SELECT t.id, t.name, t.subtasks 
            FROM tasks t
            JOIN simulation s ON t.sim_id = s.id
            JOIN users u ON s.user_id = u.id
            WHERE u.name = @username";  // Ensures we only get tasks for the current user

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var cmd = new SqliteCommand(queryTask, connection))
            {
                // Bind the current user's name to the query
                cmd.Parameters.AddWithValue("@username", Global.Username);

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        Debug.LogWarning("No tasks found for user '" + Global.Username + "'.");
                    }

                    while (reader.Read())
                    {
                        int taskId = reader.GetInt32(0);  // task_id
                        string taskName = reader.GetString(1);  // task_name
                        int subtaskCount = reader.GetInt32(2);  // subtasks count

                        // Log task info
                        Debug.Log($"Found task: {taskName}, Task ID: {taskId}, Subtask Count: {subtaskCount}");

                        // Calculate the completion percentage for each task
                        float completionPercentage = GetSubtaskCompletion(taskId, subtaskCount);

                        // Log the completion percentage
                        Debug.Log($"Task '{taskName}' completion: {completionPercentage}%");

                        // Round the completion percentage to an integer
                        int roundedCompletionPercentage = Mathf.RoundToInt(completionPercentage);

                        // Add the task with its completion percentage to the results
                        taskResults.Add(taskName, roundedCompletionPercentage);
                    }
                }
            }
        }
        return taskResults;
    }


    private float GetSubtaskCompletion(int taskId, int subtaskCount)
    {
        int completedSubtasks = 0;
        int errorSubtasks = 0;

        string querySubtask = $"SELECT completion_status FROM subtasks WHERE task_id = {taskId}";

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var cmd = new SqliteCommand(querySubtask, connection))
            using (var reader = cmd.ExecuteReader())
            {
                if (!reader.HasRows)
                {
                    Debug.LogWarning($"No subtasks found for task ID {taskId}. Subtask count: {subtaskCount}");
                }

                while (reader.Read())
                {
                    string status = reader.GetString(0).ToLower();  // Get completion status as a string

                    if (status == "completed")
                    {
                        completedSubtasks++;
                    }
                    else if (status == "error")
                    {
                        completedSubtasks++;
                        errorSubtasks++;  // Error subtasks reduce the score
                    }

                    // Log subtask completion status
                    Debug.Log($"Subtask status for task {taskId}: {status}");
                }
            }
        }

        // Calculate the completion percentage, considering errors
        float completionPercentage = ((float)completedSubtasks / subtaskCount) * 100;

        // Log the calculated percentage
        Debug.Log($"Calculated completion percentage for task {taskId}: {completionPercentage}%");

        // Subtract 2 percent points for each error subtask
        completionPercentage -= errorSubtasks * 2;

        // Ensure the percentage doesn't go below 0
        return Mathf.Max(completionPercentage, 0);
    }

    public void DisplayGrade()
    {
        float score = GetTotalScore(temp);
        // Display grade image
        if (score >= 90)
        {
            gradeImage.sprite = gradeA;
        }
        else if (score >= 80)
        {
            gradeImage.sprite = gradeB;
        }
        else if (score >= 70)
        {
            gradeImage.sprite = gradeC;
        }
        else if (score >= 60)
        {
            gradeImage.sprite = gradeD;
        }
        else
        {
            gradeImage.sprite = gradeF;
        }
    }

    private void UpdateTotalScoreFromTasks()
    {
        var taskData = DisplayTaskHelper();

        if (taskData.Count == 0)
        {
            Debug.LogWarning("No tasks found for score calculation.");
            return;
        }

        float total = 0;
        foreach (var kvp in taskData)
        {
            total += kvp.Value;  // kvp.Value is the completion %
        }

        float averageScore = total / taskData.Count;
        Debug.Log($"Calculated average score: {averageScore}");

        // Now update this score in the database
        string updateQuery = "UPDATE users SET total_score = @score WHERE name = @name";

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var cmd = new SqliteCommand(updateQuery, connection))
            {
                cmd.Parameters.AddWithValue("@score", averageScore);
                cmd.Parameters.AddWithValue("@name", Global.Username);

                int rowsAffected = cmd.ExecuteNonQuery();
                Debug.Log($"Updated total_score for '{Global.Username}' to {averageScore}%. Rows affected: {rowsAffected}");
            }
        }
    }

}
