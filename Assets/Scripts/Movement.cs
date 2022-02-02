using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour {
    public float speed = 8.0f;
    public float speedMultiplier = 1.0f;
    public Vector2 initialDirection;
    public LayerMask obstacleLayer;
    public new Rigidbody2D rb { get; private set; }
    public Vector2 direction { get; private set; }
    public Vector2 nextDirection { get; private set; }
    public Vector3 startingPosition { get; private set; }
    Ghost ghost;
    float timer;
    public float wallDetectSize = 0.75f, wallDetectLength = 1.5f;
    public float stuckCheckSpeed;
    public float campTime = 0.5f;
    private void Awake() {
        ghost = GetComponent<Ghost>();
        rb = GetComponent<Rigidbody2D>();
        startingPosition = transform.position;
    }
    private void Start() {
        ResetState();
    }
    public void ResetState() {
        speedMultiplier = 1.0f;
        direction = initialDirection;
        nextDirection = Vector2.zero;
        transform.position = startingPosition;
        rb.isKinematic = false;
        enabled = true;
    }
    private void Update() {
        // Try to move in the next direction while it's queued to make movements
        // more responsive
        if (nextDirection != Vector2.zero) {
            SetDirection(nextDirection);
        }
        if (ghost) {
            if (rb.velocity.sqrMagnitude <= stuckCheckSpeed) {
                timer += Time.deltaTime;
            } else {
                timer = 0;
            }
            if (timer >= campTime) {
                timer = 0;
                Vector2 dir = AvailableDirection();
                if (dir != Vector2.zero) {
                    SetDirection(dir);
                }
            }
        }
    }
    private void FixedUpdate() {
        Vector2 position = rb.position;
        Vector2 translation = direction * speed * speedMultiplier * Time.fixedDeltaTime;
        rb.velocity = (translation);
    }
    /// <summary> 
    /// Check if the tile in that direction is available. If so, move in that direction.
    /// If not, we set it as the next direction so it'll automatically be
    /// set when it does become available
    /// </summary>
    public void SetDirection(Vector2 dir, bool forced = false) {
        bool tileAvailable = !HitWall(dir);
        if (tileAvailable || forced) {
            direction = dir;
            nextDirection = Vector2.zero;
        } else {
            nextDirection = dir;
        }
    }
    /// <summary> 
    /// If collider is hit, then there is an obstacle in that direction.
    /// </summary>
    public bool HitWall(Vector2 direction) {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, Vector2.one * wallDetectSize, 0.0f, direction, wallDetectLength, obstacleLayer);
        return hit.collider != null;
    }
    public Vector2 AvailableDirection() {
        Vector2 dir = Vector2.zero;
        List<Vector2> availableDirs = new List<Vector2>();
        if (!HitWall(Vector2.up)) {
            availableDirs.Add(Vector2.up);
        }
        if (!HitWall(Vector2.down)) {
            availableDirs.Add(Vector2.down);
        }
        if (!HitWall(Vector2.left)) {
            availableDirs.Add(Vector2.left);
        }
        if (!HitWall(Vector2.right)) {
            availableDirs.Add(Vector2.right);
        }
        float minDistance = float.MaxValue;
        foreach (var availableDirection in availableDirs) {
            Vector3 availableDir_X_Y = new Vector3(availableDirection.x, availableDirection.y);
            Vector3 newPosition = transform.position + availableDir_X_Y;
            Vector3 pacmanPosition = ghost.target.position;
            float distance = (pacmanPosition - newPosition).sqrMagnitude;
            if (distance < minDistance) {
                dir = availableDirection;
                minDistance = distance;
            }
        }
        return dir;
    }
}
