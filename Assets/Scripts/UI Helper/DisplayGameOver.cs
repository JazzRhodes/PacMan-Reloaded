using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DisplayGameOver : MonoBehaviour {
    Text gameOverText;
    public string gameOverMessage = "Game Over";
    void Awake() {
        gameOverText = GetComponent<Text>();
    }
    void Update() {
        int playerCount = GameManager.instance.players.Count;
        int deadPlayers = 0;
        if (playerCount > 0) {
            foreach (var item in GameManager.instance.players) {
                if (item.dead) {
                    deadPlayers++;
                }
            }
        }
        bool playersDead = deadPlayers >= playerCount;
        if (GameManager.instance.lives <= 0 && playersDead) {
            gameOverText.text = gameOverMessage;
        }
    }
}
