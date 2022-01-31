using UnityEngine;
[RequireComponent(typeof(Ghost))]
public abstract class GhostBehavior : MonoBehaviour {
    public Ghost ghost { get; private set; }
    public float duration;
    private void Awake() {
        ghost = GetComponent<Ghost>();
    }
    /// <summary>
    /// Enable Behaviour
    /// </summary>
    public void Enable() {
        Enable(duration);
    }
    /// <summary>
    /// Enable Behaviour for x seconds
    /// </summary>
    /// <param name="_duration"> How long until this behaviour is disabled after being enabled.</param>
    public virtual void Enable(float _duration) {
        enabled = true;
        CancelInvoke();
        Invoke(nameof(Disable), _duration);
    }
    public virtual void Disable() {
        enabled = false;
        CancelInvoke();
    }
}
