using UnityEngine;
using NaughtyAttributes;
[RequireComponent(typeof(Collider2D))]
public class Pellet : MonoBehaviour {
    public int points = 10;
    [Layer] public string pacmanLayer = "Pacman";
    protected virtual void Eat(AudioClip sound) {
        GameManager.PelletEaten(this);
        AudioManager.PlayOneShot(sound);
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer == LayerMask.NameToLayer(pacmanLayer)) {
            AudioClip eatSound = AudioManager.instance.eatPellet;
            Eat(eatSound);
        }
    }
}
