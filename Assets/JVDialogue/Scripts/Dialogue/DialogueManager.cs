using UnityEngine;

namespace JVDialogue
{
    public class DialogueManager : MonoBehaviour
    {
        private DialogueTrigger activeTrigger;

        public Dialogue ActiveDialogue { get; private set; }

        public DialogueUI dialogueUI;

        [Range(0f, 1f)]
        public float missinputPreventionBuffer = 0.5f;
        private float missinputPreventionTimer;

        public string continueInput = "Submit";

        public Dialogue placeholderDialogue;

        public int TextboxIndex { get; private set; } = 0;

        [HideInInspector]
        public bool isTalking = false;

        void Update()
        {
            if (isTalking)
            {
                // Timer here prevents the input that opened the dialogue from possibly skpping the text.
                missinputPreventionTimer -= Time.deltaTime;

                if (Input.GetButtonDown(continueInput) && missinputPreventionTimer <= 0)
                {
                    ChangeTextbox(1, true);
                }
            }
        }

        public void ChangeTextboxIndex(int increment)
        {
            TextboxIndex += increment;

            TextboxIndex = Mathf.Clamp(TextboxIndex, 0, ActiveDialogue.Textboxes.Count);
        }

        public void StartDialogue(DialogueTrigger dialogue)
        {
            dialogueUI.OpenUI();

            isTalking = true;

            missinputPreventionTimer = missinputPreventionBuffer;
            TextboxIndex = 0;

            // Set the active Trigger and respective Dialogue. If there is no dialogue, use the placeholder.
            activeTrigger = dialogue;
            ActiveDialogue = activeTrigger.dialogueInput;

            if (ActiveDialogue == null) ActiveDialogue = placeholderDialogue;

            ChangeTextbox(0, false);
        }

        public void ChangeTextbox(int increment, bool incrementBefore)
        {
            if (ActiveDialogue != null)
            {
                // If we hit "next" and are about to go out of the upper bounds, that must mean we've hit the end, so call EndDialogue() and return early.
                if (TextboxIndex >= ActiveDialogue.Textboxes.Count - 1 && dialogueUI.lineFinished && increment > 0)
                {
                    EndDialogue();
                    return;
                }
                else if (TextboxIndex < 0)
                {
                    // And if we're less than 0, we can only go up from here, so if we try to go down we return.
                    return;
                }

                dialogueUI.DisplayTextbox(increment, incrementBefore, dialogueUI.lineFinished);
            }
        }

        public void EndDialogue()
        {
            if (ActiveDialogue != null)
            {
                dialogueUI.CloseUI();
                isTalking = false;

                activeTrigger.EndDialogue(missinputPreventionBuffer);

                activeTrigger = null;
                ActiveDialogue = null;
            }
        }
    }
}
