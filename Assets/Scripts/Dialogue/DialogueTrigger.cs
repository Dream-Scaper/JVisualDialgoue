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

        [Tooltip("If set to true, the dialogue trigger will start as soon as the scene begins.")]
        public bool beginOnStart = false;

        [Tooltip("If set to true, the dialogue trigger will only activate one time (until the scene is reloaded).")]
        public bool triggerOnce = false;

        [Tooltip("If set to true, the dialogue trigger activate as soon as the tagged object enters it.")]
        public bool triggerInstantly = false;
        public string interactionButton = "Submit";
        private bool awaitingInput = false;

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
                Debug.LogError("Dialogue Manager not found! Are you sure you have an onject with the Dialogue Manager component in the scene?");
                return;
            }

            if (beginOnStart)
            {
                TriggerDialogue();
            }
        }

        private void Update()
        {
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

        public void EndDialogue()
        {
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

        public void TestFunc()
        {
            Debug.Log($"my name is {gameObject.name}");
        }
    }
}
