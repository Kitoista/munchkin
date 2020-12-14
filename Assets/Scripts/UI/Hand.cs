using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Hand : MonoBehaviour {
    public List<Card> cards = new List<Card>();
    public RectTransform rectTransform;

    public float hiddenY = -10;
    public float speed = 2000;

    public bool isShowed;
    public bool isHovered;

    public bool addedCardThisTick = false;

    void Start() {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update() {
        if (Functions.IsMouseOnCanvas()) {
            if (Functions.IsPointInRT(rectTransform, Input.mousePosition) || Input.mousePosition.y < rectTransform.position.y) {
                if (!isHovered) {
                    OnPointerEnter();
                }
            } else if (isHovered) {
                OnPointerExit();
            }
        }
        addedCardThisTick = false;
        if (isShowed && transform.position.y < rectTransform.rect.height / 2) {
            Vector3 goal = transform.position;
            goal.y = rectTransform.rect.height /2;
            transform.position = Vector3.MoveTowards(transform.position, goal, speed * Time.deltaTime);
        } else if (!isShowed && transform.position.y > hiddenY) {
            Vector3 goal = transform.position;
            goal.y = hiddenY;
            transform.position = Vector3.MoveTowards(transform.position, goal, speed * Time.deltaTime);
        }
        if (Input.GetMouseButtonUp(0) && isHovered) {
            Debug.Log("CLICKED");
            if (Card.draggedCard != null) {
                AddCard(Card.draggedCard);
                Card.draggedCard = null;
            }
        }
        if (UICard.draggedCard != null) {
            SortCardsByLocations();
        }
    }

    void AddCard(Card card) {
        RectTransform uiCard = Functions.FindUICard(card);
        uiCard.SetParent(transform);
        uiCard.position = Input.mousePosition;

        cards.Add(card);
        uiCard.gameObject.SetActive(true);
        Functions.instance.SetActive(card.gameObject, false);

        SortCardsByLocations();
        addedCardThisTick = true;
    }

    void RemoveCard(Card card) {
        RectTransform uiCard = Functions.FindUICard(card);
    }

    void RemoveCard(RectTransform uiCard) {
        if (addedCardThisTick) {
            return;
        }
        Debug.Log("REMOVECARD " + gameObject.name);
        int index = GetChildIndex(uiCard);
        Debug.Log(index);

        uiCard.SetParent(GameObject.Find("UICards").transform);
        uiCard.SendMessage("OnDeactive");
        uiCard.gameObject.SetActive(false);

        Card card = Functions.FindCard(uiCard);
        cards.Remove(card);

        SetChildLocations();
        Functions.instance.SetActive(card.gameObject, true);
        card.SendMessage("Drag");
    }

    void AdjustCard(RectTransform uiCard) {
        SortCardsByLocations();
    }

    void OnPointerEnter() {
        isHovered = true;
        Functions.mouseOnHand = true;
        Show();
    }

    void OnPointerExit() {
        isHovered = false;
        Functions.mouseOnHand = false;
        Hide();
    }

    void Show() {
        isShowed = true;
    }

    void Hide() {
        isShowed = false;
    }

    int GetChildIndex(RectTransform uiCard) {
        int re = 0;
        for (int i = 0; i < transform.childCount; i++) {
            Transform child = transform.GetChild(i);
            if (child.localPosition.x < uiCard.localPosition.x) {
                ++re;
            }
        }
        return re;
    }

    public void SetChildLocations() {
        if (cards.Count == 0) {
            return;
        }
        // Debug.Log(cards.Count);
        RectTransform uiCardExample = Functions.FindUICard(cards[0]);
        float diff = (uiCardExample.rect.width / 2);
        float x = (cards.Count - 1) * -diff;
        // Debug.Log(x);
        for (int i = 0; i < cards.Count; i++) {
            RectTransform uiCard = Functions.FindUICard(cards[i]);
            if (UICard.draggedCard == null || UICard.draggedCard.gameObject != uiCard.gameObject) {
                uiCard.SendMessage("MoveTowards", new Vector3(x, 0, 0));
            }
            x += diff * 2;
        }
    }

    public void SortCardsByLocations() {
        cards.Sort((a, b) => {
            return Functions.FindUICard(a).localPosition.x < Functions.FindUICard(b).localPosition.x ? -1 : 1;
        });
        SetChildLocations();
    }

}
