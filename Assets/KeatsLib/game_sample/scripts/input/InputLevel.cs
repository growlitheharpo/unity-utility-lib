using System;

namespace KeatsLib.Input
{
	/// <summary>
	/// Used to mark which input types are currently enabled.
	/// </summary>
	[Flags]
	public enum InputLevel
	{
		None = 0,
		Gameplay = 1,
		HideCursor = 2,
		InGameMenu = 32,
		PauseMenu = 64,
		DevConsole = 128,
		Scorecard = 256,
		All = int.MaxValue
	}
}
