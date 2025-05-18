using UnityEngine;
using System.Collections;
using Assets.Scripts.Common.Adapters;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.BaiTuan.skin02
{
    public class ResultRankItem : MonoBehaviour
    {
        public NguiLabelAdapter PlayerName;

        public NguiLabelAdapter RankLabel;

        public NguiLabelAdapter GoldLabel;

        public UISprite RankMark;

        public LabelStyle WinLabelStyle;

        public LabelStyle LoseLabelStyle;

        /// <summary>
        /// 初始化排名信息
        /// </summary>
        /// <param name="playerName">玩家名字</param>
        /// <param name="gold">输赢</param>
        /// <param name="rank">排名(1--3)</param>
        public void InitItem(string playerName, int gold, int rank)
        {
            var labelStylt = gold >= 0 ? WinLabelStyle : LoseLabelStyle;
            SetLabelStyle(GoldLabel, labelStylt);
            PlayerName.Text(playerName);
            GoldLabel.Text(YxUtiles.ReduceNumber(gold));

            SetRank(rank);
        }

        private void SetRank(int rank)
        {
            if (rank < 4)
            {
                RankMark.spriteName = string.Format("rank_{0}", rank);
                RankLabel.gameObject.SetActive(false);
                RankMark.gameObject.SetActive(true);
            }
            else
            {
                RankLabel.Label.text = rank.ToString();
                RankLabel.gameObject.SetActive(true);
                RankMark.gameObject.SetActive(false);
            }
        }


        protected void SetLabelStyle(NguiLabelAdapter labelAdapter, LabelStyle style)
        {
            var label = labelAdapter.Label;
            if (style.NeedGradient)
            {
                label.applyGradient = true;
                label.gradientBottom = style.GradientBottom;
                label.gradientTop = style.GradientTop;
            }
            if (style.EffectStyle != UILabel.Effect.None)
            {
                label.effectStyle = style.EffectStyle;
                label.effectColor = style.EffectColor;
                label.effectDistance = style.EffectDistance;
            }
        }
    }
}