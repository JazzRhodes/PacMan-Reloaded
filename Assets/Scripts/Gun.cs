using System;
using System.Collections;
using System.Collections.Generic;
using EZCameraShake;
using UnityEngine;
using UnityEngine.Rendering.Universal;
public class Gun : MonoBehaviour {
    public enum FireMode { SemiAuto, Auto }
    public FireMode fireMode;
    public GameObject gunOwner;
    public Rigidbody2D ownerRigidbody;
    public Transform gunTip;
    public Projectile projectilePrefab;
    public int ammo, maxAmmo = 9;
    public float gunPower, fireRate;
    public AudioClip shotSfx;
    public float volume = 1;
    public float shotSfxTimeTrim = 0.05f;
    [Header("Camera Shake Options")]
    public float magnitude;
    public float roughness, fadeIn, fadeOut;
    public Light2D muzzleFlash;
    public float gunFlashTime;
    public bool triggerDown { get; set; }
    public bool triggerHeld { get; set; }
    public bool triggerUp { get; set; }
    bool canFire;
    void Awake() {
        canFire = true;
        muzzleFlash.gameObject.SetActive(false);
    }
    void Update() {
        ammo = Mathf.Clamp(ammo, 0, maxAmmo);
    }
    void FixedUpdate() {
        if (ammo > 0 && canFire) {
            if (triggerDown && fireMode == FireMode.SemiAuto) {
                triggerDown = false;
                Shoot();
                return;
            }
            if (triggerHeld && fireMode == FireMode.Auto) {
                Shoot();
                return;
            }
        } else {
            // do empty gun stuff
        }
    }
    public void Shoot() {
        CameraShakeInstance c = CameraShaker.Instance.ShakeOnce(magnitude, roughness, fadeIn, fadeOut);
        ammo--;
        var newProjectile = Instantiate(projectilePrefab);
        Vector2 pos = gunTip.position;
        newProjectile.transform.position = pos;
        newProjectile.transform.rotation = gunTip.rotation;
        newProjectile.ignoreCollisionTags.Add(gunOwner.tag);
        newProjectile.rb.velocity = Vector2.zero;
        newProjectile.rb.velocity += ownerRigidbody.velocity;
        newProjectile.rb.AddForce(newProjectile.transform.right * gunPower * 50 * newProjectile.rb.mass, ForceMode2D.Impulse);
        AudioManager.PlayOneShot(shotSfx, volume);
        AudioManager.instance.secondaryAudioSource.time = shotSfxTimeTrim;
        StartCoroutine(MuzzleFlash());
        StartCoroutine(FireDelay());
    }
    IEnumerator MuzzleFlash() {
        muzzleFlash.gameObject.SetActive(true);
        yield return new WaitForSeconds(gunFlashTime);
        muzzleFlash.gameObject.SetActive(false);
    }
    IEnumerator FireDelay() {
        canFire = false;
        yield return new WaitForSeconds(fireRate);
        canFire = true;
    }
}
