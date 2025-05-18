using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Windows;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.mdx
{
    public class ResultWindow : YxNguiWindow
    {

        public UILabel PlayerNameLabel;
        /// <summary>
        /// 输赢,未下注label
        /// </summary>
        public UILabel ResultLabel;
        /// <summary>
        /// 输赢筹码值
        /// </summary>
        public NguiLabelAdapter WinValLabel;
        public UILabel DicePointLabel;
        public UISprite[] DicesSprite;

        public GameObject WinEffect;

       

        protected override void OnFreshView()
        {
            base.OnFreshView();
            ISFSObject dcieData = GetData<ISFSObject>();
            var diceVals = dcieData.GetIntArray("dices");
            if (diceVals == null || diceVals.Length == 0)
            {
                return;
            }
            PlayerNameLabel.text = App.GameData.GetPlayerInfo().NickM;
            var response = GetData<ISFSObject>();
            if (response == null) return;

            int win = response.GetInt("win");
            if (win > 0)
            {
                Facade.Instance<MusicManager>().Play("win");
                SetWinEffectActive(true);
            }
            SetWinValLabel(win);
            SetDicesResult(diceVals);
            Invoke("Hide", 3);
        }

        private void SetDicesResult(int[] diceVals)
        {
            int len = diceVals.Length;
            bool baozi = true;
            int sum = 0;
            int lastVal = diceVals[0];
            for (int i = 0; i < len; i++)
            {
                int val = diceVals[i];
                sum += val;
                if (lastVal != val)
                {
                    baozi = false;
                }
                DicesSprite[i].spriteName = string.Format("dice{0}", val);
            }
            string baoziString = App.GetGameManager<MdxGameManager>().TipStringFormat.BaoZiFormat;
            DicePointLabel.text = baozi ? baoziString : string.Format("当前 {0}点{1}", sum, sum > 10 ? "大" : "小");
        }

        private void SetWinValLabel(int win)
        {
            if (win == 0)
            {
                ResultLabel.text = "未下注";
                WinValLabel.gameObject.SetActive(false);
                return;
            }
            ResultLabel.text = win > 0 ? "赢" : "输";
            WinValLabel.Text(win);
            WinValLabel.gameObject.SetActive(true);
        }

        public override void Hide()
        {
            CancelInvoke("Hide");
            SetWinEffectActive(false);
            base.Hide();
        }

        void SetWinEffectActive(bool active)
        {
            if (WinEffect == null) return;
            WinEffect.SetActive(active);
        }

        public void Reset()
        {
            Hide();
        }
    }
}
