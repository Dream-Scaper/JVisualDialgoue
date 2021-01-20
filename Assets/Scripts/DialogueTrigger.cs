using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JVDialogue
{
    public class DialogueTrigger : MonoBehaviour
    {
        // Public Variables
        [Header("Basics")]
        public Dialogue dialogueInput;
        public LayerMask interactableLayers = ~0; // Defaults to "Everything"

        [Tooltip("If set to true, the dialogue trigger will start as soon as the scene begins.")]
        public bool beginOnStart = false;

        [Tooltip("If set to true, the dialogue trigger will only activate one time (until the scene is reloaded).")]
        public bool triggerOnce = false;

        [Space(20)]

        [Header("Events")]
        public UnityEvent OnStartDialogue;
        public UnityEvent OnEndDialogue;

        // Private Variables
        private DialogueManager myManager;
        private bool isAlreadyTalking()
        {
            return myManager.isTalking;
        }

        void Start()
        {
            myManager = FindObjectOfType<DialogueManager>();

            if (myManager == null)
            {
                Debug.LogError("Dialogue Manager not found! Are you sure you have an onject with the Dialogue Manager component in the scene?");
                return;
            }

            if (beginOnStart)
            {
                TriggerDialogue();
            }
        }

        public void TriggerDialogue()
        {
            if (!isAlreadyTalking())
            {
                myManager.StartDialogue(this);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == interactableLayers)
            {
                TriggerDialogue();
            }
        }
    }
}
