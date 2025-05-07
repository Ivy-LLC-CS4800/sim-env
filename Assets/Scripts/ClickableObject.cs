using UnityEngine;

public class ClickableObject : MonoBehaviour
{

    public SuitSwapper suitSwapper;
    public void OnObjectClicked()
    {
        // script logic here
        if (suitSwapper != null)
        {
            suitSwapper.SwapToNextModel(); // Or whatever method you want to call
        }
        else
        {
            Debug.LogWarning("SuitSwapper reference is missing!");
        }
    }
}