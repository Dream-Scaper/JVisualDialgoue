using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace JVDialogue
{
    [CustomEditor(typeof(Dialogue))]
    public class DialogueEditor : Editor
    {
        private Dialogue dialogue;

        private void OnEnable()
        {
            dialogue = (Dialogue)target;
        }

        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();

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

            // Add the Textbox asset to be a child of this Dialogue asset.
            AssetDatabase.AddObjectToAsset(newTextbox, dialogue);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void RemoveTextbox(Textbox textbox)
        {

        }

        private void RemoveAllTextboxes()
        {
            // Movebackwards through the list...
            for (int i = dialogue.Textboxes.Count - 1; i >= 0 ; i--)
            {
                // And remove the Textbox asset from the Dialogue asset, as well as remove it from the list.
                AssetDatabase.RemoveObjectFromAsset(dialogue.Textboxes[i]);
                dialogue.Textboxes.RemoveAt(i);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
