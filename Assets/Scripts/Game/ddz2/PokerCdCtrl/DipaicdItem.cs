using UnityEngine;

namespace Assets.Scripts.Game.ddz2.PokerCdCtrl
{
    public class DipaicdItem : PokerCardItem
    {
        /*************翻转牌**************/
        private EventDelegate _changeFEvent;
        private EventDelegate _changeOEvent;

        protected void Awake()
        {
            _changeFEvent = new EventDelegate(Onfinish);
            _changeOEvent = new EventDelegate(Onfinish1);
        }

        private void Onfinish()
        {
            SetCardFront();

            TweenRotation rightTweenR = GetComponent<TweenRotation>();
            if (rightTweenR == null) return;

            rightTweenR.RemoveOnFinished(_changeFEvent);

            rightTweenR.from = new Vector3(0, -90, 0);
            rightTweenR.to = new Vector3(0, 0, 0);
            rightTweenR.AddOnFinished(_changeOEvent);
            rightTweenR.ResetToBeginning();
            rightTweenR.PlayForward();
        }

        private void Onfinish1()
        {
            TweenRotation rightTweenR = GetComponent<TweenRotation>();
            if (rightTweenR == null) return;
            rightTweenR.RemoveOnFinished(_changeOEvent);
        }

        /// <summary>
        /// 翻牌
        /// </summary>
        /// <param name="cardVal">牌值(含花色)</param>
        /// <param name="playAnim">是否播放动画</param>
        public void TurnCard(int cardVal, bool playAnim)
        {
            CdValue = cardVal;

            if (playAnim)
            {
                TurnCard();
            }
            else
            {
                SetCardFront();
            }
        }

        /// <summary>
        /// 播放翻牌动画
        /// </summary>
        private void TurnCard()
        {
            TweenRotation rightTweenR = GetComponent<TweenRotation>();
            if (rightTweenR == null) return;
            rightTweenR.from = new Vector3(0, 0, 0);
            rightTweenR.to = new Vector3(0, -90, 0);
            rightTweenR.AddOnFinished(_changeFEvent);
            rightTweenR.ResetToBeginning();
            rightTweenR.PlayForward();
        }

        /// <summary>
        /// 让牌背过来
        /// </summary>
        public void TrunBackCd()
        {
            CdBg.spriteName = "back";
            ColorLeftUp.gameObject.SetActive(false);
            ColorRightDown.gameObject.SetActive(false);
            CdValueSpUp.gameObject.SetActive(false);
            CdValueSpDown.gameObject.SetActive(false);
        }

        //public void SetDipaiValue(int cardVaI)
        //{
        //    gameObject.SetActive(true);
        //    CdBg.spriteName = "front";
        //    ColorLeftUp.gameObject.SetActive(true);
        //    ColorRightDown.gameObject.SetActive(true);
        //    CdValueSpUp.gameObject.SetActive(true);
        //    CdValueSpDown.gameObject.SetActive(true);

        //    SetCdValue(cardVaI);
        //}

     

    }
}
