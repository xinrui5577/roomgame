using System;
using Assets.Scripts.Common.Windows;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.lhc
{
    public class ApplyBankWindow : YxNguiWindow
    {
        public UILabel BankBandCoin;
        public UILabel BankBandCoin1;
        public UILabel MinBankBandCoin;
        public UILabel MaxBankBandCoin;
        public UISlider Slider;

        private double _curBandCoin;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var gdata = App.GetGameData<LhcGameData>();
            Slider.value = 0;
            var minValue= YxUtiles.GetShowNumber(gdata.CurrentBanker.Coin + gdata.BankAdd).ToString();
            BankBandCoin.text = minValue;
            BankBandCoin1.text = minValue;
            MinBankBandCoin.text= minValue;
//            var keDu = (int)Mathf.Floor(gdata.ApplyBankCondition() / gdata.BankAdd);
            MaxBankBandCoin.text = YxUtiles.GetShowNumber(gdata.GetPlayerInfo().CoinA).ToString();
        }

        public void OnChangeSlifder()
        {
            var gdata = App.GetGameData<LhcGameData>();

            var keDu=Mathf.Floor(gdata.ApplyBankCondition() / gdata.BankAdd);
            int i = (int)(1 / keDu * 100000000);
            var f = (float)(i * 1.0) / 100000000;

            int i1 = (int)(Slider.value * 100000000);
            var f1 = (float)(i1 * 1.0) / 100000000;

            if ((int)f1 == 1)
            {
                BankBandCoin.text = YxUtiles.GetShowNumber(gdata.GetPlayerInfo().CoinA).ToString();
                BankBandCoin1.text = YxUtiles.GetShowNumber(gdata.GetPlayerInfo().CoinA).ToString();
                _curBandCoin = gdata.GetPlayerInfo().CoinA;
                return;
            }
            for (int j = 0; j < keDu + 1; j++)
            {
                if (Math.Abs(f1-f * j) < 0.1)
                {
                    _curBandCoin = gdata.CurrentBanker.Coin + gdata.BankAdd + j *  gdata.BankAdd;
                    BankBandCoin.text = YxUtiles.GetShowNumber((long)_curBandCoin).ToString();
                    BankBandCoin1.text = YxUtiles.GetShowNumber((long)_curBandCoin).ToString();
                    return;
                }
                
            }
        }

        public void OnSureBtn()
        {
            App.GetRServer<LhcGameServer>().SendApplyBank((long)_curBandCoin);
            App.GetGameManager<LhcGameManager>().ApplyBankChange();
        }
    }
}
