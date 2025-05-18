using System;
using Assets.Scripts.Common.Windows;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.BaiTuan
{
    public class ResultWindow : YxNguiWindow
    {

        public UILabel[] OurLabels;
        public UILabel[] BnakerLabels;
        public UILabel CountLabel;
        public UISprite WinSprite;
        public string Countdown = "";
        public GameObject WinMark;
        public LabelStyle WinLabelStyle;
        public LabelStyle LoseLabelStyle;
        protected override void OnFreshView()
        {
            CountLabel.text = Countdown;
            var responseData = GetData<ISFSObject>();
            if (responseData == null) return;
            var gdata = App.GetGameData<BtwGameData>();
            CancelInvoke("Dountdown");
            var win = responseData.GetInt("win");
            var self = gdata.GetPlayerInfo();
            self.CoinA = responseData.GetLong("total");
            var bwin = responseData.GetLong("bwin");
            var pg = responseData.GetIntArray("pg");
            var bpg = responseData.GetIntArray("bpg");

            for (var i = 0; i < pg.Length; i++)
            {
                var p = -pg[i];
                var pstr = YxUtiles.ReduceNumber(p);
                var ourLabel = OurLabels[i];
                if (p >= 0)
                {
                    ourLabel.text = pstr;
//                    ourLabel.color = Color.green;
                    SetLabelColorEffect(ourLabel,WinLabelStyle);
                }
                else
                {
                    ourLabel.text = pstr;
//                    ourLabel.color = Color.red;
                    SetLabelColorEffect(ourLabel, LoseLabelStyle);
                }
                var bp = bpg[i];
                var bpstr = YxUtiles.ReduceNumber(bp);
                var bankLabel = BnakerLabels[i];
                if (bp >= 0)
                {
                    bankLabel.text = bpstr;
                    //                    bankLabel.color = Color.green;
                    SetLabelColorEffect(bankLabel, WinLabelStyle);
                }
                else
                {
                    bankLabel.text = bpstr;
                    //                    bankLabel.color = Color.red;
                    SetLabelColorEffect(bankLabel, LoseLabelStyle);
                }
            }
            var our3Label = OurLabels[3];
            var winStr = YxUtiles.ReduceNumber(win);
            if (win >= 0)
            {
                our3Label.text = winStr;
                //                our3Label.color = Color.green;
                SetLabelColorEffect(our3Label, WinLabelStyle);
                Facade.Instance<MusicManager>().Play("Win");
                ShowWinMark();
            }
            else
            {
                our3Label.text = winStr;
                //                our3Label.color = Color.red;
                SetLabelColorEffect(our3Label, LoseLabelStyle);
                Facade.Instance<MusicManager>().Play("Lost");
                ShowWinMark(false);
            }
            var bank3Label = BnakerLabels[3];
            var bwinStr = YxUtiles.ReduceNumber(bwin);
            if (bwin >= 0)
            {
                bank3Label.text = bwinStr;
                //                bank3Label.color = Color.green;
                SetLabelColorEffect(bank3Label, WinLabelStyle);
            }
            else
            {
                bank3Label.text = bwinStr;
                //                bank3Label.color = Color.red;
                SetLabelColorEffect(bank3Label, LoseLabelStyle);
            }

            WinSprite.spriteName = win >= 0 ? "46" : "48";
            InvokeRepeating("Dountdown", 0, 1);
        }

        protected void Dountdown()
        {
            CountLabel.text = (Convert.ToInt32(CountLabel.text) - 1)+"";
            if (Convert.ToInt32(CountLabel.text) == 2)
            {
                Facade.Instance<MusicManager>().Play("Start");
                
            }
            if (Convert.ToInt32(CountLabel.text) <= 2)
            {
                CancelInvoke("Dountdown");
               Hide();
            }
        }

        public void ShowWinMark(bool needShow=true)
        {
            if (WinMark == null)
            {
                return;
            }
            WinMark.SetActive(needShow);
        }

        void SetLabelColorEffect(UILabel label,LabelStyle style)
        {
            if (WinLabelStyle.NeedGradient)
            {
                label.applyGradient = WinLabelStyle.NeedGradient;
                label.gradientBottom = WinLabelStyle.GradientBottom;
                label.gradientTop = WinLabelStyle.GradientTop;
            }
            label.effectStyle = WinLabelStyle.EffectStyle;
            label.effectColor = WinLabelStyle.EffectColor;
            label.effectDistance = WinLabelStyle.EffectDistance;
        }
    }

    [Serializable]
    public class LabelStyle
    {
        /// <summary>
        /// 默认颜色
        /// </summary>
        public Color NomalColor = Color.white;
        /// <summary>
        /// 是否需要渐变
        /// </summary>
        public bool NeedGradient;
        /// <summary>
        /// 上部渐变
        /// </summary>
        public Color GradientBottom = Color.white;
        /// <summary>
        /// 下部渐变
        /// </summary>
        public Color GradientTop = Color.white;
        /// <summary>
        /// 效果
        /// </summary>
        public UILabel.Effect EffectStyle = UILabel.Effect.None;
        /// <summary>
        /// 效果颜色
        /// </summary>
        public Color EffectColor = Color.white;
        /// <summary>
        /// 效果距离
        /// </summary>
        public Vector2 EffectDistance = Vector2.zero;
    }
}
