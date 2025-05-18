using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.Game.biji.EventII;
using Assets.Scripts.Game.biji.Modle;
using Assets.Scripts.Game.biji.network;
using com.yxixia.utile.Utiles;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework;

namespace Assets.Scripts.Game.biji.ui
{
    public class BjResultView : MonoBehaviour
    {
        public EventObject EventObj;
        public GameObject View;
        public UILabel RoomId;
        public UILabel RoundNum;
        public GameObject CuntDown;
        public UILabel CuntDownLabel;
        public UILabel CurrentTime;
        public UISprite ResultState;
        public UISprite ResultStateBg;
        public GameObject CloseBtn;
        public GameObject ContinueBtn;

        public UIGrid ResultGrid;
        public BjResultItem BjResultItem;


        private BjGameTable _gdata
        {
            get { return App.GetGameData<BjGameTable>(); }
        }
        private int _countTime;
        private YxBaseGamePlayer[] _userInfo;
        private ResultData _resultData;
        private int _selfSeat;
        private bool _end;

        public void OnRecieve(EventData data)
        {
            switch (data.Name)
            {
                case "Ready":
                    CloseBtn.SetActive(true);
                    ContinueBtn.SetActive(true);
                    break;
                case "Rule":
                    OnRule(data.Data);
                    break;
                case "ResultData":
                    _resultData = (ResultData)data.Data;
                    break;
                case "RoomData":
                    OnRoomData(data.Data);
                    break;
                case "PlayerList":
                    _userInfo = (YxBaseGamePlayer[])data.Data;
                    break;
                case "Show":
                    OnShow(data.Data);
                    break;
                case "Close":
                    OnClose();
                    break;
            }
        }

        public void OnRule(object data)
        {
            var ruleData = (SFSObject)data;
            _countTime = ruleData.GetInt("autoTime");
        }

        public void OnRoomData(object data)
        {
            var roomData = (ISFSObject)data;
            _selfSeat = roomData.GetInt("selfSeat");
            RoomId.text = roomData.GetInt("roomId").ToString();
            var curRound = roomData.GetInt("currentRound");
            var maxRound = roomData.GetInt("maxRound");
            if (curRound == maxRound) _end = true;
            RoundNum.text = string.Format("{0}/{1}", curRound, maxRound);
            CurrentTime.text = DateTime.Now.ToString("yyyy-MM-dd HH：mm：ss", DateTimeFormatInfo.InvariantInfo);
        }


        public void OnShow(object data)
        {
            View.SetActive(true);
            if (_countTime != 0)
            {
                CuntDown.SetActive(true);
                SetTime(_countTime);
                StartCoroutine(CuntDownTime(_countTime));
            }

            var resultData = (List<CompareData>)data;
            for (int i = 0; i < _resultData.LoseSeats.Count; i++)
            {
                if (_selfSeat == _resultData.LoseSeats[i])
                {
                    ResultState.spriteName = "lose";
                    ResultStateBg.spriteName = "loseBg";
                }
            }
            for (int i = 0; i < _resultData.WinSeats.Count; i++)
            {
                if (_selfSeat == _resultData.WinSeats[i])
                {
                    ResultState.spriteName = "win";
                    ResultStateBg.spriteName = "winBg";
                }
            }

            while (ResultGrid.transform.childCount > 0)
            {
                DestroyImmediate(ResultGrid.transform.GetChild(0).gameObject);
            }
            for (int i = 0; i < resultData.Count; i++)
            {
                var item = YxWindowUtils.CreateItem(BjResultItem, ResultGrid.transform);

                item.SetResultItemView(_userInfo[_gdata.GetLocalSeat(resultData[i].Seat)].Info, resultData[i], _gdata);
            }

            ResultGrid.repositionNow = true;
        }

        private IEnumerator CuntDownTime(int cdTime)
        {
            var curTime = cdTime;
            while (curTime >= 0)
            {
                if (curTime < 0) curTime = 0;
                SetTime(curTime);
                yield return new WaitForSeconds(1f);
                curTime--;

                if (curTime < 0)
                {
                    OnClose();
                    yield break;
                }
            }
        }

        private void SetTime(int cdTime)
        {
            if (CuntDownLabel == null) return;
            CuntDownLabel.text = string.Format("{0}", cdTime);
            CuntDownLabel.gameObject.SetActive(true);
        }

        public void OnClose()
        {
            while (ResultGrid.transform.childCount > 0)
            {
                DestroyImmediate(ResultGrid.transform.GetChild(0).gameObject);
            }

            StopAllCoroutines();
            View.SetActive(false);
            CloseBtn.SetActive(false);
            ContinueBtn.SetActive(false);
            if (!_end)
            {
                EventObj.SendEvent("ServerEvent", "ReadyReq", null);
            }
        }

    }
}
