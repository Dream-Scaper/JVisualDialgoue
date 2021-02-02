using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JVDialogue
{
    public class DialogueManager : MonoBehaviour
    {
        private DialogueTrigger activeTrigger;

        // We dont want other scripts overriding this, but we do want them to be able to see it.
        [HideInInspector]
        public Dialogue ActiveDialogue { get; private set; }

        [Header("Required Game Objects")]
        public DialogueUI dialogueUI;

        [Space(10)]

        [Header("Settings")]
        [Range(0f, 1f)]
        [SerializeField]
        private float missinputPreventionBuffer = 0.5f;
        private float missinputPreventionTimer;

        [SerializeField]
        private string continueInput = "Submit";

        public Dialogue placeholderDialogue;

        // Just like with ActiveDialogue, we need the UI to share this with us but not edit it directly.
        [HideInInspector]
        private int textboxIndex = 0;
        public int TextboxIndex => textboxIndex;

        [HideInInspector]
        public bool isTalking = false;

        void Update()
        {
            if (isTalking)
            {
                missinputPreventionTimer -= Time.deltaTime;

                if (Input.GetButtonDown(continueInput) && missinputPreventionTimer <= 0)
                {
                    ChangeTextbox(1, true);
                }
            }
        }

        public void ChangeTextboxIndex(int increment)
        {
            textboxIndex += increment;

            textboxIndex = Mathf.Clamp(textboxIndex, 0, ActiveDialogue.Textboxes.Count);
        }

        public void StartDialogue(DialogueTrigger dialogue)
        {
            dialogueUI.OpenUI();

            isTalking = true;

            missinputPreventionTimer = missinputPreventionBuffer;
            textboxIndex = 0;

            activeTrigger = dialogue;
            activeTrigger.OnEndDialogue.Invoke();

            ActiveDialogue = activeTrigger.dialogueInput;

            if (ActiveDialogue == null)
            {
                ActiveDialogue = placeholderDialogue;
            }

            ChangeTextbox(0, false);
        }

        public void ChangeTextbox(int increment, bool incrementBefore)
        {
            if (textboxIndex >= ActiveDialogue.Textboxes.Count - 1 && dialogueUI.lineFinished && increment > 0)
            {
                EndDialogue();
                return;
            }
            else if (textboxIndex < 0)
            {
                return;
            }

            dialogueUI.DisplayTextbox(increment, incrementBefore, dialogueUI.lineFinished);
        }

        public void EndDialogue()
        {
            dialogueUI.CloseUI();
            isTalking = false;

            activeTrigger.OnEndDialogue.Invoke();
        }
    }
}
