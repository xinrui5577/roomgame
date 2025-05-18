using System;
using System.Globalization;
using Assets.Scripts.Game.pdk.DDz2Common;
using Assets.Scripts.Game.pdk.DDzGameListener.TotalResultPanel;
using Assets.Scripts.Game.pdk.DdzEventArgs;
using Assets.Scripts.Game.pdk.InheritCommon;
using Sfs2X.Entities.Data;
using UnityEngine;
using System.Collections.Generic;
using YxFramwork.Common;
using System.Collections;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.pdk.DDzGameListener.Handup
{
    /// <summary>
    /// 投票解散listener
    /// </summary>
    public class HandUpListener : ServEvtListener
    {
        /// <summary>
        /// 内容Label
        /// </summary>
        [SerializeField] 
        protected UILabel ContentLabel;

        /// <summary>
        /// 倒计时Label
        /// </summary>
        [SerializeField]
        protected UILabel CuntDownLabel;


        /// <summary>
        /// messagebox弹出的卷轴面板
        /// </summary>
        [SerializeField] protected MessageBoxScroll MessageBoxScrollGob;

        /// <summary>
        /// 同意按键
        /// </summary>
        [SerializeField]
        protected GameObject ConfirmBtn;

        /// <summary>
        /// 拒绝按钮
        /// </summary>
        [SerializeField]
        protected GameObject RefuseBtn;

        /// <summary>
        /// 玩家发起投票的tittle
        /// </summary>
        [SerializeField] protected UILabel PlayerHdupStr;

        //存储投票发起人的名字和对应的handType
       // protected Dictionary<string, int> NameToType = new Dictionary<string, int>();

        /// <summary>
        /// 投票需要等待的总时间
        /// </summary>
        private int _cdtime = 300;

        /// <summary>
        /// 开始倒计时的Unity时间
        /// </summary>
        private DateTime _countStartT;

        /// <summary>
        /// 玩家1信息
        /// </summary>
        [SerializeField]
        private VotePlayerInfo[] _playerInfo;


        /// <summary>
        /// 标记投票是否开始
        /// </summary>
        private bool _hasvoteStart = false;

        protected override void OnAwake()
        {
            PdkGameManager.AddOnGetRejoinDataEvt(OnRejoinGame);
            Ddz2RemoteServer.AddOnHandUpEvt(OnHandUpEvt);
            Ddz2RemoteServer.AddOnGameOverEvt(OnGameOverEvt);
        }

        public override void RefreshUiInfo()
        {
           
        }

        /// <summary>
        /// 重连游戏
        /// </summary>
        /// <param invotename="sender"></param>
        /// <param invotename="args"></param>
        protected void OnRejoinGame(object sender, DdzbaseEventArgs args)
        {
            var landRequestData = args.IsfObjData;

            //如果接收重连解散信息则不响应
            if (!landRequestData.ContainsKey("hup")) return;


            var time = (int)(landRequestData.GetLong("svt") - landRequestData.GetLong("hupstart"));
            time = 300 - time;
            time = time < 0 ? 0 : time;
            string[] ids = landRequestData.GetUtfString("hup").Split(',');
            StartCoroutine(WaitForGlobalDataUpdate(landRequestData, ids, time));
        }
        /// <summary>
        /// 等待Globaldata的信息完成加载刷新以后，在弹出投票界面
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitForGlobalDataUpdate(ISFSObject data, string[] ids,int time)
        {
            var userSelf = data.GetSFSObject(RequestKey.KeyUser);
            var userOthers = data.GetSFSArray(RequestKey.KeyUserList);
            var userIdtoNameDic = new Dictionary<int, string>();
            userIdtoNameDic[userSelf.GetInt(RequestKey.KeyId)] = userSelf.GetUtfString(RequestKey.KeyName);
            foreach (ISFSObject otheruser in userOthers)
            {
                userIdtoNameDic[otheruser.GetInt(RequestKey.KeyId)] = otheruser.GetUtfString(RequestKey.KeyName);
            }

            //等待globaldata刷新好信息
            while (true)
            {
                yield return new WaitForEndOfFrame();
                if(App.GetGameData<GlobalData>().GetUsersName().Length == userIdtoNameDic.Count) break;
            }

            //模拟每次投票的服务器发起信息，来重构投票
            var len = ids.Length;
            for (int i = 0; i < len; i++)
            {
                //2发起投票 3同意 -1拒绝
                ISFSObject userdata = new SFSObject();
                userdata.PutUtfString("username", userIdtoNameDic[int.Parse(ids[i])]);
                userdata.PutInt("type", i == 0 ? 2 : 3);
                if (i == 0) userdata.PutInt("cdTime", time);
                var args = new DdzbaseEventArgs(userdata);
                OnHandUpEvt(null, args);
            }
        }


        /// <summary>
        /// 投票事件激发 2发起 3同意 -1拒绝
        /// </summary>
        /// <param invotename="sender"></param>
        /// <param invotename="args"></param>
        void OnHandUpEvt(object sender, DdzbaseEventArgs args)
        {
            MessageBoxScrollGob.OpenMessageBox();
            var data = args.IsfObjData;
            if (data.ContainsKey("cdTime"))
            {
                _cdtime = data.GetInt("cdTime");
                CuntDownLabel.text = _cdtime.ToString(CultureInfo.InvariantCulture);
            }

            var userName = data.GetUtfString("username");

            ShowHandUpContent(data);

            //如果handup发来信息的玩家名字是 这个客户端的玩家自己，则隐藏同意 拒绝按钮
            if (userName == App.GetGameData<GlobalData>().UserSelfData.GetUtfString(RequestKey.KeyName))
            {
                ConfirmBtn.SetActive(false);
                RefuseBtn.SetActive(false);
            }
        }







        /// <summary>
        /// 当已经进入总结算阶段时，关闭投票界面
        /// </summary>
        /// <param invotename="sender"></param>
        /// <param invotename="args"></param>
        void OnGameOverEvt(object sender, DdzbaseEventArgs args)
        {
            StartCoroutine(HideHandUpPanel(2f, _hasvoteStart));
        }


        //存储投票发起人的id和对应的handType
        protected Dictionary<string, int> NameToType = new Dictionary<string, int>();


        private const string Username = "username";

        /// <summary>
        /// 显示投票信息内容
        /// </summary>
        /// <param invotename="handType"></param>
        /// <param invotename="userName"></param>
        /// <param name="data"></param>
        private void ShowHandUpContent(ISFSObject data)
        {
            var handType = data.GetInt("type");
            var invotename = data.GetUtfString(Username);
            if (invotename == null) return;
            switch (handType)
            {
                //发起
                case 2:
                    _hasvoteStart = true;
                    ResetData(invotename);
                    break;
                //拒绝；如果有人拒绝则投票失败，隐藏投票界面
                case -1:
                    _hasvoteStart = false;
                    NameToType[invotename] = handType;
                    StartCoroutine(HideHandUpPanel(2f));
                    break;
                //同意
                case 3:
                    NameToType[invotename] = handType;
                    break;
            }


            var i = 0;
            //其他人按顺序下排
            foreach (var userName in NameToType.Keys)
            {
                if (i >= _playerInfo.Length) break;

                var userdata = App.GetGameData<GlobalData>().GetPlayerUserData(userName);
                if (userdata == null)
                {
                    Debug.LogError("没有找到对应id的玩家信息");
                    return;
                }

                DDzUtil.SetUserHeadTexture(userdata, _playerInfo[i].HandTexture);

                var playerName = userdata.GetUtfString(RequestKey.KeyName);
                _playerInfo[i].gameObject.SetActive(true);
                _playerInfo[i].PlayerName.text = playerName;
                _playerInfo[i].HandTypeSp.gameObject.SetActive(true);
                switch (NameToType[userName])
                {
                    case 0:
                        _playerInfo[i].HandTypeSp.gameObject.SetActive(false);
                        break;
                    case -1:
                        _playerInfo[i].HandTypeSp.spriteName = "public_021";
                        break;
                    case 3:
                        _playerInfo[i].HandTypeSp.spriteName = "public_020";
                        break;
                    case 2:

                        PlayerHdupStr.text = playerName+ "     申请解散房间  请等待其他玩家选择";
                        _playerInfo[i].HandTypeSp.spriteName = "public_020";
                        break;
                }
                _playerInfo[i].HandTypeSp.MakePixelPerfect();
                i++;
            }


/*
            //先把发起人找到，放到第一排
            foreach (var sname in NameToType.Keys)
            {
                if (NameToType[sname] == 2)
                {
                    PlayerHdupStr.text = sname+ "     申请解散房间  请等待其他玩家选择";
                    break;
                }
            }

            //其他人按顺序下排
            foreach (var sname in NameToType.Keys)
            {
                switch (NameToType[sname])
                {
                    case 0:
                        ContentLabel.text += "[581e1e]玩家" + " [" + sname + "] " + " 正在选择[-]\n";
                        break;
                    case -1:
                        ContentLabel.text += "[581e1e]玩家" + " [" + sname + "] " + " [FF0000FF]选择拒绝[-]\n";
                        break;
                    case 3:
                        ContentLabel.text += "[581e1e]玩家" + " [" + sname + "] " + " [009013FF]选择同意[-]\n";
                        break;
                }
            }*/

        
        }

     /*   /// <summary>
        /// 重新发起投票时， 重置投票信息参数
        /// </summary>
        /// <param invotename="founderUserName"> 投票信息发起人名字 </param>
        private void ResetData(string founderUserName)
        {
            //清理
            NameToType.Clear();
            //重设开始计时的时间
            _countStartT = DateTime.Now;
            //重置内容label
            ContentLabel.text = "";
            //重置倒计时label
            CuntDownLabel.text = "";

            //重置名字列表
            var names = App.GetGameData<GlobalData>().GetUsersName();
            for (int index = 0; index < names.Length; index++)
            {
                var s = names[index];
                NameToType[s] = 0;
            }
            NameToType[founderUserName] = 2;

            //重新开始倒计时
            StopAllCoroutines();
            StartCoroutine(CuntDownTime());

            //按钮全显示出来
            ConfirmBtn.SetActive(true);
            RefuseBtn.SetActive(true);
        }*/

        /// <summary>
        /// 重新发起投票时， 重置投票信息参数
        /// </summary>
        /// <param invotename="invotename"> 投票信息发起人名字 </param>
        /// <param name="invotename"></param>
        private void ResetData(string invotename)
        {
            //清理
            NameToType.Clear();
            //重设开始计时的时间
            _countStartT = DateTime.Now;
            //重置内容label
            PlayerHdupStr.text = "";
            //重置倒计时label
            CuntDownLabel.text = "";

            //重置名字列表
            var names = App.GetGameData<GlobalData>().GetAllUsersNames();
            foreach (var name in names)
            {
                NameToType[name] = 0;
            }
            NameToType[invotename] = 2;

            //重新开始倒计时
            StopAllCoroutines();
            StartCoroutine(CuntDownTime());

            //按钮全显示出来
            ConfirmBtn.SetActive(true);
            RefuseBtn.SetActive(true);
        }
        /// <summary>
        /// 开始计时
        /// </summary>
        /// <returns></returns>
        private IEnumerator CuntDownTime()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                var curTime = _cdtime - (int)(DateTime.Now - _countStartT).TotalSeconds;
                if (curTime < 0) curTime = 0;
                CuntDownLabel.text = curTime.ToString(CultureInfo.InvariantCulture);
                if (curTime <= 0) yield break;
            }
        }

        /// <summary>
        /// delaytime时间后隐藏投票界面
        /// </summary>
        /// <param invotename="delaytime"></param>
        /// <param invotename="showTotalTresult"></param>
        /// <returns></returns>
        private IEnumerator HideHandUpPanel(float delaytime,bool showTotalTresult=false)
        {
            yield return new WaitForSeconds(delaytime);
            MessageBoxScrollGob.HideMessageBox();

            if(showTotalTresult)
                //如果有总结算信息，显示总结算信息
                TotalResultListener.Instance.RefreshUiInfo();
        }

        /// <summary>
        /// 点击同意解散
        /// </summary>
        public void OnClickConFirmBtn()
        {
            GlobalData.ServInstance.StartHandsUp(3);
        }

        /// <summary>
        /// 点击拒绝解散
        /// </summary>
        public void OnClickRefuseBtn()
        {
            GlobalData.ServInstance.StartHandsUp(-1);
        }
    }
}
