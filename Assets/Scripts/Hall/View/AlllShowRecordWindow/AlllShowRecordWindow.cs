using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Hall.View.AlllShowRecordWindow
{
    /// <summary>
    /// 用于战绩的显示 （回放 和整张房卡玩家的总输赢 和单局输赢 以及回放同时显示 ）
    /// </summary>
    public class AlllShowRecordWindow : YxNguiWindow
    {
        public RoundDataItem RoundDataItem;
        public Transform BottomPos;
        public string GameKey;

        public UIGrid TabGrid;
        public TabItemSpecial[] TabList = new TabItemSpecial[3];

        private string webHost;
        protected void Start()
        {
            var dic = new Dictionary<string, object>();
            dic["game_key_c"] = GameKey;
            dic["p"] = 1;
            Facade.Instance<TwManager>().SendAction("gameHistoryReplay", dic, OnFreshData);
        }
        private void OnFreshData(object data)
        {
            if (data==null)return;
            if (!(data is List<object>)) return;
            var infos = data as List<object>;
            var round = 0;
            if (RoundDataItem==null)return;
           
            foreach (var info in infos)
            {
                var roundData = Instantiate(RoundDataItem);
                roundData.gameObject.SetActive(true);
                roundData.transform.parent = BottomPos;
                roundData.transform.localScale = Vector3.one;
                roundData.transform.localPosition = Vector3.zero;
                var playerInfo = (IDictionary) info;
                var roomId = playerInfo["room_id"].ToString();
                if (roundData.RoomId != null) roundData.RoomId.text = string.Format("房间号：（{0}）", roomId);
                var creatTime = playerInfo["create_dt"].ToString();
                if (roundData.CreatRoomTime != null) roundData.CreatRoomTime.text = "对战时间：" + creatTime;
                var detail = playerInfo["detail"];
                InitPlayerData(detail, roundData);
                var infoH = playerInfo["info_h"];
                roundData.InitEachPlayData(infoH);
                webHost = playerInfo["web_host"].ToString();
               

                round++;
               if (round == 1)
               {
                   var obj = CreatObj(TabList[0].gameObject, TabGrid.transform);
                   obj.GetComponent<TabItemSpecial>().InitData(round, true);
                   obj.GetComponent<TabItemSpecial>().ToggledObjects.activate.Add(roundData.gameObject);
               }
               else
               {
                   if (round > 5&&round == infos.Count)
                   {
                       var obj = CreatObj(TabList[2].gameObject,TabGrid.transform);
                        obj.GetComponent<TabItemSpecial>().InitData(round, false);
                        obj.GetComponent<TabItemSpecial>().ToggledObjects.activate.Add(roundData.gameObject);
                   }
                   else
                   {
                       var obj = CreatObj(TabList[1].gameObject,TabGrid.transform);
                       obj.GetComponent<TabItemSpecial>().InitData(round, false);
                       obj.GetComponent<TabItemSpecial>().ToggledObjects.activate.Add(roundData.gameObject);
                   }
               }
            }
            TabGrid.Reposition();
            TabGrid.repositionNow = true;
        }
        private void InitPlayerData(object dataInfo, RoundDataItem roundDataItem)
        {
            if(dataInfo==null) return;
            if (!(dataInfo is List<object>)) return;
            var datas = dataInfo as List<object>;
            foreach (var data in datas)
            {
                var info = (Dictionary<string, object>) data;
                var replayId = info["replay_id"].ToString();
                var url = info["url"].ToString();
                var infoH = info["info_h"];
                var obj = CreatObj(roundDataItem.PlayerSingleItem.gameObject,roundDataItem.PlayerSingleGrid.transform);
                obj.SetActive(true);
                obj.GetComponent<PlayerSingleItem>().InitEachPlayData(infoH);
                obj.GetComponent<PlayerSingleItem>().ReplayBtn.GetComponent<UIButton>().onClick.Add(new EventDelegate(() => Application.OpenURL(string.Format(webHost + url))));
            }
            roundDataItem.PlayerSingleGrid.Reposition();
            roundDataItem.PlayerSingleGrid.repositionNow = true;
        }
        private GameObject CreatObj(GameObject child, Transform parent)
        {
            var obj = Instantiate(child);
            obj.SetActive(true);
            obj.transform.parent = parent;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            obj.transform.localRotation = Quaternion.identity;
            return obj;
        }
       
    }
}
