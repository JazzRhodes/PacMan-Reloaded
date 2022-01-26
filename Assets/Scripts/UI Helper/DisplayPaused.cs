using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DisplayPaused : MonoBehaviour {
    public static DisplayPaused instance;
    public List<GameObject> pauseScreenElements;
    public GameObject firstSelected;
    void Awake() {
        instance = this;
    }
    void Update() {
        foreach (var item in pauseScreenElements) {
            item.SetActive(GameManager.instance.paused);
        }
    }
}
