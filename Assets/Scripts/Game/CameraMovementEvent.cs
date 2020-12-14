using UnityEngine;
using Mirror;

[System.Serializable]
public struct CameraMovementEvent {

    public Vector3 position;
    public Quaternion rotation;
    public NetworkIdentity target;
    public CameraMovementEvent(Vector3 p, Quaternion r, NetworkIdentity t) {
        position = p;
        rotation = r;
        target = t;
    }

}
