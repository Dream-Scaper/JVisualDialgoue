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

        [TextArea(3,4)]
        public string text;
    }
}
