﻿using KeatsLib.Audio;
using KeatsLib.Debug;
using KeatsLib.Input;
using KeatsLib.UI;
using KeatsLib.UI.Utils;
using System;
using UnityEngine;
using Logger = KeatsLib.Debug.Logger;

namespace KeatsLib.Services
{
	/// <summary>
	/// Generates null services for the ServiceLocator when instances are not found.
	/// 
	/// You MUST create a partial implementation to provide InitializeGameNullService<T>
	/// </summary>
	public static partial class NullServices
	{
		/// <summary>
		/// Create an instance of a service from the provided interface type.
		/// </summary>
		/// <typeparam name="T">The type of service to be created.</typeparam>
		public static T Create<T>() where T : class
		{
			if (typeof(T) == typeof(IInput))
				return new NullInput() as T;
			else if (typeof(T) == typeof(IAudioManager))
				return new NullAudioManager() as T;
			else if (typeof(T) == typeof(IGameConsole))
				return new NullConsole() as T;
			else if (typeof(T) == typeof(IUIManager))
				return new NullUIManager() as T;

			T userResult = InitializeGameNullService<T>();
			if (userResult != null)
				return userResult;

			throw new NullReferenceException("Could not create null service! " + typeof(T).Name);
		}

		/// <inheritdoc />
		private class NullAudioManager : IAudioManager
		{
			/// <inheritdoc />
			private class NullAudioReference : IAudioReference
			{
				/// <inheritdoc />
				public IAudioReference Start()
				{
					return this;
				}

				/// <inheritdoc />
				public IAudioReference Kill(bool allowFade = true)
				{
					return this;
				}

				/// <inheritdoc />
				public IAudioReference SetVolume(float vol)
				{
					return this;
				}

				/// <inheritdoc />
				public IAudioReference AttachToRigidbody(Rigidbody rb)
				{
					return this;
				}

				/// <inheritdoc />
				public bool isPlaying { get { return false; } }

				/// <inheritdoc />
				public IAudioReference SetParameter(string name, float value)
				{
					return this;
				}

				/// <inheritdoc />
				public float GetParameter(string name)
				{
					return default;
				}
			}

			/// <inheritdoc />
			public void InitializeDatabase()
			{
				Logger.Info("NULL SERVICE: IAudioManager.InitializeDatabase()", Logger.System.Services);
				EventManager.Notify(EventManager.Local.InitialAudioLoadComplete);
			}

			/// <inheritdoc />
			public IAudioReference CreateSound(AudioEvent e, Transform location, bool autoPlay = true)
			{
				Logger.Info("NULL SERVICE: IAudioManager.CreateSound()", Logger.System.Services);
				return new NullAudioReference();
			}

			/// <inheritdoc />
			public IAudioReference CreateSound(AudioEvent e, Transform location, Vector3 offset, Space offsetType = Space.Self, bool autoPlay = true)
			{
				Logger.Info("NULL SERVICE: IAudioManager.CreateSound()", Logger.System.Services);
				return new NullAudioReference();
			}

			/// <inheritdoc />
			public IAudioReference PlayAnnouncerLine(AudioEvent e)
			{
				Logger.Info("NULL SERVICE: IAudioManager.PlayAnnouncerLine()", Logger.System.Services);
				return new NullAudioReference();
			}

			/// <inheritdoc />
			// ReSharper disable once RedundantAssignment
			public IAudioReference CheckReferenceAlive(ref IAudioReference reference)
			{
				reference = null;
				return null;
			}
		}
		
		/// <inheritdoc />
		private class NullConsole : IGameConsole
		{
			/// <inheritdoc />
			public void AssertCheatsEnabled()
			{
				Logger.Info("NULL SERVICE: IGameConsole.AssertCheatsEnabled()", Logger.System.Services);
			}

			/// <inheritdoc />
			public IGameConsole RegisterCommand(string command, Action<string[]> handle)
			{
				Logger.Info("NULL SERVICE: IGameConsole.RegisterCommand()", Logger.System.Services);
				return this;
			}

			/// <inheritdoc />
			public IGameConsole UnregisterCommand(string command)
			{
				return this;
			}

