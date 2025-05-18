using System;
using Assets.Scripts.Game.sssjp.ImgPress.Main;
using UnityEngine;

namespace Assets.Scripts.Game.sssjp
{
    public abstract class LineInfo : MonoBehaviour {
       
        public UILabel NormalScoreLabel;

        public UILabel AddScoreLabel;

        public UILabel CardTypeLabel;

        public UISprite CardTypeSprite;


        /// <summary>
        /// 普通分数字体格式
        /// </summary>
        public String NormalScoreFormat = "{0}";

        /// <summary>
        /// 额外字体格式
        /// </summary>
        public String AddScoreFormat = "({0})";

        public LabelStyle PostiveLabelStyle;

        public LabelStyle NegativeLabelStyle;

        [HideInInspector]
        public int NormalScore;

        [HideInInspector]
        public int AddScore;

        [HideInInspector]
        public CardType Type;

        [HideInInspector]
        public int Line;

        public void SetLineInfo(int line,UserMatchInfo matchInfo)
        {
            Type = (CardType)matchInfo.DunTypeList[line];
            Line = line;
            SetLineInfo(line, matchInfo.NormalScores[line], matchInfo.AddScore[line], Type);
        }

        public void SetLineInfo(int line, int noraml, int add = 0, CardType type = CardType.none)
        {
            SetTypeLabel(line, Type);               //设置字体牌型
            SetTypeSprite(line, Type);              //设置图片牌型
            SetNormaleScore(noraml);
            SetAddScore(add);
        }

        /// <summary>
        /// 把额外加分加入到普通加分的字体上
        /// </summary>
        public void ShowAddScoreOnNormal()
        {
            SetNormaleScore(AddScore);
        }        

        /// <summary>
        /// 设置牌型图片
        /// </summary>
        /// <param name="line"></param>
        /// <param name="type"></param>
        protected void SetTypeSprite(int line, CardType type)
        {
            if (CardTypeSprite == null) return;
            string spriteName = GetSpriteName(line, type);
            CardTypeSprite.spriteName = spriteName;
        }

        /// <summary>
        /// 设置牌型字
        /// </summary>
        /// <param name="line"></param>
        /// <param name="type"></param>
        protected void SetTypeLabel(int line, CardType type)
        {
            if (CardTypeLabel == null) return;
            string typeName = GetTypeName(line, type);
            CardTypeLabel.text = typeName;
        }

        /// <summary>
        /// 设置普通分
        /// </summary>
        /// <param name="normal"></param>
        protected void SetNormaleScore(int normal)
        {
            NormalScore += normal;
            if (NormalScoreLabel == null) return;
            NormalScoreLabel.text = string.Format(NormalScoreFormat, GetScoreString(NormalScore));
            LabelStyle style = NormalScore >= 0 ? PostiveLabelStyle : NegativeLabelStyle;
            SetLabelStyle(NormalScoreLabel, style);
        }

        /// <summary>
        /// 设置特殊分
        /// </summary>
        /// <param name="add"></param>
        protected void SetAddScore(int add)
        {
            AddScore += add;
            if (AddScoreLabel == null) return;
            AddScoreLabel.text = string.Format(AddScoreFormat, GetScoreString(AddScore));
            LabelStyle style = AddScore >= 0 ? PostiveLabelStyle : NegativeLabelStyle;
            SetLabelStyle(AddScoreLabel, style);
        }

        protected string GetTypeName(int line,CardType type)
        {
            switch (type)
            {
                case CardType.sanpai:
                    return "乌龙";
                case CardType.yidui:
                    return "对子";
                case CardType.liangdui:
                    return "两对";
                case CardType.santiao:
                    return line == 0 ? "冲三" : "三条";
                case CardType.shunzi:
                    return "顺子";
                case CardType.tonghua:
                    return "同花";
                case CardType.hulu:
                    return "葫芦";
                case CardType.tiezhi:
                    return "铁枝";
                case CardType.tonghuashun:
                    return "同花顺";
                case CardType.wutong:
                    return "五同";
                default:
                    return type.ToString();
            }
        }

        protected string GetSpriteName(int line, CardType type)
        {
            switch (type)
            {
                case CardType.santiao:
                    return line == 0 ? "chongsan" : type.ToString();
                default:
                    return type.ToString();
            }
        }

        protected string GetScoreString(int score)
        {
            return score <= 0 ? score.ToString() : string.Format("+{0}",score);
        }

        protected void SetLabelStyle(UILabel label, LabelStyle style)
        {
            label.color = style.NormalColor;
            if (style.ApplyGradient)
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


        public abstract void Show();

        public abstract void Hide();

        public abstract void Reset();

   
    }
}
