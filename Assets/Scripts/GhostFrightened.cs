using UnityEngine;
using EZCameraShake;
public class GhostFrightened : GhostBehavior {
    public SpriteRenderer body;
    public SpriteRenderer eyes;
    public SpriteRenderer blue;
    public SpriteRenderer white;
    public bool eaten { get; private set; }
    public override void Enable(float duration) {
        base.Enable(duration);
        body.enabled = false;
        eyes.enabled = false;
        blue.enabled = true;
        white.enabled = false;
        Invoke(nameof(Flash), duration / 2.0f);
    }
    public override void Disable() {
        base.Disable();
        body.enabled = true;
        eyes.enabled = true;
        blue.enabled = false;
        white.enabled = false;
    }
    public void Eaten() {
        GameManager.ShowText(ghost.pointsAdded.ToString(), transform.position + GameManager.instance.comboTextPosOffset, GameManager.instance.onEatWait, false, true, true);
        StartCoroutine(GameManager.PauseTime(GameManager.instance.onEatWait));
        eaten = true;
        ghost.SetPosition(ghost.home.inside.position);
        ghost.home.Enable(duration);
        body.enabled = false;
        eyes.enabled = true;
        blue.enabled = false;
        white.enabled = false;
    }
    public void Shot() {
        GameManager.Shot(ghost);
        RipplePostProcessor.instance.CreateRipple(Camera.main.WorldToScreenPoint(transform.position));
        Instantiate(ghost.deathParticles, transform.position, Quaternion.identity);
        Instantiate(GameManager.instance.bloodSplatter, transform.position, Quaternion.identity);
        var deadBody = Instantiate(ghost.deathBody, transform.position, Quaternion.identity);
        ExplodeOnClick.Explode(deadBody, transform.position);
        ghost.SetPosition(ghost.home.inside.position);
        ghost.home.Enable(duration);
        CameraShakeInstance c = CameraShaker.Instance.ShakeOnce(ghost.magnitude, ghost.roughness, ghost.fadeIn, ghost.fadeOut);
    }
    private void Flash() {
        if (!eaten) {
            blue.enabled = false;
            white.enabled = true;
            white.GetComponent<AnimatedSprite>().Restart();
        }
    }
    private void OnEnable() {
        blue.GetComponent<AnimatedSprite>().Restart();
        ghost.movement.speedMultiplier = 0.5f;
        eaten = false;
    }
    private void OnDisable() {
        ghost.movement.speedMultiplier = 1.0f;
        eaten = false;
    }
    private void OnTriggerEnter2D(Collider2D other) {
        Node node = other.GetComponent<Node>();
        if (node != null && enabled) {
            Vector2 direction = Vector2.zero;
            float maxDistance = float.MinValue;
            // Find the available direction that moves farthest from pacman
            foreach (Vector2 availableDirection in node.availableDirections) {
                // If the distance in this direction is greater than the current
                // max distance then this direction becomes the new farthest
                Vector3 newPosition = transform.position + new Vector3(availableDirection.x, availableDirection.y);
                float distance = (ghost.target.position - newPosition).sqrMagnitude;
                if (distance > maxDistance) {
                    direction = availableDirection;
                    maxDistance = distance;
                }
            }
            ghost.movement.SetDirection(direction);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Pacman")) {
            if (enabled) {
                Eaten();
            }
        }
    }
}
