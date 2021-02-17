using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

namespace JVDialogue
{
    [CustomEditor(typeof(Dialogue))]
    public class DialogueEditor : Editor
    {
        SerializedObject so;
        private Dialogue dialogue;

        private List<bool> foldOuts;
        private List<int> tabs;
        private List<AnimBool> additionalOptions;

        private void OnEnable()
        {
            so = serializedObject;
            dialogue = (Dialogue)target;

            // Create Lists for inspector variables.
            foldOuts = new List<bool>();
            for (int i = 0; i < dialogue.Textboxes.Count; i++)
            {
                foldOuts.Add(true);
            }

            tabs = new List<int>();
            for (int i = 0; i < dialogue.Textboxes.Count; i++)
            {
                tabs.Add(0);
            }

            additionalOptions = new List<AnimBool>();
            for (int i = 0; i < dialogue.Textboxes.Count; i++)
            {
                AnimBool temp = new AnimBool(dialogue.Textboxes[i].characters[0] != null);
                temp.valueChanged.AddListener(Repaint);
                additionalOptions.Add(temp);
            }

            // Check to make sure the character profile amount matches the settings.
            for (int i = 0; i < dialogue.Textboxes.Count; i++)
            {
                if (dialogue.Textboxes[i].characters.Length != DialogueHelper.profileNumber)
                {
                    Character[] tempChara = dialogue.Textboxes[i].characters;
                    DialogueHelper.EmotionState[] tempEmotion = dialogue.Textboxes[i].characterEmotes;

                    dialogue.Textboxes[i].characters = new Character[DialogueHelper.profileNumber];
                    dialogue.Textboxes[i].characterEmotes = new DialogueHelper.EmotionState[DialogueHelper.profileNumber];

                    for (int j = 0; j < Mathf.Min(tempChara.Length, DialogueHelper.profileNumber); j++)
                    {
                        dialogue.Textboxes[i].characters[j] = tempChara[j];
                        dialogue.Textboxes[i].characterEmotes[j] = tempEmotion[j];
                    }
                }
            }
        }

        public override void OnInspectorGUI()
        {
            so.Update();
            EditorGUI.BeginChangeCheck();

            for (int i = 0; i < dialogue.Textboxes.Count; i++)
            {
                // previewString is a sort snippet of the textbox text used for at-a-glance viewing of the textboxes
                // even when the foldout is set to be closed. This is so it's easier to keep track of your textbox order.
                string previewString = "";
                if (dialogue.Textboxes[i].text != null) previewString = dialogue.Textboxes[i].text.Substring(0, Mathf.Min(30, dialogue.Textboxes[i].text.Length));

                foldOuts[i] = EditorGUILayout.BeginFoldoutHeaderGroup(foldOuts[i], $"Textbox {i + 1}: {previewString}...");
                if (foldOuts[i])
                {
                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        // Serialized Object from specific Textbox.
                        SerializedObject soTb = new SerializedObject(dialogue.Textboxes[i]);
                        soTb.Update();
                        EditorGUI.BeginChangeCheck();

                        // Tabs
                        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                        {
                            string[] tabsLabels = new string[DialogueHelper.profileNumber];
                            for (int j = 0; j < tabsLabels.Length; j++)
                            {
                                tabsLabels[j] = $"Slot {j + 1}";
                            }

                            tabs[i] = GUILayout.Toolbar(tabs[i], tabsLabels);

                            // Character specific to open tab
                            EditorGUILayout.PropertyField(soTb.FindProperty(nameof(Textbox.characters)).GetArrayElementAtIndex(tabs[i]), new GUIContent("Character"));

                            additionalOptions[i].target = (dialogue.Textboxes[i].characters[tabs[i]] != null);

                            // This will not display if no character is assigned.
                            if (EditorGUILayout.BeginFadeGroup(additionalOptions[i].faded))
                            {
                                EditorGUILayout.PropertyField(soTb.FindProperty(nameof(Textbox.characterEmotes)).GetArrayElementAtIndex(tabs[i]), new GUIContent("Character Emotion"));

                                EditorGUILayout.PropertyField(soTb.FindProperty(nameof(Textbox.flipY)).GetArrayElementAtIndex(tabs[i]), new GUIContent("Flip Y?"));

                                // Active Character Checkbox
                                bool clicked = EditorGUILayout.Toggle("Active Speaker", dialogue.Textboxes[i].activeCharacter == tabs[i]);

                                if (clicked)
                                {
                                    dialogue.Textboxes[i].activeCharacter = tabs[i];
                                }
                            }
                            EditorGUILayout.EndFadeGroup();
                        }

                        EditorGUILayout.Space(2);

                        // These elements are outside the toolbar scope because they are viewable at all times.
                        EditorGUILayout.PropertyField(soTb.FindProperty(nameof(Textbox.background)), new GUIContent("Background"));
                        EditorGUILayout.PropertyField(soTb.FindProperty(nameof(Textbox.backgroundColor)), new GUIContent("Background Color"));

                        EditorGUILayout.PropertyField(soTb.FindProperty(nameof(Textbox.text)), new GUIContent("Textbox Contents"));

                        // BUTTONS -----------------------------------------------------------------------------------------------------------------------
                        using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                        {
                            using (new EditorGUILayout.VerticalScope())
                            {
                                // Button to clear all text from the textbox.
                                if (GUILayout.Button("Clear Textbox " + (i + 1)))
                                {
                                    ClearTextbox(i);
                                }

                                // Button to remove item from list.
                                if (GUILayout.Button("Remove Textbox " + (i + 1)))
                                {
                                    RemoveTextbox(i);
                                }
                            }

                            using (new EditorGUILayout.VerticalScope())
                            {
                                EditorGUI.BeginDisabledGroup(!(i > 0));
                                if (GUILayout.Button("Move Textbox Up"))
                                {
                                    SwapTextboxes(i, i - 1);
                                }
                                EditorGUI.EndDisabledGroup();

                                EditorGUI.BeginDisabledGroup(!(i < dialogue.Textboxes.Count - 1));
                                if (GUILayout.Button("Move Textbox Down"))
                                {
                                    SwapTextboxes(i, i + 1);
                                }
                                EditorGUI.EndDisabledGroup();
                            }
                        }

                        // Apply Changes
                        if (EditorGUI.EndChangeCheck())
                        {
                            soTb.ApplyModifiedProperties();
                            Repaint();
                        }
                    }
                }
                EditorGUILayout.EndFoldoutHeaderGroup();

                GUILayout.Space(10);
            }

