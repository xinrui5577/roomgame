using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Enums;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Common.Managers
{
    /// <summary>
    /// 显示管理
    /// </summary>
    public class YxDisplayManager : InterimMono
    {
        /// <summary>
        /// 菜单按钮（受开关管理）
        /// </summary>
        public GameObject[] GameObjects;
        protected override void OnAwake()
        {
            base.OnAwake();
            AddListeners("HallWindow_hallMenuChange", OnFreshMenu);
            OnFreshMenu();

        }

        private void OnFreshMenu(object msg=null)
        {
            var menustate = App.AppStyle == YxEAppStyle.Concise ? 1 : HallModel.Instance.OptionSwitch.HallMenue;
            //隐藏按钮
            SetBtnsActive(menustate);
        }
        protected int SetBtnsActive(int menustate)
        {
            var count = GameObjects.Length;
            var btnsCount = 0;
            for (var i = 0; i < count; i++)
            {
                var btn = GameObjects[i];
                if (btn == null) continue;
                var show = 1 << i;
                var isShow = (menustate & show) == show;
                btn.SetActive(isShow);
                if (isShow)
                {
                    btnsCount++;
                }
            }
            return btnsCount;
        }
    }
}
