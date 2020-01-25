﻿using System;
using UnityEngine;

namespace KeatsLib.UI.Providers
{
	/// <summary>
	/// A UI helper class for providing a float value to a UI manager.
	/// </summary>
	public abstract class BaseFloatProvider : MonoBehaviour
	{
		/// <summary>
		/// Get the value currently represented in the UI.
		/// </summary>
		public abstract float GetValue();

		/// <summary>
		/// Set what the value represented in the UI should be.
		/// </summary>
		public abstract void SetValue(float val);

		/// <summary>
		/// Called when the value changes.
		/// </summary>
		public event Action<float> OnValueChange = f => { };

		/// <summary>
		/// Notify that the value has changed.
		/// </summary>
		protected void ValueChanged()
		{
			OnValueChange(GetValue());
		}
	}
}
