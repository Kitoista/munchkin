using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour {
    
    public static Player localPlayer = null;
    public static NetworkIdentity localPlayerId = null;
    public static List<Player> players = new List<Player>();

    [SyncVar(hook = nameof(OnColorChange))]
    public Color color;

    public List<Color> colors;

    void Start() {
        if (isLocalPlayer) {
            transform.Find("Main Camera").gameObject.SetActive(true);
            Player.localPlayer = this;
            Player.localPlayerId = GetComponent<NetworkIdentity>();
            Functions.BroadcastAll("OnStartPlayer");
        }
        if (isServer) {
            color = colors[players.Count];
        }
        Debug.Log("Start");
        players.Add(this);
    }

    void OnColorChange(Color oldColor, Color newColor) {
        GetComponent<Renderer>().material.color = newColor;
    }

}
