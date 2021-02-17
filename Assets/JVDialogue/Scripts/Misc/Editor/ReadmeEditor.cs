using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace JVDialogue
{
    [CustomEditor(typeof(Readme))]
    public class ReadmeEditor : Editor
    {
        SerializedObject so;
        Readme rm;

        private bool stylesSetup = false;
        private GUIStyle titleStyle;
        private GUIStyle imageStyle;
        private GUIStyle headerStyle;
        private GUIStyle bodyStyle;

        private void OnEnable()
        {
            so = serializedObject;
            rm = (Readme)target;
        }

        private void SetupStyles()
        {
            if (!stylesSetup)
            {
                titleStyle = new GUIStyle(EditorStyles.label);
                titleStyle.wordWrap = true;
                titleStyle.fontSize = 24;
                titleStyle.fontStyle = FontStyle.Bold;
                titleStyle.padding.top = 15;

                imageStyle = new GUIStyle(EditorStyles.label);
                imageStyle.padding.left = 10;

                headerStyle = new GUIStyle(EditorStyles.foldoutHeader);
                headerStyle.fontSize = 16;

                bodyStyle = new GUIStyle(EditorStyles.label);
                bodyStyle.wordWrap = true;
                bodyStyle.richText = true;

                stylesSetup = true;
            }
        }

        protected override void OnHeaderGUI()
        {
            SetupStyles();

            using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                GUILayout.Label(rm.icon, imageStyle, GUILayout.Width(64), GUILayout.Height(64));
                EditorGUILayout.LabelField(rm.title, titleStyle);
            }
        }

        public override void OnInspectorGUI()
        {
            SetupStyles();

            so.Update();
            EditorGUI.BeginChangeCheck();
            
            EditorGUILayout.LabelField(rm.body[0], bodyStyle);

            for (int i = 1; i < rm.headers.Length; i++)
            {
                rm.foldOuts[i] = EditorGUILayout.BeginFoldoutHeaderGroup(rm.foldOuts[i], $"{rm.headers[i]} {(rm.completedSection[i] ? "- Compeleted!" : "")}", headerStyle);
                if (rm.foldOuts[i])
                {
                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        EditorGUILayout.LabelField(rm.body[i], bodyStyle);

                        GUI.color = rm.completedSection[i] ? Color.green : Color.white;
                        using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
                        {
                            GUI.color = Color.white;
                            EditorGUILayout.LabelField("Mark Step as Complete", EditorStyles.boldLabel);
                            EditorGUILayout.PropertyField(so.FindProperty(nameof(Readme.completedSection)).GetArrayElementAtIndex(i), new GUIContent(""));
                        }
                    }
                }
                EditorGUILayout.EndFoldoutHeaderGroup();

                GUILayout.Space(10);
            }

            if (EditorGUI.EndChangeCheck())
            {
                so.ApplyModifiedProperties();
                Repaint();
            }
        }
    }
}
