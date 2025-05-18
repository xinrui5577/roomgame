using System.Collections.Generic;
using Assets.Scripts.Common.UI;
using Assets.Scripts.Common.Utils;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common.DataBundles;
using YxFramwork.Common.Model;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View
{
    /// <summary>
    /// 设置头像
    /// </summary>
    public class ChangePortraitView : YxView
    {

        public NguiToggle PrefabPortraitItem;
        public UIGrid ItemGrid;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var names = PortraitDb.HeadNames;
            var count = names.Length;
            var pts = ItemGrid.transform;
            for (var i = 0; i < count; i++)
            {
                var item = YxWindowUtils.CreateItem(PrefabPortraitItem, pts);
                var sName = names[i];
                item.name = sName;
                item.Background.spriteName = sName;
                item.Checkmark.spriteName = sName;
            }
            ItemGrid.repositionNow = true;
            ItemGrid.Reposition();
        }

        private UIToggle _curToggle;

        public void OnToggleChange()
        {
            _curToggle = UIToggle.current;
        }

        public void OnChangePortrait()
        {
            if (_curToggle == null) return;
            var parm = new Dictionary<string, object>();
            parm["avatar_x"] = _curToggle.name;
            Facade.Instance<TwManager>().SendAction("changeAvatar", parm, OnChangeFinish);
        }

        private void OnChangeFinish(object msg)
        {
            Debug.Log(msg);
            if (msg == null) return;
            var avatar = msg.ToString();
            if (string.IsNullOrEmpty(avatar)) return;
            var userModel = UserInfoModel.Instance;
            userModel.UserInfo.AvatarX = avatar;
            userModel.Save();
            YxMessageBox.Show("头像设置成功！");
        }
    }
}
