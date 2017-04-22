using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AnimationTestUtilityScript))]
public class AnimationTestUtilityScriptEditor : Editor
{
    public AnimationTestUtilityScriptEditor()
    {
        EditorApplication.playmodeStateChanged = playmodeChanged;
    }

    private bool mShowWarning;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        drawScriptField();
        drawPlayAnimationButton();

        serializedObject.ApplyModifiedProperties();
    }

    private void playmodeChanged()
    {
        mShowWarning = false;
    }

    private void drawScriptField()
    {
        GUI.enabled = false;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"), true);
        GUI.enabled = true;
    }

    private void drawPlayAnimationButton()
    {
        if (mShowWarning)
            EditorGUILayout.HelpBox("This script will only work while in \"Play\" mode!", MessageType.Error);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("mCurrentAnimationTrigger"), new GUIContent("Animation Trigger"));
        if (GUILayout.Button("Play Animation"))
        {
            if (!EditorApplication.isPlaying)
                mShowWarning = true;
            else
            {
                string anim = serializedObject.FindProperty("mCurrentAnimationTrigger").stringValue;
                GameObject obj = ((AnimationTestUtilityScript) target).gameObject;

                AnimationUtility.PlayAnimation(obj, anim);
            }
        }
        EditorGUILayout.EndHorizontal();
    }
}
