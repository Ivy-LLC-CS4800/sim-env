using UnityEngine;

public class DisappearOnContact : MonoBehaviour {

    public GameObject targetObject; // The object that will disappear on contact

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject == targetObject)  {
            gameObject.SetActive(false); // Deactivate the target object on contact
            Debug.Log("Object has disappeared on contact with: " + collision.gameObject.name); // Log the name of the object that disappeared
        }//end if       
    }//end OnCollisionEnter()
    
    private string GetDebrisType(string debrisName) {
        // Extract debris type from the debris name (assuming the name contains the type)
        if (debrisName.Contains("Wood")) return "Wood";
        if (debrisName.Contains("Metal")) return "Metal";
        if (debrisName.Contains("Other")) return "Other";
        if (debrisName.Contains("Concrete")) return "Concrete";
        return "Unknown";
    }//end GetDebrisType()
}//end DisappearOnContact
