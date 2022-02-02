using System.Collections;
using DG.Tweening;
using UnityEngine;
public class SpriteTrail : MonoBehaviour {
    public SpriteRenderer spriteRenderer;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public float fadeTime;
    public string alphaName = "_Alpha", colorName = "_Color";
    float fadeValue;
    void Awake() {
        if (!spriteRenderer) {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        if (!meshFilter) {
            meshFilter = GetComponent<MeshFilter>();
        }
        if (!meshFilter) {
            meshRenderer = GetComponent<MeshRenderer>();
        }
        if (spriteRenderer) {
            fadeValue = spriteRenderer.material.GetFloat(alphaName);
        }
        if (meshRenderer) {
            fadeValue = meshRenderer.material.GetFloat(alphaName);
        }
        StartCoroutine(ChangeSomeValue(fadeValue, 0, fadeTime));
    }
    public IEnumerator ChangeSomeValue(float oldValue, float newValue, float duration) {
        for (float t = 0f; t < duration; t += Time.deltaTime) {
            fadeValue = Mathf.Lerp(oldValue, newValue, t / duration);
            if (spriteRenderer) {
                spriteRenderer.material.SetFloat(alphaName, fadeValue);
            }
            if (meshRenderer) {
                meshRenderer.material.SetFloat(alphaName, fadeValue);
            }
            yield return null;
        }
        Destroy(gameObject);
    }
    public void SetColor(Color color) {
        if (spriteRenderer) {
            spriteRenderer.material.SetColor(colorName, color);
        }
        if (meshFilter) {
            meshRenderer.material.SetColor(colorName, color);
        }
    }
    public static void GenerateAfterImage(Vector3 pos, Quaternion rotation, Vector3 scale, Sprite sprite, Color color, int sortingLayer, int orderInLayer) {
        var ghostAfterImage = Instantiate(GameManager.instance.spriteTrailPrefab, pos, rotation);
        ghostAfterImage.transform.localScale = scale;
        ghostAfterImage.spriteRenderer.sprite = sprite;
        ghostAfterImage.SetColor(color);
        ghostAfterImage.spriteRenderer.sortingLayerID = sortingLayer;
        ghostAfterImage.spriteRenderer.sortingOrder = orderInLayer;
    }
    public static void GenerateAfterImage(Vector3 pos, Quaternion rotation, Vector3 scale, Mesh mesh, Color color, int sortingLayer, int orderInLayer) {
        var ghostAfterImage = Instantiate(GameManager.instance.meshTrailPrefab, pos, rotation);
        ghostAfterImage.transform.localScale = scale;
        if (ghostAfterImage.meshFilter) {
            ghostAfterImage.meshFilter.mesh = mesh;
            ghostAfterImage.SetColor(color);
            ghostAfterImage.meshRenderer.sortingLayerID = sortingLayer;
            ghostAfterImage.meshRenderer.sortingOrder = orderInLayer;
        }
    }
}
