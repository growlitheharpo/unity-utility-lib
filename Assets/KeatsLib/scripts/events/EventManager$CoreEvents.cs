using KeatsLib.Data;
using KeatsLib.Input;
using System;

/// <summary>
/// A manager for a custom Event system within Unity.
/// </summary>
public partial class EventManager
{
	/// <summary>
	/// Holder class for all events that are only designed to occur on local clients.
	/// </summary>
	public static partial class Local
	{
		/// <summary>
		/// Event called when the audio system has finished loading and is ready for use.
		/// </summary>
		public static event Action OnInitialAudioLoadComplete = () => { LogEvent(); };

		/// <summary>
		/// Event called when the audio system has finished loading and is ready for use.
		/// </summary>
		public static void InitialAudioLoadComplete()
		{
			OnInitialAudioLoadComplete();
		}

		/// <summary>
		/// Event called when new OptionsData are ready to be applied.
		/// </summary>
		public static event Action<IOptionsData> OnApplyOptionsData = e => { LogEvent(); };

		/// <summary>
		/// Event called when new OptionsData are ready to be applied.
		/// </summary>
		public static void ApplyOptionsData(IOptionsData data)
		{
			OnApplyOptionsData(data);
		}

		/// <summary>
		/// Event called when the local player's input rules have changed.
		/// PARAMETER 1: The input type that was changed.
		/// PARAMETER 2: Whether that input was enabled (true) or disabled (false).
		/// </summary>
		public static event Action<InputLevel, bool> OnInputLevelChanged = (a, b) => { LogEvent(); };

		/// <summary>
		/// Event called when the local player's input rules have changed.
		/// </summary>
		/// <param name="level">The input type that was changed.</param>
		/// <param name="state">Whether that input was enabled (true) or disabled (false).</param>
		public static void InputLevelChanged(InputLevel level, bool state)
		{
			OnInputLevelChanged(level, state);
		}

		/// <summary>
		/// Event called when the player has confirmed they want to quit a game
		/// in-progress, either through the pause menu or through the "game over" panel.
		/// </summary>
		public static event Action OnConfirmQuitGame = () => { LogEvent(); };

		/// <summary>
		/// Event called when the player has confirmed they want to quit a game
		/// in-progress, either through the pause menu or through the "game over" panel.
		/// </summary>
		public static void ConfirmQuitGame()
		{
			OnConfirmQuitGame();
		}

		/// <summary>
		/// Event called when the player has requested a game pause.
		/// </summary>
		public static event Action OnTogglePause = () => { LogEvent(); };

		/// <summary>
		/// Event called when the player has requested a game pause.
		/// </summary>
		public static void TogglePause()
		{
			OnTogglePause();
		}
	}
}
