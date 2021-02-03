using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace JVDialogue
{
    [CustomEditor(typeof(DialogueTrigger))]
    public class DialogueTriggerEditor : Editor
    {
        SerializedObject so;
        private DialogueTrigger diaTrig;

        private int activeTab = 0;

        private void OnEnable()
        {
            so = serializedObject;
            diaTrig = (DialogueTrigger)target;
        }

        public override void OnInspectorGUI()
        {
            so.Update();
            EditorGUI.BeginChangeCheck();

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                activeTab = GUILayout.Toolbar(activeTab, new string[] { "Basics", "Events" });

                switch (activeTab)
                {
                    default:
                    case 0:
                        EditorGUILayout.PropertyField(so.FindProperty(nameof(DialogueTrigger.dialogueInput)));
                        EditorGUILayout.PropertyField(so.FindProperty(nameof(DialogueTrigger.interactableLayers)));
                        EditorGUILayout.PropertyField(so.FindProperty(nameof(DialogueTrigger.beginOnStart)));
                        EditorGUILayout.PropertyField(so.FindProperty(nameof(DialogueTrigger.triggerOnce)));
                        break;
                    case 1:
                        EditorGUILayout.PropertyField(so.FindProperty(nameof(DialogueTrigger.OnStartDialogue)));
                        EditorGUILayout.PropertyField(so.FindProperty(nameof(DialogueTrigger.OnEndDialogue)));
                        break;
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
