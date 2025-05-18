using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Tea
{
    public class TeaListWindow : YxNguiWindow
    {
        public TweenPosition CurrentObj;

        public BoxCollider CurrentBg;

        public UIScrollView ScrollView;

        public UIGrid TeaListGrid;

        public TeaListItem ItemPrefab;

        public UILabel TeaId;

        public UILabel TeaName;

        public UILabel TeaRooms;

        public UILabel TeaOwnerId;

        public UILabel TeaDes;

        public UIInput InputTeaId;

        public string KeyTeaId = "_teaId";

        /// <summary>
        /// 当前茶馆功能的命名 可以叫俱乐部 亲友圈 自己填写
        /// </summary>
        public string CurrentName = "茶馆";
        public bool IsShowTeaView;
        public bool Keep;
        private int _totalCount;
        private int _curPageNum = 1;
        private int _curIndex;
        private string _teaId;
        /// <summary>
        /// 用于存储茶馆ID的key
        /// </summary>
        private string SaveTeaId
        {
            get { return string.Format("{0}_{1}_{2}", Application.bundleIdentifier, App.UserId, KeyTeaId); }
        }

        protected void Start()
        {
            if (PlayerPrefs.HasKey(SaveTeaId))
            {
                _teaId = PlayerPrefs.GetString(SaveTeaId);
            }
            RequestGroupList();
        }

        public void RequestGroupList()
        {
            var dic = new Dictionary<string, object>();
            dic["p"] = 1;
            Facade.Instance<TwManager>().SendAction("group.getGroupList", dic, UpdateViewData);

            if (ScrollView != null)
            {
                ScrollView.onStoppedMoving = OnDragFinished;
            }
        }

        private void UpdateViewData(object msg)
        {
            var info = msg as Dictionary<string, object>;
            if (info == null) return;
            var data = info.ContainsKey("data") ? info["data"] : null;
            var list = data as List<object>;
            if (list == null || list.Count < 1)
            {
                YxMessageBox.Show("等待其他玩家创建或者自己创建");
                Close();
                return;
            }
            FreshTeaView(list);
            OnSetItems(list, TeaListGrid);
        }

        private void FreshTeaView(IList<object> itemLis)
        {
            if (IsShowTeaView)
            {
                for (int i = 0; i < itemLis.Count; i++)
                {
                    var teaInfo = itemLis[i] as Dictionary<string, object>;
                    if (teaInfo != null)
                    {
                        var teaId = teaInfo.ContainsKey("tea_id") ? teaInfo["tea_id"] : null;

                        if (string.IsNullOrEmpty(_teaId))
                        {
                            if (i == 0)
                            {
                                var teaName = teaInfo.ContainsKey("tea_name") ? teaInfo["tea_name"] : null;
                                var groupSign = teaInfo.ContainsKey("group_sign") ? teaInfo["group_sign"] : null;
                                var ownerId = teaInfo.ContainsKey("user_id") ? teaInfo["user_id"] : null;
                                var numTotal = teaInfo.ContainsKey("num_c") ? teaInfo["num_c"] : null;
                                var curRooms = teaInfo.ContainsKey("curRooms") ? teaInfo["curRooms"] : null;
                                var roomNum = string.Format("{0}/{1}", curRooms, numTotal);
                                if (TeaName && teaName != null) TeaName.text = teaName.ToString();
                                if (TeaOwnerId && ownerId != null) TeaOwnerId.text = ownerId.ToString();
                                if (TeaRooms && !string.IsNullOrEmpty(roomNum)) TeaRooms.text = roomNum;
                                if (TeaDes && groupSign != null) TeaDes.text = groupSign.ToString();
                                if (TeaId) TeaId.text = teaId.ToString();
                                _teaId = teaId.ToString();
                            }
                        }
                        else
                        {
                            if (_teaId.Equals(teaId))
                            {
                                var teaName = teaInfo.ContainsKey("tea_name") ? teaInfo["tea_name"] : null;
                                var groupSign = teaInfo.ContainsKey("group_sign") ? teaInfo["group_sign"] : null;
                                var ownerId = teaInfo.ContainsKey("user_id") ? teaInfo["user_id"] : null;
                                var numTotal = teaInfo.ContainsKey("num_c") ? teaInfo["num_c"] : null;
                                var curRooms = teaInfo.ContainsKey("curRooms") ? teaInfo["curRooms"] : null;
                                var roomNum = string.Format("{0}/{1}", curRooms, numTotal);
                                if (TeaName && teaName != null) TeaName.text = teaName.ToString();
                                if (TeaOwnerId && ownerId != null) TeaOwnerId.text = ownerId.ToString();
                                if (TeaRooms && !string.IsNullOrEmpty(roomNum)) TeaRooms.text = roomNum;
                                if (TeaDes && groupSign != null) TeaDes.text = groupSign.ToString();
                                if (TeaId) TeaId.text = teaId.ToString();
                            }
                        }
                    }
                }
            }
        }

        public void OpenFindTeaWindow(string objName)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();

            if (objName.Equals("quickJoinTeaBtn"))
            {
                dic["id"] = InputTeaId.value;
            }
            else
            {
                dic["id"] = _teaId;
            }
            Facade.Instance<TwManager>().SendAction("group.teaGetIn", dic, GetInTea);

        }

        private void GetInTea(object msg)
        {
            var dic = (Dictionary<string, object>)msg;
            var value = (long)dic["mstatus"];

            if (value != 4)
            {
                if (!string.IsNullOrEmpty(InputTeaId.value))
                {
                    _teaId = InputTeaId.value;
                }
                var obj = YxWindowManager.OpenWindow("TeaPanel");
                var panel = obj.GetComponent<TeaPanel>();
                panel.UpdateView(dic);
                panel.SetTeaCode(int.Parse(_teaId));
                PlayerPrefs.SetString(SaveTeaId, _teaId);
                if (!Keep)
                {
                    Close();
                } 
            }
            else
            {
                InputTeaId.value = "";
                YxMessageBox.Show(string.Format("您查找的{0}不存在或者您填写的{0}ID不对", CurrentName));
            }
        }

        private void OnSetItems(IList<object> itemList, UIGrid grid, int startIndex = 0)
        {
            var count = itemList.Count;
            _curIndex = startIndex;
            for (var i = 0; i < count; i++)
            {
                var obj = itemList[i];
                if (obj == null) continue;
                var item = YxWindowUtils.CreateItem(ItemPrefab, grid.transform);
                item.Id = (_curIndex + 1).ToString();
                item.UpdateView(obj);
                _curIndex++;
            }
            grid.repositionNow = true;
            grid.Reposition();
        }

        private void OnDragFinished()
        {
            if (TeaListGrid.transform.childCount == _totalCount && _totalCount != -1) return;
            ScrollView.UpdateScrollbars(true);
            var constraint = ScrollView.panel.CalculateConstrainOffset(ScrollView.bounds.min, ScrollView.bounds.min);
            if (constraint.y <= 1f)
            {
                var dic = new Dictionary<string, object>();
                dic["p"] = ++_curPageNum;
                Facade.Instance<TwManager>().SendAction("group.getGroupList", dic, AddItems);
            }
        }


        private void AddItems(object msg)
        {
            var info = msg as Dictionary<string, object>;
            if (info == null) return;
            _totalCount = info.ContainsKey("totalCount") ? int.Parse(info["totalCount"].ToString()) : -1;
            var data = info.ContainsKey("data") ? info["data"] : null;
            var list = data as List<object>;
            if (list == null) return;
            OnSetItems(list, TeaListGrid, _curIndex);
        }


        public void OnOpen()
        {
            RequestGroupList();
            CurrentBg.enabled = true;
            CurrentObj.enabled = true;
            CurrentObj.PlayForward();
            CurrentObj.onFinished.Clear();
        }

        public void OnClose()
        {

            CurrentBg.enabled = false;
            CurrentObj.PlayReverse();
            CurrentObj.onFinished.Add(new EventDelegate(() =>
            {
                TeaListGrid.transform.DestroyChildren();
            }));
        }

    }
}
