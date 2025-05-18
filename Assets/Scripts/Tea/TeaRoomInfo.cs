using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Tea
{
    public class TeaRoomInfo : YxNguiWindow
    {
        public string TeaId;
        public TeaRoomInfoItem InfoItem;
        public TeaStatisticsItem StatisticsItem;
        public UIGrid grid;
        public GameObject DangQianUp;
        public GameObject DangQianDown;
        public GameObject LiShiUp;
        public GameObject LiShiDown;
        public GameObject YesterdayUp;
        public GameObject YesterdayDown;
        public UIScrollView ScrollView;


        private bool _request;
        private int _curPageNum = 1;
        private int _rowIndex = 1;
        private int _totalCount;

        protected override void OnStart()
        {
            GetTableList();
            if (ScrollView != null)
            {
                ScrollView.onMomentumMove = OnDragFinished;
            }
        }

        private void OnDragFinished()
        {
            ScrollView.UpdateScrollbars(true);
            var constraint = ScrollView.panel.CalculateConstrainOffset(ScrollView.bounds.min, ScrollView.bounds.min);
            if (constraint.y <= 1f)
            {
                if (!_request)
                {
                    var currentCount = grid.transform.childCount;
                    if (_totalCount == currentCount)
                    {
                        return;
                    }
                    if (DangQianUp.activeSelf)
                    {
                        GetTableList();
                    }
                    else if(LiShiUp.activeSelf)
                    {
                        GetHisToryList();
                    }
                    else
                    {
                        GetStatisticsList();
                    }

                    _request = true;
                }
            }
        }

        public void GetTableList()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            object obj1 = TeaId;
            dic["id"] = obj1;
            dic["p"] = _curPageNum++;
            Facade.Instance<TwManager>().SendAction("group.historyRoom", dic, GetTableItem);
        }

        void GetTableItem(object msg)
        {
            Dictionary<string, object> dic = (Dictionary<string, object>)msg;
            if (dic.ContainsKey("totalCount"))
            {
                _totalCount = int.Parse(dic["totalCount"].ToString());
            }
            object obj = dic["data"];
            List<object> objList = (List<object>)obj;
            foreach (Dictionary<string, object> info in objList)
            {
                TeaRoomInfoItem item = YxWindowUtils.CreateItem(InfoItem, grid.transform);
                RoomInfoData roomInfo = new RoomInfoData();
                roomInfo.ParseData(info);
                item.UpdateView(roomInfo);
            }
            grid.Reposition();
            _request = false;
            if (ScrollView != null&&_curPageNum==2)
            {
                ScrollView.ResetPosition();
            }
        }

        public void DangQianClick()
        {
            _curPageNum = 1;
            grid.transform.DestroyChildren();
            GetTableList();
            DangQianUp.SetActive(true);
            DangQianDown.SetActive(false);
            LiShiUp.SetActive(false);
            LiShiDown.SetActive(true);
            if (YesterdayUp)
            {
                YesterdayUp.SetActive(false);
            }
            if (YesterdayDown)
            {
                YesterdayDown.SetActive(true);
            }
        }

        public void LiShiClick()
        {
            _curPageNum = 1;
            _rowIndex = 1;
            grid.transform.DestroyChildren();
            GetHisToryList();
            DangQianUp.SetActive(false);
            DangQianDown.SetActive(true);
            LiShiUp.SetActive(true);
            LiShiDown.SetActive(false);
            if (YesterdayUp)
            {
                YesterdayUp.SetActive(false);
            }
            if (YesterdayDown)
            {
                YesterdayDown.SetActive(true);
            }
        }

        public void StatisticsClick()
        {
            _curPageNum = 1;
            _rowIndex = 1;
            grid.transform.DestroyChildren();
            GetStatisticsList();
            DangQianUp.SetActive(false);
            DangQianDown.SetActive(true);
            LiShiUp.SetActive(false);
            LiShiDown.SetActive(true);
            if (YesterdayUp)
            {
                YesterdayUp.SetActive(true);
            }
            if (YesterdayDown)
            {
                YesterdayDown.SetActive(false);
            }
        }

        public void GetHisToryList()
        {
            if (TeaId == "")
            {
                return;
            }
            Dictionary<string, object> dic = new Dictionary<string, object>();
            object obj = 1;
            dic["type"] = obj;
            object obj1 = TeaId;
            dic["id"] = obj1;
            dic["p"] = _curPageNum++;
            Facade.Instance<TwManager>().SendAction("group.historyRoom", dic, BackHistoryList);
        }

        private void BackHistoryList(object msg)
        {
            Dictionary<string, object> dic = (Dictionary<string, object>)msg;
            if (dic.ContainsKey("totalCount"))
            {
                _totalCount = int.Parse(dic["totalCount"].ToString());
            }
            object obj = dic["data"];
            List<object> objList = (List<object>)obj;
            foreach (Dictionary<string, object> info in objList)
            {
                TeaRoomInfoItem item = YxWindowUtils.CreateItem(InfoItem, grid.transform);
                item.SetIndex(_rowIndex++);
                item.TeaId = TeaId;
                item.JieSanBt.SetActive(false);
                RoomInfoData roomInfo = new RoomInfoData();
                roomInfo.ParseData(info,true);
                item.UpdateView(roomInfo);
            }
            grid.Reposition();
            _request = false;
            if (ScrollView != null&& _curPageNum == 2)
            {
                ScrollView.ResetPosition();
            }
        }

        public void GetStatisticsList()
        {
            if (TeaId == "")
            {
                return;
            }
            Dictionary<string, object> dic = new Dictionary<string, object>();
            object obj = 1;
            dic["type"] = obj;
            object obj1 = TeaId;
            dic["id"] = obj1;
            dic["p"] = _curPageNum++;
            Facade.Instance<TwManager>().SendAction("group.teaStatistics", dic, BackStatisticsList);
        }

        private void BackStatisticsList(object msg)
        {
            Dictionary<string, object> dic = (Dictionary<string, object>)msg;
            if (dic.ContainsKey("totalCount"))
            {
                _totalCount = int.Parse(dic["totalCount"].ToString());
            }
            object obj = dic["data"];
            List<object> objList = (List<object>)obj;
            foreach (Dictionary<string, object> info in objList)
            {
                SatisticsData satisticsData = new SatisticsData(info);
                TeaStatisticsItem item = YxWindowUtils.CreateItem(StatisticsItem, grid.transform);
                item.UpdateView(satisticsData);
            }
            grid.Reposition();
            _request = false;
            if (ScrollView != null && _curPageNum == 2)
            {
                ScrollView.ResetPosition();
            }
        }



        public void CreateUserInfoWindow(TeaRoomInfoItem item)
        {
            YxWindow obj = CreateChildWindow("TeaUserInfoPanel");
            TeaUserInfoPanel infoPanel = obj.GetComponent<TeaUserInfoPanel>();
            infoPanel.GameName.text = item.RealGameName;
            infoPanel.RoomId.text = item.RoomId.text;
            infoPanel.RoundAndUse.text =string.Format("{0} {1}房卡", item.RealGameRound, item.UseNum);
            string rule = item.InfoStr;
            string[] strList = rule.Split(' ');
            rule = "";
            for (int i = 0; i < strList.Length; i++)
            {
                if (strList[i] == "")
                {
                    continue;
                }
                if (i == strList.Length - 1)
                {
                    rule += strList[i];
                    continue;
                }
                rule += strList[i] + "\n";
            }
            infoPanel.RuleInfo.text = rule;
            for (int i = 0; i < item.Golds.Length; i++)
            {
                infoPanel.UserNames[i].text = item.Names[i].text;
                infoPanel.UserNames[i].gameObject.SetActive(true);
                infoPanel.Scores[i].text =YxUtiles.GetShowNumber(long.Parse(item.Golds[i])).ToString();
                infoPanel.Scores[i].gameObject.SetActive(true);
                infoPanel.Heads[i].mainTexture = item.Avatars[i].GetTexture();
                infoPanel.Heads[i].gameObject.SetActive(true);
                if (infoPanel.Ids.Length != 0)
                {
                    infoPanel.Ids[i].text = item.Ids[i];
                }            
            }
        }

    }
}
