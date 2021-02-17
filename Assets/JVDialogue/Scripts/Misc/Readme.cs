using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JVDialogue
{
    public class Readme : ScriptableObject
    {
        public string title;
        public Texture2D icon;

        public string[] headers;
        public string[] body;
        public bool[] completedSection;
        public bool[] foldOuts;
    }
}