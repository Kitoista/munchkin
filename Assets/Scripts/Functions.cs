using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Functions : NetworkBehaviour {

    public static bool mouseOnHand;
    protected Dictionary<NetworkIdentity, Vector3> positions = new Dictionary<NetworkIdentity, Vector3>();

    public static Functions instance;

    void Awake() {
        instance = this;
    }

    public static bool CloseEnough(Vector3 a, Vector3 b, float e = 0.01f) {
        return (b-a).magnitude < e;
    }

    public static bool CloseEnough (Quaternion a, Quaternion b, float e = 0.01f) {
        return Mathf.Abs(a.x - b.x) < e &&
               Mathf.Abs(a.y - b.y) < e &&
               Mathf.Abs(a.z - b.z) < e &&
               Mathf.Abs(a.w - b.w) < e;
    }

    public static bool CloseEnough (float a, float b, float e = 0.01f) {
        return Mathf.Abs(a - b) < e;
    }

    public static bool IsMouseOnLayer(string name, out RaycastHit hit) {
        return Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100000, 1 << LayerMask.NameToLayer(name));
    }

    public static bool IsMouseOnLayer(string name) {
        RaycastHit hit;
        return Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100000, 1 << LayerMask.NameToLayer(name));
    }
    public static bool IsMouseOnHand() {
        return mouseOnHand;
    }

    public static Card FindCard(string name) {
        return GameObject.Find("Cards").transform.Find(name).GetComponent<Card>();
    }

    public static Card FindCard(RectTransform uiCard) {
        return FindCard(uiCard.gameObject.name.Substring(3));
    }

    public static RectTransform FindUICard(string name) {
        Transform uiCard = GameObject.Find("UICards").transform.Find("UI_" + name);
        uiCard = uiCard == null ? GameObject.Find("Hand").transform.Find("UI_" + name) : uiCard;
        return uiCard.GetComponent<RectTransform>();
    }

    public static RectTransform FindUICard(Card card) {
        return FindUICard(card.gameObject.name);
    }

    public static bool IsPointInRT(RectTransform rt, Vector2 point) {
        // Get the rectangular bounding box of your UI element
        Rect rect = rt.rect;

        // Get the left, right, top, and bottom boundaries of the rect
        float leftSide = rt.position.x - rect.width / 2;
        float rightSide = rt.position.x + rect.width / 2;
        float topSide = rt.position.y + rect.height / 2;
        float bottomSide = rt.position.y - rect.height / 2;

        // Check to see if the point is in the calculated bounds
        if (point.x >= leftSide &&
            point.x <= rightSide &&
            point.y >= bottomSide &&
            point.y <= topSide) {
            return true;
        }
        return false;
    }

    public static bool IsMouseOnCanvas() {
        return IsPointInRT(GameObject.Find("Canvas").GetComponent<RectTransform>(), Input.mousePosition);
    }

    public static bool IsDraggingAnything() {
        return Card.draggedCard != null || UICard.draggedCard != null || Dragable.draggedObject != null;
    }
    
    public static void BroadcastAll(string fun, System.Object msg) {
        GameObject[] gos = (GameObject[])GameObject.FindObjectsOfType(typeof(GameObject));
        foreach (GameObject go in gos) {
            if (go && go.transform.parent == null) {
                go.gameObject.BroadcastMessage(fun, msg, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    public static void BroadcastAll(string fun) {
        GameObject[] gos = (GameObject[])GameObject.FindObjectsOfType(typeof(GameObject));
        foreach (GameObject go in gos) {
            if (go && go.transform.parent == null) {
                go.gameObject.BroadcastMessage(fun, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    public void SetActive(GameObject go, bool value) {
        SetActiveServer(go.GetComponent<NetworkIdentity>(), value);
    }

    [Command(ignoreAuthority = true)]
    public void SetActiveServer(NetworkIdentity netId, bool value) {
        _SetActiveServer(netId, value);
    }

    [Server]
    public void _SetActiveServer(NetworkIdentity netId, bool value) {
        if (value) {
            if (positions.ContainsKey(netId)) {
                Debug.Log("SHOW " + netId.gameObject.name);
                netId.transform.localPosition = positions[netId];
                positions.Remove(netId);
            }
        } else {
            if (!positions.ContainsKey(netId)) {
                Debug.Log("HIDE " + netId.gameObject.name);
                Vector3 position = netId.transform.localPosition;
                positions[netId] = position;
                netId.transform.localPosition = new Vector3(
                    -30 + (int) Random.Range(0, 7) * 10,
                    -8,
                    45 + (int) Random.Range(0, 6) * 12);
            }
        }

    }

    [Server]
    public void _SetActiveServer(GameObject go, bool value) {
        _SetActiveServer(go.GetComponent<NetworkIdentity>(), value);
    }
    
}
