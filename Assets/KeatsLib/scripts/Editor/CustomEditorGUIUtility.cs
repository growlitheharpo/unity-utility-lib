﻿using System;
using KeatsLib.Unity;
using UnityEngine;

namespace UnityEditor
{
	/// <summary>
	/// Utility class for Editor functions.
	/// </summary>
	public static class CustomEditorGUIUtility
	{
		public const float LINE_HEIGHT_MODIFIER = 1.05f;

		/// <summary>
		/// Draws the "script" field at the top of a fully custom inspector.
		/// </summary>
		public static void DrawScriptField(SerializedObject serializedObject)
		{
			DrawDisabled(() => EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"), true));
		}

		/// <summary>
		/// Draw an easier-to-edit list in GUILayout mode.
		/// </summary>
		/// <param name="prop">The property of the list.</param>
		/// <param name="label">The label for the property as a whole.</param>
		/// <param name="deleteLabel">The label for the "delete item" button.</param>
		/// <param name="addLabel">The label for the "add item" button.</param>
		/// <param name="itemLabel">How each individual item should be labeled.</param>
		public static void DrawList(
			SerializedProperty prop, string label, string deleteLabel = "X",
			string addLabel = "+", GUIContent itemLabel = null)
		{
			EditorGUILayout.LabelField(label);

			EditorGUI.indentLevel++;

			for (int i = 0; i < prop.arraySize; i++)
			{
				EditorGUILayout.BeginHorizontal();

				EditorGUILayout.PropertyField(prop.GetArrayElementAtIndex(i), itemLabel, true);

				if (GUILayout.Button(deleteLabel, GUILayout.MaxWidth(25.0f)))
				{
					prop.DeleteArrayElementAtIndex(i);
					return;
				}

				EditorGUILayout.EndHorizontal();
			}

			if (GUILayout.Button(addLabel))
				prop.InsertArrayElementAtIndex(prop.arraySize);

			EditorGUI.indentLevel--;
		}

		/// <summary>
		/// Draw an easier-to-edit list in GUI mode.
		/// </summary>
		/// <param name="pos">The rectangle of this list.</param>
		/// <param name="prop">The property of the list.</param>
		/// <param name="label">The label for the property as a whole.</param>
		/// <param name="deleteLabel">The label for the "delete item" button.</param>
		/// <param name="addLabel">The label for the "add item" button.</param>
		/// <param name="itemLabel">How each individual item should be labeled.</param>
		public static void DrawList(
			Rect pos, SerializedProperty prop, string label, string deleteLabel = "X",
			string addLabel = "+", GUIContent itemLabel = null)
		{
			const float delW = 25.0f;
			float propW = pos.width - delW;

			float startY = label != "" ? DrawLabel(pos, label) : pos.y;

			Rect propRect = new Rect(pos.x, startY, propW, EditorGUIUtility.singleLineHeight);
			Rect delRect = propRect.ShiftAlongX(delW, EditorGUIUtility.singleLineHeight);

			EditorGUI.indentLevel++;
			propRect = itemLabel == null
				? DrawDefaultList(prop, propRect, delRect, deleteLabel)
				: DrawListWithLabel(prop, propRect, delRect, deleteLabel, itemLabel);

			if (propRect == Rect.zero)
			{
				EditorGUI.indentLevel--;
				return;
			}

			float indentSize = EditorGUI.indentLevel * 15; //let's say that's roughly right for now.
			Rect addRect = new Rect(pos.x + indentSize, propRect.y, pos.width - indentSize, EditorGUIUtility.singleLineHeight);

			if (GUI.Button(addRect, addLabel))
				prop.InsertArrayElementAtIndex(prop.arraySize);

			EditorGUI.indentLevel--;
		}

		/// <summary>
		/// Draw just a label at a given position.
		/// </summary>
		/// <returns>The y position of the next line after the label.</returns>
		private static float DrawLabel(Rect pos, string label)
		{
			Rect labelRect = new Rect(pos.x, pos.y, pos.width, EditorGUIUtility.singleLineHeight);
			EditorGUI.LabelField(labelRect, label);

			return labelRect.y + labelRect.height;
		}

		/// <summary>
		/// Draws a list with the default labels.
		/// </summary>
		private static Rect DrawDefaultList(SerializedProperty prop, Rect propRect, Rect delRect, string deleteLabel)
		{
			for (int i = 0; i < prop.arraySize; i++)
			{
				EditorGUI.PropertyField(propRect, prop.GetArrayElementAtIndex(i), GUIContent.none, true);

				if (GUI.Button(delRect, new GUIContent(deleteLabel)))
				{
					prop.DeleteArrayElementAtIndex(i);
					return Rect.zero;
				}

				propRect.y += propRect.height;
				delRect.y += delRect.height;
			}

			return propRect;
		}

		/// <summary>
		/// Draws a list with the labels the user provided.
		/// </summary>
		private static Rect DrawListWithLabel(SerializedProperty prop, Rect propRect, Rect delRect, string deleteLabel, GUIContent itemLabel)
		{
			Rect labelRect = propRect;
			labelRect.width *= 0.2f;

			propRect.width -= labelRect.width;
			propRect.x += labelRect.width;

			GUIStyle style = GUI.skin.label;
			style.fixedWidth = labelRect.width;

			for (int i = 0; i < prop.arraySize; i++)
			{
				EditorGUI.LabelField(labelRect, new GUIContent(itemLabel.text + (i + 1)), style);
				EditorGUI.PropertyField(propRect, prop.GetArrayElementAtIndex(i), GUIContent.none, true);

				if (GUI.Button(delRect, new GUIContent(deleteLabel)))
				{
					prop.DeleteArrayElementAtIndex(i);
					return Rect.zero;
				}

				labelRect.y += labelRect.height;
				propRect.y += propRect.height;
				delRect.y += delRect.height;

			}

			return propRect;
		}

		/// <summary>
		/// Wrap a provided GUI set in BeginHorizontal() and FlexibleSpace().
		/// </summary>
		public class HorizontalLayoutBlock : IDisposable
		{
			public HorizontalLayoutBlock()
			{
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
			}

			public void Dispose()
			{
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			}
		}

		/// <summary>
		/// Wrap a provided GUI set in BeginHorizontal() and FlexibleSpace().
		/// </summary>
		public static void HorizontalLayout(Action guiFunc)
		{
			using (new HorizontalLayoutBlock())
			{
				guiFunc.Invoke();
			}
		}

		/// <summary>
		/// Wrap a provided GUI set in GUI.enabled = false, then restore.
		/// </summary>
		public class DrawDisabledBlock : IDisposable
		{
			bool previousState;

			public DrawDisabledBlock()
			{
				previousState = GUI.enabled;
				GUI.enabled = false;
			}

			public void Dispose()
			{
				GUI.enabled = previousState;
			}
		}

		/// <summary>
		/// Wrap a provided GUI set in GUI.enabled = false, then restore.
		/// </summary>
		public static void DrawDisabled(Action guiFunc)
		{
			using (new DrawDisabledBlock())
			{
				guiFunc.Invoke();
			}
		}

		/// <summary>
		/// Wrap a provided GUI set with a particular GUI.color, then restore.
		/// </summary>
		public class DrawAsColorBlock : IDisposable
		{
			Color previousColor;

			public DrawAsColorBlock(Color c)
			{
				previousColor = GUI.color;
				GUI.color = c;
			}

			public void Dispose()
			{
				GUI.color = previousColor;
			}
		}

		/// <summary>
		/// Wrap a provided GUI set with a particular GUI.color, then restore.
		/// </summary>
		public static void DrawAsColor(Color color, Action guiFunc)
		{
			using (new DrawAsColorBlock(color))
			{
				guiFunc.Invoke();
			}
		}

		/// <summary>
		/// Insert a vertical space of the provided size.
		/// </summary>
		public static void VerticalSpacer(float size)
		{
			GUILayout.BeginVertical();
			GUILayout.Space(size);
			GUILayout.EndVertical();
		}

		/// <summary>
		/// Get the string value of an enum in a serialized property.
		/// </summary>
		/// <param name="property">The property (an enum) to get the string value of.</param>
		public static string GetEnumPropertyString(SerializedProperty property)
		{
			int index = property.enumValueIndex;
			var names = property.enumNames;
			return names[index];
		}
	}
}
