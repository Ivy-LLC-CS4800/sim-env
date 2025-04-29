using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Notification : MonoBehaviour
{
    public GameObject notificationPopup; // Reference to the notification popup panel
    public TextMeshProUGUI notificationText; // Reference to the notification text

    void Start()
    {
        notificationPopup.SetActive(false); // Hide notification by default
    }

    // Show error popup
    public void ShowPopup(string message)
    {
        notificationPopup.SetActive(true); // Show the popup
        notificationText.text = message; // Set the message in the popup

        // Optionally hide the popup after a few seconds
        Invoke("HidePopup", 2f); // Hide the popup after 2 seconds
    }

    // Hide the error popup
    void HidePopup()
    {
        notificationPopup.SetActive(false); // Hide the popup
    }
}
