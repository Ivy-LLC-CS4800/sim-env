using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProgressBar : MonoBehaviour
{
    private Slider progressBar;  // Reference to the Slider (Progress Bar)

    public float duration = 1f;  // Duration of the progress animation (in seconds)


    // Initialize the slider
    void Start()
    {
        progressBar = GetComponent<Slider>();  // Get the Slider component attached to this GameObject
        progressBar.value = 0f;  // Ensure the slider starts at 0 when the scene starts
        // Disable slider interaction to prevent player from adjusting it
        progressBar.interactable = false;
    }

    // Method to fill the progress bar smoothly to a target value
    public void SetProgress(float targetValue)
    {
        Debug.Log($"Setting progress to {targetValue}");  // Debug: Check target value
        StartCoroutine(FillProgressBar(targetValue));  // Start filling the slider smoothly
    }

    private IEnumerator FillProgressBar(float targetValue)
    {
        float startValue = progressBar.value;  // Initial value
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            progressBar.value = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);
            yield return null;
        }

        progressBar.value = targetValue;  // Ensure it reaches the final value
    }
}
