using UnityEngine;
using Mono.Data.Sqlite;  // SQLite support
using System.Data;  // To access SQLite commands

public class DebrisTableManager : MonoBehaviour {
    public static DebrisTableManager Instance { get; private set; }
    private string dbPath = "URI=file:" + Application.streamingAssetsPath + "/debris.db";

    // Make Database persistent across Scenes
    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);  // Destroy duplicate instance
        } else {
            Instance = this;  // Set the instance
            DontDestroyOnLoad(gameObject);  // Make sure it persists across scenes
        }
    }

    public void Start() {
        CreateDB();
    }

    // Idea: Add specific Table Create methods to load them all, Only used for this one table
    public void CreateDB() {
        CreateDebrisTable();
    }

    // Create the Debris table if not found
    public void CreateDebrisTable() {
        using (var connection = new SqliteConnection(dbPath)) {
            connection.Open();
            Debug.Log("Database Connection Opened");
            using (var command = connection.CreateCommand()) {
                command.CommandText = "CREATE TABLE IF NOT EXISTS debris (id INTEGER PRIMARY KEY AUTOINCREMENT, debris_num TEXT NOT NULL, type TEXT NOT NULL, status TEXT NOT NULL, is_active INTEGER NOT NULL, tool_needed INTEGER NOT NULL, prefab INTEGER);";
                command.ExecuteNonQuery();
                Debug.Log("Debris Table created");
            }
            connection.Close();
            Debug.Log("Database Connection Closed");
        }
    }

    // Add Debris to the Data Table
    public void AddDebris(int debrisNum, DebrisType type, string status, bool isActive, bool toolNeeded) {
        using (var connection = new SqliteConnection(dbPath)) {
            connection.Open();
            using (var command = connection.CreateCommand()) {
                command.CommandText = @"INSERT INTO debris (debris_num, type, status, is_active, tool_needed) VALUES (@debris_num, @type, @status, @is_active, @tool_needed)";

                command.Parameters.AddWithValue("@prefab_guid", debrisNum);
                command.Parameters.AddWithValue("@type", toString(type));
                command.Parameters.AddWithValue("@status", status);
                command.Parameters.AddWithValue("@is_active", isActive ? 1 : 0);
                command.Parameters.AddWithValue("@tool_needed", toolNeeded ? 1 : 0);
                command.ExecuteNonQuery();

                Debug.Log($"Added a Debris to the Database: {debrisNum}, {type}, {status}, {isActive}, {toolNeeded}");
            }//end using-command
            connection.Close();
        }//end using-connection
    }

    public DebrisData GetDebris(int debrisNum) {
        DebrisData debrisData = null;
        using (var connection = new SqliteConnection(dbPath)) {
            connection.Open();
            using (var command = connection.CreateCommand()) {
                command.CommandText = "SELECT * FROM debris WHERE debris_num = @debris_num";
                command.Parameters.AddWithValue("@debris_num", debrisNum);
                using (IDataReader reader = command.ExecuteReader()) {
                    if (reader.Read()) {
                        debrisData = new DebrisData {
                            id = reader.GetInt32(0),
                            guid = reader.GetInt32(1),
                            type = reader.GetString(2),
                            isActive = reader.GetInt32(3),
                            toolNeeded = reader.GetInt32(4)
                        };
                    }
                }
            }
            connection.Close();
        }
        return debrisData;
    }

    public void UpdateDebrisActivity(int debrisNum, bool isActive) {
        using (var connection = new SqliteConnection(dbPath)) {
            connection.Open();
            using (var command = connection.CreateCommand()) {
                command.CommandText = "UPDATE debris SET is_active = @is_active WHERE debris_num = @debris_num";
                command.Parameters.AddWithValue("@is_active", isActive ? 1 : 0);
                command.Parameters.AddWithValue("@debris_num", debrisNum);
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }

    // Clear the Debris table
    public void DeleteAll() {
        using (var connection = new SqliteConnection(dbPath)) {
            connection.Open();
            using (var command = connection.CreateCommand()) {
                command.CommandText = "DELETE FROM debris;";
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }

    public string toString(DebrisType debrisType) {
        switch (debrisType) {
            case DebrisType.Wood:
                return "Wood";
            case DebrisType.Metal:
                return "Metal";
            case DebrisType.Other:
                return "Other";
            case DebrisType.Concrete:
                return "Concrete";
            default:
                return "Unknown";
        }
    }
}

public class DebrisData {
    public int id;
    public int guid;
    public string type;
    public int isActive;
    public int toolNeeded;
}
