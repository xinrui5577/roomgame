using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.jpmj.MahjongScripts.GameUI;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Mahjong;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using Assets.Scripts.Game.jpmj.MahjongScripts.Helper;
using UnityEngine;
using YxFramwork.Controller;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using YxFramwork.ConstDefine;
using YxFramwork.Common;
using YxFramwork.Enums;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine
{
    public class TableData : MonoBehaviour
    {
        //玩家位子
        protected int _playerSeat = UtilDef.NullSeat;
        //当前操作用户位子
        protected int _currSeat = UtilDef.NullSeat;
        protected int _oldCurrSeat = UtilDef.NullSeat;
        //庄家位子
        protected int _bankerSeat = UtilDef.NullSeat;
        //抓到的麻将
        protected int _getInMahjong = UtilDef.NullMj;
        //打出的麻将
        protected int _outPutMahjong = UtilDef.NullMj;
        //剩余麻将个数
        protected int _leaveMahjongCnt = 0;
        //塞子的点数
        protected int[] _saiziPoint = new int[2];
        //重连时 手中牌状态 打出了 还是在手中0 还是已经打出1
        protected int _reconectStatus = UtilDef.DefInt;
        //游戏状态
        protected EnGameStatus _currGameStatus = EnGameStatus.None;
        //房间信息
        [HideInInspector]
        public RoomInfo RoomInfo = new RoomInfo();
        //用户信息
        [HideInInspector]
        public UserData[] UserDatas = new UserData[UtilDef.GamePlayerCnt];
        //手牌信息
        [HideInInspector]
        public List<int>[] UserHardCard = new List<int>[UtilDef.GamePlayerCnt];
        [HideInInspector]
        public int[] UserHardCardNum = new int[UtilDef.GamePlayerCnt];
        //出牌信息
        [HideInInspector]
        public List<int>[] UserOutCard = new List<int>[UtilDef.GamePlayerCnt];
        //吃碰杠信息
        [HideInInspector]
        public List<CpgData>[] UserCpg = new List<CpgData>[UtilDef.GamePlayerCnt];
        //游戏操作菜单开关
        protected SwitchCombination _switchGameOpreateMenu = new SwitchCombination(5, 0);
        //游戏结果
        [HideInInspector]
        public GameResult Result;
        //游戏总结算
        [HideInInspector]
        public TotalResult TotalResult;
        //游戏牌分组
        protected Dictionary<int, int> _typeDic = new Dictionary<int, int>();
        //是否是短线重连
        protected bool _isReconect;
        //重连时当前用户的时间
        [HideInInspector]
        public int ReconnectTime;
        //翻牌
        protected int _fanpai = UtilDef.NullMj;
        //癞子
        protected int _laizi = UtilDef.NullMj;
        //是否玩家已经打过牌
        protected bool _isOutPutCard;
        //听牌
        protected int[] _tings;
        protected int[] _youjin;
        //是否已经听牌
        protected bool[] _isTing = new bool[UtilDef.GamePlayerCnt];

        //测试用自动打牌标记
        protected bool ziDongDaPai = false;

        //是否为暗宝
        [HideInInspector]
        public bool anBao;

        //宝牌值
        [HideInInspector]
        public int bao = UtilDef.NullMj;

        //泉州麻将胡类型
        [HideInInspector]
        public QzmjHuType QzmjHuType = QzmjHuType.No;

        //使用查听功能
        public bool UsingQueryHu;
        protected QueryHuList QueryHuList;

        [HideInInspector]
        public int OwnerSeat;
        [HideInInspector]
        public int OwnerId;

        //第一次庄家位子 随机庄 计算圈用
        [HideInInspector]
        public int FristBankerSeat;

        protected virtual void Awake()
        {
            QueryHuList = gameObject.AddComponent<JpQueryHuList>();
            UtilData.UsingQueryHu = UsingQueryHu;

            RegisteEvent();
        }

        protected virtual void RegisteEvent()
        {
            EventDispatch.Instance.RegisteEvent((int)TableDataEventId.OnTrusteeshipClick, OnTrusteeship);
        }

        public TableData()
        {
            for (int i = 0; i < UtilDef.GamePlayerCnt; i++)
            {
                UserDatas[i] = new UserData();
            }

            InitData();
        }

        protected virtual void InitData()
        {
            _currSeat = UtilDef.NullSeat;
            _oldCurrSeat = UtilDef.NullSeat;
            _bankerSeat = UtilDef.NullSeat;
            _getInMahjong = UtilDef.NullMj;
            _outPutMahjong = UtilDef.NullMj;
            _leaveMahjongCnt = RoomInfo.SysMahjongCnt;
            _saiziPoint = new int[2];
            UserHardCard = new List<int>[UtilDef.GamePlayerCnt];
            UserHardCardNum = new int[UtilDef.GamePlayerCnt];
            UserOutCard = new List<int>[UtilDef.GamePlayerCnt];
            UserCpg = new List<CpgData>[UtilDef.GamePlayerCnt];
            _switchGameOpreateMenu = new SwitchCombination(5, 0);
            _typeDic = new Dictionary<int, int>();
            _isOutPutCard = false;
            _tings = null;
            _isTing = new bool[UtilDef.GamePlayerCnt];
            QzmjHuType = QzmjHuType.No;
            for (int i = 0; i < UtilDef.GamePlayerCnt; i++)
            {
                UserHardCard[i] = new List<int>();
                UserOutCard[i] = new List<int>();
                UserCpg[i] = new List<CpgData>();
            }
        }

        public virtual void resetUserDetail()
        {
            for (int i = 0; i < UtilDef.GamePlayerCnt; i++)
            {
                UserDatas[i] = new UserData();
            }
        }

        public virtual int PlayerSeat
        {
            get { return _playerSeat; }
            set
            {
                _playerSeat = value;
                UtilData.PlayerSeat = value;
            }
        }

        public virtual int GetInMahjong
        {
            get { return _getInMahjong == 0 ? UtilDef.DefInt : _getInMahjong; }
            set
            {
                _getInMahjong = value;
                //如果有赖子牌
                EventData data = new EventData(_getInMahjong);
                if (_laizi != UtilDef.NullMj)
                    data.data2 = Laizi;

                EventDispatch.Dispatch((int)GameEventId.GetInCard, data);
                UserHardCard[_currSeat].Add(value);
            }
        }

        public virtual int OutPutMahjong
        {
            get { return _outPutMahjong; }
            set
            {
                _outPutMahjong = value;
                EventDispatch.Dispatch((int)GameEventId.OutPuCard, new EventData(_outPutMahjong, Laizi));

                UserOutCard[_currSeat].Add(value);
                if (_currSeat == _playerSeat)
                {
                    _isOutPutCard = true;
                    UserHardCard[_currSeat].Remove(value);
                    UserOutCard[_currSeat].Add(value);
                }
                else
                {
                    if (UserHardCard[_currSeat].Count > 0)
                        UserHardCard[_currSeat].RemoveAt(0);
                }
            }
        }

        public virtual int LeaveMahjongCnt
        {
            get { return _leaveMahjongCnt; }
            set
            {
                _leaveMahjongCnt = value;
                EventDispatch.Dispatch((int)UIEventId.MahjongCnt, new EventData(_leaveMahjongCnt));
            }
        }

        public virtual int ReconectStatus
        {
            get { return _reconectStatus; }
        }

        public virtual int CurrSeat
        {
            get { return _currSeat; }
            set
            {
                _oldCurrSeat = _currSeat;
                _currSeat = value;
                EventDispatch.Dispatch((int)GameEventId.ChangeCurrUser, new EventData(GetChairId(_currSeat), GetChairId(_oldCurrSeat)));
                EventDispatch.Dispatch((int)UIEventId.ChangeCurrUser, new EventData(GetChairId(_currSeat), GetChairId(_oldCurrSeat)));
            }
        }

        public virtual int OldCurrSeat
        {
            get { return _oldCurrSeat; }
        }

        public virtual int CurrChair
        {
            get { return GetChairId(_currSeat); }
        }

        public virtual int OldCurrChair
        {
            get { return GetChairId(_oldCurrSeat); }
        }

        public int[] Tinglist { get { return _tings; } }
        public int[] Youjin { get { return _youjin; } }

        //设置操作菜单的值
        protected virtual int GameOpreateMenu
        {
            set
            {
                int[] checkOpArray =
                {
                    MjRequestData.MJOpreateTypeChi,
                    MjRequestData.MJOpreateTypePeng,
                    MjRequestData.MJOpreateTypeGang,
                    MjRequestData.MJOpreateTypeHu,
                    MjRequestData.MJOpreateTypeXFG,
                    MjRequestData.MJOpreateTypeTing
                };

                int[] menuArray =
                {
                    OpreateMenu.SwitchChi,
                    OpreateMenu.SwitchPeng,
                    OpreateMenu.SwitchGang,
                    OpreateMenu.SwitchHu,
                    OpreateMenu.SwitchGang,
                    OpreateMenu.SwitchTing
                };

                _switchGameOpreateMenu.DisableAll();
                for (int i = 0; i < checkOpArray.Length; i++)
                {
                    if (IsHaveOpreate(checkOpArray[i], value))
                    {
                        _switchGameOpreateMenu.EnableCondition(menuArray[i]);
                    }
                }
            }
        }

        public virtual SwitchCombination PlayerOpreateMenu
        {
            get { return _switchGameOpreateMenu; }
        }

        //改变操作值 如果设置为0时 隐藏操作菜单
        protected virtual int GameOpreateMenuWithEvent
        {
            set
            {
                GameOpreateMenu = value;

                if (value == 0)
                {
                    EventDispatch.Dispatch((int)UIEventId.OperationMenu, new EventData());
                    return;
                }

                if (_switchGameOpreateMenu.IsForbiddenAll())
                {
                    EventDispatch.Dispatch((int)UIEventId.OperationMenu, new EventData());
                    return;
                }

                EventDispatch.Dispatch((int)UIEventId.OperationMenu, new EventData(_switchGameOpreateMenu));
            }
        }

        public virtual int BankerSeat
        {
            get { return _bankerSeat; }
            set
            {
                _bankerSeat = value;
                EventDispatch.Dispatch((int)UIEventId.Banker, new EventData(GetChairId(_bankerSeat)));
                EventDispatch.Dispatch((int)GameEventId.Banker, new EventData(GetChairId(_bankerSeat)));
            }
        }

        public virtual int[] SaiziPoint
        {
            get { return _saiziPoint; }
            set
            {
                _saiziPoint = value;
                EventDispatch.Dispatch((int)GameEventId.SaiziPoint, new EventData(_saiziPoint));

            }
        }

        public virtual bool IsReconect
        {
            get { return _isReconect; }
        }

        public virtual int Laizi
        {
            get { return _laizi; }
            set
            {
                _laizi = value;
                JpMahjongPlayerHard.CaishenValue = _laizi;
                if (_laizi <= 0)
                    _laizi = UtilDef.NullMj;
            }
        }

        public virtual int Fanpai
        {
            get { return _fanpai; }
            set
            {
                _fanpai = value;
                if (_fanpai <= 0)
                    _fanpai = UtilDef.NullMj;
            }
        }

        public virtual int CurrRound
        {
            get { return RoomInfo.CurrRound; }
            set
            {
                RoomInfo.CurrRound = value;
                EventDispatch.Dispatch((int)UIEventId.CurrRound, new EventData(RoomInfo.GetGameLoopString()));
            }

        }

        public virtual bool[] IsTing
        {
            get { return _isTing; }
        }

        //设置游戏玩家
        public virtual int SetUserInfo(ISFSObject data)
        {
            UserData user = new UserData();
            user.ParseData(data);

            //如果当前用户不是空的 只更新用户信息不处理事件
            if (UserDatas[user.Seat].IsNull == false)
            {
                //重新赋值用户信息
                UserDatas[user.Seat] = user;
                return UserDatas[user.Seat].Seat;
            }

            UserDatas[user.Seat] = user;

            EventDispatch.Dispatch((int)UIEventId.GetIpAndCountry, new EventData(GetChairId(user.Seat), user.ip, user.Country));

            //如果是当前用户并且没有准被 发送准备消息
            if (user.Seat == PlayerSeat && user.IsReady == false)
            {
                EventDispatch.Dispatch((int)NetEventId.OnUserReady, new EventData());
            }

            bool isAllReady = IsAllReady();
            //如果全部准备了 发送准备消息
            if (isAllReady && !_isReconect)
            {
                App.GameData.GStatus = YxEGameStatus.Play;

                EventDispatch.Dispatch((int)UIEventId.GameStart, new EventData());
            }
            //设置声音的性别
            SoundManager.Instance.SetSex(GetChairId(user.Seat), UserDatas[user.Seat].Sex);

            return user.Seat;
        }

        public virtual void SetRoomInfo(ISFSObject data)
        {
            RoomInfo.ParseData(data);
            if (data.ContainsKey(RequestKeyOther.KeyCardLen)) LeaveMahjongCnt = data.GetInt(RequestKeyOther.KeyCardLen);
            anBao = RoomInfo.AnBao == 1;

            if (data.ContainsKey(RequestKeyOther.KeyPlayerNum))
                UtilData.CurrGamePalyerCnt = data.GetInt(RequestKeyOther.KeyPlayerNum);

            EventDispatch.Dispatch((int)GameEventId.RoomInfo, new EventData(RoomInfo));
            EventDispatch.Dispatch((int)UIEventId.RoomInfo, new EventData(RoomInfo));
        }

        public virtual void OnUserJoinGame(ISFSObject data)
        {
            int seat = SetUserInfo(data);
            int chair = GetChairId(seat);

            EventDispatch.Dispatch((int)UIEventId.UserJionIn, new EventData(chair, UserDatas[seat]));
        }

        public virtual void OnUserReady(int seat)
        {
            UserDatas[seat].IsReady = true;
            int chair = GetChairId(seat);
            EventDispatch.Dispatch((int)UIEventId.UserReady, new EventData(chair, true));

            bool isAllReady = IsAllReady();
            //如果全部准备了 发送准备消息
            if (isAllReady && !_isReconect)
            {
                App.GameData.GStatus = YxEGameStatus.Play;

                EventDispatch.Dispatch((int)UIEventId.GameStart, new EventData());
            }

        }

        public virtual void OnUserOut(int seat)
        {
            UserDatas[seat].IsNull = true;
            int chair = GetChairId(seat);
            EventDispatch.Dispatch((int)UIEventId.UserOutRoom, new EventData(chair, true));


            ////如果游戏没有开始 退出房间
            //if (App.GameData.GStatus == GameStatus.Normal)
            //{
            //    UserDatas[seat].IsNull = true;
            //    int chair = GetChairId(seat);
            //    EventDispatch.Dispatch((int)UIEventId.UserOutRoom, new EventData(chair, true));
            //}
            //else//如果游戏已经开始了 显示为离线
            //{
            //    UserDatas[seat].IsOutLine = true;
            //    int chair = GetChairId(seat);
            //    EventDispatch.Dispatch((int)UIEventId.UserOutLine, new EventData(chair, true));
            //}
        }


        public virtual void OnUserIdle(int seat)
        {
            UserDatas[seat].IsOutLine = true;
            int chair = GetChairId(seat);
            EventDispatch.Dispatch((int)UIEventId.UserOutLine, new EventData(chair, true));
        }

        public virtual void OnUserOnline(int seat)
        {

            UserDatas[seat].IsOutLine = false;
            int chair = GetChairId(seat);
            EventDispatch.Dispatch((int)UIEventId.UserOutLine, new EventData(chair, false));
        }

        // 本家以外的玩家退出房间
        public virtual void OnOtherUserOut()
        {
            for (int i = 0; i < UtilDef.GamePlayerCnt; i++)
            {
                int iSeat = UtilFunc.GetChairId(i);
                if (0 != iSeat)
                {
                    UserDatas[i].IsNull = true;
                    int chair = GetChairId(i);
                    EventDispatch.Dispatch((int)UIEventId.UserOutRoom, new EventData(chair, true));
                }
            }
        }

        public virtual void OnSaiziPoint(ISFSObject data)
        {
            _currSeat = BankerSeat = data.GetInt(RequestKeyOther.KeyStartP);
            SaiziPoint = data.GetIntArray(RequestKeyOther.KeyDiceArray);
        }

        public virtual void OnSendCard(ISFSObject data)
        {
            _currSeat = data.GetInt(RequestKey.KeySeat);
            int[] playerMj = data.GetIntArray(RequestKey.KeyCards);
            CopyIntArryToMjList(playerMj, UserHardCard[_playerSeat]);

            if (data.ContainsKey(RequestKeyOther.CardLaizi))
                Laizi = data.GetInt(RequestKeyOther.CardLaizi);

            if (data.ContainsKey(RequestKeyOther.CardFan))
            {
                Fanpai = data.GetInt(RequestKeyOther.CardFan);
            }

            var eventData = new OnSendCardEventData
            {
                laizi = Laizi,
                fanpai = Fanpai,
                playermj = playerMj,
                currChair = GetChairId(_currSeat),
                reduceMjCnt = value =>
                {
                    LeaveMahjongCnt = LeaveMahjongCnt - value;
                },

                SaiziPoint = SaiziPoint
            };
            EventDispatch.Dispatch((int)UIEventId.HideJiaPiaoEffect, new EventData());
            EventDispatch.Dispatch((int)GameEventId.SendCard, new EventData(eventData));
        }

        protected int OutCardDelayTimeIndex;
        public virtual void OnGetInCard(ISFSObject data)
        {
            CurrSeat = data.GetInt(RequestKey.KeySeat);
            GetInMahjong = data.GetInt(RequestKey.KeyOpCard);
            OnOperate(data);
            LeaveMahjongCnt--;

            //抓牌用户 听牌的 并且 当前无菜单提示 自动打牌
            if (CurrSeat == PlayerSeat && IsTing[CurrSeat] && _switchGameOpreateMenu.IsForbiddenAll())
            {
                OutCardDelayTimeIndex = DelayTimer.StartTimer(
                    GameConfig.GetInCardRoteTime + GameConfig.GetInCardWaitTime + GameConfig.TingPaiWaitTime,
                    () => EventDispatch.Dispatch((int)NetEventId.OnOutMahjong, new EventData(GetInMahjong)));
            }

            //自动打牌，抓啥打啥，仅测试用
            if (CurrSeat == PlayerSeat && ziDongDaPai)
            {
                OutCardDelayTimeIndex = DelayTimer.StartTimer(
                    GameConfig.GetInCardRoteTime + GameConfig.GetInCardWaitTime + GameConfig.TingPaiWaitTime,
                    () => EventDispatch.Dispatch((int)NetEventId.OnOutMahjong, new EventData(GetInMahjong)));
            }

            //托管
            if (CurrSeat == PlayerSeat && TrusteeshipHelper.Instance != null && TrusteeshipHelper.Instance.IsTrusteeship)
            {
                if (TrusteeshipHelper.Instance.IsHaveOperator()) return;

                OutCardDelayTimeIndex = DelayTimer.StartTimer(
                    GameConfig.GetInCardRoteTime + GameConfig.GetInCardWaitTime + GameConfig.TingPaiWaitTime,
                    () => EventDispatch.Dispatch((int)NetEventId.OnOutMahjong, new EventData(GetInMahjong)));
            }
        }

        public virtual void StopAutoOutCard()
        {
            DelayTimer.StopTimer(OutCardDelayTimeIndex);
        }

        //private int MenuDelayTimeId;
        public virtual void OnOperate(ISFSObject data)
        {
            if (data.ContainsKey(RequestKeyOther.KeyOp))
            {
                var op = data.GetInt(RequestKeyOther.KeyOp);

                //托管
                if (TrusteeshipHelper.Instance != null)
                {
                    TrusteeshipHelper.Instance.GameOpreateMenu = op;

                    if (TrusteeshipHelper.Instance.IsTrusteeship)
                    {
                        if (TrusteeshipHelper.Instance.IsHaveOperator(op))
                            GameOpreateMenuWithEvent = op;
                        else
                            EventDispatch.Dispatch((int)NetEventId.OnGuoClick, new EventData());
                        return;
                    }
                    else
                        GameOpreateMenuWithEvent = op;
                }
                else
                    GameOpreateMenuWithEvent = op;

                //有听菜单的 tinglist
                if (data.ContainsKey("tingout"))
                {
                    _tings = data.GetIntArray("tingout");
                    if (UsingQueryHu)
                    {
                        QueryHuList.ClearCache();
                        EventDispatch.Dispatch((int)GameEventId.TingList, new EventData(_tings));
                    }
                }
            }
            else
                GameOpreateMenuWithEvent = 0;

            //不带听玩法时候 tinglist
            if (data.ContainsKey("tingoutlist"))
            {
                _tings = data.GetIntArray("tingoutlist");

                if (null != _tings && _tings.Length > 0)
                {
                    if (UsingQueryHu)
                    {
                        QueryHuList.ClearCache();
                        EventDispatch.Dispatch((int)GameEventId.TingList, new EventData(_tings));
                    }
                }
            }

        }

        public virtual void OnThrowCard(ISFSObject data)
        {
            _currSeat = data.GetInt(RequestKey.KeySeat);
            OutPutMahjong = data.GetInt(RequestKey.KeyOpCard);
            GameOpreateMenuWithEvent = 0;
        }

        public virtual void OutPutForGuo()
        {
            if (CurrSeat == PlayerSeat && IsTing[CurrSeat])
            {
                EventDispatch.Dispatch((int)NetEventId.OnOutMahjong, new EventData(_getInMahjong));
            }
        }

        /// <param name="data"></param>
        /// <param name="cpgType"></param>
        public virtual void OnCpg(ISFSObject data)
        {
            CurrSeat = data.GetInt(RequestKey.KeySeat);
            CpgData cpgData = CpgDataCreater.CreateCpg(data);

            //cpg中有赖子牌，标记Icon
            cpgData.Laizi = Laizi;

            //如果是抓杠 改变类型
            if (cpgData.Type == EnGroupType.ZhuaGang)
            {
                CpgZhuaGang cpg = (CpgZhuaGang)cpgData;
                //抓杠时 会有两条数据回来 当消息为true 杠成功了
                if (cpg.Ok == false)
                    return;

                var cpgList = UserCpg[_currSeat];
                for (int i = 0; i < cpgList.Count; i++)
                {
                    var cpgItem = cpgList[i];
                    if (cpgItem.Type == EnGroupType.Peng && cpgItem.Card == cpgData.Card)
                    {
                        cpgList[i] = cpgData;
                        break;
                    }
                }
                //删除手牌数据
                bool bao = data.ContainsKey(RequestKeyOther.KeyBao);
                if (_currSeat == _playerSeat && !bao)
                {
                    UserHardCard[_playerSeat].Remove(cpgData.Card);
                }
                else
                {
                    //杠宝专用
                    if (data.ContainsKey(RequestKeyOther.KeyBao))
                    {
                        EventDispatch.Dispatch((int)GameEventId.RemoveOneWallCard, new EventData(true));
                    }
                }
            }
            else if (cpgData.Type == EnGroupType.AnGang && data.ContainsKey(RequestKeyOther.KeyBao))
            {
                //暗杠杠宝专用
                UserCpg[_currSeat].Add(cpgData);
                //删除手牌数据
                if (_currSeat == _playerSeat)
                {
                    List<int> cards_temp = cpgData.GetHardCards();
                    for (int i = 0; i < cards_temp.Count - 1; i++)
                    {
                        UserHardCard[_playerSeat].Remove(cards_temp[i]);
                    }
                }
                EventDispatch.Dispatch((int)GameEventId.Cpg, new EventData(cpgData, true));
                EventDispatch.Dispatch((int)UIEventId.Cpg, new EventData(cpgData));

                OnGangGold(data);
                return;
            }
            else
            {
                UserCpg[_currSeat].Add(cpgData);
                //删除手牌数据
                if (_currSeat == _playerSeat)
                {
                    foreach (int cardData in cpgData.GetHardCards())
                    {
                        UserHardCard[_playerSeat].Remove(cardData);
                    }
                }
            }

            //吃碰杠带回来的op 需要判断当前用户是否为本家 不是本家的隐藏当前
            if (_currSeat == _playerSeat)
            {
                OnOperate(data);
            }
            else
            {
                GameOpreateMenuWithEvent = 0;
            }

            //第二位暗杠杠宝专用 ，所以是false
            EventDispatch.Dispatch((int)GameEventId.Cpg, new EventData(cpgData, false, GetChairId(_playerSeat)));
            EventDispatch.Dispatch((int)UIEventId.Cpg, new EventData(cpgData));

            OnGangGold(data);
        }

        protected virtual void OnGangGold(ISFSObject data)
        {
            //是否有分数变化
            if (data.ContainsKey(RequestKeyOther.GangGold))
            {
                var goldsSfsObj = data.GetSFSObject(RequestKeyOther.GangGold);

                /*                var list = new List<int>();
                                foreach (var key in goldsSfsObj.GetKeys())
                                {
                                    list.Add(goldsSfsObj.GetInt(key));
                                }
                                var len = list.Count;
                                var glodList = new int[4];//人数少也可以4个长度,不然chair值会导致glodList越界
                                for (int i = 0; i < len; i++)
                                {
                                    int chair = GetChairId(i);
                                    glodList[chair] = list[i];
                                }*/


                var glodList = new int[4];//人数少也可以4个长度,不然chair值会导致glodList越界
                foreach (var key in goldsSfsObj.GetKeys())
                {
                    int chair = GetChairId(int.Parse(key));

                    glodList[chair] = goldsSfsObj.GetInt(key);
                }

                EventDispatch.Dispatch((int)UIEventId.UserGlodChange, new EventData(glodList));
            }
        }

        public virtual void OnTing(ISFSObject data)
        {
            _currSeat = data.GetInt(RequestKey.KeySeat);
            OutPutMahjong = data.GetInt(RequestKey.KeyOpCard);
            _isTing[_currSeat] = true;
            EventDispatch.Dispatch((int)GameEventId.Ting, new EventData(anBao));
            GameOpreateMenuWithEvent = 0;
        }

        public virtual void OnHu(ISFSObject data, int type)
        {
            //胡牌
            Result = new GameResult();
            Result.ParseData(data);
            //设置当前的位子 如果流局没有胡牌玩家
            if (Result.HuSeat.Count > 0) CurrSeat = Result.HuSeat[0];
            //更新手牌
            ISFSArray cards = data.GetSFSArray(RequestKey.KeyCardsArr);
            for (int i = 0; i < cards.Count; i++)
            {
                int[] cardsArr = cards.GetIntArray(i);
                UserHardCard[i] = new List<int>(cardsArr);

                SortMjValue(UserHardCard[i]);
            }
            //冲宝加一张
            if (Result.ChBao)
            {
                UserHardCard[Result.HuSeat[0]].Add(Result.HuCard);
            }

            //如果是自摸 在手牌中移除掉 抓到的牌
            if (Result.HuType == MjRequestData.MJReqTypeZiMo && !Result.ChBao)
            {
                UserHardCard[Result.HuSeat[0]].Remove(Result.HuCard);
            }

            EventDispatch.Dispatch((int)GameEventId.Result, new EventData(this));
            EventDispatch.Dispatch((int)UIEventId.Hu, new EventData(Result));

            //重置准备
            for (int i = 0; i < UtilData.CurrGamePalyerCnt; i++)
            {
                UserDatas[i].IsReady = false;
                UserDatas[i].Glod = Result.TotalGold[i];
            }

            //设置当前轮数
            GetCurrRound(data);
        }

        public virtual void GetCurrRound(ISFSObject data)
        {
            if (RoomInfo.GameLoopType == EnGameLoopType.circle)
            {
                int nextBank = data.GetInt(RequestKeyOther.KeyNextBanker);
                if (nextBank == FristBankerSeat && _bankerSeat != UtilDef.NullSeat && _bankerSeat != FristBankerSeat)
                {
                    CurrRound += 1;
                }
            }
            else if (RoomInfo.GameLoopType == EnGameLoopType.round)
            {
                CurrRound += 1;
            }

            //开房模式下才会有
            if (RoomInfo.RoomType == EnRoomType.FanKa)
            {
                //判断是否是最后一局
                EventDispatch.Dispatch((int)UIEventId.IsGameOver, new EventData(RoomInfo.IsEndRound));
            }
        }

        public virtual void OnHandUp(ISFSObject data)
        {
            if (data.ContainsKey("cmd") && data.GetUtfString("cmd") == "dismiss")
            {
                string username = data.GetUtfString("username");
                int handType = data.GetInt("type");

                if (handType == (int)EnDismissFeedBack.ApplyFor)
                {
                    string[] allUserName = new string[UtilData.CurrGamePalyerCnt];
                    allUserName[0] = UserDatas[_playerSeat].name;
                    int nameIndex = 1;
                    //获得玩家的姓名
                    for (int i = 0; i < UtilData.CurrGamePalyerCnt; i++)
                    {
                        if (i == _playerSeat) continue;
                        allUserName[nameIndex++] = UserDatas[i].name;
                    }

                    EventDispatch.Dispatch((int)UIEventId.HandUp, new EventData(handType, username, allUserName));
                }
                else
                {
                    EventDispatch.Dispatch((int)UIEventId.HandUp, new EventData(handType, username));
                }
            }
        }

        public virtual void OnUserDetail(int seat)
        {
            EventDispatch.Dispatch((int)UIEventId.ShowUserDetail, new EventData(UserDatas[seat]));
        }

        public virtual void OnGameOver(ISFSObject data)
        {
            TotalResult = new TotalResult();
            TotalResult.ParseData(data);
            //如果游戏是解散的直接显示 总结算
            if (!RoomInfo.IsEndRound)
            {
                OnTotalResult();
            }
            else
            {
                EventDispatch.Dispatch((int)UIEventId.GameOverShowOneRoundBtn, new EventData());
            }
        }

        public virtual void OnUserTalk(ISFSObject obj)
        {
            int seat = obj.GetInt(RequestKey.KeySeat);
            if (obj.ContainsKey("text"))
            {
                string text = obj.GetUtfString("text");
                EnChatType type = EnChatType.text;
                EventDispatch.Dispatch((int)UIEventId.UserTalk, new EventData(GetChairId(seat), type, text));
            }

            if (obj.ContainsKey("exp"))
            {
                int key = obj.GetInt("exp");
                EnChatType type = EnChatType.exp;
                if (key < UtilDef.SortTalkPlush)
                {
                    key -= UtilDef.ExpPlush;
                }
                else
                {
                    key -= UtilDef.SortTalkPlush;
                    type = EnChatType.sorttalk;
                }
                EventDispatch.Dispatch((int)UIEventId.UserTalk, new EventData(GetChairId(seat), type, key));
            }

            if (obj.ContainsKey("ani"))
            {
                int key = obj.GetInt("ani");
                string username = obj.GetUtfString("name");
                EventDispatch.Dispatch((int)UIEventId.UserTalk, new EventData(GetChairId(seat), EnChatType.ani, username, key));
            }
        }

        public virtual void OnUserSpeak(ISFSObject obj)
        {
            string url = obj.GetUtfString("url");
            int seat = obj.ContainsKey(RequestKey.KeySeat) ? obj.GetInt("seat") : 0;
            int len = obj.ContainsKey("len") ? obj.GetInt("len") : 1;

            EventDispatch.Dispatch((int)UIEventId.DownLoadVoice, new EventData(url, GetChairId(seat), len));
        }

        public virtual void OnTotalResult()
        {
            EventDispatch.Dispatch((int)UIEventId.ShowTotalResult, new EventData(this));
        }

        public virtual void OnXFGang(ISFSObject obj)
        {
            if (_currSeat == _playerSeat)
            {
                OnOperate(obj);
            }
            else
            {
                GameOpreateMenuWithEvent = 0;
            }

            CpgXFGang xfGang = new CpgXFGang();
            xfGang.ParseData(obj);

            UserCpg[_currSeat].Add(xfGang);
            //删除手牌数据
            if (_currSeat == _playerSeat)
            {
                foreach (int cardData in xfGang.GetHardCards())
                {
                    UserHardCard[_playerSeat].Remove(cardData);
                }
            }

            EventDispatch.Dispatch((int)GameEventId.Cpg, new EventData(xfGang));
            EventDispatch.Dispatch((int)UIEventId.Cpg, new EventData(xfGang));

            OnGangGold(obj);
        }

        public virtual void SetTableData(ISFSObject data)
        {
            SetRoomInfo(data);

            //初始化麻将管理者
            MahjongManager.Instance.InitMahjong(RoomInfo.SysCards);

            _isReconect = data.GetBool(RequestKeyOther.KeyRejoin);
            if (_isReconect)
                App.GameData.GStatus = YxEGameStatus.Play;

            ISFSObject playerInfo = data.GetSFSObject(RequestKey.KeyUser);
            PlayerSeat = playerInfo.GetInt(RequestKey.KeySeat);
            SetUserInfo(playerInfo);
            if (_playerSeat != UtilDef.NullSeat)
                GetMahjongInfoFromSFSData(playerInfo);

            ISFSArray players = data.GetSFSArray(RequestKey.KeyUserList);
            for (int i = 0; i < players.Count; i++)
            {
                ISFSObject player = players.GetSFSObject(i);
                GetMahjongInfoFromSFSData(player);
            }
            //新加房主座位号
            if (data.ContainsKey("ownerId"))
            {
                OwnerId = data.GetInt("ownerId");
                foreach (var userData in UserDatas)
                {
                    if (userData.id == OwnerId)
                    {
                        OwnerSeat = userData.Seat;
                        break;
                    }
                }
            }
            else
            {
                OwnerSeat = 0;
            }

            if (data.ContainsKey(RequestKeyOther.KeyDiceArray))
            {
                _saiziPoint = data.GetIntArray(RequestKeyOther.KeyDiceArray);
            }
            //第一次庄家位子
            if (data.ContainsKey("bank0"))
            {
                FristBankerSeat = data.GetInt("bank0");
            }
            //当前庄家位子
            if (data.ContainsKey(RequestKeyOther.KeyStartP))
            {
                BankerSeat = data.GetInt(RequestKeyOther.KeyStartP);
            }

            if (data.ContainsKey(RequestKeyOther.KeyLastIn))
            {
                _getInMahjong = data.GetInt(RequestKeyOther.KeyLastIn);

                if (IsTing[PlayerSeat] && _switchGameOpreateMenu.IsForbiddenAll())
                {
                    EventDispatch.Dispatch((int)NetEventId.OnOutMahjong, new EventData(_getInMahjong));
                }
            }
            if (data.ContainsKey(RequestKeyOther.KeLastOutCard))
            {
                _outPutMahjong = data.GetInt(RequestKeyOther.KeLastOutCard);
            }
            if (data.ContainsKey(RequestKeyOther.KeyCurrentP))
            {
                _currSeat = data.GetInt(RequestKeyOther.KeyCurrentP);
            }

            if (data.ContainsKey(RequestKeyOther.CardFan))
            {
                Fanpai = data.GetInt(RequestKeyOther.CardFan);
            }

            if (data.ContainsKey(RequestKeyOther.CardBao))
            {
                bao = data.GetInt(RequestKeyOther.CardBao);
            }

            if (data.ContainsKey(RequestKeyOther.CardLaizi))
            {
                Laizi = data.GetInt(RequestKeyOther.CardLaizi);
            }

            int outCard = 0;
            //根据用户的麻将 算出剩余的麻将个数
            for (int i = 0; i < UtilData.CurrGamePalyerCnt; i++)
            {
                outCard += UserHardCardNum[i];
                foreach (CpgData cpgData in UserCpg[i])
                {
                    outCard += cpgData.MahjongCount;
                }

                outCard += UserOutCard[i].Count;
            }

            _leaveMahjongCnt = RoomInfo.SysMahjongCnt - outCard;

            if (_isReconect)
            {
                int seq = data.GetInt(RequestKeyOther.KeySeq);
                int seq2 = data.GetInt(RequestKeyOther.KeySeq2);
                YxDebug.Log("重连状态 seq = " + seq + " seq2 = " + seq2);
                if (seq != seq2)//不相等 当前用户在抓牌
                {
                    _oldCurrSeat = (_currSeat + UtilData.CurrGamePalyerCnt - 1) % UtilData.CurrGamePalyerCnt;
                    List<int> currOutCard = UserOutCard[_oldCurrSeat];
                    if (currOutCard.Count > 0 && currOutCard[currOutCard.Count - 1] != _outPutMahjong)
                    {
                        _oldCurrSeat = UtilDef.NullSeat;
                    }
                    _reconectStatus = 0;
                }
                else//相等的时候 用户已经打牌 等待别人响应
                {
                    _oldCurrSeat = _currSeat;
                    _reconectStatus = 1;
                }

                //重连之后，玩家不是听牌状态，可以进行托管
                if (!_isTing[PlayerSeat] && TrusteeshipHelper.Instance != null && _currGameStatus != EnGameStatus.GameReady)
                {
                    TrusteeshipHelper.Instance.IsAllowTrusteeship = true;
                }
            }

            HupRejion(data);
        }

        public void ChangeZiDongDaPai()
        {
            ziDongDaPai = !ziDongDaPai;
        }

        public virtual void GetMahjongInfoFromSFSData(ISFSObject playerData)
        {
            int seat = SetUserInfo(playerData);

            if (playerData.ContainsKey(RequestKeyOther.KeyHandCards))
            {
                int[] values = playerData.GetIntArray(RequestKeyOther.KeyHandCards);
                CopyIntArryToMjList(values, UserHardCard[seat]);
            }

            if (playerData.ContainsKey(RequestKeyOther.KeyMjGroup))
            {
                ISFSArray Groups = playerData.GetSFSArray(RequestKeyOther.KeyMjGroup);

                for (int j = 0; j < Groups.Count; j++)
                {
                    CpgData cpg = CpgDataCreater.CreateCpg(Groups.GetSFSObject(j));
                    cpg.Seat = seat;
                    UserCpg[seat].Add(cpg);
                }
            }

            if (playerData.ContainsKey(RequestKeyOther.KeyHardNum))
            {
                int num = playerData.GetInt(RequestKeyOther.KeyHardNum);
                UserHardCardNum[seat] = num;
            }

            if (playerData.ContainsKey(RequestKeyOther.KeyOutCard))
            {
                int[] values = playerData.GetIntArray(RequestKeyOther.KeyOutCard);
                CopyIntArryToMjList(values, UserOutCard[seat]);
            }

            //设置开关
            if (seat == _playerSeat && playerData.ContainsKey(RequestKeyOther.KeyOp))
            {
                GameOpreateMenu = playerData.GetInt(RequestKeyOther.KeyOp);
                if (TrusteeshipHelper.Instance != null)
                {
                    TrusteeshipHelper.Instance.GameOpreateMenu = playerData.GetInt(RequestKeyOther.KeyOp);
                }
            }

            if (seat == PlayerSeat && playerData.ContainsKey("tingout"))
            {
                _tings = playerData.GetIntArray("tingout");
            }

            if (playerData.ContainsKey("hasTing"))
            {
                _isTing[seat] = playerData.GetBool("hasTing");
            }
        }

        protected virtual void HupRejion(ISFSObject data)
        {
            if (data.ContainsKey("hupstart"))
            {
                long startTime = data.GetLong("hupstart");
                long nowTime = data.GetLong("hupnow");
                string hup = data.GetUtfString("hup");

                int time = RoomInfo.TouPiaoTimeOut - (int)(nowTime - startTime);

                int LoopIndex = 0;
                while (hup.Length > 0)
                {
                    int index = hup.IndexOf(",");
                    index = index > 0 ? index : hup.Length;
                    string id = hup.Substring(0, index);
                    hup = hup.Substring(index);
                    string userNmae = "";
                    try
                    {
                        userNmae = FindeUserNameByID(int.Parse(id));
                    }
                    catch (Exception e)
                    {
                        YxDebug.LogError(e.Message);
                    }

                    if (LoopIndex == 0)
                    {
                        string[] allUserName = new string[UtilData.CurrGamePalyerCnt];
                        //获得玩家的姓名
                        allUserName[0] = UserDatas[_playerSeat].name;
                        int nameIndex = 1;
                        for (int i = 0; i < UtilData.CurrGamePalyerCnt; i++)
                        {
                            if (i == _playerSeat) continue;
                            allUserName[nameIndex++] = UserDatas[i].name;
                        }
                        EventDispatch.Dispatch((int)UIEventId.HandUp, new EventData(EnDismissFeedBack.ApplyFor, userNmae, allUserName, time));
                    }
                    else
                    {
                        EventDispatch.Dispatch((int)UIEventId.HandUp, new EventData(EnDismissFeedBack.Apply, userNmae));
                    }
                    LoopIndex++;
                }
            }
        }

        public void ChangeStatusToFree()
        {
            _currGameStatus = EnGameStatus.GameFree;
        }

        public virtual void SetGameStatus(EnGameStatus status)
        {
            if (_currGameStatus == status)
            {
                return;
            }
            _currGameStatus = status;
            if (status == EnGameStatus.None)
            {
                return;
            }
            if (_currGameStatus == EnGameStatus.GameFree)
                //正常进入游戏时 清除保存在本地的信息
                CpgLocalSave.Instance.Cleare();


            YxDebug.Log("游戏状态 " + status);

            EventDispatch.Dispatch((int)UIEventId.GameStatus, new EventData(_currGameStatus, this));
            EventDispatch.Dispatch((int)GameEventId.GameStatus, new EventData(_currGameStatus, this));

            _isReconect = false;
        }
        ///------------------------------------------------------------------------------------------------------
        public virtual void OnNetResponseEventBack(int type, ISFSObject data)
        {
            YxDebug.Log("没有处理处理网络事件 type" + type);
        }

        public virtual void OnNetRequestEventBack(NetEventId type, EventData data, YxGameServer netController)
        {
            YxDebug.Log("没有处理处理网络事件 type" + type);
        }
        ///------------------------------------------------------------------------------------------------------
        //获得碰牌数据
        public virtual void GetChiData(DVoidTArray<int> sendCall)
        {
            //查找可吃的
            List<int[]> findList = FindCanChi();
            if (findList.Count > 1)
            {
                //通知UI提示选择
                YxDebug.Log("通知UI 提示吃选择 " + findList.Count);
                DVoidInt sendCallArray = (index) =>
                {
                    if (index < findList.Count && findList[index] != null)
                        sendCall(findList[index]);
                };
                EventDispatch.Dispatch((int)UIEventId.CgChoose, new EventData(findList, sendCallArray, _outPutMahjong));
                //sendCall(findList[0]);
            }
            else
            {
                YxDebug.Log("发送服务器 吃 " + findList[0]);
                sendCall(findList[0]);
            }
        }
        public virtual void GetPengData(DVoidTArray<int> sendCall)
        {
            int[] find = FindCanPeng();
            YxDebug.Log("发送服务器 碰 " + find);
            sendCall(find);
        }

        public virtual void GetGangData(ISFSObject obj, DVoidSfsObject sendCall)
        {
            List<FindGangData> findList = FindCanGang();

            DVoidInt sendCallArray = (index) =>
            {
                YxDebug.Log("发送服务器 杠 " + findList[index].cards);
                if (findList[index].type != UtilDef.DefInt)
                    obj.PutInt(RequestKey.KeyType, findList[index].type);
                if (findList[index].ttype != UtilDef.DefInt)
                    obj.PutInt(RequestKeyOther.KeyTType, findList[index].ttype);
                if (findList[index].cards != null && findList[index].ttype != MjRequestData.CPG_ZhuaGang)
                    obj.PutIntArray(RequestKey.KeyCards, findList[index].cards);
                else if (findList[index].cards != null && findList[index].ttype == MjRequestData.CPG_ZhuaGang)
                    obj.PutInt(RequestKey.KeyOpCard, findList[index].cards[0]);

                sendCall(obj);
            };
            //如果找到的杠 大于1
            if (findList.Count > 1)
            {
                YxDebug.Log("通知UI 提示杠选择 " + findList.Count);
                var lisiCard = findList.Select(t => t.cards).ToList();
                //通知UI提示选择
                EventDispatch.Dispatch((int)UIEventId.CgChoose, new EventData(lisiCard, sendCallArray, _outPutMahjong));
            }
            else
            {
                sendCallArray(0);
            }
        }

        public virtual void GetHuData(ISFSObject obj, DVoidSfsObject sendCall)
        {
            if (_currSeat == _playerSeat)
            {
                obj.PutInt(RequestKey.KeyType, MjRequestData.MJReqTypeZiMo);
            }
            else
            {
                obj.PutInt(RequestKey.KeyType, MjRequestData.MJRequestTypeCPG);
                obj.PutInt(RequestKeyOther.KeyTType, MjRequestData.MJRequestTypeHu);
            }

            sendCall(obj);
        }

        public virtual void GetTingData()
        {
            if (_tings != null)
            {
                EventDispatch.Dispatch((int)GameEventId.ChooseTing, new EventData(_tings));
                EventDispatch.Dispatch((int)UIEventId.ChooseTing);
            }
            else
                YxDebug.LogError("获得听牌数据 为空！！！");
        }


        ///------------------------------------------------------------------------------------------------------
        //玩家的座位与桌子相对的位子转换
        public virtual int GetChairId(int userSeat)
        {
            //int chair = (userSeat - _playerSeat + UtilData.CurrGamePalyerCnt) % UtilData.CurrGamePalyerCnt;

            //if (UtilData.CurrGamePalyerCnt == 2)
            //{
            //    chair = chair==1?2:chair;
            //}
            //else if (UtilData.CurrGamePalyerCnt == 3)
            //{
            //    chair = chair==2?3:chair;
            //}

            //return chair;
            return UtilFunc.GetChairId(userSeat);
        }

        protected virtual bool IsHaveOpreate(int op, int ttype)
        {
            return (ttype & op) == op;
        }


        public virtual void CopyIntArryToMjList(int[] array, List<int> list)
        {
            list.Clear();
            for (int i = 0; i < array.Length; i++)
            {
                list.Add(array[i]);
            }
        }

        public virtual int[] CopyMjArryToIntArray(List<int> list)
        {
            int[] ret = new int[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                ret[i] = list[i];
            }
            return ret;
        }

        protected virtual void SortMjValue(List<int> values)
        {
            values.Sort((a1, a2) =>
            {
                if (a1 == Laizi && a2 != Laizi)
                    return -1;
                if (a1 != Laizi && a2 == Laizi)
                    return 1;

                if (a1 < a2)
                    return -1;

                if (a1 > a2)
                    return 1;

                return 0;
            });
        }

        protected virtual void SortType(List<int> values)
        {
            _typeDic.Clear();
            int len = values.Count;
            int singleNum = -1;
            for (int i = 0; i < len; i++)
            {
                if (values[i] != singleNum)
                {
                    singleNum = values[i];
                    _typeDic[singleNum] = 1;
                }
                else
                {
                    _typeDic[singleNum] += 1;
                }
            }
        }

        protected virtual string FindeUserNameByID(int id)
        {
            foreach (UserData mahjongPlayer in UserDatas)
            {
                if (mahjongPlayer.id == id)
                {
                    return mahjongPlayer.name;
                }
            }

            return "";
        }

        protected virtual List<int[]> FindCanChi()
        {
            var findList = new List<int[]>();

            SortMjValue(UserHardCard[_playerSeat]);
            SortType(UserHardCard[_playerSeat]);

            if (_typeDic.ContainsKey(_outPutMahjong - 1) && _outPutMahjong - 1 != Laizi)
            {
                if (_typeDic.ContainsKey(_outPutMahjong - 2) && _outPutMahjong - 2 != Laizi)
                {
                    findList.Add(new[] { _outPutMahjong - 2, _outPutMahjong - 1 });
                }

                if (_typeDic.ContainsKey(_outPutMahjong + 1) && _outPutMahjong + 1 != Laizi)
                {
                    findList.Add(new[] { _outPutMahjong - 1, _outPutMahjong + 1 });
                }
            }
            if (_typeDic.ContainsKey(_outPutMahjong + 1) && _typeDic.ContainsKey(_outPutMahjong + 2) && _outPutMahjong + 1 != Laizi && _outPutMahjong + 2 != Laizi)
            {
                findList.Add(new[] { _outPutMahjong + 1, _outPutMahjong + 2 });
            }

            UtilFunc.OutPutList(UserHardCard[_playerSeat], "用户手牌手牌");

            foreach (int[] find in findList)
            {
                UtilFunc.OutPutArray(find, "找到的吃");
            }

            return findList;
        }

        protected virtual List<FindGangData> FindCanGang()
        {
            int checkValue;
            var findList = new List<FindGangData>();

            SortMjValue(UserHardCard[_playerSeat]);
            SortType(UserHardCard[_playerSeat]);

            DBoolTParam<int> checkPengGang = (value) =>
            {
                if (_typeDic.ContainsKey(value) && _typeDic[value] >= 3)
                {
                    var cards = new int[_typeDic[value]];
                    for (int i = 0; i < _typeDic[value]; i++)
                    {
                        cards[i] = value;
                    }

                    var data = new FindGangData
                    {
                        type = MjRequestData.MJRequestTypeCPG,
                        ttype = MjRequestData.CPG_PengGang,
                        cards = cards
                    };

                    findList.Add(data);
                    return true;
                }

                return false;
            };

            DBoolNoParam checkHandCardGang = () =>
            {
                bool ret = false;
                foreach (KeyValuePair<int, int> keyValuePair in _typeDic)
                {
                    if (keyValuePair.Value > 3 && (Laizi != keyValuePair.Value))
                    {
                        ret = true;
                        var data = new FindGangData
                        {
                            type = MjRequestData.MJRequestTypeSelfGang,
                            ttype = MjRequestData.CPG_AnGang,
                            cards = new[] { keyValuePair.Key, keyValuePair.Key, keyValuePair.Key, keyValuePair.Key }
                        };
                        findList.Add(data);
                    }
                }

                return ret;
            };

            DBoolTParam<int> checkZhuaGang = (value) =>
                {
                    var findResult = false;
                    foreach (CpgData cpg in UserCpg[_playerSeat])
                    {
                        if (cpg.Type == EnGroupType.Peng && (value == cpg.Card || _typeDic.ContainsKey(cpg.Card)))
                        {
                            var data = new FindGangData
                            {
                                type = MjRequestData.MJRequestTypeSelfGang,
                                ttype = MjRequestData.CPG_ZhuaGang,
                                cards = new[] { cpg.Card, cpg.Card, cpg.Card, cpg.Card }
                            };
                            findList.Add(data);
                            findResult = true;
                        }
                    }

                    return findResult;
                };

            DBoolNoParam checkXFGang = () =>
            {
                //旋风杠 只有第一轮生效
                if (_isOutPutCard)
                    return false;
                //中发白 优先于 中发白
                if (_typeDic.ContainsKey(81) && _typeDic.ContainsKey(84) && _typeDic.ContainsKey(87))
                {
                    var data = new FindGangData
                    {
                        type = MjRequestData.MJRequestTypeXFG,
                        ttype = UtilDef.DefInt,
                        cards = new[] { 81, 84, 87 }
                    };
                    findList.Add(data);
                    return true;
                }
                //东南西北
                if (_typeDic.ContainsKey(65) && _typeDic.ContainsKey(68) && _typeDic.ContainsKey(71) &&
                    _typeDic.ContainsKey(74))
                {
                    var data = new FindGangData
                    {
                        type = MjRequestData.MJRequestTypeXFG,
                        ttype = UtilDef.DefInt,
                        cards = new[] { 65, 68, 71, 74 }
                    };
                    findList.Add(data);
                    return true;
                }

                return false;
            };

            //如果是 当前用户
            if (_currSeat == _playerSeat)
            {
                //旋风杠 最优先 直接返回 不需要用户选择
                if (RoomInfo.IsExsitXFGang && checkXFGang())
                {
                    return findList;
                }

                checkValue = _getInMahjong;
                checkHandCardGang();
                checkZhuaGang(checkValue);
            }
            else
            {
                checkValue = _outPutMahjong;
                checkPengGang(checkValue);
            }

            return findList;
        }

        protected virtual int[] FindCanPeng()
        {
            int[] ret = null;
            SortMjValue(UserHardCard[_playerSeat]);
            SortType(UserHardCard[_playerSeat]);
            if (_typeDic.ContainsKey(_outPutMahjong) && _typeDic[_outPutMahjong] >= 2)
            {
                ret = new[] { _outPutMahjong, _outPutMahjong };
            }

            UtilFunc.OutPutList<int>(UserHardCard[_playerSeat], "用户手牌手牌");

            UtilFunc.OutPutArray<int>(ret, "找到的碰");
            return ret;
        }
        public virtual void Reset()
        {
            InitData();
        }

        protected virtual bool IsAllReady()
        {
            bool isAllReady = true;
            //检测是否全部准备
            for (int i = 0; i < UtilData.CurrGamePalyerCnt; i++)
            {
                if (!UserDatas[i].IsReady)
                {
                    return false;
                }
            }
            return isAllReady;
        }

        public virtual void OnShowJiaPiaoBtn(ISFSObject data)
        {
            EventDispatch.Dispatch((int)UIEventId.ShowJiaPiaoBtn, new EventData());
        }

        public virtual void OnJiaPiao(ISFSObject data)
        {
            int[] list = data.GetIntArray("piaolist");

            if (list.Length == UtilData.CurrGamePalyerCnt)
            {
                for (int i = 0; i < UserDatas.Length; i++)
                {
                    int seat = UserDatas[i].Seat;

                    if (seat != -1)
                    {
                        int value = list[seat];
                        EventDispatch.Dispatch((int)UIEventId.ShowJiaPiaoEffect, new EventData(GetChairId(seat), value));
                    }
                }
            }
        }
        public virtual void ChooseXJFD()
        {

        }

        public virtual void OnXJFD(ISFSObject data, bool rejoin = false)
        {

        }

        public virtual void OnQueryHulist(ISFSObject data)
        {
            if (QueryHuList != null)
                QueryHuList.OnQueryHulist(data);
        }

        public virtual void RequestQueryHuList(ISFSObject obj, DVoidSfsObject sendCall, EventData evn)
        {
            if (QueryHuList != null)
                QueryHuList.Query(obj, sendCall, evn);
        }

        public virtual void OnBao(ISFSObject data)
        {
            int TempBao = data.GetInt("bao");
            if (TempBao == bao)
            {
                return;
            }
            LeaveMahjongCnt--;
            bao = TempBao;
            int lastlaizi = 0;
            if (data.ContainsKey("lastbao"))
            {
                lastlaizi = data.GetInt("lastbao");
            }
            int BaoSeat = data.GetInt("seat");
            if (BaoSeat != PlayerSeat)
            {
                lastlaizi = 0;
            }
            bool _ting = _isTing[PlayerSeat];
            //if (!_ting)
            //{
            //    bao = 0;
            //}
            YxDebug.Log("看宝数据处理，宝是:" + bao);
            EventData evn = new EventData
            {
                data1 = bao,
                data2 = lastlaizi,
                data3 = _ting,
                data4 = anBao
            };
            EventDispatch.Dispatch((int)GameEventId.GetBao, evn);
        }

        public void RejoinShowBao()
        {
            EventData evn = new EventData
            {
                data1 = bao,
                data2 = 0,
                data3 = _isTing[PlayerSeat],
                data4 = anBao
            };
            EventDispatch.Dispatch((int)GameEventId.GetBao, evn);
        }

        public virtual void OnNiuClick()
        {
        }

        public virtual void ShowLiangPai(ISFSObject data)
        {

        }

        public virtual void OnReciveGPSInfo(ISFSObject data)
        {
            int id = data.GetInt("uid");

            UserData user = null;

            for (int i = 0; i < UserDatas.Length; i++)
            {
                if (UserDatas[i].id == id)
                {
                    user = UserDatas[i];
                }
            }

            if (user != null)
            {
                EventDispatch.Dispatch((int)UIEventId.ReceiveGPSInfo, new EventData(GetChairId(user.Seat), user.ip, data));
            }
        }

        //起手听
        public virtual void OnStartTing() { }
        public virtual void StartTing(ISFSObject data) { }

        //加码
        public virtual void ShowJiamaPnl(ISFSObject data) { }
        public virtual void JiamaResult(ISFSObject data) { }

        //绝杠
        public virtual void GetJueGangData(ISFSObject obj, DVoidSfsObject sendCall) { }
        public virtual void OnAnJueGang(ISFSObject data) { }

        protected virtual void OnTrusteeship(int eventid, EventData data)
        {
            if (TrusteeshipHelper.Instance != null)
            {
                if (data.data2 != null && !(bool)data.data2)
                {
                    return;
                }
                //打出手牌中最后一张牌
                if (_currSeat == _playerSeat && TrusteeshipHelper.Instance.IsTrusteeship)
                {
                    OutCardDelayTimeIndex = DelayTimer.StartTimer(
                       GameConfig.GetInCardRoteTime + GameConfig.GetInCardWaitTime + GameConfig.TingPaiWaitTime,
                       () => EventDispatch.Dispatch((int)NetEventId.OnOutMahjong, new EventData((int)data.data1)));
                }
                if (TrusteeshipHelper.Instance.GameOpreateMenu != 0)
                {
                    EventDispatch.Dispatch((int)NetEventId.OnGuoClick, new EventData());
                }
            }
        }

    }
}