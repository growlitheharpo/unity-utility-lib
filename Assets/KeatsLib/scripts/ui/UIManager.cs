﻿using System;
using System.Collections.Generic;
using System.Linq;
using KeatsLib.Collections;
using KeatsLib.Input;
using KeatsLib.Services;
using KeatsLib.UI.Utils;
using KeatsLib.Unity;
using UnityEngine;

namespace KeatsLib.UI
{
	/// <inheritdoc cref="IUIManager" />
	public class UIManager : MonoSingleton<UIManager>, IUIManager
	{
		/// Private variables
		private Dictionary<int, WeakReference> mPropertyMap;
		private Dictionary<ScreenPanelTypes, IScreenPanel> mPanelTypeToObjectMap;
		private Dictionary<IScreenPanel, ScreenPanelTypes> mPanelObjectToTypeMap;
		private UniqueStack<IScreenPanel> mActivePanels;

		/// <summary>
		/// Unity's Awake function
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			mPropertyMap = new Dictionary<int, WeakReference>();
			mPanelObjectToTypeMap = new Dictionary<IScreenPanel, ScreenPanelTypes>();
			mPanelTypeToObjectMap = new Dictionary<ScreenPanelTypes, IScreenPanel>();
			mActivePanels = new UniqueStack<IScreenPanel>();
		}

		#region Bound Properties

		/// <inheritdoc />
		public BoundProperty<T> GetProperty<T>(int hash)
		{
			if (!mPropertyMap.ContainsKey(hash))
				return null;

			WeakReference reference = mPropertyMap[hash];
			if (reference != null && reference.IsAlive)
				return reference.Target as BoundProperty<T>;

			mPropertyMap.Remove(hash);
			return null;
		}

		/// <inheritdoc />
		public void BindProperty(int hash, BoundProperty prop)
		{
			mPropertyMap[hash] = new WeakReference(prop);
		}

		/// <inheritdoc />
		public void UnbindProperty(BoundProperty obj)
		{
			var keys = mPropertyMap
				.Where(x => ReferenceEquals(x.Value.Target, obj))
				.Select(x => x.Key)
				.ToArray();

			foreach (int key in keys)
				mPropertyMap.Remove(key);
		}

		#endregion

		#region Panel Stack Management

		/// <inheritdoc />
		public IScreenPanel PushNewPanel(ScreenPanelTypes type)
		{
			if (!mPanelTypeToObjectMap.ContainsKey(type))
				return null;

			IScreenPanel panel = mPanelTypeToObjectMap[type];
			GameObject go = panel.gameObject;

			go.SetActive(true);
			panel.OnEnablePanel();

			if (panel.disablesInput)
			{
				ServiceLocator.Get<IInput>()
					.DisableInputLevel(InputLevel.Gameplay)
					.DisableInputLevel(InputLevel.HideCursor);
			}

			mActivePanels.Push(panel);
			go.transform.SetAsLastSibling();

			return panel;
		}

		/// <inheritdoc />
		public IUIManager PopPanel(ScreenPanelTypes type)
		{
			if (!mPanelTypeToObjectMap.ContainsKey(type))
				return this;

			IScreenPanel panel = mPanelTypeToObjectMap[type];
			GameObject go = panel.gameObject;

			panel.OnDisablePanel();
			go.SetActive(false);
			mActivePanels.Remove(panel);

			if (!mActivePanels.Any(x => x.disablesInput)) // no active panels that disable input
			{
				ServiceLocator.Get<IInput>()
					.EnableInputLevel(InputLevel.Gameplay)
					.EnableInputLevel(InputLevel.HideCursor);
			}

			return this;
		}

		public IScreenPanel TogglePanel(ScreenPanelTypes type)
		{
			if (!mPanelTypeToObjectMap.ContainsKey(type))
				return null;

			IScreenPanel panel = mPanelTypeToObjectMap[type];
			if (!mActivePanels.Contains(panel))
				return PushNewPanel(type);

			PopPanel(type);
			return panel;
		}

		/// <inheritdoc />
		public IUIManager RegisterPanel(IScreenPanel panelObject, ScreenPanelTypes type, bool disablePanel = true)
		{
			if (mPanelTypeToObjectMap.ContainsKey(type) || mPanelObjectToTypeMap.ContainsKey(panelObject))
				throw new ArgumentException("Registering a panel for more than one type, or more than one panel for a type!");

			mPanelObjectToTypeMap[panelObject] = type;
			mPanelTypeToObjectMap[type] = panelObject;

			if (disablePanel)
				panelObject.gameObject.SetActive(false);

			return this;
		}

		/// <inheritdoc />
		public IUIManager UnregisterPanel(IScreenPanel panelObject)
		{
			if (!mPanelObjectToTypeMap.ContainsKey(panelObject))
				return this;

			ScreenPanelTypes type = mPanelObjectToTypeMap[panelObject];

			mPanelObjectToTypeMap.Remove(panelObject);
			mPanelTypeToObjectMap.Remove(type);

			mActivePanels.Remove(panelObject);

			return this;
		}

		#endregion
	}
}
