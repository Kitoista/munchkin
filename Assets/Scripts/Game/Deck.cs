using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Deck : NetworkBehaviour
{

    public List<Card> cards = new List<Card>();

    public bool isFaceDown = true;

    void Start() {
        if (isServer) {
            SendMessage("RedrawDeck", new DeckUpdateEvent(1, cards.Count, cards.Count > 0 ? cards[cards.Count - 1] : null));
        }
    }

    void Update() {
        RaycastHit hit;

        if (Input.GetMouseButtonDown(0) &&
            Card.draggedCard != null &&
            Functions.IsMouseOnLayer("Deck", out hit) &&
            (hit.collider.gameObject == gameObject || hit.collider.transform.parent.gameObject == gameObject)
        ) {
            Card.draggedCard.isDragging = false;
            AddCard(Card.draggedCard);
            Card.draggedCard = null;
        }
    }


    [Command(ignoreAuthority = true)]
    public void Shuffle() {
        _Shuffle();
    }

    [Server]
    public void _Shuffle() {
        cards.Sort((Card a, Card b) => {
            return Mathf.FloorToInt(Random.Range(0, 3)) - 1;
        });
        SendMessage("RedrawDeck", new DeckUpdateEvent(cards.Count, cards.Count, cards[cards.Count - 1]));
    }

    void OnMouseDown() {
        if (!Functions.IsDraggingAnything()) {
            Option.ShowOptionsFor("DeckOptions", gameObject);
        }
    }

    // Add a card
    public void AddCard(Card card) {
        AddCardServer(card.GetComponent<NetworkIdentity>());
    }

    [Command(ignoreAuthority = true)]
    void AddCardServer(NetworkIdentity cardId) {
        _AddCardServer(cardId);
    }

    public void _AddCardServer(NetworkIdentity cardId) {
        Card card = cardId.GetComponent<Card>();
        cards.Add(card);
        Debug.Log("ADDCARD " + card.gameObject.name);
        Functions.instance._SetActiveServer(card.gameObject, false);
        SendMessage("RedrawDeck", new DeckUpdateEvent(cards.Count - 1, cards.Count, cards[cards.Count - 1]));
    }


    // Take a card
    void Take() {
        TakeServer(Player.localPlayerId);
    }

    [Command(ignoreAuthority = true)]
    void TakeServer(NetworkIdentity player) {
        Card card = PopCard();
        if (card == null) {
            return;
        }
        Debug.Log(card.gameObject.name);

        card.SetFacing(!isFaceDown);
        if (!isFaceDown) {
            card.transform.rotation = card.transform.rotation * Quaternion.Euler(0, 180, 0);
        }
        Functions.instance.SetActive(card.gameObject, true);
        TakeClient(player.connectionToClient, card.GetComponent<NetworkIdentity>());
    }

    [TargetRpc]
    void TakeClient(NetworkConnection conn, NetworkIdentity cardId) {
        Card.draggedCard = cardId.GetComponent<Card>();
        Card.draggedCard.isDragging = true;
    }

    public Card PopCard() {
        if (cards.Count == 0) {
            return null;
        }
        Card card = cards[cards.Count - 1];
        cards.RemoveAt(cards.Count - 1);

        card.transform.position = TopPosition();
        SendMessage("RedrawDeck", new DeckUpdateEvent(cards.Count + 1, cards.Count, cards.Count > 0 ? cards[cards.Count - 1] : null));
        
        return card;
    }

    Vector3 TopPosition() {
        return transform.Find("Back").position;
    }

}
