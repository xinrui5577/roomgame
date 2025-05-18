using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.jlgame.EventII;
using Assets.Scripts.Game.jlgame.network;
using com.yxixia.utile.Utiles;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Controller;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.jlgame.ui
{
    public class JlGameTtResultView : MonoBehaviour
    {
        public EventObject EventObj;

        public GameObject View;
        public UILabel RommId;
        public UILabel StartTime;

        public GameObject FourLine;
        public GameObject FourTotal;
        public JlGameTtResultItem FourResultItem;
        public List<JlGameTtResultTotalItem> FourTotalList;
        public GameObject FiveLine;
        public GameObject FiveTotal;
        public JlGameTtResultItem FiveResultItem;
        public List<JlGameTtResultTotalItem> FiveTotalList;
        public GameObject SixLine;
        public GameObject SixTotal;
        public JlGameTtResultItem SixResultItem;
        public List<JlGameTtResultTotalItem> SixTotalList;

        public UIGrid ResultItemGrid;

        private object _data;
        private bool _isWait;
        private string _screenshot;
        private string _rule;
        private List<TtResultUserData> _totalUserData;
        public void OnRecieve(EventData data)
        {
            switch (data.Name)
            {
                case "UserData":
                    OnResultTotalData(data.Data);
                    break;
                case "TtResult":
                    OnResult(data.Data);
                    break;
                case "TtShow":
                    OnShow(data.Data);
                    break;
            }
        }

        public void OnShow(object data)
        {
            var ruleData = (ISFSObject) data;
            _rule = ruleData.GetUtfString("rule");

        }

        public void OnResultTotalData(object data)
        {
            _totalUserData = (List<TtResultUserData>)data;

            _totalUserData.Sort((a, b) =>
            {
                if (a.UserGold > b.UserGold)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            });

            switch (_totalUserData.Count)
            {
                case 4:
                    FourLine.SetActive(true);
                    FourTotal.SetActive(true);
                    for (int i = 0; i < _totalUserData.Count; i++)
                    {
                        FourTotalList[i].SetView(_totalUserData[i]);
                    }
                    break;
                case 5:
                    FiveLine.SetActive(true);
                    FiveTotal.SetActive(true);
                    for (int i = 0; i < _totalUserData.Count; i++)
                    {
                        FiveTotalList[i].SetView(_totalUserData[i]);
                    }
                    break;
                case 6:
                    SixLine.SetActive(true);
                    SixTotal.SetActive(true);
                    for (int i = 0; i < _totalUserData.Count; i++)
                    {
                        SixTotalList[i].SetView(_totalUserData[i]);
                    }
                    break;
            }
        }

        public void OnResult(object data)
        {
            View.SetActive(true);
            var recordDetail = (IDictionary)data;
            RommId.text =string.Format("房间号：{0} {1}", recordDetail["room_id"], _rule);
            var detail = recordDetail["detail"];
            var detailList = (List<object>)detail;
            for (int i = 0; i < detailList.Count; i++)
            {
                var detailData = (IDictionary)detailList[i];
                if (i == 0)
                {
                    StartTime.text = detailData["create_dt"].ToString();
                }
                var head = (IDictionary)detailData["head_s"];

                JlGameTtResultItem item = new JlGameTtResultItem();
                switch (_totalUserData.Count)
                {
                    case 4:
                        item = YxWindowUtils.CreateItem(FourResultItem, ResultItemGrid.transform);
                        break;
                    case 5:
                        item = YxWindowUtils.CreateItem(FiveResultItem, ResultItemGrid.transform);
                        break;
                    case 6:
                        item = YxWindowUtils.CreateItem(SixResultItem, ResultItemGrid.transform);
                        break;
                }
                item.SetCurRound(i + 1);
                for (int j = 0; j < _totalUserData.Count; j++)
                {
                    var user = (IDictionary)head[_totalUserData[j].UserId.ToString()];
                    item.SetUserWin(j, int.Parse(user["gold"].ToString()));
                }
            }

            ResultItemGrid.repositionNow = true;

            EventObj.SendEvent("ResultViewEvent", "close", null);
        }

        public void OnBackHall()
        {
            App.QuitGame();
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
            dic["type"] = 1;
            dic["image"] = _screenshot;
            dic["shareType"] = 1;
            dic["sharePlat"] = 0;
            UserController.Instance.GetShareInfo(dic, info => Facade.Instance<WeChatApi>().ShareContent(info, str =>
            {
//                var dict = new Dictionary<string, object>();
//                dict["option"] = 2;
//                dict["sharePlat"] = SharePlat.WxSenceSession.ToString();
//                Facade.Instance<TwManager>().SendAction("shareGameResultRequest", dict, null);
            }));
        }
    }
}
