using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour {
    public float speed = 8.0f;
    public float speedMultiplier = 1.0f;
    public Vector2 initialDirection;
    public LayerMask obstacleLayer;
    public new Rigidbody2D rigidbody { get; private set; }
    public Vector2 direction { get; private set; }
    public Vector2 nextDirection { get; private set; }
    public Vector3 startingPosition { get; private set; }
    private void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
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
        rigidbody.isKinematic = false;
        enabled = true;
    }
    private void Update() {
        // Try to move in the next direction while it's queued to make movements
        // more responsive
        if (nextDirection != Vector2.zero) {
            SetDirection(nextDirection);
        }
    }
    private void FixedUpdate() {
        Vector2 position = rigidbody.position;
        Vector2 translation = direction * speed * speedMultiplier * Time.fixedDeltaTime;
        rigidbody.MovePosition(position + translation);
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
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, Vector2.one * 0.75f, 0.0f, direction, 1.5f, obstacleLayer);
        return hit.collider != null;
    }
}
