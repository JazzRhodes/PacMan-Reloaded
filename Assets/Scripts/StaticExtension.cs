using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
public static class StaticExtension {
    public static void QuitGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public static IEnumerator DestroyRealtime(GameObject objectToDestroy, float time) {
        yield return new WaitForSecondsRealtime(time);
        GameObject.Destroy(objectToDestroy);
    }
    public static void CrossFadeAlphaFixed(this Graphic img, float alpha, float duration, bool ignoreTimeScale) {
        //Make the alpha 1
        Color fixedColor = img.color;
        fixedColor.a = 1;
        img.color = fixedColor;
        //Set the 0 to zero then duration to 0
        img.CrossFadeAlpha(0f, 0f, true);
        //Finally perform CrossFadeAlpha
        img.CrossFadeAlpha(alpha, duration, ignoreTimeScale);
    }
    public static float RemapRange(float input, float oldLow, float oldHigh, float newLow, float newHigh) {
        float t = Mathf.InverseLerp(oldLow, oldHigh, input);
        return Mathf.Lerp(newLow, newHigh, t);
    }
    public static float Rad2Deg() {
        return (float)(360f / (Math.PI * 2));
    }
    public static float Deg2Rad() {
        return (float)((Math.PI * 2) / 360f);
    }
    public static Quaternion LookRotationOneAxis(Vector3 lookDirection, Vector3 upwards) {
        return Quaternion.LookRotation(upwards, -lookDirection) * Quaternion.AngleAxis(90f, Vector3.right);
    }
    public static Quaternion LookAt(Vector3 startPoint, Vector3 target, Vector3 upDirection) {
        Vector3 lookDirection = (target - startPoint).normalized;
        return Quaternion.LookRotation(lookDirection, upDirection);
    }
    /// <summary>
    /// Evaluates a rotation needed to be applied to an object positioned at sourcePoint to face destPoint
    /// </summary>
    /// <param name="sourcePoint">Coordinates of source point</param>
    /// <param name="destPoint">Coordinates of destionation point</param>
    /// <returns></returns>
    public static Quaternion LookAt(Vector3 sourcePoint, Vector3 destPoint) {
        Vector3 forwardVector = Vector3.Normalize(destPoint - sourcePoint);
        float dot = Vector3.Dot(Vector3.forward, forwardVector);
        if (Math.Abs(dot - (-1.0f)) < 0.000001f) {
            return new Quaternion(Vector3.up.x, Vector3.up.y, Vector3.up.z, 3.1415926535897932f);
        }
        if (Math.Abs(dot - (1.0f)) < 0.000001f) {
            return Quaternion.identity;
        }
        float rotAngle = (float)Math.Acos(dot);
        Vector3 rotAxis = Vector3.Cross(Vector3.forward, forwardVector);
        rotAxis = Vector3.Normalize(rotAxis);
        return CreateFromAxisAngle(rotAxis, rotAngle);
    }
    // just in case you need that function also
    public static Quaternion CreateFromAxisAngle(Vector3 axis, float angle) {
        float halfAngle = angle * .5f;
        float s = (float)System.Math.Sin(halfAngle);
        Quaternion q;
        q.x = axis.x * s;
        q.y = axis.y * s;
        q.z = axis.z * s;
        q.w = (float)System.Math.Cos(halfAngle);
        return q;
    }
    public static AudioSource PlayClipAtPoint(AudioClip clip, Vector3 pos, float volume = 1) {
        var tempGO = new GameObject("TempAudio"); // create the temp object
        tempGO.transform.position = pos; // set its position
        AudioSource aSource = tempGO.AddComponent<AudioSource>(); // add an audio source
        aSource.clip = clip; // define the clip
        // set other aSource properties here, if desired
        aSource.volume = volume;
        aSource.Play(); // start the sound
        GameObject.Destroy(tempGO, clip.length); // destroy object after clip duration
        return aSource; // return the AudioSource reference
    }
    public static Vector3[] SpawnObjectsAroundCircleEvenly(int num, Transform point, float radius) {
        List<Vector3> result = new List<Vector3>();
        for (int i = 0; i < num; i++) {
            /* Distance around the circle */
            var radians = 2 * (float)System.Math.PI / num * i;
            /* Get the vector direction */
            var vertical = MathF.Sin(radians);
            var horizontal = MathF.Cos(radians);
            var spawnDir = new Vector3(horizontal, 0, vertical);
            /* Get the spawn position */
            var spawnPos = point.position + spawnDir * radius; // Radius is just the distance away from the point
            /* Now spawn */
            //var newObject = GameObject.Instantiate(prefab, spawnPos, Quaternion.identity);
            /* Rotate the enemy to face towards player */
            // if(lookAtPos)
            // newObject.transform.LookAt(point);
            /* Adjust height */
            //newObject.transform.Translate(new Vector3(0, newObject.transform.localScale.y / 2, 0));
            result.Add(spawnPos);
        }
        return result.ToArray();
    }
    public static bool NumberIsEven(float test) {
        bool result = false;
        if (test % 2 == 0) {
            result = true;
            // Is even, because something divided by two without remainder is even, i.e 4/2 = 2, remainder 0
        }
        return result;
    }
    public static bool NumberIsOdd(float test) {
        bool result = false;
        if (test % 2 == 1) {
            result = true;
            // Is odd, because something divided by two with a remainder of 1 is not even, i.e. 5/2 = 2, remainder 1
        }
        return result;
    }
    public static void DestroyAllChildren(GameObject go) {
        for (int i = go.transform.childCount - 1; i >= 0; i--) {
            GameObject.Destroy(go.transform.GetChild(i).gameObject);
        }
    }
    public static void DestroyAllChildren(Transform go) {
        for (int i = go.childCount - 1; i >= 0; i--) {
            GameObject.Destroy(go.GetChild(i).gameObject);
        }
    }
    /// <summary>
    /// Resets a transform to default values.
    /// </summary>
    /// <param name="transform"></param>
    public static void Reset(this Transform transform) {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }
    /// <summary>
    /// Resets a transform to default values locally.
    /// </summary>
    /// <param name="transform"></param>
    public static void LocalReset(this Transform transform) {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }
    /// <summary>
    /// Transform the bounds of the current rect transform to the space of another transform.
    /// </summary>
    /// <param name="source">The rect to transform</param>
    /// <param name="target">The target space to transform to</param>
    /// <returns>The transformed bounds</returns>
    // Shared array used to receive result of RectTransform.GetWorldCorners
    static Vector3[] corners = new Vector3[4];
    /// <summary>
    /// Transform the bounds of the current rect transform to the space of another transform.
    /// </summary>
    /// <param name="source">The rect to transform</param>
    /// <param name="target">The target space to transform to</param>
    /// <returns>The transformed bounds</returns>
    public static Bounds TransformBoundsTo(this RectTransform source, Transform target) {
        // Based on code in ScrollRect's internal GetBounds and InternalGetBounds methods
        var bounds = new Bounds();
        if (source != null) {
            source.GetWorldCorners(corners);
            var vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var vMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            var matrix = target.worldToLocalMatrix;
            for (int j = 0; j < 4; j++) {
                Vector3 v = matrix.MultiplyPoint3x4(corners[j]);
                vMin = Vector3.Min(v, vMin);
                vMax = Vector3.Max(v, vMax);
            }
            bounds = new Bounds(vMin, Vector3.zero);
            bounds.Encapsulate(vMax);
        }
        return bounds;
    }
    /// <summary>
    /// Normalize a distance to be used in verticalNormalizedPosition or horizontalNormalizedPosition.
    /// </summary>
    /// <param name="axis">Scroll axis, 0 = horizontal, 1 = vertical</param>
    /// <param name="distance">The distance in the scroll rect's view's coordiante space</param>
    /// <returns>The normalized scoll distance</returns>
    public static float NormalizeScrollDistance(this ScrollRect scrollRect, int axis, float distance) {
        // Based on code in ScrollRect's internal SetNormalizedPosition method
        var viewport = scrollRect.viewport;
        var viewRect = viewport != null ? viewport : scrollRect.GetComponent<RectTransform>();
        var viewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
        var content = scrollRect.content;
        var contentBounds = content != null ? content.TransformBoundsTo(viewRect) : new Bounds();
        var hiddenLength = contentBounds.size[axis] - viewBounds.size[axis];
        return distance / hiddenLength;
    }
    /// <summary>
    /// Scroll the target element to the vertical center of the scroll rect's viewport.
    /// Assumes the target element is part of the scroll rect's contents.
    /// </summary>
    /// <param name="scrollRect">Scroll rect to scroll</param>
    /// <param name="target">Element of the scroll rect's content to center vertically</param>
    public static void ScrollToCenter(ScrollRect scroller, RectTransform child) {
        Canvas.ForceUpdateCanvases();
        Vector2 contentPos = scroller.transform.InverseTransformPoint(scroller.content.position);
        Vector2 childPos = scroller.transform.InverseTransformPoint(child.position);
        var endPos = contentPos - childPos;
        // If no horizontal scroll, then don't change contentPos.x
        if (!scroller.horizontal) endPos.x = scroller.content.anchoredPosition.x;
        // If no vertical scroll, then don't change contentPos.y
        if (!scroller.vertical) endPos.y = scroller.content.anchoredPosition.y;
        scroller.content.anchoredPosition = endPos;
    }
    public static float ThumbstickAngle(Vector2 vec) {
        //we will use -1 to mean no input
        //otherwise return an angle between 0 and 360 inclusive
        float RefAngle = (Mathf.Rad2Deg * Mathf.Atan2(vec.y, vec.x));
        float MainAngle = 0;
        //not pointing at anything, centered
        if (vec.y == 0 && vec.x == 0) {
            MainAngle = -1;
        } else {
            MainAngle = RefAngle - 90;
        }
        while (MainAngle < 360) {
            MainAngle += 360;
        }
        while (MainAngle >= 360) {
            MainAngle -= 360;
        }
        return MainAngle;
    }
    public static GameObject FindParentWithTag(GameObject childObject, string tag) {
        Transform t = childObject.transform;
        while (t.parent != null) {
            if (t.parent.tag == tag) {
                return t.parent.gameObject;
            }
            t = t.parent;
        }
        return null; // Could not find a parent with given tag.
    }
    public static bool CheckParentsFromTagList(GameObject childObject, List<string> strings) {
        Transform t = childObject.transform;
        while (t.parent != null) {
            if (strings.Contains(t.parent.tag)) {
                return true;
            }
            t = t.parent;
        }
        return false; // Could not find a parent with given tag.
    }
    public static Transform GetFarthestParent(Transform child) {
        Transform t = child;
        while (t.parent != null) {
            t = t.parent;
        }
        return t;
    }
    public static Transform[] GetChildrenArray(Transform t) {
        return t.Cast<Transform>().ToArray();
    }
    public static List<Transform> GetChildrenList(Transform t) {
        return t.Cast<Transform>().ToList();
    }
    public static IEnumerator WiggleRoutine(Transform transformToWiggle) {
        float duration = 1f;
        float speed = 20f;
        float distance = 2f;
        float time = 0f;
        Vector3 pos = transformToWiggle.localPosition;
        Vector3 startPos = transformToWiggle.localPosition;
        while (time <= duration) {
            pos.x = Mathf.Lerp(Mathf.Sin(time * speed) * distance, 0, time / duration);
            transformToWiggle.localPosition = pos;
            time += Time.deltaTime;
            yield return null;
        }
        transformToWiggle.localPosition = startPos;
    }
}
