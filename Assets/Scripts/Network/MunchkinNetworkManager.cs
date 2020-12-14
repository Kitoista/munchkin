using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MunchkinNetworkManager : NetworkManager {
    
    public List<Transform> realStartPositions = new List<Transform>();

    public override void Awake() {
        base.Awake();
        NetworkManager.startPositions = realStartPositions;
    }

}
