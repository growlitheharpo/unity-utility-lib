using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MySystemExtensions;
using Random = UnityEngine.Random;

/*
 * PUBLIC UTILS
 * Here there be dragons...
 * 
 * This is a standard C# script I carry with all of my Unity projects. It grows with each one.
 * There's a bunch of extensions and useful functions and classes in here. Have fun.
 * 
 * */


public static class PublicUtils
{
    public delegate void CallOnCompleteT();

    public static void printLog(string message, int count, ref Queue<string> logQueue)
    {
        Debug.Log(message + count + " times.");

        if (count > 0)
        {
            string masterString = "";

            while (logQueue.Count > 0)
                masterString += logQueue.Dequeue() + '\n';

            Debug.Log(masterString);
        }
    }

    public static Vector3 avg(Vector3[] vecs)
    {
        Vector3 result = Vector3.zero;

        foreach (var v in vecs)
        {
            result.x += v.x;
            result.y += v.y;
            result.z += v.z;
        }

        result /= vecs.Length;
        return result;
    }

    public static float randValBetween(float a, float b)
    {
	    if (a < b)
	        return Random.value * (b - a) + a;

        return Random.value * (a - b) + b;
    }

    public static int randValBetween(int a, int b)
    {
	    if (a < b)
	        return (int)(Random.value * (b - a) + a);

        return (int)(Random.value * (a - b) + b);
    }

    public static Vector3 chooseRandomPositionWithinCube(Transform cube)
    {
        Vector3 rndPosWithin = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        rndPosWithin = cube.TransformPoint(rndPosWithin * 0.5f);

        return rndPosWithin;
    }

    public static bool cubeContainsPoint(Transform cube, Vector3 point)
    {
        if (cube == null)
            return false;

        Vector3 pointScale = cube.InverseTransformPoint(point) * 2.0f;

        return pointScale.x.isBetween(-1.0f, 1.0f) && pointScale.y.isBetween(-1.0f, 1.0f) && pointScale.z.isBetween(-1.0f, 1.0f);
    }
    
    public static IEnumerator lerpMaterialColor(Material m, Color newColor, float time, CallOnCompleteT func = null)
    {
        Color startCol = m.color;
        float currentTime = 0.0f;

        while (currentTime < time)
        {
            m.color = Color.Lerp(startCol, newColor, currentTime / time);

            currentTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        m.color = newColor;

        if (func != null)
            func();
    }

    public static float Vector3DistanceSquared(Vector3 a, Vector3 b)
    {
        return (a - b).sqrMagnitude;
    }
    

    public static IEnumerator lerpLightIntensity(Light light, float newIntensity, float time)
    {
        float elapsedTime = 0.0f;
        float startingValue = light.intensity;

        while (elapsedTime < time)
        {
            light.intensity = Mathf.Lerp(startingValue, newIntensity, elapsedTime / time);

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        light.intensity = newIntensity;
    }

    public static string TimeStamp()
    {
        return "\n" + "AT Game Time: " + Time.realtimeSinceStartup + " System time: " + DateTime.Now;
    }

    public static float distanceCubePoint(Vector3 pos, Transform transform)
    {
        Vector3 boxMin = new Vector3(transform.position.x - transform.localScale.x / 2.0f, transform.position.y - transform.localScale.y / 2.0f, transform.position.z - transform.localScale.z / 2.0f);
        Vector3 boxMax = new Vector3(transform.position.x + transform.localScale.x / 2.0f, transform.position.y + transform.localScale.y / 2.0f, transform.position.z + transform.localScale.z / 2.0f);

        Vector3 closestPoint = pos.clampValues(boxMin, boxMax);

        return Vector3.Distance(pos, closestPoint);
    }

    public static float distanceCubePoint(Vector3 pos, Bounds box)
    {
        Vector3 boxMin = box.min;
        Vector3 boxMax = box.max;

        Vector3 closestPoint = pos.clampValues(boxMin, boxMax);

        return Vector3.Distance(pos, closestPoint);
    }
}

namespace MySystemExtensions
{
    public static class PublicUtilExtensions
    {
        public static Vector3 greatestDistance(this Vector3 a, Vector3[] others)
        {
            float greatestDistance = -1.0f;
            Vector3 greatestVector = Vector3.zero;

            foreach (Vector3 b in others)
            {
                float newDist = Vector3.Distance(a, b);
                if (newDist > greatestDistance)
                {
                    greatestDistance = newDist;
                    greatestVector = b;
                }
            }

            return greatestVector;
        }

        public static Vector3 clampValues(this Vector3 a, Vector3 min, Vector3 max)
        {
            return new Vector3(
                Mathf.Clamp(a.x, min.x, max.x),
                Mathf.Clamp(a.y, min.y, max.y),
                Mathf.Clamp(a.z, min.z, max.z));
        }

        public static Vector3 clampValues(this Vector3 a, float min, float max)
        {
            return new Vector3(Mathf.Clamp(a.x, min, max), Mathf.Clamp(a.y, min, max), Mathf.Clamp(a.z, min, max));
        }

        public static Vector2 clampValues(this Vector2 a, float min, float max)
        {
            return new Vector2(Mathf.Clamp(a.x, min, max), Mathf.Clamp(a.y, min, max));
        }

        public static bool isBetween(this float val, float a, float b)
        {
            float low;
            float high;

            if (a < b)
            {
                low = a;
                high = b;
            }
            else
            {
                low = b;
                high = a;
            }

            return val >= low && val <= high;
        }

        public static float inverse(this float val)
        {
            return 1.0f / val;
        }
    }
}

