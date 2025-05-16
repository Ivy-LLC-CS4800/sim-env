using UnityEngine;
using UnityEngine.UI;

//<summary>
// Delete a user account.
//</summary>
public class Delete : MonoBehaviour
{
    public GameObject confirmationOverlay;
    public Button deleteButton;
    public Button yesButton;
    public Button noButton;
    public float delayBeforeSceneChange = 2f; // Delay in seconds


    public SceneLoader sceneCall;
    public DatabaseManager databaseCall;
    public Notification errorCall;
    public Notification successCall;

    void Start()
    {
        deleteButton.onClick.AddListener(OnDeleteButtonClicked);
        yesButton.onClick.AddListener(OnConfirmDelete);
        noButton.onClick.AddListener(OnCancelDelete);

        confirmationOverlay.SetActive(false);
    }

    void OnDeleteButtonClicked()
    {
        confirmationOverlay.SetActive(true);
    }

    void OnCancelDelete()
    {
        confirmationOverlay.SetActive(false);
    }

    void OnConfirmDelete()
    {
        bool success = DatabaseManager.Instance.DeleteAccount(Global.Username, Global.Password);

        if (success)
        {
            successCall.ShowPopup("Account successfully deleted.");
            Debug.Log("Deletion successful.");
            Invoke("ChangeToLoginScene", delayBeforeSceneChange);
        }
        else
        {
            errorCall.ShowPopup("Error: Account could not be deleted.");
            Debug.Log("Deletion failed.");
        }

        confirmationOverlay.SetActive(false);
    }

    private void ChangeToLoginScene()
    {
        sceneCall.LoadLoginScene();
    }

}
