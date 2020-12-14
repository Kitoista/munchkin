using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UICard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public bool isHovered;
    public bool isDragging;
    public float speed = 2000f;
    public Hand hand;

    public bool isMoving = false;
    Vector3 moveTarget;

    public static UICard draggedCard;

    void Start() {
        hand = transform.parent.GetComponent<Hand>();
    }

    void Update() {
        if (isHovered && (!Functions.IsDraggingAnything() || UICard.draggedCard == this)) {
            if (Input.GetMouseButtonDown(0)) {
                Drag();
                Debug.Log(gameObject.name);
            }
            if (Input.GetMouseButtonUp(0)) {
                Drop();
                if (Functions.IsMouseOnHand()) {
                    hand.SendMessage("AdjustCard", GetComponent<RectTransform>());
                } else {
                    hand.SendMessage("RemoveCard", GetComponent<RectTransform>());
                }
            }
        }
        if (isDragging) {
            transform.position = Input.mousePosition;
        } else if (isMoving) {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, moveTarget, speed * Time.deltaTime);
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        isHovered = false;
    }

    void OnDeactive() {
        isHovered = false;
    }

    void Drag() {
        if (UICard.draggedCard == null) {
            isDragging = true;
            UICard.draggedCard = this;
            isMoving = false;
            // hand.SendMessage("Hide");
        }
    }

    void Drop() {
        isDragging = false;
        UICard.draggedCard = null;
    }

    void MoveTowards(Vector3 target) {
        isMoving = true;
        moveTarget = target;
    }

}
