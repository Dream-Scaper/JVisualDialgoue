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

        public DialogueManager myManager;
        public Animator animator;

        // UI Elements
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI speechText;
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
        }

        public void OpenUI()
        {
            animator.SetBool(AnimatorVariables.dialogueUp, true);
        }

        public void DisplayTextbox(int incrementIndex, bool incrementBefore, bool scrollText)
        {
            speechText.text = "";
            endLineIndicator.SetActive(false);

            if (incrementBefore && lineFinished)
            {
                myManager.ChangeTextboxIndex(incrementIndex);
            }

            Textbox textbox = myManager.ActiveDialogue.Textboxes[myManager.TextboxIndex];
            UpdateLastNextButtons(myManager.TextboxIndex);

            if (displayBackground)
            {
                background.sprite = textbox.background;
                background.color = Color.white;
            }
            else
            {
                background.sprite = null;
                background.color = Color.clear;
            }

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

                if (!incrementBefore)
                {
                    myManager.ChangeTextboxIndex(incrementIndex);
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
            endLineIndicator.SetActive(true);

            Debug.Log("Dialogue line finished!");

            if (increment)
            {
                myManager.ChangeTextboxIndex(incrementIndex);
            }
        }
    }
}
