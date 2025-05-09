using log4net.Filter;
using UnityEngine;

public class DebrisFactory : MonoBehaviour, IDebrisFactory {
    // Array to hold the debris prefabs
    [SerializeField] public GameObject[] woodDebrisPrefabs;
    [SerializeField] public GameObject[] metalDebrisPrefabs;
    [SerializeField] public GameObject[] otherDebrisPrefabs;
    [SerializeField] public GameObject[] concreteDebrisPrefabs;

    public GameObject CreateDebris(DebrisType type, Vector3 position, Quaternion rotation) {
        // Equal to the Array for the selected prefab type
        GameObject[] prefabs = GetPrefabsForType(type);
        if (prefabs == null || prefabs.Length == 0) {
            Debug.LogError($"No prefabs assigned for DebrisType: {type}");
            return null;
        }//end if

        // Randomly select a prefab from the array
        GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
        GameObject debrisInstance = Instantiate(prefab, position, rotation);

        if(debrisInstance.GetComponent<Outline>() == null){
            debrisInstance.AddComponent<Outline>();
            debrisInstance.GetComponent<Outline>().enabled = false;
        }//end if
        if(debrisInstance.GetComponent<PickableItem>() == null){
            debrisInstance.AddComponent<PickableItem>();
        }//end if
        if(debrisInstance.GetComponent<MeshCollider>() == null){
            debrisInstance.AddComponent<MeshCollider>();
            Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
            mesh.UploadMeshData(true); // Ensure the mesh data is readable
        }//end if
        if(debrisInstance.GetComponent<Rigidbody>() == null){
            Rigidbody rb = debrisInstance.AddComponent<Rigidbody>();
            rb.collisionDetectionMode=CollisionDetectionMode.Continuous;
            rb.isKinematic = true;
        }//end if
        debrisInstance.layer = LayerMask.NameToLayer("Pickable");

        return debrisInstance;
    }//end CreateDebris()

    public GameObject[] GetPrefabsForType(DebrisType type) {
        switch (type) {
            case DebrisType.Wood:
                return woodDebrisPrefabs;
            case DebrisType.Metal:
                return metalDebrisPrefabs;
            case DebrisType.Other:
                return otherDebrisPrefabs;
            case DebrisType.Concrete:
                return concreteDebrisPrefabs;
            default:
                return null;
        }//end switch
    }//end GetPrefabsForType()
}//end DebrisFactory
