using UnityEngine.UI;

namespace KeatsLib.UI.Utils
{
	/// <summary>
	/// UI class to bind the fill value of a UI image to a BoundProperty in code.
	/// </summary>
	public class BoundFloatFillPercentField : BaseBoundFillPercentField<float>
	{
		protected override void RecalculateFill(Image image, float full, float current)
		{
			image.fillAmount = current / full;
		}
	}
}
