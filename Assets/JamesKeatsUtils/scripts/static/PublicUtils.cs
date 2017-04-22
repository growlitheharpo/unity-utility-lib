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

public struct IteratorIndex
{
    private int mValue;
    private int mSize;

    public IteratorIndex(int size, int initialValue = 0)
    {
        mSize = size;

        mValue = initialValue >= size ? size - 1 : initialValue;
    }

    public void setSize(int newSize)
    {
        mSize = newSize;
    }

    public void resetValue()
    {
        mValue = 0;
    }

    public static implicit operator int(IteratorIndex i)
    {
        return i.mValue;
    }

    public static IteratorIndex operator ++(IteratorIndex i)
    {
        i.mValue++;
        if (i.mValue >= i.mSize)
            i.mValue = 0;

        return i;
    }
}

// ReSharper disable InconsistentNaming, FieldCanBeMadeReadOnly.Local
public class Pair<TX, TY>
{
    private TX _x;
    private TY _y;

    public Pair(TX first, TY second)
    {
        _x = first;
        _y = second;
    }

    public TX first { get { return _x; } }

    public TY second { get { return _y; } }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;
        if (obj == this)
            return true;

        Pair<TX, TY> other = obj as Pair<TX, TY>;
        if (other == null)
            return false;

        return
            (((first == null) && (other.first == null))
                || ((first != null) && first.Equals(other.first)))
              &&
            (((second == null) && (other.second == null))
                || ((second != null) && second.Equals(other.second)));
    }

    public override int GetHashCode()
    {
        int hashcode = 0;
        if (first != null)
            hashcode += first.GetHashCode();
        if (second != null)
            hashcode += second.GetHashCode();

        return hashcode;
    }
}
// ReSharper restore FieldCanBeMadeReadOnly.Local, InconsistentNaming 

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

    public static IEnumerator lerpLocalScale(GameObject obj, float time, Vector3 newScale, CallOnCompleteT func = null)
    {
        Vector3 startScale = obj.transform.localScale;
        float currentTime = 0.0f;

        while (currentTime < time)
        {
            obj.transform.localScale = Vector3.Lerp(startScale, newScale, currentTime / time);
            currentTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        obj.transform.localScale = newScale;

        if (func != null)
            func();
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

    //The following function was copied from http://answers.unity3d.com/questions/572851/way-to-move-object-over-time.html
    //as the simplest way to move an object to a position without using a tweening library or a redundant animation
    public static IEnumerator moveOverSeconds(GameObject objectToMove, Vector3 end, float seconds, CallOnCompleteT func = null)
    {
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.transform.position;

        while (elapsedTime < seconds)
        {
            objectToMove.transform.position = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        objectToMove.transform.position = end;

        if (func != null)
            func();
    }

    public static IEnumerator moveOverSecondsWithEasing(GameObject objectToMove, Vector3 end, float seconds, CallOnCompleteT func = null)
    {
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.transform.position;

        while (elapsedTime < seconds)
        {
            // -6 * (x^3 / 3 - x^2 / 2)     is a good, cheap smoothing formula. Go graph it.
            // 1/2 * ((2x - 1) ^ 1/3 + 1)   is an even better one, assuming you have a bit more power and/or time to run it.
            float currentPercent = 0.5f * (Mathf.Pow(2 * (elapsedTime / seconds), 1f / 3f) + 1f);

            objectToMove.transform.position = Vector3.Lerp(startingPos, end, currentPercent);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        objectToMove.transform.position = end;

        if (func != null)
            func();
    }

    public static T randomEnum<T>()
    {
        Type type = typeof(T);
        Array values = Enum.GetValues(type);

        var choice = Random.Range(0, values.Length);

        object value = values.GetValue(choice);
        return (T)Convert.ChangeType(value, type);
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
        public static T toEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static T chooseRandom<T>(this LinkedList<T> list)
        {
            if (list.Count == 0)
                throw new ArgumentException("Trying to choose a random item from an empty list!");

            int index = (int)(Random.value * list.Count);
            if (index >= list.Count)
                index--;

            LinkedListNode<T> n = list.First;
            for (int i = 0; i != index; i++)
            {
	            if (n != null)
	                n = n.Next;
            }

	        // ReSharper disable once PossibleNullReferenceException
            return n.Value;
        }

        public static T chooseRandom<T>(this List<T> list)
        {
            if (list.Count == 0)
                throw new ArgumentException("Trying to choose a random item from an empty list!");

            int index = (int)(Random.value * list.Count);
            if (index >= list.Count)
                index--;

            return list[index];
        }

        public static T chooseRandom<T>(this T[] list)
        {
            if (list.Length == 0)
                throw new ArgumentException("Trying to choose a random item from an empty list!");

            int index = (int)(Random.value * list.Length);
            if (index >= list.Length)
                index--;

            return list[index];
        }

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

