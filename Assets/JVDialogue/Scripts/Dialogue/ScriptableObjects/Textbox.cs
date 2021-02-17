using UnityEngine;

namespace JVDialogue
{
    [System.Serializable]
    public class Textbox : ScriptableObject
    {
        public Character[] characters;
        public DialogueHelper.EmotionState[] characterEmotes;

        public int activeCharacter;

        public Sprite background;
        public Color backgroundColor;

        [TextArea(1,5)]
        public string text;
    }
}
