using UnityEngine;
public class GhostChase : GhostBehavior {
    private void OnDisable() {
        ghost.scatter.Enable();
    }
    private void OnTriggerEnter2D(Collider2D other) {
        Node node = other.GetComponent<Node>();
        // Do nothing while the ghost is frightened
        if (node != null && enabled && !ghost.frightened.enabled) {
            Vector2 direction = Vector2.zero;
            float minDistance = float.MaxValue;
            // Find the available direction that moves closet to pacman
            foreach (Vector2 availableDirection in node.availableDirections) {
                if (ghost.target) {
                    // If the distance in this direction is less than the current
                    // min distance then this direction becomes the new closest
                    Vector3 availableDir_X_Y = new Vector3(availableDirection.x, availableDirection.y);
                    Vector3 newPosition = transform.position + availableDir_X_Y;
                    Vector3 pacmanPosition = ghost.target.position;
                    float distance = (pacmanPosition - newPosition).sqrMagnitude;
                    if (distance < minDistance) {
                        direction = availableDirection;
                        minDistance = distance;
                    }
                }
            }
            ghost.movement.SetDirection(direction);
        }
    }
}
