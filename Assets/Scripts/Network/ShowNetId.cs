using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ShowNetId : NetworkBehaviour {

    void Start() {
        Debug.Log("REEEE " + GetComponent<NetworkIdentity>().netId + " = " + gameObject.name);
    }

}