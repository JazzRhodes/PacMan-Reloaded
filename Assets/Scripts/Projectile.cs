using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Nomnom.RaycastVisualization;
using UnityEngine;
public class Projectile : MonoBehaviour {
    public Rigidbody2D rb { get; set; }
    [Tag] public List<string> collisionTags, ignoreCollisionTags, wallTags;
    Collider2D collider2D;
    public ParticleSystem deathParticles;
    public float lifetime = 5f;
    public bool destroysWalls, collisionTagsDestroysWalls;
    public float destructionRadius = 1f, rayDist;
    public LayerMask rayLayerMask;
    void Awake() {
        collider2D = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifetime);
    }
    void FixedUpdate() {
        var hitWall = Physics2D.CircleCast(transform.position, destructionRadius, transform.right, rayDist, rayLayerMask);
        if (hitWall) {
            GameManager.DestroyWall(hitWall.point);
            //Destroy(gameObject);
        }
    }
    
    void OnCollisionEnter2D(Collision2D other) {
        if (ignoreCollisionTags.Contains(other.gameObject.tag)) {
            Physics2D.IgnoreCollision(other.collider, collider2D);
        }
        if (wallTags.Contains(other.gameObject.tag) && GameManager.instance.destructibleWallTilemap && destroysWalls) {
            //var objs = Physics2D.OverlapCircleAll(transform.position, destructionRadius);
            foreach (var item in other.contacts) {
                GameManager.DestroyWall(item.point);
            }
        }
        if (GameManager.ghostDictionary.ContainsKey(other.gameObject)) {
            GameManager.Shot(GameManager.ghostDictionary[other.gameObject], GameManager.ghostDictionary[other.gameObject].frightened.duration);
        }
        if (collisionTags.Contains(other.gameObject.tag)) {
            if (collisionTagsDestroysWalls && GameManager.instance.destructibleWallTilemap) {
                foreach (var item in other.contacts) {
                    GameManager.DestroyWall(item.point);
                }
            }
            Destroy(gameObject);
        }
    }
    void OnDestroy() {
        Instantiate(deathParticles, transform.position, deathParticles.transform.rotation);
    }
    void OnDrawGizmos() {
        if (Application.isPlaying) {
            DebugExtension.DrawCapsule(transform.position, transform.position + (transform.right * rayDist), destructionRadius);
        }
    }
}
