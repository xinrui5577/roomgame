using UnityEngine;

namespace Assets.Skins.SkinResource.skin_0019.Scripts.Tea.Page.TeaTotalRecordContainer
{
    public class JpmjSssLineInfo : MonoBehaviour
    {
        public UILabel NormalScoreLabel;
        public UILabel AddScoreLabel;
        public UILabel CardTypeLabel;

        /// <summary>
        /// 普通分数字体格式
        /// </summary>
        public string NormalScoreFormat = "{0}";
        /// <summary>
        /// 额外字体格式
        /// </summary>
        public string AddScoreFormat = "({0})";

        public LabelStyle PostiveLabelStyle;
        public LabelStyle NegativeLabelStyle;

        [HideInInspector]
        public int NormalScore;
        [HideInInspector]
        public int AddScore;

        public UISprite[] BgSprites;
        public JpmjSssPokerCard[] PokerItems;

        public void SetLineInfo(SssReplayFrameData data, int line)
        {
            var type = (CardType)data.CardsData[line].Type;
            var lineScoreData = data.LineScoreData[line];

            SetLinePoker(data, line);
            SetTypeLabel(line, type);
            SetNormaleScore(lineScoreData.Normal);
            SetNormaleScore(lineScoreData.Add);// 把额外加分加入到普通加分的字体上

            gameObject.SetActive(true);
        }

        protected void SetLinePoker(SssReplayFrameData data, int index)
        {
            for (int i = 0; i < PokerItems.Length; i++)
            {
                var card = data.CardsData[index].Cards[i];
                PokerItems[i].SetCardId(card);
                PokerItems[i].SetCardFront();
                PokerItems[i].SetPokerScale(1.3f, 1.3f);
                PokerItems[i].gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 设置牌型字
        /// </summary>
        /// <param name="line"></param>
        /// <param name="type"></param>
        protected void SetTypeLabel(int line, CardType type)
        {
            if (CardTypeLabel == null) return;
            CardTypeLabel.text = GetTypeName(line, type);
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
        //protected void SetAddScore(int add)
        //{
        //    AddScore += add;
        //    if (AddScoreLabel == null) return;
        //    AddScoreLabel.text = string.Format(AddScoreFormat, GetScoreString(AddScore));
        //    LabelStyle style = AddScore >= 0 ? PostiveLabelStyle : NegativeLabelStyle;
        //    SetLabelStyle(AddScoreLabel, style);
        //} 

        protected string GetTypeName(int line, CardType type)
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

        protected string GetScoreString(int score)
        {
            return score <= 0 ? score.ToString() : string.Format("+{0}", score);
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

        public void OnReset()
        {
            AddScore = 0;
            NormalScore = 0;
            gameObject.SetActive(false);
        }
    }
}
