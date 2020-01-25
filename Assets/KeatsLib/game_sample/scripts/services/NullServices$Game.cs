namespace KeatsLib.Services
{
	public static partial class NullServices
	{
		private static T InitializeGameNullService<T>() where T : class
		{
			return null;
		}
	}
}
