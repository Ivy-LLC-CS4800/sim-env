using UnityEngine;

public interface IDebrisFactory {
    GameObject CreateDebris(DebrisType type, Vector3 position, Quaternion rotation);
}
