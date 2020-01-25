namespace KeatsLib.Audio
{
	/// <summary>
	/// The enum for the audio events. Mapped 1-1 to our FMOD events.
	/// </summary>
	public enum AudioEvent
	{
		// Player
		LoopWalking = 30,
		Jump = 35,
		Land = 38,

		// UI
		MenuButtonHover = 180,
		MenuButtonPress = 185,
		HypeText = 190,
		CountdownTimer = 195,

		// Music
		MenuMusic = 200,
		IntroMusic = 205,
	}
}
