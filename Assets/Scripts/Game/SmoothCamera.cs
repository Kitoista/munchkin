using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SmoothCamera : NetworkBehaviour {
    public float movementSpeed = 50;
    private float rotationSpeed = 50;

    public bool moving = false;

    [SerializeField]
    public List<CameraMovementEvent> events = new List<CameraMovementEvent>();

    CameraMovementEvent to;

    void Start() {
        events.Add(new CameraMovementEvent(transform.position, transform.rotation, null));
    }

    float MaxAngleDiff(Quaternion a, Quaternion b) {
        Quaternion diff = a * Quaternion.Inverse(b);
        float x = diff.eulerAngles.x <= 180 ? diff.eulerAngles.x : Mathf.Abs(diff.eulerAngles.x - 360);
        float y = diff.eulerAngles.y <= 180 ? diff.eulerAngles.y : Mathf.Abs(diff.eulerAngles.y - 360);
        float z = diff.eulerAngles.z <= 180 ? diff.eulerAngles.z : Mathf.Abs(diff.eulerAngles.z - 360);
        return Mathf.Max(x, y, z);
    }

    float MovementDistance(Vector3 a, Vector3 b) {
        return (b - a).magnitude;
    }

    void Update() {
        if (moving) {
            transform.position = Vector3.MoveTowards(transform.position, to.position, movementSpeed * Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, to.rotation, rotationSpeed * Time.deltaTime);
            if (transform.position.Equals(to.position) && Functions.CloseEnough(transform.rotation, to.rotation)) {
                moving = false;
            }
        }
    }

    [Command(ignoreAuthority = true)]
    void MoveTo(CameraMovementEvent e) {
        if (events[events.Count - 1].target == e.target) {
            Back();
            return;
        }
        events.Add(e);
        SetDestination(e);
    }

    [Command(ignoreAuthority = true)]
    void Back() {
        if (events.Count <= 1) {
            return;
        }
        events.RemoveAt(events.Count - 1);
        SetDestination(events[events.Count - 1]);
    }

    void SetDestination(CameraMovementEvent e) {
        moving = true;
        to = e;

        float dist = MovementDistance(transform.position, e.position);

        float rotationTime = dist / movementSpeed;

        rotationSpeed = MaxAngleDiff(transform.rotation, e.rotation) / rotationTime;
    }

}
