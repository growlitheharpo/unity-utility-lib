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
    }
}
