using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;
using NaughtyAttributes;
using UnityEngine.Rendering.Universal;
public class Explosion : MonoBehaviour {
    public bool killsGhosts = true;
    [Header("Camera Shake Options")]
    public float magnitude;
    public float roughness, fadeIn, fadeOut;
    [Tag] public string wallTag;
    List<ParticleCollisionEvent> particleCollisionEvents;
    ParticleSystem pSystem;
    [Range(0, 1)] public float volume = 1f;
    public float eForceRadius, eForceAmount;
    public LayerMask eForceLayerMask;
    public Light2D lite;
    public float lightTime;
    public float lethalTime;
    void Awake() {
        pSystem = GetComponent<ParticleSystem>();
        particleCollisionEvents = new List<ParticleCollisionEvent>();
    }
    void OnParticleCollision(GameObject other) {
        if (GameManager.ghostDictionary.ContainsKey(other) && killsGhosts) {
            GameManager.Shot(GameManager.ghostDictionary[other], GameManager.ghostDictionary[other].frightened.duration);
        }
        if (other.tag == wallTag) {
            pSystem.GetCollisionEvents(other, particleCollisionEvents);
            GameManager.DestroyWall(transform.position);
            foreach (var item in particleCollisionEvents) {
                GameManager.DestroyWall(item.intersection);
            }
        }
    }
    public void Explode(Vector3 pos) {
        StartCoroutine(CountdownFlashTime());
        StartCoroutine(CountdownLethalTime());
        Vector3 finalPos = pos;
        finalPos.z = 0;
        transform.position = finalPos;
        RipplePostProcessor.instance.CreateRipple(Camera.main.WorldToScreenPoint(pos));
        CameraShakeInstance c = CameraShaker.Instance.ShakeOnce(magnitude, roughness, fadeIn, fadeOut);
        AudioManager.PlayOneShot(AudioManager.instance.explosion, volume);
        var bodies = Physics2D.OverlapCircleAll(transform.position, eForceRadius, eForceLayerMask);
        foreach (var item in bodies) {
            Vector2 dir = (item.transform.position - transform.position).normalized;
            float distance = Vector2.Distance(item.transform.position, transform.position);
            if (distance <= 0) {
                distance = 0.00001f;
            }
            float amount = eForceAmount / distance;
            item.attachedRigidbody.AddForce(dir * amount, ForceMode2D.Impulse);
        }
    }
    IEnumerator CountdownLethalTime() {
        float timer = lethalTime;
        while (timer > 0) {
            timer -= Time.deltaTime;
            yield return null;
        }
        //
    }
    IEnumerator CountdownFlashTime() {
        float timer = lightTime;
        while (timer > 0) {
            timer -= Time.deltaTime;
            yield return null;
        }
        lite.gameObject.SetActive(false);
    }
}
