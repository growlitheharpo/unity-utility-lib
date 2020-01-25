﻿using KeatsLib.Input;
using KeatsLib.Services;
using KeatsLib.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace KeatsLib.Debug
{
	/// <inheritdoc cref="BaseGameConsoleView"/>
	public class ImmediateModeConsoleView : BaseGameConsoleView
	{
		/// <summary>
		/// Utility struct that holds important information for a log.
		/// </summary>
		private class LogEntryHolder
		{
			public string timestamp { get; set; }
			public string message { get; set; }
			public LogType type { get; set; }
		}

		/// Private variables
		private const KeyCode CONSOLE_TOGGLE = KeyCode.BackQuote;
		private const int MAX_LOGS = 25;
		private string mCurrentCommand = "";

		private object mEntryLock = new object();
		private Queue<LogEntryHolder> mEntries;

		private Action<string[]> mEntryHandler;
		private Vector2 mViewScrollPosition;

		/// <summary>
		/// Unity's Awake function.
		/// </summary>
		private void Awake()
		{
			lock (mEntryLock)
				mEntries = new Queue<LogEntryHolder>(MAX_LOGS);
		}

		/// <summary>
		/// Unity's Start function.
		/// </summary>
		private void Start()
		{
			ServiceLocator.Get<IInput>()
				.RegisterInput(UnityEngine.Input.GetKeyDown, CONSOLE_TOGGLE, INPUT_ToggleConsole, InputLevel.None);

			ServiceLocator.Get<IUIManager>()
				.RegisterPanel(this, ScreenPanelTypes.Console);
		}

		/// <summary>
		/// Unity's OnDestroy function
		/// </summary>
		private void OnDestroy()
		{
			ServiceLocator.Get<IInput>()
				.UnregisterInput(INPUT_ToggleConsole);

			ServiceLocator.Get<IUIManager>()
				.UnregisterPanel(this);
		}

		/// <summary>
		/// INPUT HANDLER: Toggle the debug game console.
		/// </summary>
		private void INPUT_ToggleConsole()
		{
			ServiceLocator.Get<IUIManager>().TogglePanel(ScreenPanelTypes.Console);
		}

		/// <inheritdoc />
		public override void ClearLogs()
		{
			lock (mEntryLock)
				mEntries.Clear();
		}

		/// <inheritdoc />
		public override void AddMessage(string message, LogType messageType)
		{
			lock (mEntryLock)
			{
				while (mEntries.Count >= MAX_LOGS)
					mEntries.Dequeue();

				mEntries.Enqueue(new LogEntryHolder
				{
					timestamp = DateTime.Now.ToString("h:mm:ss tt"),
					message = message,
					type = messageType
				});

				ForceScrollToBottom();
			}
		}

		/// <inheritdoc />
		public override void RegisterCommandHandler(Action<string[]> handler)
		{
			mEntryHandler = handler;
		}
		
		/// <summary>
		/// Unity's OnEnable function.
		/// </summary>
		private void OnEnable()
		{
			ForceScrollToBottom();
		}

		/// <summary>
		/// Send a command to our registered handler.
		/// </summary>
		private void SendCommand()
		{
			var command = mCurrentCommand.Split(' ');
			mCurrentCommand = "";
			mEntryHandler(command);
		}

		/// <summary>
		/// Force our scroll position to the bottom.
		/// </summary>
		private void ForceScrollToBottom()
		{
			mViewScrollPosition = Vector2.positiveInfinity;
		}

		/// <summary>
		/// Unity GUI loop.
		/// </summary>
		private void OnGUI()
		{
			float baseX = Screen.width, baseY = Screen.height;
			lock (mEntryLock)
				DrawLogs(baseX, baseY);

			DrawEntryBox(baseX, baseY);
		}

		/// <summary>
		/// Called from immediate mode OnGUI. Draws the debug console logs.
		/// </summary>
		private void DrawLogs(float baseX, float baseY)
		{
			Rect consoleRect = new Rect(10.0f, baseY * 0.5f, baseX - 20.0f, baseY * 0.45f);
			GUI.Box(consoleRect, "Developer Console");

			Rect layoutRect = new Rect(consoleRect.x - 2.0f, consoleRect.y + 10.0f, consoleRect.width - 4.0f,
				consoleRect.height - 15.0f);
			GUILayout.BeginArea(layoutRect);
			mViewScrollPosition = GUILayout.BeginScrollView(mViewScrollPosition, false, true);

			foreach (LogEntryHolder log in mEntries)
			{
				GUILayout.BeginHorizontal();

				Color defaultColor = GUI.color;
				switch (log.type)
				{
					case LogType.Error:
						GUI.color = Color.red;
						GUILayout.Label("Error " + log.timestamp + ": ");
						break;
					case LogType.Warning:
						GUI.color = Color.yellow;
						GUILayout.Label("Warning " + log.timestamp + ": ");
						break;
					default:
						GUILayout.Label("Info " + log.timestamp + ": ");
						break;
				}

				GUI.color = defaultColor;

				GUILayout.Label(log.message);
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			}

			GUILayout.EndScrollView();
			GUILayout.EndArea();
		}

		/// <summary>
		/// Called from immediate mode OnGUI. Draws the text entry box.
		/// </summary>
		private void DrawEntryBox(float baseX, float baseY)
		{
			if (Event.current.type == EventType.KeyDown)
			{
				if (Event.current.keyCode == KeyCode.Return)
					SendCommand();
				else if (Event.current.keyCode == CONSOLE_TOGGLE)
					ServiceLocator.Get<IUIManager>().PopPanel(ScreenPanelTypes.Console);
			}

			Rect entryRect = new Rect(10.0f, baseY * 0.95f, baseX - 20.0f, baseY * 0.045f);
			GUI.SetNextControlName("ConsoleEntry");
			mCurrentCommand = GUI.TextField(entryRect, mCurrentCommand);
			GUI.FocusControl("ConsoleEntry");
		}
	}
}
