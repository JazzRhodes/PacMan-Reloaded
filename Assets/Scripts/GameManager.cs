using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
public class GameManager : MonoBehaviour {
    public static GameManager instance;
    public Ghost[] ghosts;
    public Pacman pacman;
    public Transform pellets;
    public Text gameOverText, scoreText, livesText, readyText;
    public int ghostMultiplier { get; private set; } = 1;
    public int score { get; private set; }
    public int lives { get; private set; }
    public float deathSoundWait1, two;
    public bool paused, pauseInputHit;
    public static Dictionary<GameObject, Ghost> ghostDictionary;
    public static Dictionary<GameObject, Rigidbody2D> rigidbodiesDictionary;
    public static Dictionary<GameObject, Collider2D> colliderDictionary;
    public GameObject bloodSplatter;
    public Tilemap destructibleWallTilemap;
    public GridLayout gridLayout;
    [Range(0, 2)] public float gameSpeed = 1;
    [Tag] public string bodyPartTag, bloodSplatterTag;
    [Scene] public string mainScene;
    void Awake() {
        instance = this;
        ghostDictionary = new Dictionary<GameObject, Ghost>();
        rigidbodiesDictionary = new Dictionary<GameObject, Rigidbody2D>();
        colliderDictionary = new Dictionary<GameObject, Collider2D>();
        for (int i = 0; i < ghosts.Length; i++) {
            ghostDictionary.Add(ghosts[i].gameObject, ghosts[i]);
        }
        foreach (var item in FindObjectsOfType<Rigidbody2D>(true)) {
            rigidbodiesDictionary.Add(item.gameObject, item);
        }
        foreach (var item in FindObjectsOfType<Collider2D>(true)) {
            if (!colliderDictionary.ContainsKey(item.gameObject))
                colliderDictionary.Add(item.gameObject, item);
        }
    }
    private void Start() {
        NewGame();
        ActivateLivingEntities(false);
    }
    private void Update() {
        //Gameover stuff
        if (this.lives <= 0 && Input.anyKey) {
            //NewGame();
            SceneManager.LoadScene(mainScene);
        }
        if (pauseInputHit) {
            pauseInputHit = false;
            paused = !paused;
            AudioManager.instance.audioSource.mute = paused;
            AudioManager.instance.secondaryAudioSource.mute = paused;
        }
        Time.timeScale = paused ? 0 : gameSpeed;
    }
    private void NewGame() {
        SetScore(0);
        SetLives(3);
        NewRound();
    }
    private void NewRound() {
        this.gameOverText.enabled = false;
        foreach (Transform pellet in pellets) {
            pellet.gameObject.SetActive(true);
        }
        ResetState();
    }
    private void ResetState() {
        for (int i = 0; i < this.ghosts.Length; i++) {
            this.ghosts[i].ResetState();
        }
        this.pacman.ResetState();
        CleanScene();
    }
    private void GameOver() {
        this.gameOverText.enabled = true;
        for (int i = 0; i < this.ghosts.Length; i++) {
            this.ghosts[i].gameObject.SetActive(false);
        }
        this.pacman.gameObject.SetActive(false);
    }
    private void SetLives(int lives) {
        this.lives = lives;
        this.livesText.text = "x" + lives.ToString();
    }
    private void SetScore(int score) {
        this.score = score;
        this.scoreText.text = score.ToString().PadLeft(2, '0');
    }
    public void PacmanEaten() {
        AudioManager.StopAll();
        AudioClip deathClip = AudioManager.instance.death;
        AudioClip deathClip2 = AudioManager.instance.death2;
        AudioManager.PlayOneShot(deathClip);
        StartCoroutine(AudioManager.PlayOneShotDelayed(deathClip2, deathSoundWait1));
        StartCoroutine(AudioManager.PlayOneShotDelayed(deathClip2, deathSoundWait1 + two));
        this.pacman.DeathSequence();
        SetLives(this.lives - 1);
        if (this.lives > 0) {
            Invoke(nameof(ResetState), 3.0f);
            AudioClip sirenCold = AudioManager.instance.sirenCold;
            AudioManager.PlayLoopedDelayed(sirenCold, 3.0f);
        } else {
            GameOver();
        }
    }
    public void CleanScene() {
        var rigidbodies = FindObjectsOfType<Rigidbody2D>().ToList();
        var listOfBodyParts = new List<Rigidbody2D>();
        var bloodSplatters = GameObject.FindGameObjectsWithTag(bloodSplatterTag);
        foreach (var item in rigidbodies) {
            if (item.gameObject.tag == bodyPartTag && !item.transform.parent) {
                listOfBodyParts.Add(item);
            }
        }
        for (int i = 0; i < listOfBodyParts.Count; i++) {
            Destroy(listOfBodyParts[i].gameObject);
        }
        for (int i = 0; i < bloodSplatters.Length; i++) {
            Destroy(bloodSplatters[i]);
        }
    }
    public void AddGhostEatenScore(Ghost ghost) {
        int points = ghost.points * this.ghostMultiplier;
        SetScore(this.score + points);
        this.ghostMultiplier++;
    }
    public static void PelletEaten(Pellet pellet) {
        pellet.gameObject.SetActive(false);
        GameManager.instance.SetScore(GameManager.instance.score + pellet.points);
        if (!GameManager.instance.HasRemainingPellets()) {
            GameManager.instance.pacman.gameObject.SetActive(false);
            GameManager.instance.Invoke(nameof(NewRound), 3.0f);
        }
    }
    public static void PowerPelletEaten(PowerPellet pellet) {
        for (int i = 0; i < GameManager.instance.ghosts.Length; i++) {
            GameManager.instance.ghosts[i].frightened.Enable(pellet.duration);
        }
        PelletEaten(pellet);
        GameManager.instance.CancelInvoke(nameof(ResetGhostMultiplier));
        GameManager.instance.Invoke(nameof(ResetGhostMultiplier), pellet.duration);
        GameManager.instance.StartCoroutine(PowerPelletWait(pellet.duration));
    }
    public static IEnumerator PowerPelletWait(float duration) {
        yield return new WaitForSeconds(duration);
        AudioManager.LoopLastClip();
    }
    private bool HasRemainingPellets() {
        foreach (Transform pellet in pellets) {
            if (pellet.gameObject.activeSelf) {
                return true;
            }
        }
        return false;
    }
    private void ResetGhostMultiplier() {
        this.ghostMultiplier = 1;
    }
    public void ActivateLivingEntities(bool isActive) {
        pacman.gameObject.SetActive(isActive);
        foreach (var item in ghosts) {
            item.gameObject.SetActive(isActive);
        }
    }
    public void SetPaused() {
        pauseInputHit = true;
    }
    public void QuitGame() {
        StaticExtension.QuitGame();
    }
}
