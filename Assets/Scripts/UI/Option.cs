using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Option : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public bool isHovered = false;
    public MaskableGraphic image;
    public bool isReady = false;

    public static GameObject optionTarget;
    RectTransform rectTransform;
    string message;

    public static void ShowOptionsFor(string name, GameObject target) {
        GameObject options = GameObject.FindGameObjectWithTag("MainCanvas").transform.Find(name).gameObject;
        Debug.Log(options);
        Option.optionTarget = target;
        options.SetActive(true);
        for (int i = 0; i < options.transform.childCount; ++i) {
            options.transform.GetChild(i).SendMessage("Activated");
        }
    }

    void Start() {
        rectTransform = image.GetComponent<RectTransform>();
        message = transform.Find("Message").GetComponent<Text>().text;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        Activate();
    }

    public void OnPointerExit(PointerEventData eventData) {
        Deactivate();
    }

    void Update() {
        if (Input.GetMouseButtonUp(0) && isHovered && isReady) {
            Deactivate();
            optionTarget.SendMessage(message);
            transform.parent.gameObject.SetActive(false);
        }
        isReady = true;
    }

    void Activate() {
        rectTransform.SetSizeWithCurrentAnchors( RectTransform.Axis.Vertical, 1000);
        rectTransform.SetSizeWithCurrentAnchors( RectTransform.Axis.Horizontal, 500);
        Color color = image.color;
        color.a = 200f / 255f;
        image.color = color;
        
        Vector3 pos = rectTransform.localPosition;
        pos *= 1.1f;
        rectTransform.localPosition = pos;
        isHovered = true;
    }

    void Deactivate() {
        rectTransform.SetSizeWithCurrentAnchors( RectTransform.Axis.Vertical, 900);
        rectTransform.SetSizeWithCurrentAnchors( RectTransform.Axis.Horizontal, 450);
        Color color = image.color;
        color.a = 0.5f;
        image.color = color;

        Vector3 pos = rectTransform.localPosition;
        pos *= 1f / 1.1f;
        rectTransform.localPosition = pos;
        isHovered = false;
    }

    void Activated() {
        // isReady = false;
    }

}
