using System;
using System.Collections;
using UnityEngine;

namespace KeatsLib.Unity
{
    public static class Coroutines
    {
        public delegate void CallOnComplete();
        public delegate float PercentageEvaluator(float current, float total);

        public static readonly PercentageEvaluator LINEAR_EVALUATOR = 
            (current, total) => current / total;
        public static readonly PercentageEvaluator MATHF_SMOOTHSTEP = 
            (current, total) => Mathf.SmoothStep(0.0f, 1.0f, current / total);
        public static readonly PercentageEvaluator CHEAP_SMOOTH_EVALUATOR =
            (current, total) => -6.0f * (Mathf.Pow(current / total, 3.0f) / 3.0f - Mathf.Pow(current / total, 2.0f) / 2.0f);
        public static readonly PercentageEvaluator EXPENSIVE_SMOOTH_EVALUATOR = 
            (current, total) => 0.5f * (Mathf.Pow(2.0f * (current / total), 1.0f / 3.0f) + 1.0f);

        /// <summary>
        /// Generic coroutine to lerp the position of an object in 3D.
        /// </summary>
        /// <param name="tRef">The transform to move.</param>
        /// <param name="target">The new target position.</param>
        /// <param name="time">Time to lerp over.</param>
        /// <param name="space">Whether to apply this transformation to the local or world position.</param>
        /// <param name="tScale">The method used to evaluate T as the lerp occurs. Defaults to linear.</param>
        /// <param name="callback">Function to call when this lerp has completed.</param>
        public static IEnumerator lerpPosition(Transform tRef, Vector3 target, float time, Space space = Space.Self, PercentageEvaluator tScale = null, CallOnComplete callback = null)
        {
            if (tScale == null)
                tScale = LINEAR_EVALUATOR;

            if (space == Space.Self)
                yield return lerpLocalPos(tRef, target, time, tScale);
            else
                yield return lerpWorldPos(tRef, target, time, tScale);

            if (callback != null)
                callback();
        }

        private static IEnumerator lerpLocalPos(Transform obj, Vector3 p, float t, PercentageEvaluator tScale)
        {
            float currentTime = 0.0f;
            Vector3 startPos = obj.localPosition;

            while (currentTime < t)
            {
                obj.transform.localPosition = Vector3.Lerp(startPos, p, tScale(currentTime, t));

                currentTime += Time.deltaTime;
                yield return null;
            }

            obj.transform.localPosition = p;
        }

        private static IEnumerator lerpWorldPos(Transform obj, Vector3 p, float t, PercentageEvaluator tScale)
        {
            float currentTime = 0.0f;
            Vector3 startPos = obj.position;

            while (currentTime < t)
            {
                obj.transform.position = Vector3.Lerp(startPos, p, tScale(currentTime, t));

                currentTime += Time.deltaTime;
                yield return null;
            }

            obj.transform.position = p;
        }

        /// <summary>
        /// Generic coroutine to lerp the rotation of an object in 3D.
        /// </summary>
        /// <param name="tRef">The transform to move.</param>
        /// <param name="target">The new target rotation.</param>
        /// <param name="time">Time to lerp over.</param>
        /// <param name="space">Whether to apply this transformation to the local or world rotation.</param>
        /// <param name="tScale">The method used to evaluate T as the lerp occurs. Defaults to linear.</param>
        /// <param name="callback">Function to call when this lerp has completed.</param>
        public static IEnumerator lerpRotation(Transform tRef, Quaternion target, float time, Space space = Space.Self, PercentageEvaluator tScale = null, CallOnComplete callback = null)
        {
            if (tScale == null)
                tScale = LINEAR_EVALUATOR;

            if (space == Space.Self)
                yield return lerpLocalRot(tRef, target, time, tScale);
            else
                yield return lerpWorldRot(tRef, target, time, tScale);

            if (callback != null)
                callback();
        }

        private static IEnumerator lerpWorldRot(Transform t, Quaternion rot, float time, PercentageEvaluator tScale)
        {
            float currentTime = 0.0f;
            Quaternion startRot = t.rotation;

            while (currentTime < time)
            {
                t.rotation = Quaternion.Slerp(startRot, rot, tScale(currentTime, time));

                currentTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            t.rotation = rot;
        }

        private static IEnumerator lerpLocalRot(Transform t, Quaternion rot, float time, PercentageEvaluator tScale)
        {
            float currentTime = 0.0f;
            Quaternion startRot = t.localRotation;

            while (currentTime < time)
            {
                t.localRotation = Quaternion.Slerp(startRot, rot, tScale(currentTime, time));

                currentTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            t.localRotation = rot;
        }

        /// <summary>
        /// Generic coroutine to scale the rotation of an object in 3D.
        /// </summary>
        /// <param name="tRef">The transform to move.</param>
        /// <param name="target">The new target position.</param>
        /// <param name="time">Time to lerp over.</param>
        /// <param name="tScale">The method used to evaluate T as the lerp occurs. Defaults to linear.</param>
        /// <param name="callback">Function to call when this lerp has completed.</param>
        public static IEnumerator lerpScale(Transform tRef, Vector3 target, float time, PercentageEvaluator tScale = null, CallOnComplete callback = null)
        {
            if (tScale == null)
                tScale = LINEAR_EVALUATOR;

            yield return lerpLocalScale(tRef, target, time, tScale);

            if (callback != null)
                callback();
        }

        private static IEnumerator lerpLocalScale(Transform obj, Vector3 scale, float t, PercentageEvaluator tScale)
        {
            float currentTime = 0.0f;
            Vector3 startPos = obj.localScale;

            while (currentTime < t)
            {
                obj.transform.localScale = Vector3.Lerp(startPos, scale, tScale(currentTime, t));

                currentTime += Time.deltaTime;
                yield return null;
            }

            obj.transform.localScale = scale;
        }
    }
}