using UnityEngine;

public class SharedMesh : MonoBehaviour {
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        mesh.UploadMeshData(true); // Ensure the mesh data is readable
    }//end Start()
}//end SharedMesh
