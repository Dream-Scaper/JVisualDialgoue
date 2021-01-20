using UnityEngine;

namespace JVDialogue
{
    public class Textbox : ScriptableObject
    {
        public Sprite background;

        public Character[] characters;
        public Character.EmotionState[] characterEmotes;

        [Range(0, 4)]
        public int activeCharacter;

        [TextArea(2,4)]
        public string text;
    }
}
