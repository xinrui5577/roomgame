using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Controller;
using YxFramwork.Framework;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View.FriendWindows
{
    public class FriendMessageView : YxView
    {
        public GameObject NoFriendTip;
        public GameObject HasFriendTip;

        public FriendMessageItemView PerfabItem;
        public UIGrid ListGrid;
        private bool _hasSend;
        private UIGrid _curItemParent;
        private readonly List<FriendMessageItemView> _items = new List<FriendMessageItemView>(); 
        protected override void OnEnableEx()
        {
            FreshView();
        }

        protected override void OnFreshView()
        {
            if (_hasSend) return;
            _hasSend = true;
            FriendController.Instance.SendApplyInfoNote(OnSuccessInfo);
        } 

        private void OnSuccessInfo(object msg)
        {
            var list = (List<object>)msg;
            _hasSend = false;
            if (list == null || list.Count < 1)
            {
                UpdateFriendTip();
                return;
            } 
            SetData(list);
            UpdateFriendTip();
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
                item.SetParentView(this);
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

        public void Reposition()
        {
            if (_curItemParent == null) return;
            _curItemParent.repositionNow = true;
            _curItemParent.Reposition();
        }

        public void OnDeleteFriend(FriendMessageItemView friendItem)
        {
            YxMessageBox.Show("确定要删除该消息吗?", "", (box, btnName) =>
            {
                if (btnName == YxMessageBox.BtnLeft)
                {
                    var uinfo = friendItem.GetData<UserInfo>();
                    if (uinfo == null) return;
                    FriendController.Instance.SendApplyUser(uinfo.Id, "4", msg =>
                    {
                        if (!(msg is Dictionary<string, object>)) return;
                        var parm = (Dictionary<string, object>)msg;
                        if (!parm.ContainsKey("message")) YxMessageBox.Show("已成功删除消息！");
                        _items.Remove(friendItem);
                        Destroy(friendItem.gameObject);
                        _curItemParent.repositionNow = true;
                        _curItemParent.Reposition();
                        UpdateFriendTip();
                    });
                }
            }, true, YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle);
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

        public void DeleteItem(FriendMessageItemView friendMessageItemView)
        {
            _items.Remove(friendMessageItemView);
            Destroy(friendMessageItemView.gameObject);
            Reposition();
            UpdateFriendTip();
        }

        private void UpdateFriendTip()
        {
            var hasFriendMsg = _items.Count > 0;
            if (NoFriendTip != null) NoFriendTip.gameObject.SetActive(!hasFriendMsg);
            if (HasFriendTip != null) HasFriendTip.gameObject.SetActive(hasFriendMsg);
        }
    }
}
