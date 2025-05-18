using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace Assets.Scripts.Game.brnn3d
{
    public class BeiShuMode : MonoBehaviour
    {
        public Transform BeishuBg;
        private bool _isOpenBeiShuUI;
        public Text Beishus;

        //播放倍数效果
        public void PlayBeiShuEff()
        {
            if (_isOpenBeiShuUI)
                return;
            BeishuBg.transform.DOLocalMoveX(180, 0.5f);
            _isOpenBeiShuUI = true;
        }

        //停止倍数的效果
        public void StopBeiShuEff()
        {
            if (!_isOpenBeiShuUI)
                return;
            BeishuBg.transform.DOLocalMoveX(287, 0.5f);
            _isOpenBeiShuUI = false;
        }

        public string[] NiuTypeNames;

        //倍数显示的按钮
        public void BeiShuBtnClick()
        {
            if (_isOpenBeiShuUI)
                StopBeiShuEff();
            else
                PlayBeiShuEff();
        }

        //设置倍数信息
        public void SetRateInfo(string[] rates)
        {
            var cont = Mathf.Min(NiuTypeNames.Length, rates.Length);
            string info = "";

            for (int i = 0; i < cont; i++)
            {
                info += string.Format("{0}×{1}\n", NiuTypeNames[i], rates[cont - i - 1]);
            }
            info = info.Trim();

            Beishus.text = info;
        }
    }
}
