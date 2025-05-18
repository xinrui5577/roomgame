using System;
using System.Collections.Generic;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI
{
    public class HandUpPnl : PopPnlBase
    {
        public GameObject[] Btns;

        public Text Context;
        public Text Time;
        public Text DismissUser;

        public  int TimeTotal = 300;
        protected int _timeCnt;

        protected string _strDismissUser;
        protected string _strContext;

        protected static DateTime _startTime;

        protected Dictionary<string, EnDismissFeedBack> _isApply;

        protected string[] _allUserName;
        void Start()
        {
            //BoxCollider sprCollider = bg.GetComponent<BoxCollider>();
            //sprCollider.size = new Vector3(Screen.width, Screen.height,0);
        }

        public void DismissApplyeFor(string username,string[] AllUserName, int time = UtilDef.DefInt)
        {
            if (AllUserName == null || AllUserName.Length == 0 || string.IsNullOrEmpty(name)) return; 
            _strDismissUser = "";

            Show();
            setBtnsActive(true);

            _timeCnt = time==UtilDef.DefInt?TimeTotal:time;
            _startTime = DateTime.Now;
            _allUserName = (string[])AllUserName.Clone();
            _isApply = new Dictionary<string, EnDismissFeedBack>();
            for (int i = 0; i < UtilData.CurrGamePalyerCnt; i++)
            {
                string str = _allUserName[i];
                if (!string.IsNullOrEmpty(str))
                {
                    _isApply[str] = EnDismissFeedBack.None;
                }
            }
            _isApply[username] = EnDismissFeedBack.ApplyFor;

            _strDismissUser += "【";
            _strDismissUser += username;
            _strDismissUser += "】申请解散房间";
            DismissUser.text = _strDismissUser;

            SetContext();
            SetTime();

            InvokeRepeating("UpdataTime",1.0f,1.0f);
            if (username == AllUserName[0])
            {
                setBtnsActive(false);
            }
        }

        public virtual void DismissForQdjt(string username, UserData[] Usersdata, int time = UtilDef.DefInt)
        {

        }

        public virtual void DismissForHzmj(string userName, UserData[] Usersdata, int time = UtilDef.DefInt)
        {

        }

        protected void OnApplicationPause(bool paused)
        {
            if (!paused)
            {
                int seconds = (int)(DateTime.Now - _startTime).TotalSeconds;
                _timeCnt = TimeTotal - seconds;
                //Debug.LogError("暂停回来时间：" + DateTime.Now + "  开始时间startTime:" + _startTime + "  用了seconds:" + seconds);
            }
        }

        protected void UpdataTime()
        {
            _timeCnt--;
            SetTime();
            if (_timeCnt<=0)
            {
                CancelInvoke("UpdataTime");
                setBtnsActive(false);
            }
        }

        public void DismissFeedBack(string name, EnDismissFeedBack feedBack)
        {
            if (string.IsNullOrEmpty(name)) return;     
   
            _isApply[name] = feedBack;
            SetContext();

            if (name == _allUserName[0])
            {
                setBtnsActive(false);
            }

            CheckApply();
        }

        protected virtual void SetContext()
        {
            _strContext = "";

            foreach (var player in _allUserName)
            {
                if (string.IsNullOrEmpty(player)) continue;
                switch (_isApply[player])
                {
                    case EnDismissFeedBack.None:
                        _strContext += "【" + player + "】正在选择" + "\n";
                        break;
                    case EnDismissFeedBack.ApplyFor:
                        _strContext += "【" + player + "】申请解散" + "\n";
                        break;
                    case EnDismissFeedBack.Apply:
                        _strContext += "【" + player + "】同意解散" + "\n";
                        break;
                    case EnDismissFeedBack.Refuse:
                        _strContext += "【" + player + "】拒绝解散" + "\n";
                        if (IsInvoking("UpdataTime")) CancelInvoke("UpdataTime");
                        Time.gameObject.SetActive(false);                       
                        NoDismiss();
                        break;
                }
            }

            Context.text = _strContext;
        }

        protected void NoDismiss()
        {
            if (IsInvoking("UpdataTime")) CancelInvoke("UpdataTime");
            Hide();
        }


        public void OnApplyBtn()
        {
            YxDebug.Log("接受解散");
            setBtnsActive(false);

            if (_isApply[_allUserName[0]] != EnDismissFeedBack.None) return;
            //发送解散接受
            EventDispatch.Dispatch((int)NetEventId.OnDismissRoom, new EventData(EnDismissFeedBack.Apply));

        }

        public void OnRefuseBtn()
        {
            YxDebug.Log("拒绝解散");
            setBtnsActive(false);

            if (_isApply[_allUserName[0]] != EnDismissFeedBack.None) return;
            //发送解散拒绝
            EventDispatch.Dispatch((int)NetEventId.OnDismissRoom, new EventData(EnDismissFeedBack.Refuse));

        }

        protected virtual void setBtnsActive(bool isActive)
        {
            for (int i = 0; i < Btns.Length; i++)
            {
                Btns[i].SetActive(isActive);
            }
        }

        protected virtual void SetTime()
        {
            Time.gameObject.SetActive(true);
            if (_timeCnt <= 0) _timeCnt = 0;
            Time.text = "倒计时:" + _timeCnt + "秒";
        }

        protected void CheckApply()
        {
            bool isAllApply = true;
            foreach (KeyValuePair<string, EnDismissFeedBack> userDismissFeedBack in _isApply)
            {
                if (userDismissFeedBack.Value != EnDismissFeedBack.ApplyFor && userDismissFeedBack.Value != EnDismissFeedBack.Apply)
                {
                    isAllApply = false;
                    break;
                }
            }
            //如果 三家都表示同意解散
            if (isAllApply)
            {
                //Invoke("NoDismiss", 1.0f);
                NoDismiss();
            }
        }

        public void Reset()
        {
            //if (IsInvoking("NoDismiss")) CancelInvoke("NoDismiss");
            NoDismiss();
        }
    }
}
