using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour {


    public Deck doorDeck;
    public Deck doorTrashDeck;
    public Deck treasureDeck;
    public Deck treasureTrashDeck;

    public Transform cards;

    [Server]
    public override void OnStartServer() {
        doorDeck = GameObject.Find("DoorDeck").GetComponent<Deck>();
        doorTrashDeck = GameObject.Find("DoorTrashDeck").GetComponent<Deck>();
        treasureDeck = GameObject.Find("TreasureDeck").GetComponent<Deck>();
        treasureTrashDeck = GameObject.Find("TreasureTrashDeck").GetComponent<Deck>();

        cards = GameObject.Find("Cards").transform;

        for (int i = 0; i < cards.childCount; i++) {
            NetworkIdentity card = cards.GetChild(i).GetComponent<NetworkIdentity>();
            if (card.gameObject.name.StartsWith("Door")) {
                doorDeck._AddCardServer(card);
            } else {
                treasureDeck._AddCardServer(card);
            }
        }

        doorDeck._Shuffle();
        treasureDeck._Shuffle();
    }

}
