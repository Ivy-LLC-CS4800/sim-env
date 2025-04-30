using UnityEngine;
using UnityEngine.UI;

//<summary>
// Takes user inputted username and password and checks against database before moving to main menu
//</summary>
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

    //TODO: Enables button behaviors with listeners
    //Parameters: Application start
    void Start()
    {
        // Add button listeners
        loginButton.onClick.AddListener(OnLoginButtonClick);
        registerButton.onClick.AddListener(OnRegisterButtonClick);
    }

    //TODO: When login is clicked, takes textfields and checks them against database
    //Parameters: login click, username, password, initialized database instance
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

    //TODO: Register button click event
    //Parameters: Button listeners
    public void OnRegisterButtonClick()
    {
        sceneCall.LoadRegisterScene();
    }

    //TODO: Changes scene to display the main menu
    //Parameters: Button listeners, valid username, valid password, main menu scene
    private void ChangeToMainScene()
    {
        sceneCall.LoadMainScene();
    }

}
