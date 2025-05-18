using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Controller;
using YxFramwork.Framework;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View.FriendWindows
{
    public class FriendListView : YxView
    {
        public GameObject NoFriendTip;
        public GameObject HasFriendTip;

        public FriendItemView PerfabItem;
        public UIGrid ListGrid;
        private bool _hasSend;
        private UIGrid _curItemParent;
        private readonly List<FriendItemView> _items = new List<FriendItemView>(); 

        protected override void OnEnable()
        {
            FreshView();
        }

        protected override void OnFreshView()
        {
            if (_hasSend) return;
            _hasSend = true;
            FriendController.Instance.SendFriendList(OnSuccessInfo);
        }


        private void OnSuccessInfo(object msg)
        { 
            var list = (List<object>)msg;
            _hasSend = false;
            if (list == null || list.Count < 1)
            {
                if (NoFriendTip != null) NoFriendTip.gameObject.SetActive(true);
                if (HasFriendTip != null) HasFriendTip.gameObject.SetActive(false);
                return;
            }
            if (NoFriendTip != null) NoFriendTip.gameObject.SetActive(false);
            if (HasFriendTip != null) HasFriendTip.gameObject.SetActive(true);
            SetData(list); 
        }

        /// <summary>
        /// 设置数据 
        /// </summary>
        /// <param name="lists"></param>
        private void SetData(IList<object> lists)
        {
            if (lists == null) return;
            _items.Clear();
            CreateItemParent();
            var count = lists.Count;
            var pts = _curItemParent.transform;
            for (var i = 0; i < count; i++)
            { 
                var item = Instantiate(PerfabItem);
                var friendInfo = new UserInfo();
                friendInfo.Parse((IDictionary)lists[i]);
                item.UpdateView(friendInfo);
                var ts = item.transform;
                ts.parent = pts;
                ts.localScale = Vector3.one;
                ts.localPosition = Vector3.zero;
                item.gameObject.SetActive(true);
                _items.Add(item);
            }
            _curItemParent.repositionNow = true;
            _curItemParent.Reposition();
        }

        public void OnDeleteFriend(FriendItemView friendItem)
        {
            if (friendItem == null) return;
            var uinfo = friendItem.GetData<UserInfo>();
            if (uinfo == null) return;
            YxMessageBox.Show(string.Format("确定要删除【{0}】好友吗?",uinfo.NickM), "", (box, btnName) =>
                {
                    if (btnName == YxMessageBox.BtnLeft)
                    { 
                        FriendController.Instance.SendDeleteFriend(uinfo.UserId,msg =>
                            {
                                if (!(msg is Dictionary<string, object>)) return;
                                var parm = (Dictionary<string, object>)msg;
                                if(!parm.ContainsKey("message"))YxMessageBox.Show("好友已成功删除！");
                                _items.Remove(friendItem);
                                Destroy(friendItem.gameObject); 
                                _curItemParent.repositionNow = true;
                                _curItemParent.Reposition();
                                var hasFriend = _items.Count > 0;
                                if (NoFriendTip != null) NoFriendTip.gameObject.SetActive(!hasFriend);
                                if (HasFriendTip != null) HasFriendTip.gameObject.SetActive(hasFriend);
                            });
                    }
                },true,YxMessageBox.LeftBtnStyle|YxMessageBox.RightBtnStyle);
        }

        private void CreateItemParent()
        {
            if (_curItemParent != null) Destroy(_curItemParent.gameObject);
            var perfabTs = ListGrid.transform;
            _curItemParent = Instantiate(ListGrid);
            var ts = _curItemParent.transform;
            ts.parent = perfabTs.parent;
            ts.gameObject.SetActive(true);
            ts.localPosition = perfabTs.localPosition;
            ts.localScale = perfabTs.localScale;
            ts.localRotation = perfabTs.localRotation;
        }
    }
}
