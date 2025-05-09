using UnityEngine;

public class DisappearOnContact : MonoBehaviour
{

    public GameObject targetObject; // The object that will disappear on contact
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == targetObject) 
        {
            DebrisTableManager.Instance.UpdateDebrisActivity(gameObject.name, false);
            gameObject.SetActive(false); // Deactivate the target object on contact
            Debug.Log("Object has disappeared on contact with: " + collision.gameObject.name); // Log the name of the object that disappeared
        }       
    }
}