			/// <inheritdoc />
			public IGameConsole UnregisterCommand(Action<string[]> handle)
			{
				return this;
			}

			/// <inheritdoc />
			public Logger.System enabledLogLevels
			{
				get
				{
					Logger.Info("NULL SERVICE: IGameConsole.enabledLogLevels", Logger.System.Services);
					return (Logger.System)int.MaxValue;
				}
			}

			KeatsLib.Debug.Logger.System IGameConsole.enabledLogLevels => throw new NotImplementedException();
		}
		

		/// <inheritdoc />
		private class NullUIManager : IUIManager
		{
			/// <inheritdoc />
			public BoundProperty<T> GetProperty<T>(int hash)
			{
				Logger.Info("NULL SERVICE: NullGameplayUIManager.GetProperty<T>()", Logger.System.Services);
				return null;
			}

			/// <inheritdoc />
			public void BindProperty(int hash, BoundProperty prop)
			{
				Logger.Info("NULL SERVICE: NullGameplayUIManager.BindProperty()", Logger.System.Services);
			}

			/// <inheritdoc />
			public void UnbindProperty(BoundProperty prop)
			{
				Logger.Info("NULL SERVICE: NullGameplayUIManager.UnbindProperty()", Logger.System.Services);
			}

			/// <inheritdoc />
			public IScreenPanel PushNewPanel(ScreenPanelTypes type)
			{
				return null;
			}

			/// <inheritdoc />
			public IUIManager PopPanel(ScreenPanelTypes type)
			{
				return this;
			}

			/// <inheritdoc />
			public IScreenPanel TogglePanel(ScreenPanelTypes type)
			{
				return null;
			}

			/// <inheritdoc />
			public IUIManager RegisterPanel(IScreenPanel panelObject, ScreenPanelTypes type, bool disablePanel = true)
			{
				return this;
			}

			/// <inheritdoc />
			public IUIManager UnregisterPanel(IScreenPanel panelObject)
			{
				return this;
			}
		}

		/// <inheritdoc />
		private class NullInput : IInput
		{
			/// <inheritdoc />
			public IInput RegisterInput<T>(Func<T, bool> method, T key, Action command, InputLevel level, bool allowOtherKeys = true)
			{
				Logger.Info("NULL SERVICE: IInput.RegisterInput()", Logger.System.Services);
				return this;
			}

			/// <inheritdoc />
			public IInput UnregisterInput(Action command)
			{
				Logger.Info("NULL SERVICE: IInput.UnregisterInput()", Logger.System.Services);
				return this;
			}

			/// <inheritdoc />
			public IInput RegisterAxis(
				Func<string, float> method, string axis, Action<float> command, InputLevel level, bool allowOtherAxes = true)
			{
				Logger.Info("NULL SERVICE: IInput.RegisterAxis()", Logger.System.Services);
				return this;
			}

			/// <inheritdoc />
			public IInput UnregisterAxis(Action<float> command)
			{
				Logger.Info("NULL SERVICE: IInput.UnregisterAxis()", Logger.System.Services);
				return this;
			}

			/// <inheritdoc />
			public IInput SetInputLevel(InputLevel level)
			{
				Logger.Info("NULL SERVICE: IInput.SetInputLevel()", Logger.System.Services);
				return this;
			}

			/// <inheritdoc />
			public IInput SetInputLevelState(InputLevel level, bool state)
			{
				Logger.Info("NULL SERVICE: IInput.SetInputLevelState()", Logger.System.Services);
				return this;
			}

			/// <inheritdoc />
			public IInput EnableInputLevel(InputLevel level)
			{
				Logger.Info("NULL SERVICE: IInput.EnableInputLevel()", Logger.System.Services);
				return this;
			}

			/// <inheritdoc />
			public IInput DisableInputLevel(InputLevel level)
			{
				Logger.Info("NULL SERVICE: IInput.DisableInputLevel()", Logger.System.Services);
				return this;
			}

			/// <inheritdoc />
			public bool IsInputEnabled(InputLevel level)
			{
				Logger.Info("NULL SERVICE: IInput.IsInputEnabled()", Logger.System.Services);
				return false;
			}
		}
	}
}
