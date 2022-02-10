using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class NewGameIntro : MonoBehaviour {
    public static NewGameIntro instance;
    public float introTime = 5f;
    public string pressKeyToJoinMessage = "Press Any Key/Gamepad Button To Join.";
    void Awake() {
        instance = this;
    }
    void Start() {
        StartCoroutine(StartGame());
    }
    void Update() {
        if (GameManager.ready && GameManager.instance.players.Count > 0) {
            GameManager.instance.readyText.gameObject.SetActive(false);

        }
    }
    IEnumerator StartGame() {
        AudioClip introClip = AudioManager.instance.intro;
        AudioManager.PlayOneShot(introClip);
        yield return new WaitForSeconds(introTime);
        GameManager.ready = true;
        GameManager.instance.ActivateLivingEntities(true);
        AudioClip sirenCold = AudioManager.instance.sirenCold;
        if (GameManager.instance.players.Count <= 0){
            GameManager.instance.readyText.text = pressKeyToJoinMessage;
        }
        AudioManager.PlayLooped(sirenCold);
    }
}
