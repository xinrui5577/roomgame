using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon;
using UnityEngine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI
{
    public class PopPnlBase : MonoBehaviour
    {

        public GameObject Frame;

        public virtual GameObject FrameObj
        {
            get { 
                if (Frame == null) Frame = gameObject;
                return Frame;
            }
        }

        public virtual void Show()
        {
            FrameObj.SetActive(true);
            UtilData.HandMjTouchEnable = false;
            OnShow();
        }

        protected virtual void OnShow()
        {
            
        }

        public virtual void Hide()
        {
            OnHide();
            FrameObj.SetActive(false);
            UtilData.HandMjTouchEnable = true;
        }

        protected virtual void OnHide()
        {
            
        }
    }
}
