using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Card : NetworkBehaviour
{
    public bool isDragging;
    public bool isFlipping;
    public bool isTurning;

    public static Card draggedCard;

    public float speed = 20;

    Quaternion flipDestination;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0) && isDragging) {
            Drop();
        }
        if (isFlipping && Functions.CloseEnough(transform.localRotation.z, flipDestination.z, 0.0001f)) {
            isFlipping = false;
        }
        if (isTurning && Functions.CloseEnough(transform.localRotation, flipDestination, 0.00001f)) {
            isTurning = false;
        }

        if (isDragging) {
            RaycastHit hit;
            
            if (!Functions.IsMouseOnLayer("Desktop", out hit)) {
                //didn't hit anything. don't do anything
                return;
            }
            MoveTowards(hit.point);

        }
        if (isFlipping || isTurning) {
            
            // Vector3 position = Vector3.MoveTowards(transform.localPosition, transform.localPosition + new Vector3(0, 0.5f, 0), 0.2f);
            // transform.localPosition = position;

            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, flipDestination, 300 * Time.deltaTime);
        }
    }

     void OnMouseDown() {
        if (!isDragging && !Functions.IsMouseOnHand()) {
            Option.ShowOptionsFor("CardOptions", gameObject);
        }
    }

    void Drag() {
        Debug.Log(gameObject.name + " Drag");
        isDragging = true;
        draggedCard = this;
    }

    void Drop() {
        isDragging = false;
        if (!Functions.IsMouseOnLayer("Deck") && !Functions.IsMouseOnHand()) {
            Debug.Log("DROP");
            Card.draggedCard = null;
        }
    }

    [Command(ignoreAuthority = true)]
    void MoveTowards(Vector3 pos) {
        transform.position = Vector3.MoveTowards(transform.position, pos, speed * Time.deltaTime);
    }

    [Command(ignoreAuthority = true)]
    void Flip() {
        Debug.Log(gameObject.name + " Flip");
        isFlipping = true;

        Quaternion rotation = transform.localRotation * Quaternion.Euler(0, 0.1f, 0);
        Quaternion operation = Quaternion.Euler(0, 0, 180);
        flipDestination = rotation * operation;

        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(new Vector3(rigidbody.velocity.x, 15, rigidbody.velocity.z), ForceMode.Impulse);
    }

    [Command(ignoreAuthority = true)]
    void Turn() {
        Debug.Log(gameObject.name + " Turn");
        isTurning = true;

        Quaternion rotation = transform.localRotation;
        Quaternion operation = Quaternion.Euler(0, 90, 0);
        Debug.Log(operation);
        flipDestination = rotation * operation;

        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(new Vector3(rigidbody.velocity.x, 15, rigidbody.velocity.z), ForceMode.Impulse);
    }

    public void SetFacing(bool up) {
        if ((IsFaceUp() && up) || (!IsFaceUp() && !up)) {
            return;
        }
        transform.rotation = transform.rotation * Quaternion.Euler(180, 0, 0);
    }

    public bool IsFaceUp() {
        Transform front = transform.Find("Front");
        Transform back = transform.Find("Back");
        return front.position.y > back.position.y;
    }

    public void SetFaceUp() {
        SetFacing(true);
    }

    public void SetFaceDown() {
        SetFacing(false);

    }
    
}
