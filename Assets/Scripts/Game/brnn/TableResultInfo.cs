using UnityEngine;
using System.Collections;
using Sfs2X.Entities.Data;
using Assets.Scripts.Common.Adapters;
using LabelStyle = Assets.Scripts.Game.brnn.ResultWindow.LabelStyle;
using YxFramwork.Common;



namespace Assets.Scripts.Game.brnn
{
    public class TableResultInfo : MonoBehaviour
    {
        public NguiLabelAdapter BankerResultLabel;

        public NguiLabelAdapter PlayersResultLabel;

        public NguiLabelAdapter MResultLabel;

        public LabelStyle WinLabelStyle;

        public LabelStyle LoseLabelStyle;

        [SerializeField]
        private float _hideTime;


        public void ShowTableResultInfo(ISFSObject responseData)
        {
            ShowPanel();
            if(responseData.ContainsKey("bwin"))
            {
                var bankerWin = responseData.GetLong("bwin");
                SetLabel(bankerWin, BankerResultLabel);
                SetLabel(-bankerWin, PlayersResultLabel);
            }


            if (responseData.ContainsKey("win"))
            {
                int myWin = responseData.GetInt("win");
                SetLabel(myWin, MResultLabel);
            }
        }

        private void ShowPanel()
        {
            gameObject.SetActive(true);
            StartCoroutine(TimerHide());
        }

        private IEnumerator TimerHide()
        {
            yield return new WaitForSeconds(_hideTime);
            HidePabel();
        }

        private void HidePabel()
        {
            gameObject.SetActive(false);
            HideLabel(BankerResultLabel);
            HideLabel(PlayersResultLabel);
            HideLabel(MResultLabel);
        }

        void HideLabel(NguiLabelAdapter adapter)
        {
            adapter.Label.text = string.Empty;
            adapter.gameObject.SetActive(false);
        }

        void SetLabel(long gold , NguiLabelAdapter adapter)
        {
            adapter.Text(gold);
            var labelStyle = gold >= 0 ? WinLabelStyle : LoseLabelStyle;
            SetLabelStyle(adapter, labelStyle);
            adapter.gameObject.SetActive(true);
        }

        protected void SetLabelStyle(NguiLabelAdapter labelAdapter, LabelStyle style)
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
    }
}