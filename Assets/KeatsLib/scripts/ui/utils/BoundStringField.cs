﻿using UIText = UnityEngine.UI.Text;

namespace KeatsLib.UI.Utils
{
	/// <inheritdoc />
	public class BoundStringField : BoundUIElement<string>
	{
		/// Private variables
		private UIText mTextElement;

		/// <inheritdoc />
		protected override void Awake()
		{
			base.Awake();
			mTextElement = GetComponent<UIText>();
		}

		/// <inheritdoc />
		protected override void HandlePropertyChanged()
		{
			mTextElement.text = property.value;
		}
	}
}
