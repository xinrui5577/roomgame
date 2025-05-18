using Assets.Scripts.Hall.View.RecordWindows;
using UnityEngine;

namespace Assets.Skins.SkinResource.skin_0019.Scripts.Tea.Page.TeaTotalRecordContainer
{
    public class JpmjSssRecordDetailItem : MonoBehaviour
    {
        public JpmjSssLineInfo[] LinesInfo;
        public HeadItem HeadItem;
        public UILabel TotalScoreLabel;
        public UILabel ShootCountLabel;
        public string ShootFormat = "打人{0}枪";
        public UILabel BeShootCountLabel;
        public string BeShootFormat = "被打{0}枪";

        public LabelStyle PostiveLabelStyle;
        public LabelStyle NegativeLabelStyle;     

        public void SetRecordDetail(SssReplayFrameData data)
        {
            // 打枪信息
            var shootCount = data.ShootCount;
            var beShootCount = data.BeShootCount;
            ShootCountLabel.text = string.Format(ShootFormat, shootCount);
            BeShootCountLabel.text = string.Format(BeShootFormat, beShootCount);
            // 总分
            var score = data.AountScore;
            TotalScoreLabel.text = string.Format("{0}{1}", score > 0 ? "+" : string.Empty, score);
            SetLabelStyle(score, TotalScoreLabel);
            //设置line 数据
            SetLineScore(data);
            gameObject.SetActive(true);
            //设置头像数据
            HeadItem.UpdateView(data.HeadData);
        }

        protected void SetLineScore(SssReplayFrameData data)
        {
            for (int i = 0; i < LinesInfo.Length; i++)
            {
                LinesInfo[i].SetLineInfo(data, i);
            }
        }

        public virtual void OnReset()
        {
            TotalScoreLabel.text = "";
            ShootCountLabel.text = string.Format(ShootFormat, 0);
            BeShootCountLabel.text = string.Format(BeShootFormat, 0);
            foreach (var item in LinesInfo) item.OnReset();
            gameObject.SetActive(false);
        }

        protected virtual void SetLabelStyle(int score, UILabel label)
        {
            LabelStyle style = score >= 0 ? PostiveLabelStyle : NegativeLabelStyle;
            SetLabelStyle(label, style);
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
    }
}