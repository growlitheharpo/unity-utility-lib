using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityUtils
{
    /// <summary>
    /// A collection of static functions and extensions used to make Unity code cleaner.
    /// </summary>
	public static class UnityUtilities
	{
        public delegate void CallOnCompleteT();

        // Use a static System.Random to avoid conflicts with the Unity Random.
		private static readonly System.Random RNG = new System.Random();

        /// <summary>
        /// Creates a position within a unit circle.
        /// </summary>
        /// <returns>A random Vector with a magnitude less than or equal to 1.</returns>
        public static Vector2 generatePositionInCircle()
        {
            float x, y;
            do
            {
                x = 2.0f * Random.value - 1.0f;
                y = 2.0f * Random.value - 1.0f;
            } while (x * x + y * y > 1.0);

            return new Vector2(x, y);
        }

        /// <summary> Gets a random position within a Bounds object. </summary>
        /// <param name="bounding">The bounding box for the position.</param>
        /// <returns>A valid position</returns>
		public static Vector3 getRandomContainingPosition(this Bounds bounding)
		{
			return new Vector3 (
				getRandomFloat(bounding.min.x, bounding.max.x),
				getRandomFloat(bounding.min.y, bounding.max.y),
				getRandomFloat(bounding.min.z, bounding.max.z));
		}

        /// <summary>
        /// Get a random float value.
        /// </summary>
        /// <param name="min">Minimum value, inclusive.</param>
        /// <param name="max">Maxiumum value, inclusive.</param>
        /// <returns>A random float value within the provided range.</returns>
		public static float getRandomFloat(float min = 0.0f, float max = 1.0f)
		{
            return (float)(RNG.NextDouble() * (max - min) + min);
		}

        /// <summary>
        /// Get a random position that places an object with the provided width and height completely on the screen in 2D.
        /// </summary>
        /// <param name="objectWidth">Width of the object. Cannot be less than 0.</param>
        /// <param name="objectHeight">Height of the object. Cannot be less than 0.</param>
        /// <param name="zValue">Depth value to force for the position. Defaults to 0.</param>
        /// <returns>A random position on the screen in 2D.</returns>
		public static Vector3 getRandomOnScreenPosition(float objectWidth = 0.0f, float objectHeight = 0.0f, float zValue = 0.0f)
		{
		    Bounds screenBounds = new Bounds
		    {
		        min = Camera.main.ScreenToWorldPoint(Vector3.zero),
		        max = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0.0f))
		    };

		    screenBounds.Expand (new Vector3 (-objectWidth / 2.0f, -objectHeight / 2.0f, 0.0f));

			screenBounds.center = new Vector3 (screenBounds.center.x, screenBounds.center.y, zValue);
			screenBounds.size = new Vector3 (screenBounds.size.x, screenBounds.size.y, 0.0f);

			return screenBounds.getRandomContainingPosition ();
		}

        /// <summary>
        /// Generic Coroutine to lerp the local position of an object in 2D.
        /// </summary>
        /// <param name="tToLerp">Transform of the object to move.</param>
        /// <param name="newPosition">New target local position.</param>
        /// <param name="lengthOfTime">Time to lerp over.</param>
        /// <exception cref="System.ArgumentException">toToLerp is null.</exception>
        /// <returns></returns>
        public static IEnumerator lerpToPosition2D(Transform tToLerp, Vector2 newPosition, float lengthOfTime)
        {
            if (tToLerp == null)
                throw new System.ArgumentException("Cannot perform a lerpToPosition on a null Transform!");

            float currentTime = 0.0f;
            Vector2 originalPos = tToLerp.localPosition;

            while (currentTime < lengthOfTime)
            {
                tToLerp.localPosition = Vector3.Lerp(originalPos, newPosition, currentTime / lengthOfTime);
                currentTime += Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }

            tToLerp.localPosition = newPosition;
        }

        /// <summary>
        /// Generic Coroutine to lerp a local rotation.
        /// </summary>
        /// <param name="tToLerp">Transform of the object to rotate.</param>
        /// <param name="newRotation">New target local rotation.</param>
        /// <param name="lengthOfTime">Time to lerp over.</param>
        /// <exception cref="System.ArgumentException">toToLerp is null.</exception>
        /// <returns></returns>
        public static IEnumerator lerpRotation(Transform tToLerp, Quaternion newRotation, float lengthOfTime)
        {
            if (tToLerp == null)
                throw new System.ArgumentException("Cannot perform a lerpRotation on a null Transform!");

            float currentTime = 0.0f;
            Quaternion originalRot = tToLerp.localRotation;

            while (currentTime < lengthOfTime)
            {
                tToLerp.localRotation = Quaternion.Lerp(originalRot, newRotation, currentTime / lengthOfTime);
                currentTime += Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }

            tToLerp.localRotation = newRotation;
        }

        /// <summary>
        /// Generic Coroutine to lerp a local scale.
        /// </summary>
        /// <param name="tToLerp">Transform of the object to scale.</param>
        /// <param name="newScale">New target local scale.</param>
        /// <param name="lengthOfTime">Time to lerp over.</param>
        /// <exception cref="System.ArgumentException">toToLerp is null.</exception>
        /// <returns></returns>
        public static IEnumerator lerpScale(Transform tToLerp, Vector3 newScale, float lengthOfTime)
        {
            if (tToLerp == null)
                throw new System.ArgumentException("Cannot perform a lerpScale on a null Transform!");

            Vector3 startScale = tToLerp.localScale;
            float currentTime = 0.0f;

            while (currentTime < lengthOfTime)
            {
                if (tToLerp == null)
                    yield break;

                tToLerp.localScale = Vector3.Lerp(startScale, newScale, currentTime / lengthOfTime);
                currentTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            if (tToLerp != null)
                tToLerp.localScale = newScale;
        }

        /// <summary>
        /// Create a copy of a vector with a magnitude less than or equal to a maximum value.
        /// </summary>
        /// <param name="vector">The Vector to clamp.</param>
        /// <param name="maxMagnitude">The maximum magnitude to allow.</param>
        /// <returns>A new Vector2 in the direction of vector with a magnitude less than or equal to maxMagnitude.</returns>
        public static Vector2 clampVector3Magnitude(Vector2 vector, float maxMagnitude)
        {
            if (vector.magnitude > maxMagnitude)
                return vector.normalized * maxMagnitude;

            return vector;
        }

        /// <summary>
        /// Get the current size of the screen in world coordinates.
        /// </summary>
        /// <returns>The current size of the screen in world coordinates.</returns>
        public static Vector2 screenSizeInWorld()
        {
            return Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        }

        /// <summary>
        /// CONDITIONAL: DEVELOPMENT_BUILD || UNITY_EDITOR
        /// Sets the name of a GameObject for testing purposes.
        /// </summary>
        /// <param name="g">The GameObject to change.</param>
        /// <param name="name">The new name to set the GameObject to.</param>
        [System.Diagnostics.Conditional("DEVELOPMENT_BUILD"), System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DebugSetGameObjectName(GameObject g, string name)
        {
            g.name = name;
        }
    }

    /// <summary>
    /// A collection of static functions and extensions used to make collections (List, etc.) code cleaner.
    /// </summary>
    public static class SystemCollectionsExtensions
    {
        // Use a static System.Random to avoid conflicts with the Unity Random.
        private static readonly System.Random RNG = new System.Random();

        /// <summary>
        /// Extension to select and return a random item from a list or array.
        /// </summary>
        /// <param name="list">The list to choose the item from.</param>
        /// <exception cref="System.ArgumentException">The list does not contain any items.</exception>
        /// <returns>A random item from the provided list.</returns>
        public static T chooseRandom<T>(this IList<T> list)
        {
            if (list.Count == 0)
                throw new System.ArgumentException("Trying to choose random item from an empty list!");

            return list[RNG.Next(list.Count)];
        }

        /// <summary>
        /// Extension to select and return a random item from any IEnumerable. 
        /// NOTE: This method is much slower than the IList version. It must first count all objects in the container before it can choose one.
        /// </summary>
        /// <param name="list">The IEnumerable to iterate over and choose an item from.</param>
        /// <returns>A random item from the provided IEnumerable.</returns>
        public static T chooseRandom<T>(this IEnumerable<T> list)
        {
            IList<T> list1 = list as IList<T>;
            if (list1 != null)
                return list1.chooseRandom();

            int index = RNG.Next(list.Count());
            return list.ElementAt(index);
        }

        /// <summary>
        /// Extension to randomly rearrange the items in an IList.
        /// </summary>
        /// <param name="list">The list to rearrange.</param>
        public static void Shuffle<T>(this IList<T> list)
        {
            for (int i = 0; i < list.Count; i++)
                list.SwapElement(i, RNG.Next(i, list.Count));
        }

        /// <summary>
        /// Swap two elements in a list based on their index.
        /// </summary>
        /// <param name="list">The list to swap the elements in.</param>
        /// <param name="i">The index of the first element to swap.</param>
        /// <param name="j">The index of the second element to swap.</param>
        /// <exception cref="System.IndexOutOfRangeException">Either i or j is out of range of the list.</exception>
        public static void SwapElement<T>(this IList<T> list, int i, int j)
        {
            T tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }
    }
}