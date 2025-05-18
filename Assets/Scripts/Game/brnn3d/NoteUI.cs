using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.brnn3d
{
    public class NoteUI : MonoBehaviour
    {
        public Text NoteText;
        public void Note(string str)
        {
            if (NoteText.gameObject.activeSelf)
                NoteText.gameObject.SetActive(false);
            NoteText.gameObject.SetActive(true);
            NoteText.text = str;
        }
    }

}
