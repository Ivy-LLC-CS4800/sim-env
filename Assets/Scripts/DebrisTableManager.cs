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
        }//end if-else
    }//end Awake()

    public void Start() {
        CreateDB();
    }//end Start()

    // Idea: Add specific Table Create methods to load them all, Only used for this one table
    public void CreateDB() {
        CreateDebrisTable();
    }//end CreateDB()

    // Create the Debris table if not found
    public void CreateDebrisTable() {
        using (var connection = new SqliteConnection(dbPath)) {
            connection.Open();
            Debug.Log("Database Connection Opened");
            using (var command = connection.CreateCommand()) {
                command.CommandText = "CREATE TABLE IF NOT EXISTS debris (id INTEGER PRIMARY KEY AUTOINCREMENT, prefab_guid TEXT NOT NULL, type TEXT NOT NULL, status TEXT NOT NULL, is_active INTEGER NOT NULL, tool_needed INTEGER NOT NULL, prefab INTEGER);";
                command.ExecuteNonQuery();
                Debug.Log("Table created");
            }//end using
            connection.Close();
            Debug.Log("Database Connection Closed");
        }//end using
    }//end CreateDebrisTable()

    // Add Debris to the Data Table
    public void AddDebris(string prefabGuid, DebrisType type, string status, bool isActive, bool toolNeeded) {
        using (var connection = new SqliteConnection(dbPath)) {
            connection.Open();
            using (var command = connection.CreateCommand()) {
                command.CommandText = @"INSERT INTO debris (prefab_guid, type, status, is_active, tool_needed) VALUES (@prefab_guid, @type, @status, @is_active, @tool_needed)";

                command.Parameters.AddWithValue("@prefab_guid", prefabGuid);
                command.Parameters.AddWithValue("@type", type);
                command.Parameters.AddWithValue("@status", status);
                command.Parameters.AddWithValue("@is_active", isActive ? 1 : 0);
                command.Parameters.AddWithValue("@tool_needed", toolNeeded ? 1 : 0);
                command.ExecuteNonQuery();

                Debug.Log($"Added a Debris to the Database: {prefabGuid}, {type}, {status}, {isActive}, {toolNeeded}");
            }//end using
            connection.Close();
        }//end using
    }//end AddDebris()

    public DebrisData GetDebris(string prefabGuid) {
        DebrisData debrisData = null;
        using (var connection = new SqliteConnection(dbPath)) {
            connection.Open();
            using (var command = connection.CreateCommand()) {
                command.CommandText = "SELECT * FROM debris WHERE prefab_guid = @prefab_guid";
                command.Parameters.AddWithValue("@prefab_guid", prefabGuid);
                using (IDataReader reader = command.ExecuteReader()) {
                    if (reader.Read()) {
                        debrisData = new DebrisData {
                            id = reader.GetInt32(0),
                            guid = reader.GetString(1),
                            type = reader.GetString(2),
                            isActive = reader.GetInt32(3),
                            toolNeeded = reader.GetInt32(4)
                        };
                    }//end if
                }//end using-reader
            }//end using-command
            connection.Close();
        }//end using-connection
        return debrisData;
    }//end GetDebris()

    public void UpdateDebrisActivity(string prefabGuid, bool isActive) {
        using (var connection = new SqliteConnection(dbPath)) {
            connection.Open();
            using (var command = connection.CreateCommand()) {
                command.CommandText = "UPDATE debris SET is_active = @is_active WHERE prefab_guid = @prefab_guid";
                command.Parameters.AddWithValue("@is_active", isActive ? 1 : 0);
                command.Parameters.AddWithValue("@prefab_guid", prefabGuid);
                command.ExecuteNonQuery();
            }//end using-command
            connection.Close();
        }//end using-connection
    }//end UpdateDebrisActivity()

    // Clear the Debris table
    public void DeleteAll() {
        using (var connection = new SqliteConnection(dbPath)) {
            connection.Open();
            using (var command = connection.CreateCommand()) {
                command.CommandText = "DELETE FROM debris;";
                command.ExecuteNonQuery();
            }//end using
            connection.Close();
        }//end using
    }//end DeleteAll()

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
        }//end switch
    }//end toString()
}//end class

public class DebrisData {
    public int id;
    public string guid;
    public string type;
    public int isActive;
    public int toolNeeded;
}
