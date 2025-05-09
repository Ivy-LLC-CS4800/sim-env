using log4net.Filter;
using UnityEngine;

public class DebrisFactory : MonoBehaviour, IDebrisFactory {
    [SerializeField] public GameObject[] woodDebrisPrefabs;
    [SerializeField] public GameObject[] metalDebrisPrefabs;
    [SerializeField] public GameObject[] otherDebrisPrefabs;
    [SerializeField] public GameObject[] concreteDebrisPrefabs;
    [SerializeField] public GameObject DisapperOnContact;

    public GameObject CreateDebris(DebrisType type, Vector3 position, Quaternion rotation) {
        GameObject[] prefabs = GetPrefabsForType(type);
        if (prefabs == null || prefabs.Length == 0) {
            Debug.LogError($"No prefabs assigned for DebrisType: {type}");
            return null;
        }//end if

        GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
        GameObject debrisInstance = Instantiate(prefab, position, rotation);

        if (debrisInstance.GetComponent<Outline>() == null) {
            debrisInstance.AddComponent<Outline>();
            debrisInstance.GetComponent<Outline>().enabled = false;
        }//end if
        if (debrisInstance.GetComponent<PickableItem>() == null) {
            debrisInstance.AddComponent<PickableItem>();
        }//end if
        if (debrisInstance.GetComponent<MeshCollider>() == null) {
            debrisInstance.AddComponent<MeshCollider>();
        }//end if
        if (debrisInstance.GetComponent<Rigidbody>() == null) {
            Rigidbody rb = debrisInstance.AddComponent<Rigidbody>();
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.isKinematic = true;
        }//end if

        debrisInstance.layer = LayerMask.NameToLayer("Pickable");

        // Assign a unique ID and log to the database
        string debrisId = System.Guid.NewGuid().ToString();
        debrisInstance.name = debrisId;
        DebrisTableManager.Instance.AddDebris(debrisId, type, "Active", true, false);

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