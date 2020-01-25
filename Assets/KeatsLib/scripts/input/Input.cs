﻿using KeatsLib.Debug;
using KeatsLib.Unity;
using System;
using System.Collections.Generic;

namespace KeatsLib.Input
{
	/// <inheritdoc cref="IInput"/>
	/// <summary>
	/// Global input manager using the command pattern.
	/// </summary>
	public class Input : MonoSingleton<Input>, IInput
	{
		/// <summary>
		/// Base input map to see if some form of Unity input collection has been activated.
		/// </summary>
		private abstract class BaseInputMap
		{
			private readonly InputLevel mInputLevel;

			protected BaseInputMap(InputLevel l)
			{
				mInputLevel = l;
			}

			/// <summary>
			/// Returns true if the current enabled inputs match our input level
			/// </summary>
			public bool Enabled()
			{
				return instance.IsInputEnabled(mInputLevel);
			}

			/// <summary>
			/// Returns true if this input is activated based on its internal rules.
			/// </summary>
			public abstract bool Activated();
		}

		/// <summary>
		/// Useable input map to bind Unity input collection.
		/// </summary>
		/// <typeparam name="T">String or KeyCode</typeparam>
		private class InputMap<T> : BaseInputMap
		{
			private readonly Func<T, bool> mFunction;
			private readonly T mValue;

			public InputMap(Func<T, bool> check, T value, InputLevel level) : base(level)
			{
				mFunction = check;
				mValue = value;
			}

			/// <inheritdoc />
			public override bool Activated()
			{
				try
				{
					return mFunction(mValue);
				}
				catch (ArgumentException)
				{
					Logger.Warn("Axis or button is not set up: " + mValue, Logger.System.Input);
					return false;
				}
			}
		}

		/// <summary>
		/// Useable input map to bind Unity input collection.
		/// </summary>
		private class AxisMap : BaseInputMap
		{
			private readonly Func<string, float> mFunction;
			private readonly string mAxisName;

			public AxisMap(Func<string, float> check, string value, InputLevel level) : base(level)
			{
				mFunction = check;
				mAxisName = value;
			}

			/// <inheritdoc />
			public override bool Activated()
			{
				return Math.Abs(mFunction(mAxisName)) > 0.01f;
			}

			/// <summary>
			/// Get the current value of this axis.
			/// </summary>
			public float CurrentValue()
			{
				try
				{
					return mFunction(mAxisName);
				}
				catch (ArgumentException)
				{
					Logger.Warn("Axis or button is not set up: " + mAxisName, Logger.System.Input);
					return 0.0f;
				}
			}
		}

		private readonly Dictionary<Action, List<BaseInputMap>> mCommands = new Dictionary<Action, List<BaseInputMap>>();
		private readonly Dictionary<Action<float>, List<AxisMap>> mAxes = new Dictionary<Action<float>, List<AxisMap>>();
		private InputLevel mEnabledInputs = InputLevel.None;

		#region IInput Implementation

		/// <inheritdoc />
		public IInput RegisterInput<T>(Func<T, bool> method, T key, Action command, InputLevel level, bool allowOtherKeys = true)
		{
			Logger.Info("Registering " + command.Method.Name + " as a new input on key " + key + ".", Logger.System.Input);

			if (command.Method.Name.Contains("Cmd") || command.Method.Name.Contains("Rpc"))
				throw new ArgumentException("It is forbidden to bind network calls directly to the input system!", "command");

			var newInput = new InputMap<T>(method, key, level);
			ClearInputFromOtherCommands(newInput);

			if (!allowOtherKeys)
				ClearOtherInputsForCommand(command, mCommands);

			if (!mCommands.ContainsKey(command))
				mCommands[command] = new List<BaseInputMap>();

			mCommands[command].Add(newInput);
			return this;
		}

		/// <inheritdoc />
		public IInput UnregisterInput(Action command)
		{
			Logger.Info("Unegistering " + command.Method.Name + " from all inputs.", Logger.System.Input);
			mCommands.Remove(command);
			return this;
		}

		/// <inheritdoc />
		public IInput RegisterAxis(Func<string, float> method, string axis, Action<float> command, InputLevel level, bool allowOtherAxes = true)
		{
			Logger.Info("Registering " + command.Method.Name + " as a new input on axis " + axis + ".", Logger.System.Input);
			AxisMap newInput = new AxisMap(method, axis, level);
			ClearInputFromOtherCommands(newInput);

			if (!allowOtherAxes)
				ClearOtherInputsForCommand(command, mAxes);

			if (!mAxes.ContainsKey(command))
				mAxes[command] = new List<AxisMap>();

			mAxes[command].Add(newInput);
			return this;
		}

		/// <inheritdoc />
		public IInput UnregisterAxis(Action<float> command)
		{
			Logger.Info("Unegistering " + command.Method.Name + " from all inputs.", Logger.System.Input);
			mAxes.Remove(command);
			return this;
		}

		/// <inheritdoc />
		public IInput SetInputLevel(InputLevel level)
		{
			mEnabledInputs = level;
			foreach (object value in Enum.GetValues(typeof(InputLevel)))
			{
				InputLevel v = (InputLevel)value;

				if ((level & v) == v)
					EventManager.Notify(() => EventManager.Local.InputLevelChanged(v, true));
				else
					EventManager.Notify(() => EventManager.Local.InputLevelChanged(v, false));
			}

			return this;
		}

		/// <inheritdoc />
		public IInput SetInputLevelState(InputLevel flag, bool state)
		{
			return state ? EnableInputLevel(flag) : DisableInputLevel(flag);
		}

		/// <inheritdoc />
		public IInput DisableInputLevel(InputLevel level)
		{
			if (!IsInputEnabled(level))
				return this;

			mEnabledInputs &= ~level;
			EventManager.Notify(() => EventManager.Local.InputLevelChanged(level, false));

			return this;
		}

		/// <inheritdoc />
		public IInput EnableInputLevel(InputLevel level)
		{
			if (IsInputEnabled(level))
				return this;

			mEnabledInputs |= level;
			EventManager.Notify(() => EventManager.Local.InputLevelChanged(level, true));

			return this;
		}

		/// <inheritdoc />
		public bool IsInputEnabled(InputLevel level)
		{
			return (mEnabledInputs & level) == level;
		}

		#endregion

		/// <summary>
		/// Remove all other inputs for this command.
		/// </summary>
		private static void ClearOtherInputsForCommand<T1, T2>(T1 c, IDictionary<T1, List<T2>> commands)
		{
			if (commands.ContainsKey(c))
				commands[c].Clear();
		}

		/// <summary>
		/// Remove this input from all other commands.
		/// </summary>
		private void ClearInputFromOtherCommands(BaseInputMap i)
		{
			foreach (var pair in mCommands)
				pair.Value.Remove(i);
		}

		/// <summary>
		/// Unity Update loop.
		/// </summary>
		private void Update()
		{
			foreach (var i in mCommands)
			{
				foreach (BaseInputMap x in i.Value)
				{
					if (x.Enabled() && x.Activated())
					{
						i.Key.Invoke();
						break;
					}
				}
			}

			foreach (var i in mAxes)
			{
				float value = 0.0f;
				foreach (var input in i.Value)
				{
					if (input.Enabled())
						value += input.CurrentValue();
				}
				i.Key.Invoke(value);
			}
		}
	}
}
