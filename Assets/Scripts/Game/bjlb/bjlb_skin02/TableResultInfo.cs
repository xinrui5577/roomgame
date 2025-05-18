using UnityEngine;
using System.Collections;
using Sfs2X.Entities.Data;
using Assets.Scripts.Common.Adapters;



namespace Assets.Scripts.Game.bjlb.bjlb_skin02
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
            if (responseData.ContainsKey("win"))
            {
                int selfResult = responseData.GetInt("win");
                SetLabel(selfResult, MResultLabel);
            }

            if(responseData.ContainsKey("bpg"))
            {
                long bankerResult = GetTotalGold(responseData,"bpg");
                SetLabel(bankerResult, BankerResultLabel);
                SetLabel(-bankerResult, PlayersResultLabel);
            }
        }

        /// <summary>
        /// 获取总输赢
        /// </summary>
        /// <param name="responseData"></param>
        /// <param name="key">int型数组key值</param>
        /// <returns></returns>
        long GetTotalGold(ISFSObject responseData,string key)
        {
            var array = responseData.GetIntArray(key);
            int len = array.Length;
            long res = 0;
            for (int i = 0; i < len; i++)
            {
                res += array[i];
            }
            return res;
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

        [System.Serializable]
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