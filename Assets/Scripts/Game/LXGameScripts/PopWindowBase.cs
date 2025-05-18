using UnityEngine;

namespace Assets.Scripts.Game.LXGameScripts
{
    public class PopWindowBase : MonoBehaviour
    {
        public GameObject Frame;

        public virtual GameObject FrameObj
        {
            get
            {
                if (Frame == null) Frame = gameObject;
                return Frame;
            }
        }

        public virtual void Show()
        {
            FrameObj.SetActive(true);
            OnShow();
        }

        protected virtual void OnShow()
        {
            
        }

        public virtual void Hide()
        {
            FrameObj.SetActive(false);
            OnHide();
        }

        protected virtual void OnHide()
        {
            
        }
    }
}

