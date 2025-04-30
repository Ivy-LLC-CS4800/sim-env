using UnityEngine;
using Mono.Data.Sqlite;  // SQLite support
using System.Data;  // To access SQLite commands
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;

//<summary>
// Creates a database instance and connects it to the local device database to check usernames,passwords, and register new users
//</summary>
public class DatabaseManager : MonoBehaviour
{
    private string dbPath = "URI=file:users.db";

    //TODO: Initialize database
    //Parameters: Start application
    void Start()
    {
        CreateDB();
        CreateTasksTable();
        CreateSubtasksTable();
    }

    //TODO: Creates a database instance and connects to local device database
    //Parameters:
    public void CreateDB()
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                // Create table with username and password fields
                command.CommandText = "CREATE TABLE IF NOT EXISTS users (id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT NOT NULL UNIQUE, password TEXT NOT NULL);";
                command.ExecuteNonQuery();
                Debug.Log("Table created");
            }
            connection.Close();
        }
    }

    //TODO: Verifies if user exists in database and the password is correct
    //Parameters: Username, password, initialized database
    public bool CheckUsernameAndPassword(string username, string password)
    {
        return CheckUsernameAndPasswordHelper(username, password);
    }

    //TODO: Checks username and password against database
    //Parameters: Username, password, initialized database instance
    private bool CheckUsernameAndPasswordHelper(string username, string password)
    {
        string hashedPassword = HashPassword(password);
        //string hashedPassword = password; 

        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT COUNT(*) FROM users WHERE name = @name AND password = @password";
                command.Parameters.Add(new SqliteParameter("@name", username));
                command.Parameters.Add(new SqliteParameter("@password", hashedPassword));

                long result = (long)command.ExecuteScalar();
                connection.Close();

                return result > 0; // Returns true if username and password match, false otherwise
            }
        }
    }

    //TODO: Adds new username and password database
    //Parameters: Username, password, initialized database instance
    public bool RegisterUsername(string username, string password)
    {
        return RegisterUsernameHelper(username, password);
    }

    //TODO: Adds new username and password database
    //Parameters: Username, password, initialized database instance
    private bool RegisterUsernameHelper(string username, string password)
    {
        // Check if the username already exists
        if (CheckUsernameExists(username))
        {
            return false; // Username already exists
        }

        string hashedPassword = HashPassword(password);
        //string hashedPassword = password;

        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO users (name, password) VALUES (@name, @password)";
                command.Parameters.Add(new SqliteParameter("@name", username));
                command.Parameters.Add(new SqliteParameter("@password", hashedPassword));
                command.ExecuteNonQuery();
                connection.Close();

                return true; // Registration successful
            }
        }
    }

    //TODO: Checks for existing username in database
    //Parameters: Username, initialized database instance
    bool CheckUsernameExists(string username)
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT COUNT(*) FROM users WHERE name = @name";
                command.Parameters.Add(new SqliteParameter("@name", username));

                long result = (long)command.ExecuteScalar();
                connection.Close();

                return result > 0; // Returns true if username exists, false otherwise
            }
        }
    }

    //TODO: Encrpts password using SHA-256
    //Parameters: Password, initialized database instance
    string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                stringBuilder.Append(b.ToString("x2"));
            }
            return stringBuilder.ToString();
        }
    }

    public void CreateTasksTable()
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"CREATE TABLE IF NOT EXISTS Tasks (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT,
                                            sim_id INTEGER,
                                            name TEXT NOT NULL,
                                            subtasks INTEGER,
                                            time_limit INTEGER,
                                            completion_status TEXT
                                        );";
                command.ExecuteNonQuery();
            }
        }
    }

    public void AddTask(int simId, string name, int subtasks, int timeLimit, string completionStatus)
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"INSERT INTO Tasks (sim_id, name, subtasks, time_limit, completion_status)
                                        VALUES (@sim_id, @name, @subtasks, @time_limit, @completion_status)";
                command.Parameters.AddWithValue("@sim_id", simId);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@subtasks", subtasks);
                command.Parameters.AddWithValue("@time_limit", timeLimit);
                command.Parameters.AddWithValue("@completion_status", completionStatus);
                command.ExecuteNonQuery();
            }
        }
    }

    public void UpdateTaskCompletionStatus(int taskId, string newStatus)
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE Tasks SET completion_status = @completion_status WHERE id = @id";
                command.Parameters.AddWithValue("@completion_status", newStatus);
                command.Parameters.AddWithValue("@id", taskId);
                command.ExecuteNonQuery();
            }
        }
    }

    public void DeleteTask(int taskId){
        using (var connection = new SqliteConnection(dbPath)){
            connection.Open();
            using (var command = connection.CreateCommand()){
                command.CommandText = "DELETE FROM Tasks WHERE id = @id"; 
                command.Parameters.AddWithValue("@id", taskId);
                command.ExecuteNonQuery();
            }
        }
    }

    public List<TaskData> GetAllTasks()
    {
        List<TaskData> tasks = new List<TaskData>();

        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Tasks";
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TaskData task = new TaskData
                        {
                            id = reader.GetInt32(0),
                            sim_id = reader.GetInt32(1),
                            name = reader.GetString(2),
                            subtasks = reader.GetInt32(3),
                            time_limit = reader.GetInt32(4),
                            completion_status = reader.GetString(5)
                        };
                        tasks.Add(task);
                    }
                }
            }
        }
        return tasks;
    }

    public void CreateSubtasksTable()
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"CREATE TABLE IF NOT EXISTS Subtasks (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT,
                                            task_id INTEGER NOT NULL,
                                            name TEXT NOT NULL,
                                            completion_status TEXT,
                                            FOREIGN KEY(task_id) REFERENCES Tasks(id) ON DELETE CASCADE
                                        );";
                command.ExecuteNonQuery();
            }
        }
    }

    public void AddSubtask(int taskId, string name, string completionStatus)
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"INSERT INTO Subtasks (task_id, name, completion_status)
                                     VALUES (@task_id, @name, @completion_status)";
                command.Parameters.AddWithValue("@task_id", taskId);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@completion_status", completionStatus);
                command.ExecuteNonQuery();
            }
        }
    }

    public void UpdateSubtaskCompletionStatus(int subtaskId, string newStatus)
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE Subtasks SET completion_status = @completion_status WHERE id = @id";
                command.Parameters.AddWithValue("@completion_status", newStatus);
                command.Parameters.AddWithValue("@id", subtaskId);
                command.ExecuteNonQuery();
            }
        }
    }


    public List<SubtaskData> GetSubtasksForTask(int taskId)
    {
        List<SubtaskData> subtasks = new List<SubtaskData>();
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Subtasks WHERE task_id = @task_id";
                command.Parameters.AddWithValue("@task_id", taskId);
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SubtaskData subtask = new SubtaskData
                        {
                            id = reader.GetInt32(0),
                            task_id = reader.GetInt32(1),
                            name = reader.GetString(2),
                            completion_status = reader.GetString(3)
                        };
                        subtasks.Add(subtask);
                    }
                }
            }
        }

        return subtasks;
    }




    //Data Structure
    public class TaskData{
        public int id;
        public int sim_id;
        public string name;
        public int subtasks;
        public int time_limit;
        public string completion_status;
    }

    public class SubtaskData{
        public int id;
        public int task_id;
        public string name;
        public string completion_status;
    }
}
