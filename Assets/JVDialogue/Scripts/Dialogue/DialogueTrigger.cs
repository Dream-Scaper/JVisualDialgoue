using UnityEngine;
using UnityEngine.Events;

namespace JVDialogue
{
    [RequireComponent(typeof(Collider))]
    public class DialogueTrigger : MonoBehaviour
    {
        // Public Variables
        public Dialogue dialogueInput;
        public string interactionTag = "Player";

        public bool triggerOnStart = false;

        public bool triggerOnce = false;

        public bool triggerInstantly = false;
        public string interactionButton = "Submit";
        
        private bool awaitingInput = false;
        private float missinputPreventionTimer;

        // Events
        public UnityEvent OnStartDialogue;
        public UnityEvent OnEndDialogue;
        public UnityEvent OnEnterTrigger;
        public UnityEvent OnExitTrigger;

        // Private Variables
        private DialogueManager myManager;
        private bool IsAlreadyTalking()
        {
            return myManager.isTalking;
        }

        void Start()
        {
            myManager = FindObjectOfType<DialogueManager>();

            if (myManager == null)
            {
                Debug.LogError("Dialogue Manager not found! Are you sure you have an object with the Dialogue Manager component in the scene?");
                return;
            }

            if (triggerOnStart)
            {
                TriggerDialogue();
            }
        }

        private void Update()
        {
            if (missinputPreventionTimer > 0)
            {
                missinputPreventionTimer -= Time.deltaTime;
                return;
            }

            if (awaitingInput)
            {
                if (Input.GetButtonDown(interactionButton))
                {
                    TriggerDialogue();
                }
            }
        }

        public void TriggerDialogue()
        {
            if (!IsAlreadyTalking())
            {
                myManager.StartDialogue(this);
                OnStartDialogue.Invoke();
            }
        }

        public void EndDialogue(float missInputPrevent)
        {
            missinputPreventionTimer = missInputPrevent;

            OnEndDialogue.Invoke();

            if (triggerOnce)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(interactionTag) && triggerInstantly)
            {
                TriggerDialogue();
            }

            OnEnterTrigger.Invoke();
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.CompareTag(interactionTag) && !triggerInstantly)
            {
                awaitingInput = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag(interactionTag) && !triggerInstantly)
            {
                awaitingInput = false;
            }

            OnExitTrigger.Invoke();
        }
    }
}
