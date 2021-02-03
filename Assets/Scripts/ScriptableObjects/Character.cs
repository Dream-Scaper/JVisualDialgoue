using System;
using UnityEngine;

namespace JVDialogue
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Character", menuName = "JVDialogue/Character")]
    public class Character : ScriptableObject
    {
        public enum EmotionState { Neutral, Happy, Sad, Angry };

        public string npcName;

        public Sprite fallback;
        public Sprite[] emotions;

        private void OnEnable()
        {
            if (emotions == null)
            {
                emotions = new Sprite[Enum.GetNames(typeof(EmotionState)).Length];
            }

            if (emotions.Length != Enum.GetNames(typeof(EmotionState)).Length)
            {
                ResizeArray();
            }
        }

        private void ResizeArray()
        {
            Sprite[] temp = emotions;

            emotions = new Sprite[Enum.GetNames(typeof(EmotionState)).Length];

            for (int i = 0; i < Mathf.Min(temp.Length, emotions.Length); i++)
            {
                emotions[i] = temp[i];
            }
        }
    }
}
