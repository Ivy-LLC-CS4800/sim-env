using UnityEngine;

public class RotateDebrisUpright : MonoBehaviour {
    public float xRotation = -89.98f; // Desired rotation around the X axis

    void Start() {
        // Get the current rotation of the instantiated object
        Vector3 currentRotation = transform.rotation.eulerAngles;

        // Adjust the X rotation while keeping the Y and Z rotations
        transform.rotation = Quaternion.Euler(xRotation, currentRotation.y, currentRotation.z);
    }//end Start()
}//end RotateDebrisUpright
