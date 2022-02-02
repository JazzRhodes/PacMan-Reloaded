using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using EZCameraShake;
using UnityEngine.Audio;
using DG.Tweening;
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
    public bool haltTime;
    public static Dictionary<GameObject, Ghost> ghostDictionary;
    public static Dictionary<GameObject, Rigidbody2D> rigidbodiesDictionary;
    public static Dictionary<GameObject, Collider2D> colliderDictionary;
    public List<GameObject> bloodSplatter;
    public Tilemap destructibleWallTilemap;
    public GridLayout gridLayout;
    [Range(0, 2)] public float gameSpeed = 1;
    [Tag] public string bodyPartTag, bloodSplatterTag;
    [Scene] public string mainScene;
    public float onEatWait = 1f;
    public Text comboTextPrefab;
    public Canvas worldSpaceCanvas, screenSpaceCanvas;
    public Vector3 comboTextPosOffset;
    public static int killCombo;
    public int killComboBonus;
    public float killComboTimer = 3;
    public Coroutine killComboCoroutine { get; set; }
    public class FadingTexts {
        public Text text;
        public float time, startingTime;
    }
    List<FadingTexts> fadingTexts;
    public bool useComboNames = true;
    public List<string> comboNames;
    public Explosion explosionPrefab;
    public bool explodeOnClick;
    [Layer] public string homeLayer;
    [Header("Slow Mo Shit")]
    public float slowMoTime = 0.4f, slowMoLerpLength = 0.5f;
    float normalGameSpeed;
    public bool slowMo { get; set; }
    public AudioMixerGroup slowMoReverb;
    public float reverbMixNormal = -80f, slowmoVerbMix = -24f;
    public List<GameObject> wallDebris;
    public SpriteTrail spriteTrailPrefab, meshTrailPrefab;
    public float spriteTrailTime;
    float spriteTrailTimer, slowMoFadeTimer;
    public List<SpriteRenderer> spriteRenderersInScene { get; set; }
    public List<MeshRenderer> meshRenderersInScene { get; set; }
    public bool useAllRenderersOnSlowMo;
    public static float superPelletDuration;
    void Awake() {
        normalGameSpeed = gameSpeed;
        instance = this;
        spriteRenderersInScene = new List<SpriteRenderer>();
        meshRenderersInScene = new List<MeshRenderer>();
        fadingTexts = new List<FadingTexts>();
        killCombo = 0;
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
        if (!haltTime) {
            Time.timeScale = paused ? 0 : gameSpeed;
            if (slowMo) {
                slowMoFadeTimer += Time.deltaTime;
                Time.timeScale = Mathf.Lerp(gameSpeed, slowMoTime, slowMoFadeTimer / slowMoLerpLength);
                AudioManager.instance.audioSource.pitch = Time.timeScale;
                AudioManager.instance.secondaryAudioSource.pitch = Time.timeScale;
                AudioManager.instance.audioSource.outputAudioMixerGroup = slowMoReverb;
                AudioManager.instance.secondaryAudioSource.outputAudioMixerGroup = slowMoReverb;
            } else {
                slowMoFadeTimer = 0;
                AudioManager.instance.audioSource.outputAudioMixerGroup = null;
                AudioManager.instance.secondaryAudioSource.outputAudioMixerGroup = null;
                AudioManager.instance.audioSource.pitch = 1;
                AudioManager.instance.secondaryAudioSource.pitch = 1;
            }
        }
        for (int i = 0; i < fadingTexts.Count; i++) {
            if (fadingTexts[i].text) {
                fadingTexts[i].time -= Time.deltaTime;
                var color = fadingTexts[i].text.color;
                color.a = StaticExtension.RemapRange(fadingTexts[i].time, 0, fadingTexts[i].startingTime, 0, 1);
                fadingTexts[i].text.color = color;
                if (fadingTexts[i].time <= 0) {
                    fadingTexts.RemoveAt(i);
                }
            }
        }
        if (explodeOnClick && Input.GetMouseButtonDown(0)) {
            InstantiateExplosionOnClick(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
        if (slowMo && spriteTrailPrefab) {
            spriteTrailTimer += Time.deltaTime;
            if (spriteTrailTimer >= spriteTrailTime) {
                spriteTrailTimer = 0;
                if (useAllRenderersOnSlowMo) {
                    if (spriteRenderersInScene.Count > 0) {
                        foreach (var item in spriteRenderersInScene) {
                            if (item && item.enabled) {
                                SpriteTrail.GenerateAfterImage(item.transform.position, item.transform.rotation, item.transform.localScale, item.sprite, item.color, item.sortingLayerID, item.sortingOrder);
                            }
                        }
                    } else {
                        spriteRenderersInScene = FindObjectsOfType<SpriteRenderer>().ToList();
                    }
                    if (meshRenderersInScene.Count > 0) {
                        foreach (var item in meshRenderersInScene) {
                            if (item && item.enabled) {
                                var mesh = item.GetComponent<MeshFilter>().mesh;
                                SpriteTrail.GenerateAfterImage(item.transform.position, item.transform.rotation, item.transform.localScale, mesh, item.material.color, item.sortingLayerID, item.sortingOrder);
                            }
                        }
                    } else {
                        meshRenderersInScene = FindObjectsOfType<MeshRenderer>().ToList();
                    }
                } else {
                    foreach (var item in ghosts) {
                        foreach (var rend in item.GetComponentsInChildren<SpriteRenderer>()) {
                            if (rend.enabled) {
                                SpriteTrail.GenerateAfterImage(rend.transform.position, rend.transform.rotation, rend.transform.localScale, rend.sprite, rend.color, rend.sortingLayerID, rend.sortingOrder);
                            }
                        }
                    }
                    SpriteTrail.GenerateAfterImage(pacman.transform.position, pacman.transform.rotation, pacman.transform.localScale, pacman.spriteRenderer.sprite, pacman.spriteRenderer.color, pacman.spriteRenderer.sortingLayerID, pacman.spriteRenderer.sortingOrder);
                }
            }
        } else {
            if (spriteRenderersInScene.Count > 0) {
                spriteRenderersInScene.Clear();
            }
            if (meshRenderersInScene.Count > 0) {
                meshRenderersInScene.Clear();
            }
            spriteTrailTimer = 0;
        }
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
    public static void SetScore(int score) {
        GameManager.instance.score = score;
        GameManager.instance.scoreText.text = score.ToString().PadLeft(2, '0');
    }
    public void PacmanEaten() {
        AudioManager.StopAll();
        AudioClip deathClip = AudioManager.instance.death;
        AudioClip deathClip2 = AudioManager.instance.death2;
        AudioManager.PlayOneShot(deathClip);
        StartCoroutine(AudioManager.PlayOneShotDelayed(deathClip2, deathSoundWait1));
        StartCoroutine(AudioManager.PlayOneShotDelayed(deathClip2, deathSoundWait1 + two));
        pacman.DeathSequence();
        pacman.rb.velocity = Vector2.zero;
        SetLives(lives - 1);
        if (lives > 0) {
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
    public void AddGhostEatenScore(Ghost ghost, int pointsToAdd) {
        SetScore(this.score + pointsToAdd);
        this.ghostMultiplier++;
    }
    public static void PelletEaten(Pellet pellet) {
        pellet.gameObject.SetActive(false);
        GameManager.SetScore(GameManager.instance.score + pellet.points);
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
        superPelletDuration = pellet.duration;
        if (GameManager.instance.pacman.enableSuper != null) {
            GameManager.instance.StopCoroutine(GameManager.instance.pacman.enableSuper);
        }
        GameManager.instance.pacman.enableSuper = GameManager.instance.StartCoroutine(GameManager.instance.pacman.SuperPacMan());
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
    public static IEnumerator PauseTime(float seconds) {
        GameManager.instance.haltTime = true;
        float previousGameSpeed = GameManager.instance.gameSpeed;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(seconds);
        Time.timeScale = previousGameSpeed;
        GameManager.instance.haltTime = false;
    }
    public static void ShowText(string message, Vector3 position, float displayTime, bool fade = false, bool worldSpace = false, bool destroyRealtime = false) {
        var comboText = Instantiate(GameManager.instance.comboTextPrefab);
        Transform chosenParent = worldSpace ? GameManager.instance.worldSpaceCanvas.transform : GameManager.instance.screenSpaceCanvas.transform;
        comboText.transform.SetParent(chosenParent);
        comboText.transform.localScale = GameManager.instance.comboTextPrefab.transform.localScale;
        comboText.text = message;
        if (destroyRealtime) {
            GameManager.instance.StartCoroutine(StaticExtension.DestroyRealtime(comboText.gameObject, displayTime));
        } else {
            Destroy(comboText.gameObject, displayTime);
        }
        Vector3 pos = position;
        pos.z = 0;
        comboText.transform.position = pos;
        if (fade) {
            FadingTexts newFadingText = new FadingTexts();
            newFadingText.text = comboText;
            newFadingText.startingTime = displayTime;
            newFadingText.time = newFadingText.startingTime;
            GameManager.instance.fadingTexts.Add(newFadingText);
        }
    }
    public static IEnumerator CountdownKillCombo() {
        float timer = GameManager.instance.killComboTimer;
        while (timer > 0) {
            timer -= Time.deltaTime;
            yield return null;
        }
        killCombo = 0;
    }
    public static void Shot(Ghost ghost, float duration) {
        killCombo++;
        if (GameManager.instance.killComboCoroutine != null) {
            GameManager.instance.StopCoroutine(GameManager.instance.killComboCoroutine);
        }
        GameManager.instance.killComboCoroutine = GameManager.instance.StartCoroutine(CountdownKillCombo());
        int killComboScore = killCombo * GameManager.instance.killComboBonus;
        SetScore(GameManager.instance.score + killComboScore);
        if (killCombo > 1) {
            bool comboOverMessageCount = killCombo >= GameManager.instance.comboNames.Count;
            string comboAsNumber = killCombo.ToString();
            string killComboMessage = comboAsNumber;
            string currentComboName = "";
            if (!comboOverMessageCount) {
                currentComboName = GameManager.instance.comboNames[killCombo - 1];
            }
            string lastComboName = GameManager.instance.comboNames[GameManager.instance.comboNames.Count - 1];
            if (GameManager.instance.useComboNames) {
                killComboMessage = currentComboName;
                if (comboOverMessageCount) {
                    killComboMessage = lastComboName + " - " + comboAsNumber;
                }
            }
            string message = (killComboMessage + " = " + killComboScore).ToString();
            Vector3 pos = ghost.transform.position + GameManager.instance.comboTextPosOffset;
            float time = GameManager.instance.killComboTimer;
            ShowText(message, pos, time, true, true);
        }
        AudioManager.PlayOneShot(AudioManager.instance.gotShot);
        RipplePostProcessor.instance.CreateRipple(Camera.main.WorldToScreenPoint(ghost.transform.position));
        Instantiate(ghost.deathParticles, ghost.transform.position, Quaternion.identity);
        foreach (var item in GameManager.instance.bloodSplatter) {
            Instantiate(item, ghost.transform.position, Quaternion.identity);
        }
        var deadBody = Instantiate(ghost.deathBody, ghost.transform.position, Quaternion.identity);
        var parts = deadBody.GetComponentsInChildren<MeshRenderer>(true);
        if (parts.Length > 0 && GameManager.instance.meshRenderersInScene.Count > 0) {
            GameManager.instance.meshRenderersInScene.AddRange(parts);
        }
        ExplodeOnClick.Explode(deadBody, ghost.transform.position);
        ghost.SetPosition(ghost.home.inside.position);
        ghost.home.Enable(duration);
        CameraShakeInstance c = CameraShaker.Instance.ShakeOnce(ghost.magnitude, ghost.roughness, ghost.fadeIn, ghost.fadeOut);
    }
    public static void DestroyWall(Vector3 position) {
        Vector3Int pos = GameManager.instance.gridLayout.WorldToCell(position);
        var tile = GameManager.instance.destructibleWallTilemap.GetTile(pos);
        if (tile) {
            GameManager.instance.destructibleWallTilemap.SetTile(pos, null);
            foreach (var item in GameManager.instance.wallDebris) {
                Instantiate(item, pos, Quaternion.identity);
            }
        }
    }
    public static void InstantiateExplosionOnClick(Vector3 pos) {
        var pre = Instantiate(
        GameManager.instance.explosionPrefab);
        pre.Explode(pos);
    }
}
