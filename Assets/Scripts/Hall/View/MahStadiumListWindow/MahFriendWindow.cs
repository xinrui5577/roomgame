using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.Common.Windows;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Hall.View.MahStadiumListWindow
{
    public class MahFriendWindow : YxNguiWindow
    {
        public UIGrid FriendItemGrid;
        public UIWidget ParentWidget;
        public UIScrollView ScrollView;
        public UIScrollBar ScrollBar;
        public GameObject NoDataTexture;

        public MahFriendItem MahFriendItem;
        [Tooltip("MainWindow需要隐藏的内容")]
        public GameObject[] HideUIs;

        private new void OnEnable()
        {
            YxWindowUtils.DisplayUI(HideUIs, false);
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            var dic = new Dictionary<string, object>();
            Facade.Instance<TwManager>().SendAction("mahjongwm.wmFriends", dic, OnFreshData);
        }
        

        protected override void OnDisable()
        {
            YxWindowUtils.DisplayUI(HideUIs);
        }

        protected void OnFreshData(object info)
        {
            while (FriendItemGrid.transform.childCount > 0)
            {
                DestroyImmediate(FriendItemGrid.transform.GetChild(0).gameObject);
            }
            var dataInfo = info as Dictionary<string, object>;
            if (dataInfo == null) return;
            var data = dataInfo.ContainsKey("data") ? dataInfo["data"] : null;
            if (!(data is List<object>)) return;
            var friendDatas = data as List<object>;
            if (friendDatas.Count == 0)
            {
                if(NoDataTexture) NoDataTexture.gameObject.SetActive(true);
                return;
            }
            foreach (var friendData in friendDatas)
            {
                if (!(friendData is Dictionary<string, object>)) continue;
                var friendInfo = friendData as Dictionary<string, object>;
                var obj = YxWindowUtils.CreateItem(MahFriendItem, FriendItemGrid.transform);
                var userId = friendInfo.ContainsKey("user_id") ? int.Parse(friendInfo["user_id"].ToString()) : 0;
                var nickM = friendInfo.ContainsKey("nick_m") ? friendInfo["nick_m"].ToString() : "";
                var avatarX = friendInfo.ContainsKey("avatar_x") ? friendInfo["avatar_x"].ToString() : "";
                obj.name = userId.ToString(CultureInfo.InvariantCulture);
                obj.InitData(avatarX, nickM, userId);
            }
            ScrollView.ResetPosition();
            FriendItemGrid.repositionNow = true;
        }

        public void OnDeleteBtn(GameObject obj)
        {
            var dic = new Dictionary<string, object>();
            dic["delId"] = obj.name;
            Facade.Instance<TwManager>().SendAction("mahjongwm.delWmFriends", dic, (data) =>
                {
                    var dicF = new Dictionary<string, object>();
                    Facade.Instance<TwManager>().SendAction("mahjongwm.wmFriends", dicF, OnFreshData);
                });
        }

        public void OnChangeAlpha()
        {
            if (ScrollBar.alpha <= 0)
            {
                ParentWidget.transform.localScale=new Vector3(0,1,1);
            }
            else
            {
                ParentWidget.transform.localScale = new Vector3(1, 1, 1);
            }
        }

    }
}
