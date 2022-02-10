using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEditor;

[RequireComponent(typeof(Movement))]
public class Ghost : MonoBehaviour {
    public Movement movement { get; set; }
    public GhostHome home { get; set; }
    public GhostScatter scatter { get; set; }
    public GhostChase chase { get; set; }
    public GhostFrightened frightened { get; set; }
    public GhostBehavior initialBehavior;
    public Transform target;
    public int points = 200;
    [Layer] public string pacmanLayer;
    public ParticleSystem deathParticles;
    public Explodable deathBody;
    [Header("Camera Shake Options")]
    public float magnitude;
    public float roughness, fadeIn, fadeOut;
    public int pointsAdded { get; set; }
    private void Awake() {
        movement = GetComponent<Movement>();
        home = GetComponent<GhostHome>();
        scatter = GetComponent<GhostScatter>();
        chase = GetComponent<GhostChase>();
        frightened = GetComponent<GhostFrightened>();
    }
    private void Start() {
        ResetState();
    }
    void Update() {
        pointsAdded = points * GameManager.instance.ghostMultiplier;
        if (GameManager.instance.players.Count > 0) {
            if (!target) {
                if (GameManager.instance.players.Count == 1) {
                    target = GameManager.instance.players[0].transform;
                } else {
                    int randomPlayer = Random.Range(0, GameManager.instance.players.Count);
                    target = GameManager.instance.players[randomPlayer].transform;
                }
            } else {
                if (GameManager.instance.players.Count > 1) {
                    float closestPlayerDistance = Mathf.Infinity;
                    foreach (var item in GameManager.instance.players) {
                        float distanceFromPlayer = Vector3.Distance(transform.position, item.transform.position);
                        if (distanceFromPlayer < closestPlayerDistance) {
                            closestPlayerDistance = distanceFromPlayer;
                            target = item.transform;
                        }
                    }
                }
            }
        }
    }
    public void ResetState() {
        gameObject.SetActive(true);
        movement.ResetState();
        frightened.Disable();
        chase.Disable();
        // scatter.Enable();
        if (home != initialBehavior) {
            home.Disable();
        }
        if (initialBehavior != null) {
            initialBehavior.Enable();
        }
    }
    public void SetPosition(Vector3 position) {
        // Keep the z-position the same since it determines draw depth
        position.z = transform.position.z;
        transform.position = position;
    }
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer(pacmanLayer)) {
            if (frightened.enabled) {
                GameManager.instance.AddGhostEatenScore(this, pointsAdded);
                AudioClip eatenSound = AudioManager.instance.eatGhost;
                AudioManager.PlayOneShot(eatenSound);
            } else {
                GameManager.instance.StartCoroutine(GameManager.instance.PacmanEaten());
            }
        }
    }
}
