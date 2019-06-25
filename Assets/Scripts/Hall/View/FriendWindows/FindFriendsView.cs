using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Controller;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View.FriendWindows
{
    public class FindFriendsView : YxView
    {
        public UIInput FindInput;
        public AddFriendItemView PerfabItem;
        public UIGrid PerfabListGrid;
        private UIGrid _curItemParent;

        public void OnFindFriend()
        {
            CreateItemParent();
            FriendController.Instance.SendFindUser(FindInput.value,OnFindSuccess);
        }

        private void OnFindSuccess(object msg)
        {
            SetData(msg, _curItemParent.transform);
//            var list = (List<object>)msg;
//            if (list == null || list.Count < 1) SetData(msg, _curItemParent.transform);
//            else SetListData(list, _curItemParent.transform);
            _curItemParent.repositionNow = true;
            _curItemParent.Reposition();
        }

        private void SetListData(IList<object> list, Transform parentTs)
        {
            var count = list.Count;
            for (var i = 0; i < count; i++)
            {
                SetData(list[i],parentTs);
            }
        }

        private void SetData(object msg,Transform parentTs)
        {
            var item = Instantiate(PerfabItem);
            var friendInfo = new UserInfo();
            friendInfo.Parse((IDictionary)msg);
            item.UpdateView(friendInfo);
            var ts = item.transform;
            ts.parent = parentTs;
            ts.localScale = Vector3.one;
            ts.localPosition = Vector3.zero;
            item.gameObject.SetActive(true);
        }

        private void CreateItemParent()
        {
            if (_curItemParent!=null)
            {
                Destroy(_curItemParent.gameObject);
            }
            var perfabTs = PerfabListGrid.transform;
            _curItemParent = Instantiate(PerfabListGrid);
            var ts = _curItemParent.transform;
            ts.parent = perfabTs.parent;
            ts.gameObject.SetActive(true);
            ts.localPosition = perfabTs.localPosition;
            ts.localScale = perfabTs.localScale;
            ts.localRotation = perfabTs.localRotation;
        }
    }
}
