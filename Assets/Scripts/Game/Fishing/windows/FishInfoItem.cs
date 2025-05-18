using Assets.Scripts.Game.Fishing.datas;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Framework;

namespace Assets.Scripts.Game.Fishing.windows
{
    public class FishInfoItem : YxView
    {
        /// <summary>
        /// 
        /// </summary>
        public Image Icon;

        public Toggle TheToggle;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var data = GetData<FishDescriptionData>();
            if (data == null) return;
            Icon.sprite = data.Icon;
        }

        public void SetSelected(bool b)
        {
            if (TheToggle == null) return;
            TheToggle.isOn = b;
        }
    }
}
