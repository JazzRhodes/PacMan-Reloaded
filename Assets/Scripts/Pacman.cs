using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Movement))]
public class Pacman : MonoBehaviour {
    public AnimatedSprite deathSequence;
    public SpriteRenderer spriteRenderer { get; set; }
    public new Collider2D collider { get; private set; }
    public Movement movement { get; private set; }
    bool up, down, left, right;
    public Gun assignedGun;
    bool slowMoInput;
    public Rigidbody2D rb{ get; set; }
    public bool super{ get; set; }
    public Coroutine enableSuper;
    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
        movement = GetComponent<Movement>();
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update() {
        // Set the new direction based on the current input
        if (up) {
            movement.SetDirection(Vector2.up);
        } else if (down) {
            movement.SetDirection(Vector2.down);
        } else if (left) {
            movement.SetDirection(Vector2.left);
        } else if (right) {
            movement.SetDirection(Vector2.right);
        }
        // Rotate pacman to face the movement direction
        float angle = Mathf.Atan2(movement.direction.y, movement.direction.x);
        transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
        GameManager.instance.slowMo = slowMoInput;
    }
    public void OnMove(InputValue value) {
        if (!GameManager.instance.paused) {
            Vector2 _value = value.Get<Vector2>();
            up = _value.y > 0;
            down = _value.y < 0;
            left = _value.x < 0;
            right = _value.x > 0;
        }
    }
    public void OnShoot(InputValue value) {
        if (assignedGun && !GameManager.instance.paused && enabled) {
            assignedGun.triggerDown = value.isPressed;
            assignedGun.triggerHeld = value.isPressed;
        }
    }
    public void OnPause(InputValue value) {
        GameManager.instance.pauseInputHit = true;
        EventSystem.current.SetSelectedGameObject(DisplayPaused.instance.firstSelected);
    }
    void OnSlowMotion(InputValue value){
        slowMoInput = value.isPressed;
    }
    public void ResetState() {
        enabled = true;
        spriteRenderer.enabled = true;
        collider.enabled = true;
        deathSequence.enabled = false;
        deathSequence.spriteRenderer.enabled = false;
        movement.ResetState();
        gameObject.SetActive(true);
    }
    public void DeathSequence() {
        enabled = false;
        spriteRenderer.enabled = false;
        collider.enabled = false;
        movement.enabled = false;
        deathSequence.enabled = true;
        deathSequence.spriteRenderer.enabled = true;
        deathSequence.Restart();
    }
    public IEnumerator SuperPacMan(){
        super = true;
        float timer = GameManager.superPelletDuration;
        while (timer > 0){
            timer -= Time.deltaTime;
            yield return null;
        }
        super = false;
    }
}
