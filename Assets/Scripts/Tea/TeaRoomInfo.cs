using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.Windows;
using Assets.Scripts.Common.Windows.TabPages;
using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Tea
{
    public class TeaRoomInfo : YxNguiWindow
    {
        public string TeaId;
        public TeaRoomInfoItem InfoItem;
        public UIGrid grid;
        public GameObject DangQianUp;
        public GameObject DangQianDown;
        public GameObject LiShiUp;
        public GameObject LiShiDown;
       

        void Start()
        {
            GetTableList();
        }

        public void GetTableList()
        {
            grid.transform.DestroyChildren();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            object obj1 = TeaId;
            dic["id"] = obj1;
            Facade.Instance<TwManger>().SendAction("group.historyRoom", dic, GetTableItem);
        }

        void GetTableItem(object msg)
        {
            Dictionary<string, object> dic = (Dictionary<string, object>)msg;
            object obj = dic["history"];
            List<object> objList = (List<object>)obj;
            foreach (Dictionary<string, object> info in objList)
            {
                TeaRoomInfoItem item = YxWindowUtils.CreateItem(InfoItem, grid.transform);
                RoomInfoData roomInfo = new RoomInfoData();
                roomInfo.ParseData(info);
                item.UpdateView(roomInfo);
            }
            grid.Reposition();
        }

        public void DangQianClick()
        {
            GetTableList();
            DangQianUp.SetActive(true);
            DangQianDown.SetActive(false);
            LiShiUp.SetActive(false);
            LiShiDown.SetActive(true);
        }

        public void LiShiClick()
        {
            GetHisToryList();
            DangQianUp.SetActive(false);
            DangQianDown.SetActive(true);
            LiShiUp.SetActive(true);
            LiShiDown.SetActive(false);
        }

        public void GetHisToryList()
        {
            if (TeaId == "")
            {
                return;
            }
            grid.transform.DestroyChildren();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            object obj = 1;
            dic["type"] = obj;
            object obj1 = TeaId;
            dic["id"] = obj1;
            Facade.Instance<TwManger>().SendAction("group.historyRoom", dic, BackHistoryList);
        }

        private void BackHistoryList(object msg)
        {
            int RowIndex = 1;
            Dictionary<string, object> dic = (Dictionary<string, object>)msg;
            object obj = dic["history"];
            List<object> objList = (List<object>)obj;
            foreach (Dictionary<string, object> info in objList)
            {
                TeaRoomInfoItem item = YxWindowUtils.CreateItem(InfoItem, grid.transform);
                item.SetIndex(RowIndex++);
                item.TeaId = TeaId;
                item.JieSanBt.SetActive(false);
                RoomInfoData roomInfo = new RoomInfoData();
                roomInfo.ParseData(info,true);
                item.UpdateView(roomInfo);
            }
            grid.Reposition();
        }
     
        public void CreateUserInfoWindow(TeaRoomInfoItem item)
        {
            YxWindow obj = CreateChildWindow("TeaUserInfoPanel");
            TeaUserInfoPanel infoPanel = obj.GetComponent<TeaUserInfoPanel>();
            infoPanel.GameName.text = item.RealGameName;
            infoPanel.RoomId.text = item.RoomId.text;
            infoPanel.RoundAndUse.text = item.RealGameRound+"圈" + " " + item.UseNum+"房卡";
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
                infoPanel.Scores[i].text = item.Golds[i];
                infoPanel.Scores[i].gameObject.SetActive(true);
                infoPanel.Heads[i].mainTexture = item.Avatars[i].mainTexture;
                infoPanel.Heads[i].gameObject.SetActive(true);
                if (infoPanel.Ids.Length != 0)
                {
                    infoPanel.Ids[i].text = item.Ids[i];
                }            
            }
        }

    }
}
