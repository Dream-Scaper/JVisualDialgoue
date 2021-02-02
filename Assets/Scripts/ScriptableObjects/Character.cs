using UnityEngine;

namespace JVDialogue
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Character", menuName = "JVDialogue/Character")]
    public class Character : ScriptableObject
    {
        public enum EmotionState { NEUTRAL, HAPPY, SAD, ANGRY };

        [Header("Character Attributes")]
        public string npcName;

        [Header("Character Sprites")]
        public Sprite fallback;

        [Space(10)]

        public Sprite neutral;
        public Sprite happy;
        public Sprite sad;
        public Sprite angry;

        [HideInInspector]
        public Sprite[] emotions;

        private void OnEnable()
        {
            if (emotions == null)
            {
                emotions = new Sprite[] { neutral, happy, sad, angry };
            }
        }
    }
}
