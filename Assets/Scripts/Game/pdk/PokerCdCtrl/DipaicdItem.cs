using UnityEngine;

namespace Assets.Scripts.Game.pdk.PokerCdCtrl
{
    public class DipaicdItem : PokerCardItem
    {
        /// <summary>
        /// 让牌背过来
        /// </summary>
        public void TrunBackCd()
        {
            CdBg.spriteName = "back";
            ColorLeftUp.gameObject.SetActive(false);
            ColorRightDown.gameObject.SetActive(false);
            CdValueSpUp.gameObject.SetActive(false);
            //CdBackSp.gameObject.SetActive(false);
            gameObject.transform.localScale = new Vector3(0.4f, 0.4f, 1f);
        }

        public void SetDipaiValue(int cd)
        {
            gameObject.SetActive(true);
            CdBg.spriteName = "front";
            ColorLeftUp.gameObject.SetActive(true);
            ColorRightDown.gameObject.SetActive(true);
            CdValueSpUp.gameObject.SetActive(true);
            //CdBackSp.gameObject.SetActive(true);

            SetCdValue(cd);
            gameObject.transform.localScale = new Vector3(0.4f,0.4f,1f);
        }
    }
}
