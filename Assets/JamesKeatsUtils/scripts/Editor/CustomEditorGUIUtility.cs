using UnityEngine;
using UnityEditor;


// ReSharper disable once InconsistentNaming
public static class CustomEditorGUIUtility
{
    public const float LINE_HEIGHT_MODIFIER = 1.05f;
    private static float kOneLineHeight;

    public static void setLineHeight(float h)
    {
        kOneLineHeight = h;
    }

    public static float getLineHeight()
    {
        return kOneLineHeight;
    }

    public static void drawList(Rect pos, SerializedProperty prop, string label, string deleteLabel = "X",
        string addLabel = "Add", float deleteWidth = 0.2f, GUIContent itemLabel = null)
    {
        deleteWidth = Mathf.Clamp(deleteWidth, 0.0f, 1.0f);

        float propW = pos.width * (1.0f - deleteWidth);
        float delW = pos.width * deleteWidth;

        float startY = drawLabel(pos, label);

        Rect propRect = new Rect(pos.x, startY, propW, kOneLineHeight);
        Rect delRect = new Rect(propRect.x + propRect.width, startY, delW, kOneLineHeight);

        EditorGUI.indentLevel++;
        if (itemLabel == null)
            propRect = drawDefaultList(prop, propRect, delRect, deleteLabel);
        else
            propRect = drawListWithLabel(prop, propRect, delRect, deleteLabel, itemLabel);

        if (propRect == Rect.zero)
        {
            EditorGUI.indentLevel--;
            return;
        }

        float indentSize = EditorGUI.indentLevel * 15; //let's say that's roughly right for now.
        Rect addRect = new Rect(pos.x + indentSize, propRect.y, pos.width - indentSize, kOneLineHeight);

        if (GUI.Button(addRect, addLabel))
            prop.InsertArrayElementAtIndex(prop.arraySize);

        EditorGUI.indentLevel--;
    }

    private static float drawLabel(Rect pos, string label)
    {
        Rect labelRect = new Rect(pos.x, pos.y, pos.width, kOneLineHeight);
        EditorGUI.LabelField(labelRect, label);

        return labelRect.y + labelRect.height;
    }

    private static Rect drawDefaultList(SerializedProperty prop, Rect propRect, Rect delRect, string deleteLabel)
    {
        for (int i = 0; i < prop.arraySize; i++)
        {
            EditorGUI.PropertyField(propRect, prop.GetArrayElementAtIndex(i), true);

            if (GUI.Button(delRect, new GUIContent(deleteLabel)))
            {
                prop.DeleteArrayElementAtIndex(i);
                prop.DeleteArrayElementAtIndex(i);
                return Rect.zero;
            }

            propRect.y += propRect.height;
            delRect.y += delRect.height;
        }

        return propRect;
    }

    private static Rect drawListWithLabel(SerializedProperty prop, Rect propRect, Rect delRect, string deleteLabel, GUIContent itemLabel)
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

}
