using NUnit.Framework;

public class GlobalTest
{
    /// <summary>
    /// Test: Verify that the default value of `Username` is set correctly.
    /// Predicted: The `Username` variable should have the default value "user1".
    /// Checked: The value of `Global.Username` is compared to "user1".
    /// </summary>
    [Test]
    public void Username_HasDefaultValue()
    {
        // Assert
        Assert.AreEqual("user1", Global.Username, "Username should have the default value 'user1'.");
    }

    /// <summary>
    /// Test: Verify that the `Username` variable can be updated.
    /// Predicted: The `Username` variable should reflect the new value after being updated.
    /// Checked: The value of `Global.Username` is compared to the updated value.
    /// </summary>
    [Test]
    public void Username_CanBeUpdated()
    {
        // Arrange
        string newUsername = "TestUser";

        // Act
        Global.Username = newUsername;

        // Assert
        Assert.AreEqual(newUsername, Global.Username, "Username should be updated to the new value.");
    }

    /// <summary>
    /// Test: Verify that multiple updates to `Username` are handled correctly.
    /// Predicted: The `Username` variable should reflect the most recent value after multiple updates.
    /// Checked: The value of `Global.Username` is compared to the most recent updated value.
    /// </summary>
    [Test]
    public void Username_HandlesMultipleUpdates()
    {
        // Arrange
        string firstUpdate = "FirstUser";
        string secondUpdate = "SecondUser";

        // Act
        Global.Username = firstUpdate;
        Global.Username = secondUpdate;

        // Assert
        Assert.AreEqual(secondUpdate, Global.Username, "Username should reflect the most recent value after multiple updates.");
    }
}