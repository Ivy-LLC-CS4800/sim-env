using NUnit.Framework;

public class GlobalTest
{
    /// <summary>
    /// Test: Verify that the default value of `GlobalUser` is set correctly.
    /// Predicted: The `GlobalUser` variable should have the default value "user1".
    /// Checked: The value of `Global.GlobalUser` is compared to "user1".
    /// </summary>
    [Test]
    public void GlobalUser_HasDefaultValue()
    {
        // Assert
        Assert.AreEqual("user1", Global.GlobalUser, "GlobalUser should have the default value 'user1'.");
    }

    /// <summary>
    /// Test: Verify that the `GlobalUser` variable can be updated.
    /// Predicted: The `GlobalUser` variable should reflect the new value after being updated.
    /// Checked: The value of `Global.GlobalUser` is compared to the updated value.
    /// </summary>
    [Test]
    public void GlobalUser_CanBeUpdated()
    {
        // Arrange
        string newUsername = "TestUser";

        // Act
        Global.GlobalUser = newUsername;

        // Assert
        Assert.AreEqual(newUsername, Global.GlobalUser, "GlobalUser should be updated to the new value.");
    }

    /// <summary>
    /// Test: Verify that multiple updates to `GlobalUser` are handled correctly.
    /// Predicted: The `GlobalUser` variable should reflect the most recent value after multiple updates.
    /// Checked: The value of `Global.GlobalUser` is compared to the most recent updated value.
    /// </summary>
    [Test]
    public void GlobalUser_HandlesMultipleUpdates()
    {
        // Arrange
        string firstUpdate = "FirstUser";
        string secondUpdate = "SecondUser";

        // Act
        Global.GlobalUser = firstUpdate;
        Global.GlobalUser = secondUpdate;

        // Assert
        Assert.AreEqual(secondUpdate, Global.GlobalUser, "GlobalUser should reflect the most recent value after multiple updates.");
    }
}