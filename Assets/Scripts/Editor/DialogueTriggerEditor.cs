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
                        EditorGUILayout.PropertyField(so.FindProperty(nameof(DialogueTrigger.interactionTag)), new GUIContent("Interaction Tag", "Name of the tag used to compare against in collision checks. If something with this tag enters the collider, begin checks for opening dialogue."));
                        EditorGUILayout.PropertyField(so.FindProperty(nameof(DialogueTrigger.triggerOnStart)), new GUIContent("Trigger On Start", "If set to true, the dialogue trigger will start as soon as the scene begins."));
                        EditorGUILayout.PropertyField(so.FindProperty(nameof(DialogueTrigger.triggerOnce)), new GUIContent("Trigger Once", "If set to true, the dialogue trigger will only activate one time (until the scene is reloaded)."));
                        EditorGUILayout.PropertyField(so.FindProperty(nameof(DialogueTrigger.triggerInstantly)), new GUIContent("Trigger Instantly", "If set to true, the dialogue trigger activate as soon as the tagged object enters it."));
                        if (!diaTrig.triggerInstantly)
                        {
                            EditorGUILayout.PropertyField(so.FindProperty(nameof(DialogueTrigger.interactionButton)), new GUIContent("Trigger Interaction Button", "Name of the Unity Input Axis that determines which button triggers the dialogue."));
                        }
                        break;
                    case 1:
                        EditorGUILayout.PropertyField(so.FindProperty(nameof(DialogueTrigger.OnStartDialogue)));
                        EditorGUILayout.PropertyField(so.FindProperty(nameof(DialogueTrigger.OnEndDialogue)));
                        EditorGUILayout.PropertyField(so.FindProperty(nameof(DialogueTrigger.OnEnterTrigger)));
                        EditorGUILayout.PropertyField(so.FindProperty(nameof(DialogueTrigger.OnExitTrigger)));
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
