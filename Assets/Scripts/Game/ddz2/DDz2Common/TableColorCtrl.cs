using UnityEngine;

namespace Assets.Scripts.Game.ddz2.DDz2Common
{
    public class TableColorCtrl : MonoBehaviour {

        public const string OntableColorChangeKey = "tbcolorChangeKey0613";
        [SerializeField]
        private UITexture _showTexture;

        [SerializeField] private ColorSelect _colorSelect;
        // Use this for initialization
        void Start () {
            ColorSelect.OnColorChange = OnTotalColroChange;
            _colorSelect.OnValueChangeDo(PlayerPrefs.GetFloat(OntableColorChangeKey, 0.5f));
        }

        public void OnTotalColroChange(Color totalColor, float saveV)
        {
            _showTexture.color = totalColor;

            PlayerPrefs.SetFloat(OntableColorChangeKey, saveV);
            PlayerPrefs.Save();
        }

    }
}
