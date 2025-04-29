using UnityEngine;
using UnityEngine.UI;


public class Login : MonoBehaviour
{
    public InputField usernameInputField;
    public InputField passwordInputField; // Added password input field
    public Button loginButton;
    public Button registerButton;
    public float delayBeforeSceneChange = 2f; // Delay in seconds


    public SceneLoader sceneCall;
    public DatabaseManager databaseCall;
    public Notification errorCall;
    public Notification successCall;

    void Start()
    {
        // Add button listeners
        loginButton.onClick.AddListener(OnLoginButtonClick);
        registerButton.onClick.AddListener(OnRegisterButtonClick);
    }

      // Login button click event
    public void OnLoginButtonClick()
    {
        string username = usernameInputField.text;
        string password = passwordInputField.text;
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            return;
        }

        // Check if the username exists and password is correct
        if (databaseCall.CheckUsernameAndPassword(username, password))
        {
            successCall.ShowPopup("Login successful for: " + username);
            Debug.Log("Login successful for: " + username);
            Invoke("ChangeToMainScene", delayBeforeSceneChange);
            Global.GlobalUser = username;
        }
        else
        {
            errorCall.ShowPopup("Username or password is incorrect.");
            Debug.Log("Username or password is incorrect.");
        }
    }

    // Register button click event
    public void OnRegisterButtonClick()
    {
        sceneCall.LoadRegisterScene();
    }

    private void ChangeToMainScene()
    {
        sceneCall.LoadMainScene();
    }

}
