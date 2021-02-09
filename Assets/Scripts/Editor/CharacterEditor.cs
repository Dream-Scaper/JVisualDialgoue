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

        private int tab = 9999;

        private void OnEnable()
        {
            so = serializedObject;
            chara = (Character)target;
        }

        public override void OnInspectorGUI()
        {
            so.Update();
            EditorGUI.BeginChangeCheck();

            // Character Variables
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Character Attributes", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(so.FindProperty(nameof(Character.npcName)));
            }

            EditorGUILayout.Space(5);

            // Sprite Selection
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Character Sprites", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(so.FindProperty(nameof(Character.fallback)));

                EditorGUILayout.Space(5);

                for (int i = 0; i < Enum.GetNames(typeof(DialogueHelper.EmotionState)).Length; i++)
                {
                    EditorGUILayout.PropertyField(so.FindProperty(nameof(Character.emotions)).GetArrayElementAtIndex(i), new GUIContent($"{(DialogueHelper.EmotionState)i}"));
                }
            }

            EditorGUILayout.Space(5);

            // Preview Selected Sprites
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Sprite Previews", EditorStyles.boldLabel);

                using (new GUILayout.HorizontalScope())
                {
                    // Texture
                    using (new GUILayout.VerticalScope())
                    {
                        Texture2D tex;
                        string emotionName;

                        if (tab == 9999)
                        {
                            tex = AssetPreview.GetAssetPreview(chara.fallback);
                            emotionName = "Fallback";
                        }
                        else
                        {
                            tex = AssetPreview.GetAssetPreview(chara.emotions[tab]);
                            emotionName = $"{(DialogueHelper.EmotionState)tab}";
                        }

                        EditorGUILayout.LabelField(emotionName);
                        GUILayout.Label(tex);
                    }

                    // Buttons
                    using (new GUILayout.VerticalScope())
                    {
                        if (GUILayout.Button("Fallback")) tab = 9999;

                        for (int i = 0; i < Enum.GetNames(typeof(DialogueHelper.EmotionState)).Length; i++)
                        {
                            if (GUILayout.Button($"{(DialogueHelper.EmotionState)i}")) tab = i;
                        }
                    }
                }
            }

            // Sprite Null Check
            bool nullSprite = false;

            if (chara.fallback == null) nullSprite = true;

            for (int i = 0; i < Enum.GetNames(typeof(DialogueHelper.EmotionState)).Length; i++)
            {
                if (chara.emotions[i] == null)
                {
                    nullSprite = true;
                    break;
                }
            }

            if (nullSprite)
            {
                EditorGUILayout.HelpBox("One or more of the Emotion Sprites on this Character are unassigned!\n\nIf an Emotion is unassigned, the Character will default to the Fallback Sprite.\nIf no Fallback is assigned, a Missing Sprite will display in the UI.", MessageType.Warning);
            }

            if (EditorGUI.EndChangeCheck())
            {
                so.ApplyModifiedProperties();
                Repaint();
            }
        }
    }
}
