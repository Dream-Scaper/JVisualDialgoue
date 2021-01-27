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
        public bool lineFinished = true;
        private int scrollIndex = 0;
        private Coroutine currentScroll;

        [HideInInspector]
        public bool UiUp = false;

        void Start()
        {
            endTextIndicator.SetActive(false);

            if (nextTextboxButton != null)
            {
                nextTextboxButton.onClick.RemoveAllListeners();
                nextTextboxButton.onClick.AddListener(delegate { myManager.ChangeTextbox(1, true); });
            }

            if (lastTextboxButton != null)
            {
                lastTextboxButton.onClick.RemoveAllListeners();
                lastTextboxButton.onClick.AddListener(delegate { myManager.ChangeTextbox(-1, true); });
            }
        }

        public void OpenUI()
        {
            animator.SetBool(AnimatorVariables.dialogueUp, true);
        }

        public void DisplayTextbox(int incrementIndex, bool incrementBefore, bool scrollText)
        {
            speechText.text = "";
            endTextIndicator.SetActive(false);

            if (incrementBefore && lineFinished)
            {
                myManager.IncrementTextboxIndex(incrementIndex);
            }

            Textbox textbox = myManager.ActiveDialogue.Textboxes[myManager.TextboxIndex];
            UpdateLastNextButtons(myManager.TextboxIndex);

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

            if (scrollText)
            {
                currentScroll = StartCoroutine(ScrollText(textbox.text, incrementIndex, !incrementBefore));
            }
            else
            {
                Debug.Log("Dialogue line interrupted!");

                StopCoroutine(currentScroll);

                speechText.text = textbox.text;
                lineFinished = true;
                endTextIndicator.SetActive(true);

                if (!incrementBefore)
                {
                    myManager.IncrementTextboxIndex(incrementIndex);
                }
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

        private IEnumerator ScrollText(string text, int incrementIndex, bool increment)
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

            Debug.Log("Dialogue line finished!");

            if (increment)
            {
                myManager.IncrementTextboxIndex(incrementIndex);
            }
        }
    }
}
