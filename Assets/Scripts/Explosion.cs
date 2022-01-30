using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;
using NaughtyAttributes;

public class Explosion : MonoBehaviour {
    public bool killsGhosts = true;
    [Header("Camera Shake Options")]
    public float magnitude;
    public float roughness, fadeIn, fadeOut;
    [Tag] public string wallTag;
    List<ParticleCollisionEvent> particleCollisionEvents;
    ParticleSystem pSystem;
    [Range(0, 1)] public float volume = 1f;
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
        Vector3 finalPos = pos;
        finalPos.z = 0;
        transform.position = finalPos;
        RipplePostProcessor.instance.CreateRipple(Camera.main.WorldToScreenPoint(pos));
        CameraShakeInstance c = CameraShaker.Instance.ShakeOnce(magnitude, roughness, fadeIn, fadeOut);
        AudioManager.PlayOneShot(AudioManager.instance.explosion);
    }
    public static void ExplodeOnClick(Vector3 pos) {
        var pre = Instantiate(
        GameManager.instance.explosionPrefab);
        pre.Explode(pos);
    }
}
