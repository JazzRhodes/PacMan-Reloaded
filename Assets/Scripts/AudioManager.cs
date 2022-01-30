using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class AudioManager : MonoBehaviour {
    public static AudioManager instance;
    public AudioSource audioSource, secondaryAudioSource;
    public AudioClip intro, insertCredit, OneUpBonus, intermission;
    public AudioClip sirenCold, sirenWarm, sirenWarmer, sirenHot, sirenOnFire;
    public AudioClip powerPelletSiren, retreatSiren;
    public AudioClip eatPellet, eatPowerPellet, eatGhost, eatFruit;
    public AudioClip death, death2, gotShot;
    public AudioClip explosion;
    public static AudioClip lastClip;
    void Awake() {
        instance = this;
        lastClip = null;
    }
    public static void PlayOneShot(AudioClip clip) {
        instance.secondaryAudioSource.PlayOneShot(clip);
    }
    public static void PlayOneShot(AudioClip clip, float volume) {
        instance.secondaryAudioSource.PlayOneShot(clip, volume);
    }
    public static IEnumerator PlayOneShotDelayed(AudioClip clip, float delay) {
        yield return new WaitForSeconds(delay);
        PlayOneShot(clip);
    }
    public static void PlayLooped(AudioClip clip) {
        if (instance.audioSource.clip && instance.audioSource.clip != clip) {
            lastClip = instance.audioSource.clip;
        }
        instance.audioSource.clip = clip;
        instance.audioSource.loop = true;
        instance.audioSource.Play();
    }
    public static void PlayLoopedDelayed(AudioClip clip, float delay) {
        if (instance.audioSource.clip && instance.audioSource.clip != clip) {
            lastClip = instance.audioSource.clip;
        }
        instance.audioSource.clip = clip;
        instance.audioSource.loop = true;
        instance.audioSource.PlayDelayed(delay);
    }
    public static void LoopLastClip() {
        PlayLooped(lastClip);
    }
    public static void StopAll() {
        instance.audioSource.Stop();
        instance.secondaryAudioSource.Stop();
    }
}
