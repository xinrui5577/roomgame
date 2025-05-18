using Assets.Scripts.Game.sssjp.ImgPress.Main;
using UnityEngine;

namespace Assets.Scripts.Game.sssjp
{
    public class HandCardType : MonoBehaviour  
    {

        [SerializeField]
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private LineInfo[] _linesInfo;

        public GameObject SpecialMark;

        public UILabel TotalScoreLabel;

        public UILabel ShootCountLabel;

        public string ShootFormat = "打人{0}枪";

        public UILabel BeShootCountLabel;

        public string BeShootFormat = "被打{0}枪";

        private UserMatchInfo _matchInfo;

        public LabelStyle PostiveLabelStyle;

        public LabelStyle NegativeLabelStyle;

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

        /// <summary>
        /// 显示每行的牌型
        /// </summary>
        /// <param name="line">第几行(0,1,2)</param>
        /// <param name="matchInfo"></param>
        public virtual void ShowType(int line, UserMatchInfo matchInfo)
        {
            _matchInfo = matchInfo;
            var curLine = _linesInfo[line];
            curLine.SetLineInfo(line, matchInfo);
            curLine.Show();
        }

        public virtual void HideType(int line)
        {
            _linesInfo[line].Hide();
        }

        public virtual void Reset()
        {
            foreach (var item in _linesInfo)
            {
                item.Reset();
            }
            ShootCountLabel.gameObject.SetActive(false);
            BeShootCountLabel.gameObject.SetActive(false);
            SpecialMark.SetActive(false);
            TotalScoreLabel.gameObject.SetActive(false);
        }

        public void SetSpecialMarkActive(bool active)
        {
            SpecialMark.SetActive(active);
        }

        public void SetLineTotalScore(int line)
        {
            var lineInfo = _linesInfo[line];
            lineInfo.ShowAddScoreOnNormal();
        }

        protected virtual void SetLabelStyle(int score,UILabel label)
        {
            LabelStyle style = score >= 0 ? PostiveLabelStyle : NegativeLabelStyle;
            SetLabelStyle(label, style);
        }

        public void ShowTotalScore()
        {
            if (TotalScoreLabel == null) return;
            int ttscore = _matchInfo.TtScore;
            ShowTotalScore(ttscore);
            //TotalScoreLabel.text = ttscore.ToString();
            //SetLabelStyle(ttscore, TotalScoreLabel);
            //TotalScoreLabel.gameObject.SetActive(true);
        }

        public void ShowTotalScore(int score)
        {
            if (TotalScoreLabel == null) return;
            TotalScoreLabel.text = string.Format("{0}{1}", score > 0 ? "+" : string.Empty, score);
            SetLabelStyle(score, TotalScoreLabel);
            TotalScoreLabel.gameObject.SetActive(true);
        }


        /// <summary>
        /// 显示打枪信息
        /// </summary>
        /// <param name="shootCount">打枪次数</param>
        /// <param name="beShootCount">被打枪次数</param>
        public void ShowShootInfo(int shootCount, int beShootCount)
        {
            ShootCountLabel.text = string.Format(ShootFormat, shootCount);
            ShootCountLabel.gameObject.SetActive(true);
            BeShootCountLabel.text = string.Format(BeShootFormat, beShootCount);
            BeShootCountLabel.gameObject.SetActive(true);
        }

        public void OnGetGameInfoInif(UserMatchInfo matchInfo)
        {

            for (int i = 0; i < 3; i++)
            {
                ShowType(i, matchInfo);
                SetLineTotalScore(i);
            }

            ShowTotalScore(matchInfo.TtScore);
            var shootInfo = matchInfo.Shoot;
            ShowShootInfo(shootInfo.ShootCount, shootInfo.BeShootCount);
        }
    }
}