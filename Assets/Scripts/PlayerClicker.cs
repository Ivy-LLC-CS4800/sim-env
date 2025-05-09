using UnityEngine;

public class PlayerClicker : MonoBehaviour
{
    public float interactDistance = 50f;

    void Update()
    {   
        
        
        if (Input.GetMouseButtonDown(0)) // Left-click
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                Debug.Log("Hit: " + hit.collider.name);
            }

            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                ClickableObject clickable = hit.collider.GetComponent<ClickableObject>();
                if (clickable != null)
                {
                    clickable.OnObjectClicked();
                }
            }
        }
    }
}
