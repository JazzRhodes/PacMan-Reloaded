using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
public class Explosive : MonoBehaviour {
    public float fuseTime;
    public bool setFuseOnAwake = true;
    public Explosion explosionPrefab;
    public bool setFuseOnContact, explodeOnContact;
    [Tag]public List<string> contactTags;
    Projectile projectile;
    void Awake() {
        projectile = GetComponent<Projectile>();
        if(projectile){
            projectile.enabled = false;
        }
        if (setFuseOnAwake) {
            GameManager.instance.StartCoroutine(FuseCountdown());
        }
    }
    IEnumerator FuseCountdown() {
        float fuseTimer = fuseTime;
        while (fuseTimer > 0) {
            fuseTimer -= Time.deltaTime;
            yield return null;
        }
        Detonate();
    }
    void Detonate() {
        var newExplosion = Instantiate(explosionPrefab);
        newExplosion.Explode(transform.position);
        Destroy(gameObject);
    }
    void OnCollisionEnter2D(Collision2D other) {
        if (contactTags.Contains(other.gameObject.tag)) {
            if (explodeOnContact) {
                Detonate();
            }
            if (setFuseOnContact) {
                GameManager.instance.StartCoroutine(FuseCountdown());
            }
        }
    }
}
