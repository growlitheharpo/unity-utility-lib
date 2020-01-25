namespace KeatsLib.UI
{
	/// <summary>
	/// The different types of screen panels that will pause input.
	/// </summary>
	public enum ScreenPanelTypes
	{
		Pause,
		Console,
		DebugMenu,
	}

	public static class PropertyHashes
	{
		public static int SampleProperty => "sample_property".GetHashCode();
	}
}
