using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class NewGameIntro : MonoBehaviour {
    public static NewGameIntro instance;
    public float introTime = 5f;
    void Awake() {
        instance = this;
    }
    void Start() {
        StartCoroutine(StartGame());
    }
    IEnumerator StartGame() {
        AudioClip introClip = AudioManager.instance.intro;
        AudioManager.PlayOneShot(introClip);
        yield return new WaitForSeconds(introTime);
        GameManager.instance.ActivateLivingEntities(true);
        GameManager.instance.readyText.gameObject.SetActive(false);
        AudioClip sirenCold = AudioManager.instance.sirenCold;
        AudioManager.PlayLooped(sirenCold);
    }
}
