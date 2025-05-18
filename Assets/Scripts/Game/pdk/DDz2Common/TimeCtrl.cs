using UnityEngine;

namespace Assets.Scripts.Game.pdk.DDz2Common
{
    public class TimeCtrl : MonoBehaviour
    {
        [SerializeField]
        protected UILabel TimeLabel;

        // Use this for initialization
        void Start () {
            TimeLabel.text = System.DateTime.Now.ToString("HH:mm");
        }

        private int _index = 0;

        private void FixedUpdate()
        {
            if (_index % 60 == 0)
            {
                _index = 0;
                TimeLabel.text = System.DateTime.Now.ToString("HH:mm");
            }

            _index++;
        }
    }
}
