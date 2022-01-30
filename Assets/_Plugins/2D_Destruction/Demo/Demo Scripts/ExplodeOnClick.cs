using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Explodable))]
public class ExplodeOnClick : MonoBehaviour {

    private Explodable _explodable;

    void Start() {
        _explodable = GetComponent<Explodable>();
    }
    void OnMouseDown() {
        Explode(_explodable, transform.position);
    }
    public static void Explode(Explodable explosion, Vector3 position) {
        explosion.explode();
        ExplosionForce ef = GameObject.FindObjectOfType<ExplosionForce>();
        ef.doExplosion(position);
    }
}
