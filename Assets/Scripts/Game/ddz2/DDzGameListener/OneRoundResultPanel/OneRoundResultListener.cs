using System.Collections;
using System.Globalization;
using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Assets.Scripts.Game.ddz2.DDz2Common;
using Assets.Scripts.Game.ddz2.DDzGameListener.InfoPanel;
using Assets.Scripts.Game.ddz2.InheritCommon;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.ConstDefine;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Game.ddz2.DDzGameListener.OneRoundResultPanel
{
    public class OneRoundResultListener : ServEvtListener
    {

        [SerializeField]
        protected GameObject OneRoundUiGob;

        /// <summary>
        /// 单局成绩的缓存
        /// </summary>
        protected ISFSObject OneRoundRecordData;

        //---ui信息项--start

        [SerializeField]
        protected UILabel BoomLabel;

        [SerializeField]
        protected UILabel SpringLabel;

        [SerializeField]
        protected UILabel QdzLabel;

        [SerializeField]
        protected GameObject OnerdRecordItemOrg;


        [SerializeField]
        protected GameObject PlayerRecordGrid;

        /// <summary>
        /// 获得金币
        /// </summary>
        [SerializeField]
        protected UILabel GetRewordLabel;

        /// <summary>
        /// 共**倍
        /// </summary>
        [SerializeField]
        protected UILabel TotalBeiLabel;

        /// <summary>
        /// 返回到大厅的按钮，只有娱乐房中可以出现
        /// </summary>
        [SerializeField]
        protected GameObject BackToHallBtn;
        
        /// <summary>
        /// 继续游戏按钮
        /// </summary>
        [SerializeField]
        protected GameObject ContinueBtn;

        /// <summary>
        /// 返回大厅与继续游戏按钮控制
        /// </summary>
        [SerializeField]
        protected UIGrid BtnGrid;


        /// <summary>
        /// 标记地主座位
        /// </summary>
        private int _dizhuSeat;

        /// <summary>
        /// 规则说明
        /// </summary>
        [SerializeField]
        protected UILabel RuleInfoLabel;

        /// <summary>
        /// 查看战绩贴图normal状态贴图名
        /// </summary>
        [SerializeField] protected string GameOverSpriteNameNormal;

        /// <summary>
        /// 查看战绩贴图over状态贴图名
        /// </summary>
        [SerializeField] protected string GameOverSpriteNameOver;


        protected bool FinishAnim;

        protected bool AllowReady;

        //----------------------------------------------------end

        protected override void OnAwake()
        {
            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyGetGameInfo, OnGameInfo);
            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyOnRejoin, OnRejoinGame);
            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyOnUserReady, OnUserReady);
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeFirstOut, OnTypeFirstOut);
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeOneRoundOver, OnTypeOneRoundOver);
            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyRoomGameOver, OnGameOverEvt);
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeAllocate,OnAllocate);
            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.AllowReady, OnAllowReady);
        }


        private void OnAllocate(DdzbaseEventArgs obj)
        {
            GetAllPlayerInfo();
        }

        void GetAllPlayerInfo()
        {
            var gdata = App.GetGameData<DdzGameData>();
            var len = _userRecords.Length;
            for (int i = 0; i < len; i++)
            {
                var userInfo = gdata.GetOnePlayerInfo(i, true);
                if (userInfo == null) continue;
                var userRecord = _userRecords[i];
                if (userRecord == null)
                {
                    userRecord = new UserRecord();
                    _userRecords[i] = userRecord;
                }
                GetOnePlayerInfo(userRecord, userInfo);
            }
        }

        void GetOnePlayerInfo(UserRecord userRecord,YxBaseGameUserInfo userInfo)
        {
            userRecord.PlayerName = userInfo.NickM;
            userRecord.AvatarX = userInfo.AvatarX;
            userRecord.SexI = userInfo.SexI;
        }

        /// <summary>
        /// 游戏结束
        /// </summary>
        /// <param name="args"></param>
        private void OnGameOverEvt(DdzbaseEventArgs args)
        {
            //游戏结束的时候,将继续游戏按钮的贴图换为查看战绩
            if (ContinueBtn == null) return;
            var spr = ContinueBtn.GetComponent<UISprite>();
            spr.spriteName = GameOverSpriteNameNormal;

            var btn = ContinueBtn.GetComponent<UIButton>();
            btn.normalSprite = GameOverSpriteNameNormal;
            btn.hoverSprite = GameOverSpriteNameOver;
            btn.pressedSprite = GameOverSpriteNameOver;
        }


        //当收到自己已经成功准备了的时候，隐藏onRoundpanel
        protected void OnUserReady(DdzbaseEventArgs args)
        {
            if (args.IsfObjData.GetInt(RequestKey.KeySeat) == App.GetGameData<DdzGameData>().SelfSeat)
            {
                FinishAnim = false;
                AllowReady = false;
                OneRoundUiGob.SetActive(false);
            }
        }



        /// <summary>
        /// 重连游戏
        /// </summary>
        /// <param name="args"></param>
        protected void OnRejoinGame(DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            if (data.ContainsKey(NewRequestKey.KeyRule))
                if (RuleInfoLabel != null) RuleInfoLabel.text = data.GetUtfString(NewRequestKey.KeyRule);

            if (data.ContainsKey(NewRequestKey.KeyLandLord))
                _dizhuSeat = data.GetInt(NewRequestKey.KeyLandLord);
        }


        /// <summary>
        /// 当确定地主后,更新地主标记
        /// </summary>
        /// <param name="args"></param>
        private void OnTypeFirstOut(DdzbaseEventArgs args)
        {
            _dizhuSeat = args.IsfObjData.GetInt(RequestKey.KeySeat);
        }

        IEnumerator ShowUiInfoLater()
        {
            yield return new WaitForSeconds(3f);
            FinishAnim = true;
            RefreshUiInfo();
        }

        public void OnClickContinueBtn()
        {
            if (App.GetGameData<DdzGameData>().IsRoomGame)
            {
                App.GetRServer<DdzGameServer>().SendPlayerReadyServCmd();
            }

            OneRoundUiGob.SetActive(false);
            Facade.EventCenter.DispatchEvent<string, DdzbaseEventArgs>(GlobalConstKey.KeyShowReadyBtn);
        }
       

        /// <summary>
        /// 返回大厅
        /// </summary>
        public void BackToHall()
        {
            App.QuitGame();
        }


        protected UserRecord[] GetUsersRecord(ISFSObject sfsObject)
        {
            ISFSArray tempArr = sfsObject.GetSFSArray("users");
            var totalArr = new UserRecord[3];
            var gdata = App.GetGameData<DdzGameData>();
            for (int i = 0; i < tempArr.Count; i++)
            {
                var userData = tempArr.GetSFSObject(i);
                int seat = userData.GetInt("seat");
                totalArr[i] = new UserRecord
                {
                    SuperBoom = userData.GetInt("sboom"),
                    Gold = userData.GetInt("gold"),
                    ThrowOutNum = userData.GetInt("throwout"),
                    BoomNum = userData.GetInt("boom"),
                    TotalGold = userData.GetLong("ttscore"),
                    Score = userData.GetInt("score"),
                    RocketNum = userData.GetInt("rocket"),
                    Cards = userData.GetIntArray("cards"),
                    Rate = userData.GetInt("rate"),
                    Nm = userData.GetInt("nm"),
                    Dz = userData.GetInt("dz"),
                    Index = i,
                    Seat = seat,
                    PlayerName = gdata.GetOnePlayerInfo(seat).NickM
                };

                gdata.OnUserSocreChanged(userData.GetInt("seat"), userData.GetInt("gold"));
            }
            return totalArr;
        }

        private string _soundNameResultWin = "k_win";

        private string _soundNameResultLose = "k_lose";


        private void AddUserItemInfo(UserRecord data)
        {
            var gob = PlayerRecordGrid.AddChild(OnerdRecordItemOrg);
            gob.SetActive(true);

            var userRecordItem = gob.GetComponent<OneRoundRecordItem>();
            userRecordItem.SetOneRoundRecordItemData(data,_dizhuSeat);

            //播放音效
            if (data.Seat == App.GameData.SelfSeat)
            {
                string soundName = data.Gold >= 0 ? _soundNameResultWin : _soundNameResultLose;
                Facade.EventCenter.DispatchEvent(GlobalConstKey.PlaySoundAndPauseBgSound, soundName);
            }

        }


        public GameObject WinTexture;

        public GameObject LostTexture;

        public GameObject WinEffect;

        public GameObject LostEffect;

        /// <summary>
        /// 玩家头像
        /// </summary>
        public GameObject HeadImage;

        private readonly UserRecord[] _userRecords = new UserRecord[3];

        /// <summary>
        /// 当一局游戏结算时
        /// </summary>
        /// <param name="args"></param>     
        protected void OnTypeOneRoundOver(DdzbaseEventArgs args)
        {
            OneRoundRecordData = args.IsfObjData;
            if (OneRoundRecordData == null) return;

            if (BackToHallBtn)
            {
                SetActive(BackToHallBtn, !App.GetGameData<DdzGameData>().IsRoomGame);
                if (BtnGrid != null)
                {
                    BtnGrid.repositionNow = true;
                    BtnGrid.Reposition();
                }
            }

            var userDataArray = OneRoundRecordData.GetSFSArray("users");
            
            int ante = OneRoundRecordData.GetInt("ante");
            int len = _userRecords.Length;
            for (int i = 0; i < len; i++)
            {
                SetUserRecord(_userRecords[i], userDataArray.GetSFSObject(i), i, ante);
            }

            RefreshPlayerInfo(args.IsfObjData);         //刷新玩家信息
            StartCoroutine(ShowUiInfoLater());          //延迟显示结算界面
        }


        private void SetUserRecord(UserRecord userRecord, ISFSObject userData,int index,int ante)
        {
            userRecord.SuperBoom = userData.GetInt("sboom");
            userRecord.Gold = userData.GetInt("gold");
            userRecord.ThrowOutNum = userData.GetInt("throwout");
            userRecord.BoomNum = userData.GetInt("boom");
            userRecord.TotalGold = userData.GetLong("ttscore");
            userRecord.Score = userData.GetInt("score");
            userRecord.RocketNum = userData.GetInt("rocket");
            userRecord.Cards = userData.GetIntArray("cards");
            userRecord.Rate = userData.GetInt("rate");
            userRecord.Nm = userData.ContainsKey("nm") ? userData.GetInt("nm") : userData.GetInt("dz");
            userRecord.Dz = userData.GetInt("dz");
            userRecord.Seat = userData.GetInt("seat");
            userRecord.Ante = ante;
            userRecord.Index = index;
        }


        /// <summary>
        /// 刷新玩家数据
        /// </summary>
        /// <param name="data"></param>
        void RefreshPlayerInfo(ISFSObject data)
        {
            //刷新玩家的数据
            var users = data.GetSFSArray(RequestKey.KeyUserList);
            var gdata = App.GetGameData<DdzGameData>();
            foreach (ISFSObject u in users)
            {
                int seat = u.GetInt(RequestKey.KeySeat);
                var player = gdata.GetPlayer<DdzPlayer>(seat, true);
                player.Coin = u.GetLong("ttgold");
            }
        }


        public override void RefreshUiInfo()
        {
            if (!AllowReady || !FinishAnim)
            {
                StartCoroutine(ShowUiInfoLater());          //延迟显示结算界面
                return;
            }
            if (OneRoundRecordData == null) return;

            OneRoundUiGob.SetActive(true);
            DDzUtil.ClearPlayerGrid(PlayerRecordGrid);


            var boom = OneRoundRecordData.GetInt("boom");

            var rocket = OneRoundRecordData.GetInt("rocket");
            var spring = OneRoundRecordData.GetInt(NewRequestKey.KeySpring);
            var modeSize = OneRoundRecordData.GetInt(GlobalConstKey.C_Score);//得分

            var ante = OneRoundRecordData.GetInt("ante");

            BoomLabel.text = (boom + rocket).ToString(CultureInfo.InvariantCulture);

            SpringLabel.text = spring.ToString(CultureInfo.InvariantCulture);

            QdzLabel.text = modeSize.ToString(CultureInfo.InvariantCulture);

            var usersLen = _userRecords.Length;
            for (var i = 0; i < usersLen; i++)
            {
                if (_userRecords[i] == null) continue;

                var data = _userRecords[i];
                if (data.Seat == App.GetGameData<DdzGameData>().SelfSeat)
                {
                    GetRewordLabel.text = data.Gold.ToString(CultureInfo.InvariantCulture);
                    int rate = data.Seat == _dizhuSeat ? Mathf.Abs(data.Dz) / ante : Mathf.Abs(data.Nm) / ante;
                    TotalBeiLabel.text = string.Format("共{0}倍", rate);
                }

                AddUserItemInfo(data);
            }

            var grid = PlayerRecordGrid.GetComponent<UIGrid>();
            grid.repositionNow = true;
            grid.Reposition();

            ISFSArray usersrecord = OneRoundRecordData.GetSFSArray("users");
            foreach (ISFSObject go in usersrecord)
            {
                int seat = go.GetInt("seat");
                if (seat == App.GameData.SelfSeat)
                {
                    int gold = go.GetInt("gold");
                    bool isWinner = gold >= 0;
                    SetWinEffect(isWinner);
                    break;
                }
            }
        }


        private void OnAllowReady(DdzbaseEventArgs obj)
        {
            AllowReady = true;
            RefreshUiInfo();
        }


        /// <summary>
        /// 设置胜利失败的效果
        /// </summary>
        /// <param name="win">是否胜利</param>
        void SetWinEffect(bool win)
        {
            SetActive(WinTexture, win);
            SetActive(WinEffect, win);
            SetActive(HeadImage, win);

            SetActive(LostTexture, !win);
            SetActive(LostEffect, !win);
        }

        void SetActive(GameObject go, bool active)
        {
            if (go != null)
            {
                go.SetActive(active);
            }
        }

        protected void OnGameInfo(DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            if (data.ContainsKey(NewRequestKey.KeyRule))
                if (RuleInfoLabel != null)
                    RuleInfoLabel.text = data.GetUtfString(NewRequestKey.KeyRule);

            FinishAnim = false;
            AllowReady = false;

            if (HeadImage == null) return;
            var selfInfo = App.GameData.GetPlayerInfo();
            if (selfInfo == null) return;

            var ad = HeadImage.GetComponent<Common.Adapters.NguiTextureAdapter>();         
            YxFramwork.Common.DataBundles.PortraitDb.SetPortrait(selfInfo.AvatarX, ad, selfInfo.SexI);         //只显示自己的头像

            GetAllPlayerInfo();     //获取所有玩家信息
        }

        /// <summary>
        /// 每盘结束时数据统计记录
        /// </summary>
        public class UserRecord
        {
            /// <summary>
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
            public int Dz;
            /// <summary>
            /// 玩家的姓名
            /// </summary>
            public string PlayerName;
            /// <summary>
            /// 索引
            /// </summary>
            public int Index;
            /// <summary>
            /// 头像
            /// </summary>
            public string AvatarX;
            /// <summary>
            /// 性别
            /// </summary>
            public int SexI;

            public int Ante;
        }

    }


}