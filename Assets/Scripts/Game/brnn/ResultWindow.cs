using System;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Windows;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.brnn
{
    public class ResultWindow : YxNguiWindow
    {
        public NguiLabelAdapter ResultMe;
        public NguiLabelAdapter ResultBanker;
        public LabelStyle WinLabelStyle;
        public LabelStyle LoseLabelStyle;
        protected override void OnFreshView()
        { 
            var sfsdata = GetData<ISFSObject> ();
            if (sfsdata == null) return;
            var gdata = App.GetGameData<BrnnGameData>();
            gdata.GetPlayerInfo().CoinA = sfsdata.GetLong("gold");
            var labelStylt = gdata.ResultUserTotal >= 0 ? WinLabelStyle : LoseLabelStyle;
            ResultMe.Text(gdata.ResultUserTotal);
            SetLabelStyle(ResultMe, labelStylt);
            labelStylt = gdata.ResultBnakerTotal >= 0 ? WinLabelStyle : LoseLabelStyle;
            SetLabelStyle(ResultBanker, labelStylt);
            ResultBanker.Text(gdata.ResultBnakerTotal);
            gameObject.SetActive(true);
            Invoke("Hide", 6);
        }

        public virtual void ShowResultView(ISFSObject response)
        {
            ShowWithData(response);
        }

        protected void SetLabelStyle(NguiLabelAdapter labelAdapter,LabelStyle style)
        {
            var label = labelAdapter.Label;
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

        [Serializable]
        public class LabelStyle
        {
            public Color NormalColor = Color.white;
            public bool ApplyGradient;
            public Color GradientBottom = Color.white;
            public Color GradientTop = Color.white;
            public UILabel.Effect EffectStyle = UILabel.Effect.None;
            public Color EffectColor = Color.white;
            public Vector2 EffectDistance = new Vector2(2, 2);
        }
    }
}
