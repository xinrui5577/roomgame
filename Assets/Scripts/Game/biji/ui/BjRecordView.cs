using System;
using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.Game.biji.EventII;
using Assets.Scripts.Game.biji.Modle;
using Assets.Scripts.Game.biji.network;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Abstracts;

namespace Assets.Scripts.Game.biji.ui
{
    public class BjRecordView : MonoBehaviour
    {
        public EventObject EventObj;
        public GameObject View;
        public UILabel CurrentTime;
        public UILabel ShowCurIndex;
        public UILabel ShowIndex;
        public GameObject NoData;
        public UIGrid RecordGrid;
        public BjRecordItem BjRecordItem;
        public GameObject Bottom;

        private int _curRound;
        private int _defaultIndex = 1;
        private int _curIndex = 1;

        private List<List<CompareData>> _allCompareDatas = new List<List<CompareData>>();
        private List<ResultData> _allResultDatas = new List<ResultData>();

        private BjGameTable _gdata
        {
            get { return App.GetGameData<BjGameTable>(); }
        }
        public void OnRecieve(EventData data)
        {
            switch (data.Name)
            {
                case "Join":
                    OnJoin(data.Data);
                    break;
                case "CompareData":
                    OnCompareData(data.Data);
                    break;
                case "ResultData":
                    OnResult(data.Data);
                    break;
                case "RoomData":
                    OnRoom(data.Data);
                    break;
                case "Show":
                    OnShow();
                    break;
            }
        }

        public void OnJoin(object data)
        {
            var round = (int)data;
            if (round == 0)
            {
                _defaultIndex = 1;
            }
            else
            {
                _defaultIndex = round + 1;
                _curIndex = round + 1;
            }
        }

        public void OnRoom(object data)
        {
            var roomData = (YxCreateRoomInfo)data;
            _curRound = roomData.CurRound;
        }

        public void OnCompareData(object data)
        {
            var compareDatas = (List<CompareData>)data;
            _allCompareDatas.Add(compareDatas);
        }

        public void OnResult(object data)
        {
            var resultDatas = (ResultData)data;
            _allResultDatas.Add(resultDatas);
        }

        public void OnShow()
        {
            CurrentTime.text = DateTime.Now.ToString("yyyy-MM-dd HH：mm：ss", DateTimeFormatInfo.InvariantInfo);
            View.SetActive(true);
            if (_allResultDatas.Count == 0)
            {
                NoData.SetActive(true);
                Bottom.SetActive(false);
            }
            else
            {
                NoData.SetActive(false);
                Bottom.SetActive(true);
                CreatViewData();
            }
        }

        public void CreatViewData()
        {
            ShowCurIndex.text = _curIndex.ToString();
            ShowIndex.text = string.Format("{0}/{1}", _curIndex, _curRound);

            for (int i = 0; i < _allCompareDatas[_curIndex - _defaultIndex].Count; i++)
            {
                BjRecordItem item = null;
                while (RecordGrid.transform.childCount >_allCompareDatas[_curIndex - _defaultIndex].Count)
                {
                   DestroyImmediate(RecordGrid.transform.GetChild(0).gameObject);
                }
                item = RecordGrid.transform.childCount == _allCompareDatas[_curIndex - _defaultIndex].Count?RecordGrid.transform.GetChild(i).GetComponent<BjRecordItem>() : YxWindowUtils.CreateItem(BjRecordItem, RecordGrid.transform);
                var compareData = _allCompareDatas[_curIndex - _defaultIndex][i];
                item.SetView(_gdata.GetPlayerInfo(compareData.Seat, true).NickM, _allResultDatas[_curIndex - _defaultIndex].TotalResultDataList[compareData.Seat], compareData);
            }

            RecordGrid.repositionNow = true;
        }

        public void TurnLeftMin()
        {
            _curIndex = _defaultIndex;
            CreatViewData();
        }

        public void TurnLeftOne()
        {
            if (_curIndex == _defaultIndex)
            {
                return;
            }
            else
            {
                _curIndex--;
                CreatViewData();
            }
        }


        public void TurnRightOne()
        {
            if (_curIndex == _curRound)
            {
                return;
            }
            else
            {
                _curIndex++;
                CreatViewData();
            }
        }
        public void TurnLeftMax()
        {
            _curIndex = _curRound;
            CreatViewData();
        }


        public void OnClose()
        {
            View.SetActive(false);
        }

    }
}
