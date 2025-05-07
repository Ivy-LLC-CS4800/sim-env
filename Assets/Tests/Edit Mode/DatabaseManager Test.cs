using NUnit.Framework;
using System.IO;
using UnityEngine;

public class DatabaseManagerTests
{
    private GameObject dbGO;
    private DatabaseManager dbManager;

    private string testDBPath = "URI=file:test_users.db";

    /// <summary>
    /// Creats an instance of a dummy database for testing.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        dbGO = new GameObject("DBManager");
        dbManager = dbGO.AddComponent<DatabaseManager>();

        // Override the path with a test database
        var pathField = typeof(DatabaseManager).GetField("dbPath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        pathField.SetValue(dbManager, testDBPath);

        dbManager.CreateDB(); // Ensure table is ready
    }

    /// <summary>
    /// Resets testing enviornment for accurate tests
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(dbGO);

        // Delete test DB file to reset environment
        var realPath = Path.Combine(Application.dataPath, "../test_users.db");
        if (File.Exists(realPath))
        {
            File.Delete(realPath);
        }
    }

    //Test: Registering a new user with their username and password
    //Predicted: true, user was successfully registered
    //Checked: RegisterUsername(), with testUser and testPass
    [Test]
    public void RegisterUsername_Succeeds_WhenNewUser()
    {
        bool result = dbManager.RegisterUsername("testUser", "testPass");
        Assert.IsTrue(result);
    }

    //Test: User exsists when attempted to register new user, but different passwords used
    //Predicted: False, user failed regristation 
    //Checked: RegisterUsername(), with duplicateUser, pass1, pass2
    [Test]
    public void RegisterUsername_Fails_WhenDuplicateUser()
    {
        dbManager.RegisterUsername("duplicateUser", "pass1");
        bool result = dbManager.RegisterUsername("duplicateUser", "pass2");
        Assert.IsFalse(result);
    }

    //Test: User can successfully log in using correct username and password
    //Predicted: True, username and password match database
    //Checked: CheckUsernameAndPassword(), with validUser and secret
    [Test]
    public void CheckUsernameAndPassword_ReturnsTrue_WithCorrectCredentials()
    {
        dbManager.RegisterUsername("validUser", "secret");
        bool result = dbManager.CheckUsernameAndPassword("validUser", "secret");
        Assert.IsTrue(result);
    }

    //Test: User input the incorrect password and correct username
    //Predicted: False, password did not match user profile
    //Checked: CheckUsernameAndPassword, validUser2, correctPass, wrongPass
    [Test]
    public void CheckUsernameAndPassword_ReturnsFalse_WithWrongPassword()
    {
        dbManager.RegisterUsername("validUser2", "correctPass");
        bool result = dbManager.CheckUsernameAndPassword("validUser2", "wrongPass");
        Assert.IsFalse(result);
    }

    //Test: User attempts to login without registering a profile first
    //Predicted: False, username and password do not match anything in database
    //Checked: CheckUsernameAndPassword(), ghostUser and nope
    [Test]
    public void CheckUsernameAndPassword_ReturnsFalse_WithNonExistentUser()
    {
        bool result = dbManager.CheckUsernameAndPassword("ghostUser", "nope");
        Assert.IsFalse(result);
    }
}

