using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
public class Projectile : MonoBehaviour {
    public Rigidbody2D rb { get; set; }
    [Tag] public List<string> collisionTags;
    [Tag] public List<string> ignoreCollisionTags;
    Collider2D collider2D;
    public ParticleSystem deathParticles;
    public float lifetime = 5f;
    void Awake() {
        collider2D = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifetime);
    }
    void OnCollisionEnter2D(Collision2D other) {
        if (ignoreCollisionTags.Contains(other.gameObject.tag)) {
            Physics2D.IgnoreCollision(other.collider, collider2D);
        }
        if (collisionTags.Contains(other.gameObject.tag)) {
            if (GameManager.ghostDictionary.ContainsKey(other.gameObject)){
                GameManager.ghostDictionary[other.gameObject].frightened.Shot();
            }
            Destroy(gameObject);
        }
    }
    void OnDestroy() {
        Instantiate(deathParticles, transform.position, deathParticles.transform.rotation);
    }
}
