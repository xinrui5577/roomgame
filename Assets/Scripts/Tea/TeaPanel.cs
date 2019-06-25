using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.Windows;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Controller;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Tea
{
    public class TeaPanel : YxNguiWindow
    {
        public GameObject AddGame;
        public GameObject ShenQing;
        public GameObject GuanLiBts;

        public UILabel Code;

        public UILabel Name;

        public UIGrid Grid;

        public UILabel TableNumLabel;

        [Tooltip("积分文本")]
        public UILabel CoinTLabel;
        [Tooltip("金币文本")]
        public UILabel CoinALabel;

        public float AutoFrashTime = 10f;

        public int roomNum;
        [Tooltip("积分字段")]
        public string KeyCoinT= "coin_t";


        //以下两个一起使用，根据Gamekey找到预设颜色
        public string [] TableGameKey;
        public Color[] TableColor;

        List<TeaTableItem> ItemList=new List<TeaTableItem>();
        private List<TeaTableItem> UsedItemList;
        public int onlyOwner = -1;

        int _tableNum;
        public int TableNum
        {
            get { return _tableNum; }
            set
            {
                _tableNum = value;
                if (TableNumLabel)
                {
                    TableNumLabel.text = "桌数：" + _tableNum + "/" + roomNum;
                }
                else
                {
                    YxDebug.LogError("TableNumLabel 是空的为毛线");
                }
            }
        }
        //1馆主2是成员3非成员4没有此茶馆
        public int TeaState;

        public TeaTableItem TableItem;

        void Start()
        {
            GetTableList();
            StartCoroutine(AutoFrash());
        }

        IEnumerator AutoFrash()
        {
            while (true)
            {
                yield return new WaitForSeconds(AutoFrashTime);
                GetTableList(false);
            }
            
        }

        public void OpPowerWindow()
        {
            YxWindow obj = CreateChildWindow("TeaMember");
            TeaPower power = obj.GetComponent<TeaPower>();
            power.TeaId = Code.text;
        }

        public void OpTeaRoomInfoWindow()
        {
            YxWindow obj = CreateChildWindow("TeaRoomInfo");
            TeaRoomInfo infoWindow = obj.GetComponent<TeaRoomInfo>();
            infoWindow.TeaId = Code.text;
        }

        public void CasePower(int index)
        {
            switch (index)
            {
                case 1:
                    ShenQing.SetActive(false);
                    GuanLiBts.SetActive(true);
                    if (AddGame != null) AddGame.SetActive(true);
                    break;
                case 2:
                    ShenQing.SetActive(false);
                    GuanLiBts.SetActive(false);
                    if (AddGame != null) AddGame.SetActive(onlyOwner == 0);
                    break;
                case 3:
                    ShenQing.SetActive(true);
                    GuanLiBts.SetActive(false);
                    if (AddGame != null) AddGame.SetActive(false);
                    break;

            }
        }

        public void SetTeaCode(int code)
        {
            Code.text = code + "";
        }

        public void OtherTea()
        {
            CreateOtherWindow("TeaFindRoom");
            Close();
        }

        public void ApplyGetIn()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic["id"] = Code.text;
            Facade.Instance<TwManger>().SendAction("group.teaHouseApply", dic, ApplyBackMs);
        }

        private void ApplyBackMs(object msg)
        {
            TeaUtil.GetBackString(msg);
        }

        public void SetTeaName(string teaName)
        {
            Name.text = teaName;
        }

        private int times = 3;
        private float TimeLag = 2f;
        private float LastSecond = 0;
        public void GetTableList(bool WaitBox=true)
        {
            if (!WaitBox)
            {
                LastSecond = 0;
            }
            float NowSecond = Time.time;
            if (NowSecond - LastSecond < 2f)
            {
                return;
            }
            LastSecond = NowSecond;
            UsedItemList=new List<TeaTableItem>();
            foreach (Transform child in Grid.transform)
            {
                TeaTableItem item = child.GetComponent<TeaTableItem>();
                UsedItemList.Add(item);
            }

            Dictionary<string, object> dic = new Dictionary<string, object>();
            object obj1 = Code.text;
            dic["id"] = obj1;
            if (TeaState == 1 || TeaState == 2)
            {
               RoomListController.Instance.GetGroupRoomList(dic, GetTeaRoomList, WaitBox);
            }
        }
        private void AddEmptyTable()
        {
            TeaTableItem item = YxWindowUtils.CreateItem(TableItem, Grid.transform);
            item.SetTableState(TableState.Empty);
        }

        private float GetTeaRoomListLast = 0;
        private void GetTeaRoomList(object msg)
        {
            if (Time.time-GetTeaRoomListLast<0.1f)
            {
                return;
            }
            if(this==null)
            {
                YxDebug.LogError("当前对象Teapanel是空的........");
                return;
            }
            GetTeaRoomListLast = Time.time;
            TableNum = 0;
            //Grid.transform.DestroyChildren();
            List<object> dic = (List<object>)msg;
            foreach (Dictionary<string, object> dicItem in dic)
            {
                TeaTableItem item;
                if (UsedItemList.Count > 0&&UsedItemList[0]!=null)
                {
                    item = UsedItemList[0];
                    UsedItemList.RemoveAt(0);
                }
                else
                {
                   item = YxWindowUtils.CreateItem(TableItem, Grid.transform); 
                }
                if (TeaState==1)
                {
                    item.SetTableState(TableState.BeforePlay);
                }
                if (TeaState == 2)
                {
                    item.SetTableState(TableState.PlayerBeforPlay);
                }
                
                item.TeaPanel = this;
                RoomInfoData roomInfo = new RoomInfoData();
                roomInfo.ParseGameServerData(dicItem);
                item.UpdateView(roomInfo);
                TableNum++;
                ItemList.Add(item);
            }

            if (TeaState == 1)
            {
                Dictionary<string, object> dic2 = new Dictionary<string, object>();
                object obj1 = Code.text;
                dic2["id"] = obj1;
                Facade.Instance<TwManger>().SendAction("group.historyRoom", dic2, GetTableItem, false,null,false);
            }
            if (TeaState == 2)
            {
                Grid.Reposition();
                foreach (TeaTableItem item in UsedItemList)
                {
                    if (item != null)
                    {
                        Destroy(item.gameObject);
                    }
                }
            }

        }


        void GetTableItem(object msg)
        {

            Dictionary<string, object> dic = (Dictionary<string, object>)msg;
            object obj = dic["history"];
            List<object> objList = (List<object>)obj;
            foreach (Dictionary<string, object> info in objList)
            {
                TeaTableItem item;
                if (UsedItemList.Count > 0 && UsedItemList[0] != null)
                {
                    item = UsedItemList[0];
                    UsedItemList.RemoveAt(0);
                }
                else
                {
                    item = YxWindowUtils.CreateItem(TableItem, Grid.transform);
                }
                item.TeaPanel = this;
                item.SetTableState(TableState.Over);
                RoomInfoData roomInfo = new RoomInfoData();
                roomInfo.ParseData(info);
                item.UpdateView(roomInfo);
                TableNum++;
                ItemList.Add(item);
            }
            foreach (TeaTableItem item in UsedItemList)
            {
                if (item!=null)
                {
                    Destroy(item.gameObject);
                }    
            }
            if(Grid)
            {
                Grid.Reposition();
            }
        }

        public void ClickFlesh()
        {
            GetTableList();
        }

        public void OnTaskInfo()
        {
            var win = CreateChildWindow("DefTaskWindow");
            //if (win == null) return;
            //win.UpdateView(HallModel.Instance.OptionSwitch.Task);
        }

        protected override void OnFreshView()
        {
            if(Data==null)
            {
                return;
            }
            if(Data is Dictionary<string,object>)
            {
                var dics = Data as Dictionary<string, object>;
                long coin_t;
                YxTools.TryGetValueWitheKey(dics, out coin_t, KeyCoinT);
                YxTools.TrySetComponentValue(CoinTLabel, coin_t.ToString());
                YxTools.TrySetComponentValue(CoinALabel, YxUtiles.GetShowNumber(UserInfoModel.Instance.UserInfo.CoinA).ToString());
            }
        }

        public void OnOpenCreateWindow(string objName)
        {
            if (_tableNum>=roomNum)
            {
                YxMessageBox.Show("房间数已达上限，无法继续创建房间"); 
                return;
            }

            //var win = YxWindowManager.OpenWindow("TeaCreateRoomWindow", true);
            var win = CreateChildWindow("TeaCreateRoomWindow");
            var createWin = (TeaCreateRoomWindow)win;
            if (createWin == null) return;
            createWin.teaPanel = this;
            createWin.GameKey = "";
        }

        public override void Close()
        {
            base.Close();
            //UserController.Instance.GetUserDate();
            UserController.Instance.GetBackPack();
        }
    }
}
