using System.Collections;
using System.Collections.Generic;
using UnityEditor;


namespace JVDialogue
{
    [CustomEditor(typeof(DialogueManager))]
    public class DialogueEditorManager : Editor
    {
        SerializedObject so;
        DialogueManager dm;

        private void OnEnable()
        {
            so = serializedObject;
            dm = (DialogueManager)target;
        }

        public override void OnInspectorGUI()
        {
            so.Update();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(so.FindProperty(nameof(DialogueManager.dialogueUI)));

            EditorGUILayout.Space(5);

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.PropertyField(so.FindProperty(nameof(DialogueManager.missinputPreventionBuffer)));
                EditorGUILayout.PropertyField(so.FindProperty(nameof(DialogueManager.continueInput)));
                EditorGUILayout.PropertyField(so.FindProperty(nameof(DialogueManager.placeholderDialogue)));
            }

            if (dm.placeholderDialogue == null)
            {
                EditorGUILayout.HelpBox("Placeholder Dialogue is missing!\nIf Placeholder Dialogue is left unassigned, errors will appear on trying to open a Trigger without a valid Dialogue.", MessageType.Warning);
            }

            if (EditorGUI.EndChangeCheck())
            {
                so.ApplyModifiedProperties();
                Repaint();
            }
        }
    }
}
