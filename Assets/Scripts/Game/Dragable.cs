using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Dragable : NetworkBehaviour
{

    public static Dragable draggedObject;
    public bool isDragging = false;

    public float speed = 2000;

    public bool keepLocalX = false;
    public bool keepLocalY = false;
    public bool keepLocalZ = false;

    public Vector3 minLocalConstraits;
    public Vector3 maxLocalConstraits;

    void Update() {
        if (Input.GetMouseButtonUp(0) && Dragable.draggedObject == this) {
            Dragable.draggedObject = null;
            isDragging = false;
        }
        if (isDragging) {
            RaycastHit hit;
                
            if (!Functions.IsMouseOnLayer("Desktop", out hit)) {
                //didn't hit anything. don't do anything
                return;
            }
            
            ChangePosition(hit.point);
        }
    }

    void OnMouseOver() {
        if (Input.GetMouseButtonDown(0) && !Functions.IsDraggingAnything()) {
            Dragable.draggedObject = this;
            isDragging = true;
        }
    }

    [Command(ignoreAuthority = true)]
    void ChangePosition(Vector3 hitPoint) {
        Vector3 localPosition = transform.localPosition;
        transform.position = Vector3.MoveTowards(transform.position, hitPoint, speed * Time.deltaTime);
        Vector3 pos = transform.localPosition;

        if (keepLocalX) {
            pos.x = localPosition.x;
        }
        if (keepLocalY) {
            pos.y = localPosition.y;
        }
        if (keepLocalZ) {
            pos.z = localPosition.z;
        }
        pos.x = Mathf.Max(minLocalConstraits.x, pos.x);
        pos.y = Mathf.Max(minLocalConstraits.y, pos.y);
        pos.z = Mathf.Max(minLocalConstraits.z, pos.z);

        pos.x = Mathf.Min(maxLocalConstraits.x, pos.x);
        pos.y = Mathf.Min(maxLocalConstraits.y, pos.y);
        pos.z = Mathf.Min(maxLocalConstraits.z, pos.z);

        transform.localPosition = pos;
    }

}
