using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class AudioManager : MonoBehaviour {
    public static AudioManager instance;
    public static AudioSource audioSource, secondaryAudioSource;
    public AudioClip intro, insertCredit, OneUpBonus, intermission;
    public AudioClip sirenCold, sirenWarm, sirenWarmer, sirenHot, sirenOnFire;
    public AudioClip powerPelletSiren, retreatSiren;
    public AudioClip eatPellet, eatPowerPellet, eatGhost, eatFruit;
    public AudioClip death, death2;
    public static AudioClip lastClip;
    void Awake() {
        instance = this;
        audioSource = GetComponent<AudioSource>();
        secondaryAudioSource = GetComponentInChildren<AudioSource>();
        lastClip = null;
    }
    public static void PlayOneShot(AudioClip clip) {
        secondaryAudioSource.PlayOneShot(clip);
    }
    public static IEnumerator PlayOneShotDelayed(AudioClip clip, float delay) {
        yield return new WaitForSeconds(delay);
        PlayOneShot(clip);
    }
    public static void PlayLooped(AudioClip clip) {
        if (audioSource.clip && audioSource.clip != clip) {
            lastClip = audioSource.clip;
        }
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
    }
    public static void PlayLoopedDelayed(AudioClip clip, float delay) {
        if (audioSource.clip && audioSource.clip != clip) {
            lastClip = audioSource.clip;
        }
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.PlayDelayed(delay);
    }
    public static void LoopLastClip() {
        PlayLooped(lastClip);
    }
    public static void StopAll() {
        audioSource.Stop();
        secondaryAudioSource.Stop();
    }
}
