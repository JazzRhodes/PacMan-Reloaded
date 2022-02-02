using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GhostGun : MonoBehaviour {
    public Gun assignedGun;
    public LayerMask raycastLayerMask;
    public float eyesightDistance = 5f;
    Movement movement;
    float originalY;
    public Transform rightHand, leftHand;
    Vector3 originalLocalPos;
    void Awake() {
        assignedGun.gunOwner = gameObject;
        assignedGun.ownerRigidbody = GetComponent<Rigidbody2D>();
        movement = GetComponent<Movement>();
        originalY = assignedGun.transform.localScale.y;
        originalLocalPos = assignedGun.transform.localPosition;
    }
    void Update() {
        if (movement) {
            Quaternion rot = Quaternion.LookRotation(Vector3.forward, movement.direction);
            assignedGun.transform.localRotation = rot;
            assignedGun.transform.rotation *= Quaternion.Euler(0f, 0f, 90f);
            var dot = Vector2.Dot(movement.direction, Vector2.left);
            if (dot > 0) {
                assignedGun.transform.SetParent(leftHand);
                assignedGun.transform.localPosition = Vector3.zero;
                var scale = assignedGun.transform.localScale;
                scale.y = -Mathf.Abs(scale.y);
                assignedGun.transform.localScale = scale;
            } else {
                assignedGun.transform.SetParent(rightHand);
                assignedGun.transform.localPosition = Vector3.zero;
                var scale = assignedGun.transform.localScale;
                scale.y = originalY;
                assignedGun.transform.localScale = scale;
            }
            var dirFromPacman = (GameManager.instance.pacman.transform.position - transform.position).normalized;
            var movedot = Vector2.Dot(movement.direction, dirFromPacman);
            if (movedot > 0.5f) {
                var hitPacman = Physics2D.Raycast(transform.position, movement.direction, eyesightDistance, raycastLayerMask);
                Ray r = new Ray(transform.position, movement.direction);
                Debug.DrawRay(r.origin, r.direction);
                if (hitPacman.collider == GameManager.instance.pacman.collider) {
                    assignedGun.triggerDown = true;
                    assignedGun.triggerHeld = true;
                } else {
                    assignedGun.triggerDown = false;
                    assignedGun.triggerHeld = false;
                }
            }
        }
    }
}
