using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Checkable : NetworkBehaviour
{
    public float yDistance = 12;
    GameObject mainCamera;

    GameObject GetCamera() {
        if (mainCamera == null) {
            mainCamera = Player.localPlayer.gameObject;
        }
        return mainCamera;
    }

    void OnMouseOver() {
        if (Input.GetMouseButtonDown(1)) {
            MoveCamera(GetCamera().GetComponent<NetworkIdentity>());
        }
    }

    [Command(ignoreAuthority = true)]
    void MoveCamera(NetworkIdentity netId) {
        netId.SendMessage("MoveTo", new CameraMovementEvent(
            new Vector3(transform.position.x, transform.position.y + yDistance, transform.position.z),
            Quaternion.Euler(90, transform.eulerAngles.y, 0),
            GetComponent<NetworkIdentity>()
        ));
    }

}
