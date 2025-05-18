using System;
using Assets.Scripts.Game.jh.EventII;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.jh.ui
{
    public class JhTableView : MonoBehaviour
    {

        public EventObject EventObj;

        public UILabel SingleBeat;

        public UILabel TotalBeat;

        public GameObject ReadyBtn;

        public GameObject KaiFang;

        public GameObject YuLe;

        public GameObject StartBtn;

        public JhTableInfo TableInfo;
        public void OnReceive(EventData data)
        {
            switch (data.Name)
            {
                case "Status":
                    OnStatus(data.Data);
                    break;
                case "Ready":
                    ReadyBtn.SetActive(false);
                    break;
                case "Bet":
                    int totalBet = (int) data.Data;
                    TotalBeat.text = "￥" + YxUtiles.ReduceNumber(totalBet);
                    break;
                case "Result":
                    TotalBeat.text = "￥" + 0;

                    break;
                case "CurrPlayer":
                    OnCurrPlayer(data.Data);
                    break;
                case "Start":
                    OnStart(data.Data);
                    break;
            }
        }

        private void OnStart(object data)
        {
            bool show = (bool) data;
            StartBtn.SetActive(show);
        }

        protected void OnCurrPlayer(object data)
        {
            ISFSObject eData = (SFSObject)(data);
            int singleBeat = eData.GetInt("SingleBet");
            SingleBeat.text = YxUtiles.ReduceNumber(singleBeat);
            int curlun = eData.GetInt("CurLun");
            int maxlun = eData.GetInt("MaxLun");
            TableInfo.SetLunShu(curlun,maxlun);
        }

        protected void OnStatus(object data)
        {
            ISFSObject eData = (SFSObject)(data);
            int singleBeat = eData.GetInt("SingleBet");
            SingleBeat.text = YxUtiles.ReduceNumber(singleBeat);
            TotalBeat.text = "￥" + Convert.ToString(0);
            if (eData.ContainsKey("ShowReady"))
            {
                ReadyBtn.SetActive(true);
            }
            else
            {
                ReadyBtn.SetActive(false);
            }
            if (eData.ContainsKey("TotalBeat"))
            {
                int allBeat = eData.GetInt("TotalBeat");
                TotalBeat.text = "￥" + YxUtiles.ReduceNumber(allBeat);
            }

            if (eData.ContainsKey("ShowStart"))
            {
                bool show = eData.GetBool("ShowStart");
                StartBtn.SetActive(show);
            }

            bool isKaiFang = eData.GetBool("IsFangKa");

            if(isKaiFang)
            {
                KaiFang.SetActive(true);
                YuLe.SetActive(false);
                if (TableInfo == null)
                {
                    TableInfo = KaiFang.GetComponent<JhTableInfo>();
                }
                
                int maxju = eData.GetInt("MaxJu");
                int roomId = eData.GetInt("RoomId");
                int curju = eData.GetInt("CurJu");

                TableInfo.SetRoomId(roomId);
                TableInfo.SetJuShu(curju,maxju);
            }
            else
            {
                KaiFang.SetActive(false);
                YuLe.SetActive(true);
                if (TableInfo == null)
                {
                    TableInfo = YuLe.GetComponent<JhTableInfo>();
                }
            }

            int curlun = eData.GetInt("CurLun");
            int maxlun = eData.GetInt("MaxLun");

            TableInfo.SetLunShu(curlun,maxlun);

        }
        public void OnBackBtnClick()
        {
            EventObj.SendEvent("GameManagerEvent", "Quit", null);
        }

        public void OnReadyBtnClick()
        {

            EventObj.SendEvent("ServerEvent", "ReadyReq", null);
            ReadyBtn.SetActive(false);
        }

        public void OnChangeRoomBtnClick()
        {
            EventObj.SendEvent("GameManagerEvent", "ChangeRoom", null);
        }

        public void OnSettingBtnClick()
        {
            EventObj.SendEvent("SettingEvent", "Show", null);
        }

        public void OnHelpBtnClick()
        {
            EventObj.SendEvent("HelpEvent", "Show", null);
        }

        public void OnRecordBtnClick()
        {
            EventObj.SendEvent("TableEvent","ShowResult",null);
//            EventObj.SendEvent("ReultViewEvent", "Show", null);
        }

        public void OnKaiFangBackClick()
        {
            EventObj.SendEvent("ServerEvent", "HupReq", 2);
        }

        public void OnRuleInfpClick()
        {
            EventObj.SendEvent("RuleInfoEvent", "Show", null);
        }

        public void OnStartBtnClick()
        {
            StartBtn.SetActive(false);
            EventObj.SendEvent("ServerEvent", "StartReq", null);
        }

        public void OnHallHelpBtnClick()
        {
            
        }
    }
}
