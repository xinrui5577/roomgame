using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class TingTipCtrl : MonoBehaviour
    {
        public Image Mark;

        private void OnEnable()
        {
            Mark.gameObject.SetActive(MahjongUtility.TingTipCtrl == 0);
        }

        // 0： 开启 1：关闭
        public void OnMarkClick()
        {
            int value = MahjongUtility.TingTipCtrl;
            MahjongUtility.TingTipCtrl = value == 0 ? 1 : 0;
            Mark.gameObject.SetActive(MahjongUtility.TingTipCtrl == 0);
        }
    }
}