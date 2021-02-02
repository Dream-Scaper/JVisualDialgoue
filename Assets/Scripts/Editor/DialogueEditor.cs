using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace JVDialogue
{
    [CustomEditor(typeof(Dialogue))]
    public class DialogueEditor : Editor
    {
        private Dialogue dialogue;
        private List<bool> foldOuts;
        private List<int> tabs;

        private void OnEnable()
        {
            dialogue = (Dialogue)target;

            if (foldOuts == null)
            {
                foldOuts = new List<bool>();

                foreach (var tb in dialogue.Textboxes)
                {
                    foldOuts.Add(true);
                }
            }

            if (tabs == null)
            {
                tabs = new List<int>();

                foreach (var tb in dialogue.Textboxes)
                {
                    tabs.Add(0);
                }
            }
        }

        public override void OnInspectorGUI()
        {
            for (int i = 0; i < dialogue.Textboxes.Count; i++)
            {
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    foldOuts[i] = EditorGUILayout.Foldout(foldOuts[i], "Textbox " + (i + 1), true);
                    if (foldOuts[i])
                    {
                        SerializedObject soTb = new SerializedObject(dialogue.Textboxes[i]);

                        // Tabs
                        tabs[i] = GUILayout.Toolbar(tabs[i], new string[] { "Slot 1", "Slot 2", "Slot 3", "Slot 4", "Slot 5", });

                        for (int j = 0; j < dialogue.Textboxes[i].characters.Length; j++)
                        {


                        }

                        // Tabs End
                        EditorGUILayout.PropertyField(soTb.FindProperty(nameof(Textbox.text)));

                        if (GUILayout.Button("Remove Dialogue Line " + i))
                        {
                            RemoveTextbox(i);
                        }
                    }
                }

                GUILayout.Space(5);
            }

            if (GUILayout.Button("Add Textbox"))
            {
                AddTextbox();
            }

            if (GUILayout.Button("Remove All Textboxes"))
            {
                RemoveAllTextboxes();
            }
        }

        private void AddTextbox()
        {
            // Add a textbox with some default values.
            Textbox newTextbox = ScriptableObject.CreateInstance<Textbox>();
            newTextbox.name = (dialogue.Textboxes.Count + 1).ToString();
            newTextbox.characters = new Character[5];
            newTextbox.characterEmotes = new Character.EmotionState[5];
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
            // Remove the textbox from the dialogue, then refresh the database.
            AssetDatabase.RemoveObjectFromAsset(dialogue.Textboxes[index]);
            dialogue.Textboxes.RemoveAt(index);
            foldOuts.RemoveAt(index);
            tabs.RemoveAt(index);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void RemoveAllTextboxes()
        {
            // Movebackwards through the list...
            for (int i = dialogue.Textboxes.Count - 1; i >= 0 ; i--)
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
