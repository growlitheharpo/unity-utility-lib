using System;
using System.Collections;
using UnityEngine;

public static class CoroutineUtils
{ 
    public static IEnumerator lerpPosition(Transform t, Vector3 target, float time, Space space = Space.Self)
    {
        switch (space)
        {
            case Space.Self:
                return lerpLocalPos(t, target, time);
            case Space.World:
            default:
                return lerpWorldPos(t, target, time);
        }
    }

    private static IEnumerator lerpLocalPos(Transform obj, Vector3 p, float t)
    {
        float currentTime = 0.0f;
        Vector3 startPos = obj.localPosition;

        while (currentTime < t)
        {
            obj.transform.localPosition = Vector3.Lerp(startPos, p, currentTime / t);

            currentTime += Time.deltaTime;
            yield return null;
        }

        obj.transform.localPosition = p;
    }

    private static IEnumerator lerpWorldPos(Transform obj, Vector3 p, float t)
    {
        float currentTime = 0.0f;
        Vector3 startPos = obj.position;

        while (currentTime < t)
        {
            obj.transform.position = Vector3.Lerp(startPos, p, currentTime / t);

            currentTime += Time.deltaTime;
            yield return null;
        }

        obj.transform.position = p;
    }

    public static IEnumerator lerpRotation(Transform obj, Quaternion rot, float time, Space space = Space.Self)
    {
        switch (space)
        {
            case Space.Self:
                return lerpLocalRot(obj, rot, time);
            case Space.World:
            default:
                return lerpWorldRot(obj, rot, time);
        }
    }

    private static IEnumerator lerpWorldRot(Transform t, Quaternion rot, float time)
    {
        float currentTime = 0.0f;
        Quaternion startRot = t.rotation;

        while (currentTime < time)
        {
            t.rotation = Quaternion.Slerp(startRot, rot, currentTime / time);

            currentTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        t.rotation = rot;
    }

    private static IEnumerator lerpLocalRot(Transform t, Quaternion rot, float time)
    {
        float currentTime = 0.0f;
        Quaternion startRot = t.localRotation;

        while (currentTime < time)
        {
            t.localRotation = Quaternion.Slerp(startRot, rot, currentTime / time);

            currentTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        t.localRotation = rot;
    }

    public static IEnumerator lerpScale(Transform obj, Vector3 scale, float time, Space space = Space.Self)
    {
        switch (space)
        {
            case Space.Self:
                return lerpLocalScale(obj, scale, time);
            case Space.World:
            default:
                throw new ArgumentException("Error: Cannot lerp world scale.");
        }
    }

    private static IEnumerator lerpLocalScale(Transform obj, Vector3 scale, float t)
    {
        float currentTime = 0.0f;
        Vector3 startPos = obj.localScale;

        while (currentTime < t)
        {
            obj.transform.localScale = Vector3.Lerp(startPos, scale, currentTime / t);

            currentTime += Time.deltaTime;
            yield return null;
        }

        obj.transform.localScale = scale;
    }
}
