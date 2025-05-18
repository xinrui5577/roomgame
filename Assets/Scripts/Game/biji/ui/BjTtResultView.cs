using System;
using System.Collections.Generic;
using Assets.Scripts.Game.biji.EventII;
using Assets.Scripts.Game.biji.network;
using com.yxixia.utile.Utiles;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Controller;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.biji.ui
{
    public class BjTtResultView : MonoBehaviour
    {
        public EventObject EventObj;
        public GameObject View;
        public UILabel RoomId;
        public UILabel StartTime;
        public UIGrid TtResultGrid;
        public BjTtResultItem BjTtResultItem;

        private string _screenshot;
        private string _gameKey;

        public void OnRecieve(EventData data)
        {
            switch (data.Name)
            {
                case "RoomData":
                    OnRoomView(data.Data);
                    break;
                case "Show":
                    OnShow(data.Data);
                    break;
            }
        }

        public void OnRoomView(object data)
        {
            var roomData = (ISFSObject)data;
            _gameKey = roomData.GetUtfString("gameKey");
            RoomId.text = roomData.GetInt("roomId").ToString();
            var time = GetDateTime(roomData.GetLong("startTime"));
            StartTime.text = time.ToString("yyyy-MM-dd HH：mm：ss");
        }

        public void OnShow(object data)
        {
            View.SetActive(true);
            List<TtResultUserData> ttResultUserDatas = (List<TtResultUserData>)data;
            for (int i = 0; i < ttResultUserDatas.Count; i++)
            {
                var item = YxWindowUtils.CreateItem(BjTtResultItem, TtResultGrid.transform);
                item.SetView(ttResultUserDatas[i]);
            }

            TtResultGrid.repositionNow = true;
        }

        public void OnClickShare()
        {
            _screenshot = App.UI.CaptureScreenshot();
            Invoke("CaptureScreenshot", 1f);
        }

        void CaptureScreenshot()
        {
            Facade.Instance<WeChatApi>().InitWechat(App.Config.WxAppId);
            var dic = new Dictionary<string, object>();
            dic["type"] = 0;
            dic["image"] = _screenshot;
            dic["shareType"] = 1;
            dic["sharePlat"] = 0;
            dic["game_key_c"] = _gameKey;
            UserController.Instance.GetShareInfo(dic, info => Facade.Instance<WeChatApi>().ShareContent(info, str =>
            {
            }));
        }

        public void OnReturnHall()
        {
            App.QuitGame();
        }

        public void OnShowRecord()
        {
            EventObj.SendEvent("RecordViewEvent", "Show", null);
        }

        /// <summary>  
        /// 时间戳Timestamp转换成日期  
        /// </summary>  
        /// <param name="timeStamp"></param>  
        /// <returns></returns>  
        private DateTime GetDateTime(long timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = timeStamp * 10000000;
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime targetDt = dtStart.Add(toNow);
            return targetDt;
        }
    }
}
