using System.Collections.Generic;
using UnityEngine;

namespace KeatsLib.Unity
{
    public static class Vector3Ext
    {
        /// <summary>
        /// Get the average of a collection of Vectors.
        /// </summary>
        /// <param name="vecs">The collection of Vectors to average</param>
        /// <returns>The average of all the provided Vectors.</returns>
        public static Vector3 Average(ICollection<Vector3> vecs)
        {
            Vector3 result = Vector3.zero;

            foreach (var v in vecs)
                result += v;

            result /= vecs.Count;
            return result;
        }

        /// <summary>
        /// Get the square of the distance between two Vectors.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>The square of the distance between a and b.</returns>
        public static float DistanceSqrd(Vector3 a, Vector3 b)
        {
            return (a - b).sqrMagnitude;
        }

        /// <summary> Gets a random position within a Bounds object. </summary>
        /// <param name="bounding">The bounding box for the position.</param>
        /// <returns>A valid position</returns>
        public static Vector3 RandomContainedPosition(Bounds bounding)
        {
            return new Vector3(
                Random.Range(bounding.min.x, bounding.max.x),
                Random.Range(bounding.min.y, bounding.max.y),
                Random.Range(bounding.min.z, bounding.max.z));
        }

        /// <summary>
        /// Provide a copy of a Vector with its magnitude clamped to maxMagnitude.
        /// </summary>
        /// <param name="v">The Vector to copy.</param>
        /// <param name="maxMagnitude">The new maximum magnitude.</param>
        /// <returns>A new Vector in the direction of v with a maximum magnitude of maxMagnitude.</returns>
        public static Vector3 ClampMagnitude(this Vector3 v, float maxMagnitude)
        {
            if (v.magnitude > maxMagnitude)
                return v.normalized * maxMagnitude;

            return v;
        }
        
        /// <summary>
        /// Provide a copy of a Vector with its components individually clamped without regard for the original direction of the Vector.
        /// </summary>
        /// <param name="v">The Vector to copy with clamped components.</param>
        /// <param name="min">The Vector whose components will be used as the minimum values.</param>
        /// <param name="max">The Vector whose components will be used as the maximum values.</param>
        /// <returns>A new Vector copy of v with its components clamped without regard for the original direction.</returns>
        public static Vector3 ClampComponentwise(this Vector3 v, Vector3 min, Vector3 max)
        {
            return new Vector3(
                Mathf.Clamp(v.x, min.x, max.x),
                Mathf.Clamp(v.y, min.y, max.y),
                Mathf.Clamp(v.z, min.z, max.z));
        }

        /// <summary>
        /// Provide a copy of a Vector with its components individually clamped without regard for the original direction of the Vector.
        /// </summary>
        /// <param name="v">The Vector to copy with clamped components.</param>
        /// <param name="min">The minimum value for each component.</param>
        /// <param name="max">The maximum value for each component.</param>
        /// <returns>A new Vector copy of v with its components clamped without regard for the original direction.</returns>
        public static Vector3 ClampComponentwise(this Vector3 v, float min, float max)
        {
            return new Vector3(
                Mathf.Clamp(v.x, min, max),
                Mathf.Clamp(v.y, min, max),
                Mathf.Clamp(v.z, min, max));
        }
    }

    public static class Vector2Ext
    {
        /// <summary>
        /// Get the average of a collection of Vectors.
        /// </summary>
        /// <param name="vecs">The collection of Vectors to average</param>
        /// <returns>The average of all the provided Vectors.</returns>
        public static Vector2 Average(ICollection<Vector2> vecs)
        {
            Vector2 result = Vector2.zero;

            foreach (Vector2 v in vecs)
                result += v;

            result /= vecs.Count;
            return result;
        }

        /// <summary>
        /// Get the square of the distance between two Vectors.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>The square of the distance between a and b.</returns>
        public static float DistanceSqrd(Vector2 a, Vector2 b)
        {
            return (a - b).sqrMagnitude;
        }

        /// <summary>
        /// Provide a copy of a Vector with its magnitude clamped to maxMagnitude.
        /// </summary>
        /// <param name="v">The Vector to copy.</param>
        /// <param name="maxMagnitude">The new maximum magnitude.</param>
        /// <returns>A new Vector in the direction of v with a maximum magnitude of maxMagnitude.</returns>
        public static Vector2 ClampMagnitude(this Vector2 v, float maxMagnitude)
        {
            if (v.magnitude > maxMagnitude)
                return v.normalized * maxMagnitude;

            return v;
        }

        /// <summary>
        /// Provide a copy of a Vector with its components individually clamped without regard for the original direction of the Vector.
        /// </summary>
        /// <param name="v">The Vector to copy with clamped components.</param>
        /// <param name="min">The Vector whose components will be used as the minimum values.</param>
        /// <param name="max">The Vector whose components will be used as the maximum values.</param>
        /// <returns>A new Vector copy of v with its components clamped without regard for the original direction.</returns>
        public static Vector2 ClampComponentwise(this Vector2 v, Vector2 min, Vector2 max)
        {
            return new Vector2(
                Mathf.Clamp(v.x, min.x, max.x),
                Mathf.Clamp(v.y, min.y, max.y));
        }

        /// <summary>
        /// Provide a copy of a Vector with its components individually clamped without regard for the original direction of the Vector.
        /// </summary>
        /// <param name="v">The Vector to copy with clamped components.</param>
        /// <param name="min">The minimum value for each component.</param>
        /// <param name="max">The maximum value for each component.</param>
        /// <returns>A new Vector copy of v with its components clamped without regard for the original direction.</returns>
        public static Vector2 ClampComponentwise(this Vector2 v, float min, float max)
        {
            return new Vector2(
                Mathf.Clamp(v.x, min, max),
                Mathf.Clamp(v.y, min, max));
        }
    }
}
