using System;
using UnityEngine;

namespace JVDialogue
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Character", menuName = "JVDialogue/Character")]
    public class Character : ScriptableObject
    {
        public string npcName;

        public Sprite fallback;
        public Sprite[] emotions;

        private void OnEnable()
        {
            // If the emotions array is completely empty (happens on newly created Character).
            if (emotions == null)
            {
                emotions = new Sprite[Enum.GetNames(typeof(DialogueHelper.EmotionState)).Length];
            }

            // If the array isn't properly sized (happens on addition to the DialogueHelper.EmotionState enum).
            if (emotions.Length != Enum.GetNames(typeof(DialogueHelper.EmotionState)).Length)
            {
                Sprite[] temp = emotions;

                emotions = new Sprite[Enum.GetNames(typeof(DialogueHelper.EmotionState)).Length];

                for (int i = 0; i < Mathf.Min(temp.Length, emotions.Length); i++)
                {
                    emotions[i] = temp[i];
                }
            }
        }
    }
}
