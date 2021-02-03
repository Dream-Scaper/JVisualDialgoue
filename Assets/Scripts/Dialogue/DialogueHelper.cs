using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JVDialogue
{
    public class DialogueHelper : MonoBehaviour
    {
        // This class stores datatypes and settings for the entire dialogue system.
        // Make any additions necessary, but be sure to not remove anything! 

        // This is the enum used in determining what profile emotions characters can display.
        // The character's emotions will automatically update when an addition is made here.
        public enum EmotionState { Neutral, Happy, Sad, Angry };

        // This is used in determining how many profile sprites/character slots
        // the overall dialogue system attempts to use.
        public static int profileNumber = 5;

        // When pressing "New Textbox" in the editor, a copy of the previous textbox is made rather than a completely blank slate.
        public static bool populateByDuplicating = true;
    }
}
