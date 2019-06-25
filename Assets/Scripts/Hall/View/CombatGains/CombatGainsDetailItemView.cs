using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Controller;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View.CombatGains
{
    public class CombatGainsDetailItemView : YxView
    {
        public UILabel RoundLabel;
        public UILabel TimeLabel;
        public UILabel ResultLabel;
        public InfoData[] PlayerDetais;

        protected override void OnFreshView()
        {
            var cgdData = GetData<CombatGainsDetailItemData>();
            if (cgdData == null) return;

            name = cgdData.ReplayId;
            if (RoundLabel != null) RoundLabel.text = string.Format("第{0}局", cgdData.Index);
            if (TimeLabel != null) TimeLabel.text = cgdData.CreateDt;
            if (ResultLabel != null) ResultLabel.text = cgdData.Score.ToString();
            var infos = cgdData.Infos;
            if (infos == null) return;
            var count = PlayerDetais.Length;
            var len = Mathf.Min(infos.Count, count);
            var i = 0;
            for (; i < len; i++)
            {
                var ciData = infos[i]; 
                var detais = PlayerDetais[i];
                var goldL = detais.GoldLabel;
                var userL = detais.UserNameLabel; 
                if (goldL != null)
                {
                    goldL.text = ciData.Value.ToString();
                    goldL.gameObject.SetActive(true);
                }
                if (userL != null)
                {
                    userL.text = ciData.Name;
                    userL.gameObject.SetActive(true);
                }
            }

            for (; i < count; i++)
            {
                var detais = PlayerDetais[i];
                if (detais == null) return;
                var goldL = detais.GoldLabel;
                if (goldL != null) goldL.gameObject.SetActive(false);
                var userL = detais.UserNameLabel;
                if (userL != null) userL.gameObject.SetActive(false);
            }
        }

        public void OnReplay()
        {
            var cgdData = GetData<CombatGainsDetailItemData>();
            if (cgdData == null) return;
            if (!string.IsNullOrEmpty(cgdData.Url))
            {
                Application.OpenURL(cgdData.GetFullUrl());
                return;
            }
            var ctr = CombatGainsController.Instance;
            if (ctr.CurType < 1) return;
            var replayData = new ReplayData
                {
                    ReplayId = name,
                    GameKey = ctr.CurGameKey,
                    Type = ctr.CurType,
                    DetailData = cgdData
                };
            CombatGainsController.Instance.JoinReplay(replayData);
        }
    } 
}
