using System;
using System.Collections;
using System.Globalization;
using Assets.Scripts.Game.pdk.DDz2Common;
using Assets.Scripts.Game.pdk.DDzGameListener.TotalResultPanel;
using Assets.Scripts.Game.pdk.DdzEventArgs;
using Assets.Scripts.Game.pdk.InheritCommon;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.pdk.DDzGameListener.OneRoundResultPanel
{
    public class OneRoundResultListener : ServEvtListener
    {
        /// <summary>
        /// 单局结算面板
        /// </summary>
        [SerializeField]
        protected GameObject OneRoundUiGob;
        
        /// <summary>
        /// 继续游戏按钮
        /// </summary>
        [SerializeField]
        protected GameObject GameContinueBtn;

        /// <summary>
        /// 单局成绩的缓存
        /// </summary>
        private ISFSObject _oneRoundRecordData;

        //---ui信息项--start

        [SerializeField]
        protected GameObject OnerdRecordItemOrg;


        [SerializeField] 
        protected GameObject PlayerRecordGrid;


        /// <summary>
        /// 标记地主座位
        /// </summary>
        private int _dizhuSeat;


/*        [SerializeField] protected GameObject GirlWin;
        [SerializeField]
        protected GameObject GirlLose;


        [SerializeField]
        protected GameObject TittleWin;
        [SerializeField]
        protected GameObject TittleLose;*/


        [SerializeField]
        protected GameObject WinTittle;
        [SerializeField]
        protected GameObject LoseTittle;

        //----------------------------------------------------end
        /// <summary>
        /// 只隐藏结算面板，继续按钮仍然保留
        /// </summary>
        public void HideOneRoundUiGob()
        {
            OneRoundUiGob.SetActive(false);
        }



        protected override void OnAwake()
        {
            PdkGameManager.AddOnGetRejoinDataEvt(OnRejoinGame);
            PdkGameManager.AddOnServResponseEvtDic(GlobalConstKey.TypeGameOver, OnTypeGameOver);
            PdkGameManager.AddOnServResponseEvtDic(GlobalConstKey.TypeFirstOut, OnTypeFirstOut);
            Ddz2RemoteServer.AddOnUserReadyEvt(OnUserReady);
        }


        public override void RefreshUiInfo()
        {
            if(_oneRoundRecordData==null) return;

            OneRoundUiGob.SetActive(true);
            GameContinueBtn.SetActive(true);
            DDzUtil.ClearPlayerGrid(PlayerRecordGrid);

            var boom = 0;
            if(_oneRoundRecordData.ContainsKey("boom"))
                boom = _oneRoundRecordData.GetInt("boom");
            //var superBoom = _oneRoundRecordData.GetInt("sboom");
           // var rocket = _oneRoundRecordData.GetInt("rocket");
           // var spring = _oneRoundRecordData.GetInt(NewRequestKey.KeySpring);
           // var modeSize = _oneRoundRecordData.GetInt(GlobalConstKey.C_Score);//得分
            //var nextLandSeat = _oneRoundRecordData.GetInt("bkp");//下一个地主座位
            var ante = _oneRoundRecordData.GetInt("ante");




            //BoomLabel.text = boom.ToString(CultureInfo.InvariantCulture);

           // SpringLabel.text = spring.ToString(CultureInfo.InvariantCulture);

            //QdzLabel.text = modeSize.ToString(CultureInfo.InvariantCulture);


            
            //赋值用户成绩列表
            var usersrecord = GetUsersRecord(_oneRoundRecordData);
            var usersLen = usersrecord.Length;
            for (var i = 0; i < usersLen; i++)
            {
                if(usersrecord[i]==null) continue;

                var data = usersrecord[i];
/*                if (data.Seat == App.GetGameData<GlobalData>().GetSelfSeat)
                {
                    GetRewordLabel.text = data.Gold.ToString(CultureInfo.InvariantCulture);
                    int rate = data.Seat == _dizhuSeat ? Math.Abs(data.Dz) / ante : Math.Abs(data.Nm) / ante;
                    TotalBeiLabel.text = string.Format("共{0}倍", rate);
                }*/

                AddUserItemInfo(data);

            }
            PlayerRecordGrid.GetComponent<UIGrid>().repositionNow = true;

        }

        //当收到自己已经成功准备了的时候，隐藏onRoundpanel
        protected void OnUserReady(object sender, DdzbaseEventArgs args)
        {
            if (args.IsfObjData.GetInt(RequestKey.KeySeat) == App.GetGameData<GlobalData>().GetSelfSeat)
            {
                OneRoundUiGob.SetActive(false);
                GameContinueBtn.SetActive(false);
            }
           
        }

        /// <summary>
        /// 重连游戏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void OnRejoinGame(object sender, DdzbaseEventArgs args)
        {
            _dizhuSeat = args.IsfObjData.GetInt(NewRequestKey.KeyLandLord);
        }

        /// <summary>
        /// 当确定地主后,更新地主标记
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnTypeFirstOut(object sender, DdzbaseEventArgs args)
        {
           _dizhuSeat = args.IsfObjData.GetInt(RequestKey.KeySeat);
        }


        /// <summary>
        /// 当游戏结算时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected virtual void OnTypeGameOver(object sender, DdzbaseEventArgs args)
        {
            _oneRoundRecordData = args.IsfObjData;
            StartCoroutine(ShowUiInfoLater());
        }

        IEnumerator  ShowUiInfoLater()
        {
            yield return new WaitForSeconds(2f);
            RefreshUiInfo();
        }

        public void OnClickContinueBtn()
        {
            if (TotalResultListener.Instance.IsEndAllRound)
            {
                OneRoundUiGob.SetActive(false);
                GameContinueBtn.SetActive(false);
                //如果有总结算信息，显示总结算信息
                TotalResultListener.Instance.RefreshUiInfo();
                return;
            }

            //如果不是最后一局弹出结算，则发送准备消息给服务器
            GlobalData.ServInstance.SendPlayerReadyServCmd();
        }



        private UserRecord[] GetUsersRecord(ISFSObject sfsObject)
        {
            ISFSArray tempArr = sfsObject.GetSFSArray("users");
            var totalArr = new UserRecord[3];
            for (int i = 0; i < tempArr.Count; i++)
            {
                var userData = tempArr.GetSFSObject(i);
                totalArr[i] = new UserRecord
                {
  /*                  SuperBoom = userData.GetInt("sboom"),
                    Gold = userData.GetInt("gold"),
                    ThrowOutNum = userData.GetInt("throwout"),
                    BoomNum = userData.GetInt("boom"),
                    TotalGold = userData.GetLong("ttscore"),
                    Score = userData.GetInt("score"),
                    RocketNum = userData.GetInt("rocket"),
                    Seat = userData.GetInt("seat"),
                    Cards = userData.GetIntArray("cards"),
                    Rate = userData.GetInt("rate"),
                    Nm = userData.GetInt("nm"),
                    Dz = userData.GetInt("dz"),*/

                    SuperBoom = userData.GetInt("sboom"),
                    Gold = userData.GetInt("gold"),
                    ThrowOutNum = userData.GetInt("throwout"),
                    BoomNum = userData.GetInt("boom"),
                    TotalGold = userData.GetLong("ttscore"),
                    Score = userData.GetInt("score"),
                    CardNum = userData.GetInt("cardnum"),
                    RocketNum = userData.GetInt("rocket"),
                    Seat = userData.GetInt("seat"),
                    Ttgold = userData.GetLong("ttgold"),
                    Cards = userData.GetIntArray("cards"),
                    Win = userData.GetBool("win")
                };

                App.GetGameData<GlobalData>().OnUserSocreChanged(userData.GetInt("seat"), userData.GetInt("gold"));
            }
            return totalArr;
        }

        private void AddUserItemInfo(UserRecord data)
        {
            var gob = NGUITools.AddChild(PlayerRecordGrid, OnerdRecordItemOrg);
            gob.SetActive(true);

            var userRecordItem = gob.GetComponent<OneRoundRecordItem>();

            var userSeat = data.Seat;




            //昵称
            var userInfo = App.GetGameData<GlobalData>().GetUserInfo(userSeat);
            if (userInfo != null) userRecordItem.NiChengLabel.text = userInfo.GetUtfString(RequestKey.KeyName);

            //剩余牌数
            userRecordItem.LeftCdsLabel.text = data.CardNum.ToString(CultureInfo.InvariantCulture);

            //炸弹数量
            userRecordItem.BombLabel.text = data.BoomNum.ToString(CultureInfo.InvariantCulture);

            var gold = data.Gold;

            //成绩
            userRecordItem.RecorLabel.text = gold.ToString(CultureInfo.InvariantCulture);

            //是否胜利
            userRecordItem.Winsp.SetActive(gold > 0);


            //设置如果是玩家自己则设置字体为黄色,并设置相应成绩
            if (userSeat == App.GetGameData<GlobalData>().GetSelfSeat)
            {
                userRecordItem.NiChengLabel.color = Color.yellow;
  
                WinTittle.SetActive(gold>0);
                LoseTittle.SetActive(gold<0);
                Facade.Instance<MusicManager>().Play(gold > 0 ? "k_win" : "k_lose");
            }

        }


        /// <summary>
        /// 每盘结束时数据统计记录
        /// </summary>
        class UserRecord
        {
/*            /// <summary>
            /// 当前余额
            /// </summary>
            public long TotalGold;
            /// <summary>
            /// 本局打出的炸弹数
            /// </summary>
            public int BoomNum;
            /// <summary>
            /// 火箭
            /// </summary>
            public int RocketNum;
            /// <summary>
            /// 本局出牌的次数
            /// </summary>
            public int ThrowOutNum;
            /// <summary>
            /// 超级炸弹
            /// </summary>
            public int SuperBoom;
            public int Score;
            public int[] Cards;

            /// <summary>
            ///  输赢的分数
            /// </summary>
            public int Gold;
            /// <summary>
            /// 座位号
            /// </summary>
            public int Seat;

            /// <summary>
            /// 加倍信息
            /// </summary>
            public int Rate;
            /// <summary>
            /// 农民得分
            /// </summary>
            public int Nm;
            /// <summary>
            /// 地主得分
            /// </summary>
            public int Dz;*/

            public int SuperBoom { get; set; }

            public int Gold { get; set; }

            public int ThrowOutNum { get; set; }

            public int BoomNum { get; set; }

            public long TotalGold { get; set; }

            public int Score { get; set; }

            public int CardNum { get; set; }

            public int RocketNum { get; set; }

            public int Seat { get; set; }

            public long Ttgold { get; set; }

            public int[] Cards { get; set; }

            public bool Win { get; set; }
        }
    }
}
