using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
//using TMPro;

namespace JVDialogue
{
    [RequireComponent(typeof(Animator))]
    public class DialogueUI : MonoBehaviour
    {
        private struct AnimatorVariables
        {
            public const string dialogueUp = "DialogueUp";
        }

        public DialogueManager myManager;
        public Animator animator;

        // UI Elements
        public Text nameText;
        public Text speechText;
        //public TextMeshProUGUI nameText;
        //public TextMeshProUGUI speechText;
        public Image background;
        public Image[] characterProfiles;
        public Button nextTextboxButton;
        public Button lastTextboxButton;
        public GameObject endLineIndicator;
        public bool UiUp = false; // Hidden

        // Text Settings
        public string placeholderName = "????";
        public bool scrollTextOverride = true;
        public float textScrollSpeed = 0.025f;
        public int charactersUntilNewline = 100;
        public bool lineFinished = true; // Hidden
        private int scrollIndex = 0;
        private Coroutine currentScroll;

        // Image Settings
        public Color speakerColor = Color.white;
        public Color inactiveColor = Color.gray;
        public bool displayBackground = true;

        void Start()
        {
            endLineIndicator.SetActive(false);

            if (nextTextboxButton != null)
            {
                nextTextboxButton.onClick.RemoveAllListeners();
                nextTextboxButton.onClick.AddListener(delegate { myManager.ChangeTextbox(1, scrollTextOverride); });
            }

            if (lastTextboxButton != null)
            {
                lastTextboxButton.onClick.RemoveAllListeners();
                lastTextboxButton.onClick.AddListener(delegate { myManager.ChangeTextbox(-1, scrollTextOverride); });
            }

            if (characterProfiles.Length != DialogueHelper.profileNumber)
            {
                Debug.LogWarning("The number of characterProfile images doesn't match the amount of profile sprites defined in DialogueHelpers.\nSome sprites may not be shown in the UI!");
            }
        }

        public void OpenUI()
        {
            animator.SetBool(AnimatorVariables.dialogueUp, true);
        }

        public void DisplayTextbox(int incrementIndex, bool incrementBefore, bool scrollText)
        {
            // Set the text to be blank, and the end-text indicator to off.
            speechText.text = "";
            endLineIndicator.SetActive(false);

            // If we need to increment before begining, do so here.
            if (incrementBefore && lineFinished) myManager.ChangeTextboxIndex(incrementIndex);

            // Grab the active textbox, aand update the UI buttons based on our index.
            Textbox textbox = myManager.ActiveDialogue.Textboxes[myManager.TextboxIndex];
            UpdateLastNextButtons(myManager.TextboxIndex);

            // Set the background.
            if (displayBackground)
            {
                background.sprite = textbox.background;
                background.color = textbox.backgroundColor;
            }
            else
            {
                background.sprite = null;
                background.color = Color.clear;
            }

            // Set the active NPC speaking name.
            nameText.text = textbox.characters[textbox.activeCharacter] != null ? textbox.characters[textbox.activeCharacter].name : placeholderName;

            // Set the character profile sprites.
            for (int i = 0; i < Mathf.Min(characterProfiles.Length, textbox.characters.Length); i++)
            {
                characterProfiles[i].color = i == textbox.activeCharacter ? speakerColor : inactiveColor;

                Vector3 imageEulers = characterProfiles[i].gameObject.transform.eulerAngles;
                characterProfiles[i].gameObject.transform.rotation = Quaternion.Euler(imageEulers.x, textbox.flipY[i] ? 180f : 0f, imageEulers.z);

                // Check if there is a character in this slot at all.
                if (textbox.characters[i] != null)
                {
                    Sprite emotion = textbox.characters[i].emotions[(int)textbox.characterEmotes[i]];

                    // Check for the emotion not being null.
                    if (emotion != null)
                    {
                        characterProfiles[i].sprite = emotion;
                    }
                    else
                    {
                        // If its null go to fallback.
                        Sprite fallback = textbox.characters[i].fallback;

                        // If fallback is null put a warning in console.
                        if (fallback != null)
                        {
                            characterProfiles[i].sprite = fallback;
                            Debug.LogWarning($"Character in slot {i} ({textbox.characters[i].npcName}) is missing an Emotion! They have defaulted to the Fallback sprite.");
                        }
                        else
                        {
                            characterProfiles[i].sprite = null;
                            characterProfiles[i].color = Color.clear;

                            if (DialogueHelper.missingEmotionProfileWarnings)
                            {
                                Debug.LogWarning($"Character in slot {i} ({textbox.characters[i].npcName}) is missing an Emotion and Fallback sprite!\nPlease go to the scriptable object and assign it there.");
                            }
                        }
                    }
                }
                else
                {
                    // If theres no character, assume a blank space.
                    characterProfiles[i].sprite = null;
                    characterProfiles[i].color = Color.clear;
                }
            }

            // Scroll (or don't) the text in the text box!
            if (scrollText && scrollTextOverride)
            {
                currentScroll = StartCoroutine(ScrollText(textbox.text, incrementIndex, !incrementBefore));
            }
            else
            {
                StopCoroutine(currentScroll);

                speechText.text = textbox.text;
                lineFinished = true;
                endLineIndicator.SetActive(true);

                if (!incrementBefore) myManager.ChangeTextboxIndex(incrementIndex);
            }
        }

        public void CloseUI()
        {
            animator.SetBool(AnimatorVariables.dialogueUp, false);
        }

        private void UpdateLastNextButtons(int idx)
        {
            if (lastTextboxButton != null)
            {
                lastTextboxButton.gameObject.SetActive(!(idx == 0));
            }
        }

        private IEnumerator ScrollText(string intputText, int incrementIndex, bool increment)
        {
            scrollIndex = 0;
            lineFinished = false;

            // Wait until the animation has set the UI to being fully up.
            yield return new WaitUntil(() => UiUp);

            int newLineIndex = 0;

            // While the UI text's length is less than the input text's length...
            while (speechText.text.Length < intputText.Length + newLineIndex)
            {
                if (scrollIndex > intputText.Length + newLineIndex)
                {
                    break;
                }

                // If it's around time to newline, do so as long as the character up next is a space.
                if (scrollIndex >= charactersUntilNewline * (newLineIndex + 1) && intputText[scrollIndex] == ' ')
                {
                    speechText.text += Environment.NewLine;
                    scrollIndex++;
                    newLineIndex++;
                }

                // Add the single character to the UI text, and wait.
                speechText.text += intputText[scrollIndex];
                scrollIndex++;
                yield return new WaitForSeconds(textScrollSpeed);
            }

            // Exit of the loop, line is fully displayed!
            lineFinished = true;
            endLineIndicator.SetActive(true);

            if (increment) myManager.ChangeTextboxIndex(incrementIndex);
        }
    }
}
