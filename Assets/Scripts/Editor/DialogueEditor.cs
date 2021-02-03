using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace JVDialogue
{
    [CustomEditor(typeof(Dialogue))]
    public class DialogueEditor : Editor
    {
        SerializedObject so;
        private Dialogue dialogue;

        private List<bool> foldOuts;
        private List<int> tabs;

        private void OnEnable()
        {
            so = serializedObject;
            dialogue = (Dialogue)target;

            if (foldOuts == null)
            {
                foldOuts = new List<bool>();

                for (int i = 0; i < dialogue.Textboxes.Count; i++)
                {
                    foldOuts.Add(true);
                }
            }

            if (tabs == null)
            {
                tabs = new List<int>();

                for (int i = 0; i < dialogue.Textboxes.Count; i++)
                {
                    tabs.Add(0);
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

                        // Visual Preview
                        // TODO

                        // Tabs
                        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                        {
                            tabs[i] = GUILayout.Toolbar(tabs[i], new string[] { "Slot 1", "Slot 2", "Slot 3", "Slot 4", "Slot 5", });

                            // Character specific to open tab
                            EditorGUILayout.PropertyField(soTb.FindProperty(nameof(Textbox.characters)).GetArrayElementAtIndex(tabs[i]), new GUIContent("Character"));

                            if (dialogue.Textboxes[i].characters[tabs[i]] != null)
                            {
                                // This will not display if no character is assigned.
                                EditorGUILayout.PropertyField(soTb.FindProperty(nameof(Textbox.characterEmotes)).GetArrayElementAtIndex(tabs[i]), new GUIContent("Character Emotion"));

                                // Active Character Checkbox
                                bool clicked = EditorGUILayout.Toggle("Active Speaker", dialogue.Textboxes[i].activeCharacter == tabs[i]);

                                if (clicked)
                                {
                                    dialogue.Textboxes[i].activeCharacter = tabs[i];
                                }
                            }
                        }

                        EditorGUILayout.Space(2);

                        // These elements are outside the toolbar scope because they are viewable at all times.
                        EditorGUILayout.PropertyField(soTb.FindProperty(nameof(Textbox.background)), new GUIContent("Background"));
                        EditorGUILayout.PropertyField(soTb.FindProperty(nameof(Textbox.text)), new GUIContent("Textbox Contents"));

                        // Button to remove item from list.
                        if (GUILayout.Button("Remove Textbox " + (i + 1)))
                        {
                            RemoveTextbox(i);
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
            dialogue.Textboxes.Add(newTextbox);
            foldOuts.Add(true);
            tabs.Add(0);

            // Add the Textbox asset to be a child of this Dialogue asset.
            AssetDatabase.AddObjectToAsset(newTextbox, dialogue);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
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
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}
