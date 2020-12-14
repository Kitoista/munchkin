using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DeckRenderer : NetworkBehaviour {

    public Transform top;
    public Card cardExample;
    public Deck deck;
    Renderer topRenderer;

    void Awake() {
        Debug.Log("Awake");
        Init();
    }

    private void Init() {
        if (cardExample == null) {
            throw new System.Exception("CardExample mustn't be empty (" + gameObject.name + ")");
        }
        topRenderer = top.GetComponent<Renderer>();
        if (topRenderer == null) {
            throw new System.Exception("TopRenderer cant be null (" + gameObject.name + ")");
        }
        deck = GetComponent<Deck>();
    }

    [Client]
    void OnStartPlayer() {
        Debug.Log("UPDATE ME BITCH");
        UpdateMe(Player.localPlayerId);
    }

    [Server]
    public void RedrawDeck(DeckUpdateEvent e) {
        if (e.oldValue == 0) {
            NotEmpty();
        }

        float cardHeight = cardExample.transform.localScale.y;
        int cardDiff = e.newValue - e.oldValue;

        Vector3 scale = transform.localScale;
        Vector3 position = transform.position;
        
        scale.y += cardHeight * cardDiff;
        position.y += cardHeight * cardDiff * 0.5f;
        
        transform.localScale = scale;
        transform.position = position;

        float topDiff = 0.0001f;
        if (e.newValue < 100) {
            topDiff = 0.01f - e.newValue * 0.0001f;
        }
        top.localPosition = new Vector3(0, 0.5f + topDiff, 0);

        if (e.newValue == 0) {
            Empty();
        }
        if (Player.players.Count > 0) {
            SetClientMaterial(e);
        }
        //   else if (e.topCard) {
        //     if (!deck.isFaceDown) {
        //         topRenderer.material.mainTexture = e.topCard.transform.Find("Front").GetComponent<Renderer>().material.mainTexture;
        //     } else {
        //         topRenderer.material.mainTexture = e.topCard.transform.Find("Back").GetComponent<Renderer>().material.mainTexture;
        //     }
        // }

    }

    [Command(ignoreAuthority = true)]
    private void UpdateMe(NetworkIdentity player) {
        if (deck.cards.Count > 0) {
            SetTargetMaterial(player.connectionToClient, new DeckUpdateEvent(0, deck.cards.Count, deck.cards[deck.cards.Count - 1]));
        } else {
            SetTargetMaterial(player.connectionToClient, new DeckUpdateEvent(1, 0, null));
        }
    }

    void Empty() {
        Vector3 scale = transform.localScale;
        scale.y += 0.1f;
        transform.localScale = scale;
    }

    void NotEmpty() {
        Vector3 scale = transform.localScale;
        scale.y -= 0.1f;
        transform.localScale = scale;
    }

    [ClientRpc]
    void SetClientMaterial(DeckUpdateEvent e) {
        _RedrawClient(e);
    }

    [TargetRpc]
    void SetTargetMaterial(NetworkConnection conn, DeckUpdateEvent e) {
        _RedrawClient(e);
    }

    [Client]
    void _RedrawClient(DeckUpdateEvent e) {
        if (e.oldValue == 0) {
            topRenderer.material.color = Color.white;
        }
        if (e.newValue == 0) {
            topRenderer.material.color = Color.black;
        } else if (e.topCard) {
            if (!deck.isFaceDown) {
                topRenderer.material.mainTexture = e.topCard.transform.Find("Front").GetComponent<Renderer>().material.mainTexture;
            } else {
                topRenderer.material.mainTexture = e.topCard.transform.Find("Back").GetComponent<Renderer>().material.mainTexture;
            }
        }
    }

}
