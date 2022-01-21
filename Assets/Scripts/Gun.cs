using System;
using System.Collections;
using System.Collections.Generic;
using EZCameraShake;
using UnityEngine;
public class Gun : MonoBehaviour {
    public GameObject gunOwner;
    public Rigidbody2D ownerRigidbody;
    public Transform gunTip;
    public Projectile projectilePrefab;
    public int ammo, maxAmmo = 9;
    public float gunPower;
    public AudioClip shotSfx;
    public float shotSfxTimeTrim = 0.05f;
    [Header("Camera Shake Options")]
    public float magnitude;
    public float roughness, fadeIn, fadeOut;
    void Update() {
        ammo = Mathf.Clamp(ammo, 0, maxAmmo);
    }
    public void Shoot() {
        CameraShakeInstance c = CameraShaker.Instance.ShakeOnce(magnitude, roughness, fadeIn, fadeOut);
        ammo--;
        var newProjectile = Instantiate(projectilePrefab);
        newProjectile.transform.position = gunTip.position;
        newProjectile.transform.rotation = gunTip.rotation;
        newProjectile.ignoreCollisionTags.Add(gunOwner.tag);
        newProjectile.rb.velocity = Vector2.zero;
        newProjectile.rb.velocity += ownerRigidbody.velocity;
        newProjectile.rb.AddForce(newProjectile.transform.right * gunPower * 50 * newProjectile.rb.mass, ForceMode2D.Impulse);
        AudioManager.PlayOneShot(shotSfx);
        AudioManager.instance.secondaryAudioSource.time = shotSfxTimeTrim;
    }
}
