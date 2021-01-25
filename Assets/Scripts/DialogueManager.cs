using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JVDialogue
{
    public class DialogueManager : MonoBehaviour
    {
        private DialogueTrigger activeTrigger;
        private Dialogue input;

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

        [HideInInspector]
        private int textboxIndex = 0;

        [HideInInspector]
        public bool isTalking = false;

        void Update()
        {
            if (isTalking)
            {
                missinputPreventionTimer -= Time.deltaTime;

                if (Input.GetButtonDown(continueInput) && missinputPreventionTimer <= 0)
                {
                    NextTextbox();
                }
            }
        }

        public void IncrementTextboxIndex(int increment)
        {
            textboxIndex += increment;
        }

        public void StartDialogue(DialogueTrigger dialogue)
        {
            dialogueUI.OpenUI();

            isTalking = true;

            missinputPreventionTimer = missinputPreventionBuffer;
            textboxIndex = 0;

            activeTrigger = dialogue;
            input = activeTrigger.dialogueInput;

            if (input == null)
            {
                input = placeholderDialogue;
            }

            NextTextbox();
        }

        public void NextTextbox()
        {
            if (textboxIndex >= input.Textboxes.Count && dialogueUI.lineFinished)
            {
                EndDialogue();
                return;
            }

            Debug.Log("Current index: " + textboxIndex);
            dialogueUI.DisplayTextbox(input.Textboxes[textboxIndex], 1, dialogueUI.lineFinished);
        }

        public void LastTextbox()
        {
            if (textboxIndex < 0)
            {
                textboxIndex = 0;
                return;
            }

            Debug.Log("Current index: " + textboxIndex);
            dialogueUI.DisplayTextbox(input.Textboxes[textboxIndex], -1, dialogueUI.lineFinished);
        }

        public void EndDialogue()
        {
            dialogueUI.CloseUI();
            isTalking = false;
        }
    }
}
