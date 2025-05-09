using UnityEngine;

public class CheckForMesh : MonoBehaviour {
    public GameObject debrisInstance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        CheckMeshReadability(debrisInstance);
    }

    private void CheckMeshReadability(GameObject debrisInstance) {
        MeshFilter meshFilter = debrisInstance.GetComponent<MeshFilter>();
        if (meshFilter != null) {
            Mesh mesh = meshFilter.sharedMesh;
            if (mesh != null && !mesh.isReadable) {
                Debug.LogWarning($"Mesh '{mesh.name}' is not readable. Enable 'Read/Write' in import settings.");
            }
        }
    }
}
