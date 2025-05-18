using System;
using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Game.jlgame.EventII;
using Assets.Scripts.Game.jlgame.Modle;
using Assets.Scripts.Game.jlgame.network;
using com.yxixia.utile.Utiles;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.DataBundles;

namespace Assets.Scripts.Game.jlgame.ui
{
    public class JlGameResultView : MonoBehaviour
    {
        public EventObject EventObj;
        public GameObject Result;
        public GameObject WinEffect;
        public GameObject LoseEffect;
        public GameObject BackHallBtn;
        public UILabel RoomData;
        public UILabel RoomTime;

        public UIGrid BtnGrid;

        public NguiTextureAdapter UserHead;
        public UIGrid ResultGrid;
        public JlGameResultItem ResultItem;

        private JlGameGameTable _gdata
        {
            get { return App.GetGameData<JlGameGameTable>(); }
        }

        private bool _ready;

        public void OnRecieve(EventData data)
        {
            switch (data.Name)
            {
                case "show":
                    OnShow(data.Data);
                    break;
                case "Rule":
                    OnRule(data.Data);
                    break;
                case "gameOver":
                    OnGameOver(data.Data);
                    break;
                case "result":
                    OnResultShow(data.Data);
                    break;
                case "close":
                    Result.SetActive(false);
                    break;
            }
        }

        public void OnShow(object data)
        {
            BackHallBtn.SetActive(true);
            BtnGrid.repositionNow = true;
        }

        public void OnRule(object data)
        {
            var roomData = (ISFSObject)data;
            var roomId = roomData.GetInt("roomId");
            var roomRule = roomData.GetUtfString("rule");
            RoomData.text = string.Format("房间号：{0} {1}", roomId, roomRule);
        }

        public void OnGameOver(object data)
        {
            _ready = (bool) data;
        }

        public void OnResultShow(object data)
        {
            RoomTime.text = string.Format("对战时间{0}",
                DateTime.Now.ToString("yyyy-MM-dd HH：mm：ss", DateTimeFormatInfo.InvariantInfo));
            Result.SetActive(true);
            if (data is List<ResultData>)
            {
                var resultDatas = data as List<ResultData>;
                for (int i = 0; i < resultDatas.Count; i++)
                {
                    var resultItem = YxWindowUtils.CreateItem(ResultItem, ResultGrid.transform);
                    resultItem.SetData(resultDatas[i]);
                    _gdata.GetPlayerInfo(resultDatas[i].Seat, true).CoinA += resultDatas[i].Win;
                    if (resultDatas[i].IsYouSelf)
                    {
                        if (resultDatas[i].Win > 0)
                        {
                            WinEffect.SetActive(true);
                            PortraitDb.SetPortrait(resultDatas[i].Head, UserHead, resultDatas[i].Sex);
                            UserHead.gameObject.SetActive(true);
                            LoseEffect.SetActive(false);
                        }
                        else
                        {
                            LoseEffect.SetActive(true);
                            WinEffect.SetActive(false);
                            UserHead.gameObject.SetActive(false);
                        }
                    }
                }

                ResultGrid.repositionNow = true;
            }
        }

        public void OnContinuGame()
        {
            Result.SetActive(false);
            if (_ready)
            {
//                EventObj.SendEvent("ServerEvent", "GameDetail", null);
                return;
            }
            EventObj.SendEvent("TableViewEvent", "Reset", null);
            while (ResultGrid.transform.childCount > 0)
            {
                DestroyImmediate(ResultGrid.transform.GetChild(0).gameObject);
            }

        }

        public void OnBackHallBtn()
        {
            App.QuitGameWithMsgBox();
        }
    }
}
