using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class GmBao : MonoBehaviour
    {
        public Text Text;
        private bool mState;

        public bool GmBaoState
        {
            get { return mState; }
            set
            {
                mState = value;
                if (mState)
                {
                    Text.text = "要宝：开";
                }
                else
                {
                    Text.text = "要宝：关";
                }
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
            GmBaoState = false;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            GmBaoState = false;
        }

        public void SetGmBaoState()
        {
            GmBaoState = !GmBaoState;
        }
    }
}