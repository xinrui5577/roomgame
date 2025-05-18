using Assets.Scripts.Common.Windows;
using UnityEngine;

namespace Assets.Scripts.Game.BaiTuan
{
    public class EffectCtrl : YxNguiWindow
    {
        public GameObject WinEffect;
        public GameObject LoseEffect;
        protected override void OnAwake()
        {
            base.OnAwake();
            WinEffect.SetActive(false);
            LoseEffect.SetActive(false);
        }

        protected override void OnFreshView()
        {
            if (!(Data is int)) return;
            var type = (int)Data;
            ChangeEffect(type != 0);
            Invoke("Hide", 2);
        }

        protected void ChangeEffect(bool isWin)
        {
            WinEffect.SetActive(isWin);
            LoseEffect.SetActive(!isWin);
        }
    }
}
