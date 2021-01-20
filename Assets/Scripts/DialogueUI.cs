using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace JVDialogue
{
    [RequireComponent(typeof(Animator))]
    public class DialogueUI : MonoBehaviour
    {
        private struct AnimatorVariables
        {
            public const string dialogueUp = "DialogueUp";
        }

        [Header("Required Game Objects")]
        public DialogueManager myManager;

        [Header("Components")]
        public Animator animator;

        [Header("UI Elements")]
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI speechText;

        public Image background;
        public Image[] characterProfiles;

        public Button nextTextboxButton;
        public Button lastTextboxButton;

        public GameObject endTextIndicator;

        [Header("Text Settings")]
        public float textScrollSpeed = 0.025f;
        public int charactersUntilNewline = 100;
        public string placeholderName = "????";

        [Header("Image Settings")]
        public Color speakerColor = Color.white;
        public Color inactiveColor = Color.gray;

        [HideInInspector]
        public bool lineFinished = false;
        private int scrollIndex = 0;

        [HideInInspector]
        public bool UiUp = false;

        void Start()
        {
            endTextIndicator.SetActive(false);

            if (nextTextboxButton != null)
            {
                nextTextboxButton.onClick.RemoveAllListeners();
                nextTextboxButton.onClick.AddListener(delegate { myManager.NextTextbox(); });
            }

            if (lastTextboxButton != null)
            {
                lastTextboxButton.onClick.RemoveAllListeners();
                lastTextboxButton.onClick.AddListener(delegate { myManager.LastTextbox(); });
            }
        }

        public void OpenUI()
        {
            animator.SetBool(AnimatorVariables.dialogueUp, true);
        }

        public void DisplayTextbox(Textbox textbox, int incrementIndex, bool scrollText)
        {
            //speechText.text = "";
            speechText.text = textbox.text;

            Debug.Log("Displaying: " + textbox.text);

            endTextIndicator.SetActive(false);

            background.sprite = textbox.background;

            if (textbox.characters[textbox.activeCharacter] != null)
            {
                nameText.text = textbox.characters[textbox.activeCharacter].name;
            }
            else
            {
                nameText.text = placeholderName;
            }

            for (int i = 0; i < characterProfiles.Length; i++)
            {
                characterProfiles[i].color = i == textbox.activeCharacter ? speakerColor : inactiveColor;
                
                if (textbox.characters[i] != null)
                {
                    characterProfiles[i].sprite = textbox.characters[i].emotions[(int)textbox.characterEmotes[i]];
                }
                else
                {
                    characterProfiles[i].sprite = null;
                    characterProfiles[i].color = Color.clear;
                }
            }

            endTextIndicator.SetActive(true);

            lineFinished = true;
            myManager.IncrementTextboxIndex(incrementIndex);

            // TODO: fix this
            //if (scrollText)
            //{
            //    StartCoroutine(ScrollText(textbox.text, incrementIndex));
            //}
            //else
            //{
            //    StopCoroutine(ScrollText(textbox.text, incrementIndex));
            //    endTextIndicator.SetActive(true);

            //    lineFinished = true;
            //    myManager.IncrementTextboxIndex(incrementIndex);
            //}
        }

        public void CloseUI()
        {
            animator.SetBool(AnimatorVariables.dialogueUp, false);
        }

        private IEnumerator ScrollText(string text, int incrementIndex)
        {
            scrollIndex = 0;
            lineFinished = false;

            yield return new WaitUntil(() => UiUp);

            int newLineIndex = 0;

            while (speechText.text.Length < text.Length + newLineIndex)
            {
                if (scrollIndex > text.Length + newLineIndex)
                {
                    break;
                }

                if (scrollIndex >= charactersUntilNewline * (newLineIndex + 1) && text[scrollIndex] == ' ')
                {
                    speechText.text += Environment.NewLine;
                    scrollIndex++;
                    newLineIndex++;
                }

                speechText.text += text[scrollIndex];
                scrollIndex++;
                yield return new WaitForSeconds(textScrollSpeed);
            }

            lineFinished = true;
            endTextIndicator.SetActive(true);
            myManager.IncrementTextboxIndex(incrementIndex);
        }
    }
}
