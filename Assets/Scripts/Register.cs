using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Register : MonoBehaviour
{
    public InputField usernameInputField;
    public InputField passwordInputField; // Added password input field
    public Button registerButton;
    public TextMeshProUGUI lengthConditionText; // Reference to the notification text
    public Color validColor = Color.green;
    public Color invalidColor = Color.red;
    public float delayBeforeSceneChange = 2f; // Delay in seconds

    public SceneLoader sceneCall;
    public DatabaseManager databaseCall;
    public Notification errorCall;
    public Notification successCall;

    void Start()
    {
        passwordInputField.onValueChanged.AddListener(ValidatePassword);
        UpdateConditionText();
        registerButton.onClick.AddListener(OnRegisterButtonClick);
    }

    // Check password
    public void ValidatePassword(string password)
    {
        bool isLengthValid = password.Length >= 6;

        // Update condition texts based on validation
        lengthConditionText.text = "At least 6 characters";
        lengthConditionText.color = isLengthValid ? validColor : invalidColor;
    }

    // Method to update condition texts
    void UpdateConditionText()
    {
        lengthConditionText.text = "At least 6 characters";

        lengthConditionText.color = invalidColor;
    }

     // Register button click event
    public void OnRegisterButtonClick()
    {
        string username = usernameInputField.text;
        string password = passwordInputField.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            return;
        }

        if (password.Length < 6)
        {
            errorCall.ShowPopup("Password must be at least 6 characters.");
        }
        // Register the new user with hashed password
        else if (databaseCall.RegisterUsername(username, password))
        {
            successCall.ShowPopup("User registered: " + username);
            Debug.Log("User registered: " + username);
            Invoke("ChangeToMainScene", delayBeforeSceneChange);
        }
        else
        {
            errorCall.ShowPopup("Username already exists.");
            Debug.Log("Username already exists.");
        }
    }

    private void ChangeToMainScene()
    {
        sceneCall.LoadMainScene();
    }

}
