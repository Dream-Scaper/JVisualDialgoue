using System.Collections.Generic;
using UnityEngine;

namespace JVDialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "JVDialogue/Dialogue")]
    public class Dialogue : ScriptableObject
    {
        public List<Textbox> Textboxes;

        private void OnEnable()
        {
            if (Textboxes == null) Textboxes = new List<Textbox>();
        }
    }
}
