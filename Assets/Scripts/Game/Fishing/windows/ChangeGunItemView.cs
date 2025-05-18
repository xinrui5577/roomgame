using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.Fishing.datas;
using Assets.Scripts.Game.Fishing.entitys;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework;

namespace Assets.Scripts.Game.Fishing.windows
{
    /// <summary>
    /// 切换大炮item
    /// </summary>
    public class ChangeGunItemView : YxView
    {
        public GameObject LockSign;
        public GameObject SelectedSign;
        public GameObject LockTip;
        public YxBaseLabelAdapter CostLabel;
        public YxBaseLabelAdapter GainLabel;

        protected override void OnAwake()
        {
            base.OnAwake();
            SelectedSign.SetActive(false);
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var data = GetData<ChangeGunData>();
            if (data == null) return;
            SetLabel(CostLabel, data.Bet);
            SetLabel(GainLabel, data.Bet);
            SetSign(data.IsSelected);
            SetLock(data.IsLock);
            SetLockTip(data.HasLockTip);
        }

        private void SetLock(bool isLock)
        {
            if (LockSign != null)
            {
                LockSign.SetActive(isLock);
            } 
        }
        private void SetLockTip(bool needShow)
        {
            if (LockTip!=null)
            {
                LockTip.SetActive(needShow);
            }
        }

        private void SetSign(bool b)
        {
            if (SelectedSign == null) return;
            SelectedSign.SetActive(b);
        }

        private void SetLabel(YxBaseLabelAdapter label,int value)
        {
            if (label == null) return;
            label.Text(value.ToString());
        }

        public int GunLeve;
        public void OnChangeGun()
        {
            var battery = App.GameData.GetPlayer<Battery>();
            var player = battery.Player;
            player.GunLeve = GunLeve; 
            player.BetLeve = 0; 
            Close();
        }
    }
}
