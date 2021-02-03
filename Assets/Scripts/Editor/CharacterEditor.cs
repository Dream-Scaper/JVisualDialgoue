using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace JVDialogue
{
    [CustomEditor(typeof(Character))]
    public class CharacterEditor : Editor
    {
        SerializedObject so;
        private Character chara;

        private void OnEnable()
        {
            so = serializedObject;
            chara = (Character)target;
        }

        public override void OnInspectorGUI()
        {
            so.Update();
            EditorGUI.BeginChangeCheck();

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Character Attributes", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(so.FindProperty(nameof(Character.npcName)));
            }

            EditorGUILayout.Space(5);

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Character Sprites", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(so.FindProperty(nameof(Character.fallback)));

                EditorGUILayout.Space(5);

                for (int i = 0; i < Enum.GetNames(typeof(Character.EmotionState)).Length; i++)
                {
                    EditorGUILayout.PropertyField(so.FindProperty(nameof(Character.emotions)).GetArrayElementAtIndex(i), new GUIContent($"{(Character.EmotionState)i}"));
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                so.ApplyModifiedProperties();
                Repaint();
            }
        }
    }
}
