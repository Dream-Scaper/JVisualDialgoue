using UnityEngine;
using UnityEditor;

namespace JVDialogue
{
    [CustomEditor(typeof(DialogueUI))]
    public class DialogueUIEditor : Editor
    {
        SerializedObject so;
        private DialogueUI diaUI;

        private bool showScrollSettings = true;
        private bool foldoutDebug = false;
        private int activeTab = 0;

        private void OnEnable()
        {
            so = serializedObject;
            diaUI = (DialogueUI)target;
        }

        public override void OnInspectorGUI()
        {
            so.Update();
            EditorGUI.BeginChangeCheck();

            // Using FindProperty looks a little more obtuse, but in the case of changing names
            // of variables this prevents the Editor UI from breaking- where it'll instead throw
            // a more readabile compiler error OR if using the proper rename function on a variable,
            // it just gets renamed here too.
            EditorGUILayout.PropertyField(so.FindProperty(nameof(DialogueUI.animator)));
            EditorGUILayout.PropertyField(so.FindProperty(nameof(DialogueUI.myManager)));

            GUILayout.Space(5);

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                activeTab = GUILayout.Toolbar(activeTab, new string[] { "UI Elements", "Text Settings", "Image Settings" });

                GUILayout.Space(3);

                switch (activeTab)
                {
                    default:
                    case 0:
                        EditorGUILayout.PropertyField(so.FindProperty(nameof(DialogueUI.nameText)));
                        EditorGUILayout.PropertyField(so.FindProperty(nameof(DialogueUI.speechText)));

                        EditorGUILayout.PropertyField(so.FindProperty(nameof(DialogueUI.nextTextboxButton)));
                        EditorGUILayout.PropertyField(so.FindProperty(nameof(DialogueUI.lastTextboxButton)));

                        EditorGUILayout.PropertyField(so.FindProperty(nameof(DialogueUI.endLineIndicator)));

                        EditorGUILayout.PropertyField(so.FindProperty(nameof(DialogueUI.background)));
                        EditorGUILayout.PropertyField(so.FindProperty(nameof(DialogueUI.characterProfiles)));
                        break;

                    case 1:
                        EditorGUILayout.PropertyField(so.FindProperty(nameof(DialogueUI.placeholderName)));

                        EditorGUILayout.PropertyField(so.FindProperty(nameof(DialogueUI.scrollTextOverride)));
                        showScrollSettings = ((DialogueUI)target).scrollTextOverride;

                        if (showScrollSettings)
                        {
                            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                            {
                                EditorGUILayout.PropertyField(so.FindProperty(nameof(DialogueUI.textScrollSpeed)));
                                EditorGUILayout.PropertyField(so.FindProperty(nameof(DialogueUI.charactersUntilNewline)));
                            }
                        }
                        break;

                    case 2:
                        EditorGUILayout.PropertyField(so.FindProperty(nameof(DialogueUI.displayBackground)));
                        EditorGUILayout.PropertyField(so.FindProperty(nameof(DialogueUI.speakerColor)));
                        EditorGUILayout.PropertyField(so.FindProperty(nameof(DialogueUI.inactiveColor)));
                        break;
                }
            }

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                foldoutDebug = EditorGUILayout.Foldout(foldoutDebug, "Debug");

                if (foldoutDebug)
                {
                    EditorGUILayout.LabelField($"UI Up?: {diaUI.UiUp}");
                    EditorGUILayout.LabelField($"Line Finished?: {diaUI.lineFinished}");
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
