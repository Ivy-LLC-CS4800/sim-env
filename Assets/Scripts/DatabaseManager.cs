using UnityEngine;
using Mono.Data.Sqlite;  // SQLite support
using UnityEngine.UI;
using System.Data;  // To access SQLite commands
using System.Security.Cryptography;
using System.Text;  // For password hashing

public class DatabaseManager : MonoBehaviour
{
    private string dbPath = "URI=file:users.db";

    void Start()
    {
        CreateDB();
    }

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

    public bool CheckUsernameAndPassword(string username, string password)
    {
        return CheckUsernameAndPasswordHelper(username, password);
    }

    // Check if the username exists and password is correct
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

    public bool RegisterUsername(string username, string password)
    {
        return RegisterUsernameHelper(username, password);
    }

    // Register a new username with a hashed password
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

    // Check if the username exists in the database
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

    // Hash the password using SHA-256 (for example)
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
}
