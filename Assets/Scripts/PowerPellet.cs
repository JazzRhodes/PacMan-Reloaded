using UnityEngine;
public class PowerPellet : Pellet {
    public float duration = 8.0f;
    protected override void Eat(AudioClip sound) {
        GameManager.PowerPelletEaten(this);
        AudioManager.PlayOneShot(sound);
        AudioClip powerPelletSiren = AudioManager.instance.powerPelletSiren;
        AudioManager.PlayLooped(powerPelletSiren);
    }
}