            if (GUILayout.Button("Add Textbox"))
            {
                AddTextbox();
            }

            if (GUILayout.Button("Remove All Textboxes"))
            {
                RemoveAllTextboxes();
            }

            if (EditorGUI.EndChangeCheck())
            {
                so.ApplyModifiedProperties();
                Repaint();
            }
        }

        private void AddTextbox()
        {
            // Add a textbox with some default values.
            Textbox newTextbox = ScriptableObject.CreateInstance<Textbox>();
            newTextbox.name = (dialogue.Textboxes.Count + 1).ToString();
            newTextbox.characters = new Character[DialogueHelper.profileNumber];
            newTextbox.characterEmotes = new DialogueHelper.EmotionState[DialogueHelper.profileNumber];
            newTextbox.flipY = new bool[DialogueHelper.profileNumber];
            newTextbox.backgroundColor = Color.white;

            // If this bool is set in the helpers, then we copy over the values from the last entry.
            if (DialogueHelper.populateByDuplicating)
            {
                // Check if we have a previous Entry to try and copy.
                if (dialogue.Textboxes.Count > 0)
                {
                    newTextbox.activeCharacter = dialogue.Textboxes[dialogue.Textboxes.Count - 1].activeCharacter;
                    newTextbox.background = dialogue.Textboxes[dialogue.Textboxes.Count - 1].background;
                    newTextbox.backgroundColor = dialogue.Textboxes[dialogue.Textboxes.Count - 1].backgroundColor;

                    for (int i = 0; i < DialogueHelper.profileNumber; i++)
                    {
                        newTextbox.characters[i] = dialogue.Textboxes[dialogue.Textboxes.Count - 1].characters[i];
                        newTextbox.characterEmotes[i] = dialogue.Textboxes[dialogue.Textboxes.Count - 1].characterEmotes[i];
                        newTextbox.flipY[i] = dialogue.Textboxes[dialogue.Textboxes.Count - 1].flipY[i];
                    }
                }
            }

            dialogue.Textboxes.Add(newTextbox);
            foldOuts.Add(true);
            tabs.Add(0);
            AnimBool temp = new AnimBool(false);
            temp.valueChanged.AddListener(Repaint);
            additionalOptions.Add(temp);

            // Add the Textbox asset to be a child of this Dialogue asset.
            AssetDatabase.AddObjectToAsset(newTextbox, dialogue);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void SwapTextboxes(int a, int b)
        {
            Textbox temp = dialogue.Textboxes[a];

            dialogue.Textboxes[a] = dialogue.Textboxes[b];
            dialogue.Textboxes[b] = temp;
        }

        private void ClearTextbox(int index)
        {
            if (EditorUtility.DisplayDialog($"Clear Textbox {index + 1}", $"Are you sure you want to clear all fields in Textbox ({index + 1}) in the Dialogue ({dialogue.name})?\nThis action is not reverseable.", "Clear", "Cancel"))
            {
                dialogue.Textboxes[index].text = "";
                dialogue.Textboxes[index].background = null;
                dialogue.Textboxes[index].activeCharacter = 0;

                for (int i = 0; i < dialogue.Textboxes[index].characters.Length; i++)
                {
                    dialogue.Textboxes[index].characters[i] = null;
                    dialogue.Textboxes[index].characterEmotes[i] = DialogueHelper.EmotionState.Neutral;
                    dialogue.Textboxes[index].flipY[i] = false;
                }
            }
        }

        private void RemoveTextbox(int index)
        {
            if (EditorUtility.DisplayDialog($"Remove Textbox {index + 1}", $"Are you sure you want to remove Textbox ({index + 1}) from the Dialogue ({dialogue.name})?\nThis action is not reverseable.", "Remove", "Cancel"))
            {
                // Remove the textbox from the dialogue, then refresh the database.
                AssetDatabase.RemoveObjectFromAsset(dialogue.Textboxes[index]);
                dialogue.Textboxes.RemoveAt(index);
                foldOuts.RemoveAt(index);
                tabs.RemoveAt(index);
                additionalOptions.RemoveAt(index);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        private void RemoveAllTextboxes()
        {
            if (EditorUtility.DisplayDialog("Remove All Textboxes", $"Are you sure you want to remove ALL Textboxes from the Dialogue ({dialogue.name})?\nThis action is not reverseable.", "Remove All", "Cancel"))
            {
                // Movebackwards through the list...
                for (int i = dialogue.Textboxes.Count - 1; i >= 0; i--)
                {
                    // And remove the Textbox asset from the Dialogue asset, as well as remove it from the list.
                    AssetDatabase.RemoveObjectFromAsset(dialogue.Textboxes[i]);
                    dialogue.Textboxes.RemoveAt(i);
                    foldOuts.RemoveAt(i);
                    tabs.RemoveAt(i);
                    additionalOptions.RemoveAt(i);
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}
