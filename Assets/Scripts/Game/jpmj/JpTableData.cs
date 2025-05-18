using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Mahjong;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using com.yxixia.utile.YxDebug;
using YxFramwork.Enums;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.jpmj
{
    public class JpTableData : TableData
    {
        protected List<BuZhangData> BuZhang = new List<BuZhangData>();
        protected List<int> BuZhangGetIn = new List<int>();
        protected int BuZhangGetInSeat = UtilDef.NullSeat;
        protected bool IsBuZhangFinish = true;
        protected bool IsGetInBuZhangFinish = true;
        public List<int>[] BuZhangList = new List<int>[UtilDef.GamePlayerCnt];


        /// <summary>
        /// 要胡牌时的台数
        /// </summary>
        [HideInInspector]
        public int HuTai { private set; get; }
        /// <summary>
        /// 要胡牌时的花数
        /// </summary>
        [HideInInspector]
        public int HuHua { private set; get; }
        /// <summary>
        /// 要胡牌时的剩余花数
        /// </summary>
        [HideInInspector]
        public int HuYu { private set; get; }


        /// <summary>
        /// 当需要显示太花时进行显示
        /// </summary>
        [HideInInspector]
        public Action<ISFSObject, int> ShowTaiHuaAction;

        /// <summary>
        /// 当不需要显示太花 则隐藏的
        /// </summary>
        [HideInInspector]
        public Action HideTaiHuaAction;

        /// <summary>
        /// 麻将的吃碰杠牌组
        /// </summary>
        [SerializeField]
        protected MahjongCpgGroup[] MahjongCpgGrps;

        [SerializeField]
        protected GameObject GameReadyBtn;

        protected override void RegisteEvent()
        {
            base.RegisteEvent();
            EventDispatch.Instance.RegisteEvent((int)TableDataEventId.OnSendLeaveRoomState, SendLeaveState);
        }

        /// <summary>
        /// 游戏没有开始前离开游戏，需要告诉服务器离开时状态
        /// </summary>
        private void SendLeaveState(int eventid, EventData data)
        {   
            var type = UserDatas[_playerSeat].IsReady ? 1 : 0;
            var apiInfo = new Dictionary<string, object>()
            {
                { "roomId", RoomInfo.RoomID },
                { "type", type }
            };
            var state = (int)data.data1;
            if (state == 0)
            {
                Facade.Instance<TwManager>().SendAction("group.leaveRoom", apiInfo, null);
            }
            else
            {
                Facade.Instance<TwManager>().SendAction("group.leaveGame", apiInfo, null);
            }

            YxDebug.Log("SendLeaveState " + type + "-" + state);
        }

        /// <summary>
        /// 获得吃碰杠的总牌数，杠不算4张，直接算3张，保证总牌数是17
        /// </summary>
        /// <returns></returns>
        public int GetSelfCpgCdNum()
        {
            var selfCpgGrp = MahjongCpgGrps[0];
            var cpgList = selfCpgGrp.CpgList;
            if (cpgList != null) return cpgList.Count * 3;
            return 0;
        }

        public override void OnCpg(ISFSObject data)
        {
            base.OnCpg(data);
            var chair = GetChairId(data.GetInt(RequestKey.KeySeat));

            foreach (var mjgp in MahjongCpgGrps)
            {
                if (mjgp.Chair == chair)
                {
                    var cpglist = mjgp.CpgList;

                    foreach (var mahjongCpg in cpglist)
                    {
                        var mjlist = mahjongCpg.MahjongList;

                        foreach (var mahjongItem in mjlist)
                        {
                            if (mahjongItem.Value == Laizi)
                                mahjongItem.IsSign = true;
                        }
                    }
                }
            }

        }

        public override int BankerSeat
        {
            get { return _bankerSeat; }
            set
            {
                _bankerSeat = value;
                UtilData.BankerSeat = value;
                EventDispatch.Dispatch((int)UIEventId.Banker, new EventData(GetChairId(_bankerSeat)));
                EventDispatch.Dispatch((int)GameEventId.Banker, new EventData(GetChairId(_bankerSeat)));
            }
        }

        public override int SetUserInfo(ISFSObject data)
        {
            JustCheckShowTaihuainfoOnreJoin(data);
            //return base.SetUserInfo(data);

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

            if (RoomInfo.RoomType == EnRoomType.YuLe)
            {
                if (user.Seat == PlayerSeat && user.IsReady == false)
                {
                    EventDispatch.Dispatch((int)NetEventId.OnUserReady, new EventData());
                }
                GameReadyBtn.SetActive(false);
            }
            else
            {
                if (user.Seat == PlayerSeat)
                {
                    //如果是当前用户并且没有准被 发送准备消息
                    GameReadyBtn.SetActive(user.IsReady == false);
                }

            }
            /*            if (user.Seat == PlayerSeat && user.IsReady == false)
                        {
                            EventDispatch.Dispatch((int)NetEventId.OnUserReady, new EventData());
                        }*/

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

        public override void OnOperate(ISFSObject data)
        {
            base.OnOperate(data);

            CheckTaiHuaInfoShow(data);
        }

        /// <summary>
        /// 重连时只显示taihuainfo
        /// </summary>
        /// <param name="data"></param>
        private void JustCheckShowTaihuainfoOnreJoin(ISFSObject data)
        {
            if (data.ContainsKey("tai") && data.ContainsKey("hua"))
            {
                HuTai = data.GetInt("tai");
                HuHua = data.GetInt("hua");
                HuYu = data.GetInt("cnt");
                //触发相应的台花信息显示事件
                if (ShowTaiHuaAction != null)
                {
                    int opcdValue = -1;
                    if (data.ContainsKey(RequestKeyOther.KeyWillhucard))
                    {
                        opcdValue = data.GetInt(RequestKeyOther.KeyWillhucard);
                    }

                    ShowTaiHuaAction(data, opcdValue);
                }
            }
        }

        /// <summary>
        /// 检查是否显示台和花
        /// </summary>
        /// <param name="data"></param>
        private void CheckTaiHuaInfoShow(ISFSObject data)
        {
            if (data.ContainsKey("tai") && data.ContainsKey("hua"))
            {
                HuTai = data.GetInt("tai");
                HuHua = data.GetInt("hua");
                HuYu = data.GetInt("cnt");
                //触发相应的台花信息显示事件
                if (ShowTaiHuaAction != null)
                {
                    int opcdValue = -1;
                    if (data.ContainsKey(RequestKeyOther.KeyWillhucard))
                    {
                        opcdValue = data.GetInt(RequestKeyOther.KeyWillhucard);
                    }

                    ShowTaiHuaAction(data, opcdValue);
                }
            }
            else
            {
                if (HideTaiHuaAction != null) HideTaiHuaAction();
            }
        }


        public override void OnSendCard(ISFSObject data)
        {
            OnClearBuZhang();
            base.OnSendCard(data);
        }

        /// <summary>
        /// 清除补张标记
        /// </summary>
        public void OnClearBuZhang()
        {
            BuZhang.Clear();
            BuZhangGetIn.Clear();
        }

        /// <summary>
        /// 房主id
        /// </summary>
        public int OwerId { get; private set; }

        public string Rule { get; private set; }


        public override void SetTableData(ISFSObject data)
        {
            base.SetTableData(data);
            //房主id
            if (data.ContainsKey("ownerId"))
                OwerId = data.GetInt("ownerId");

            if (data.ContainsKey("rule"))
            {
                Rule = data.GetUtfString("rule");
            }
        }

        public override int CurrRound
        {
            get { return RoomInfo.CurrRound; }
            set
            {
                RoomInfo.CurrRound = value;
                EventDispatch.Dispatch((int)UIEventId.CurrRound, new EventData(RoomInfo.CurrRound + "/" + RoomInfo.MaxRound));
            }
        }

        public override void OnNetResponseEventBack(int type, ISFSObject data)
        {
            switch (type)
            {
                case MjRequestData.MJRequestTypeBuZhang:
                    App.GetRServer<NetWorkManager>().BuzhangState = true;
                    OnBuZhang(data);
                    break;
                case MjRequestData.MJRequestTypeBuZhangFinish:
                    App.GetRServer<NetWorkManager>().BuzhangState = true;
                    OnBuZhangFinish(data);
                    break;
                case MjRequestData.MJRequestTypeBuZhangGetIn:
                    App.GetRServer<NetWorkManager>().BuzhangState = true;
                    OnGetInBuZhang(data);
                    break;
            }           
        }

        protected void OnBuZhang(ISFSObject data)
        {
            if (IsBuZhangFinish)
            {
                IsBuZhangFinish = false;
                BuZhang.Clear();
            }

            var seat = data.GetInt(RequestKey.KeySeat);
            var buZhangCard = data.GetIntArray("buZhangCard");
            var cards = data.GetIntArray(RequestKey.KeyCards);
            var buData = new BuZhangData { Chair = UtilFunc.GetChairId(seat), Cards = cards, BuZhangCards = buZhangCard };
            BuZhang.Add(buData);
            foreach (var bCard in buZhangCard)
            {
                BuZhangList[seat].Add(bCard);
            }
        }

        protected void OnBuZhangFinish(ISFSObject data)
        {
            IsBuZhangFinish = true;
            DVoidInt leaveNum = value => { LeaveMahjongCnt -= value; };
            EventDispatch.Dispatch((int)GameEventId.BuZhang, new EventData(BuZhang, leaveNum));
            //补张的牌 添加到手牌中
            foreach (var buData in BuZhang)
            {
                if (buData.Chair == 0)
                {
                    foreach (var card in buData.BuZhangCards)
                    {
                        UserHardCard[PlayerSeat].Remove(card);
                    }

                    foreach (var card in buData.Cards)
                    {
                        UserHardCard[PlayerSeat].Add(card);
                    }
                }
            }
        }

        protected void OnGetInBuZhang(ISFSObject data)
        {
            if (IsGetInBuZhangFinish)
            {
                IsGetInBuZhangFinish = false;
                BuZhangGetIn.Clear();
            }

            BuZhangGetInSeat = data.GetInt(RequestKey.KeySeat);
            var card = data.GetInt(RequestKey.KeyOpCard);
            BuZhangGetIn.Add(card);
            BuZhangList[BuZhangGetInSeat].Add(card);

        }

        /// <summary>
        /// 清理补张数据
        /// </summary>
        public void ClearBuzhangGetIn()
        {
            BuZhangGetInSeat = UtilDef.NullSeat;
            BuZhangGetIn.Clear();
        }

        protected void OnGenZhuang(ISFSObject data)
        {
            if (data.ContainsKey("genZhuang"))
            {
                var gold = data.GetIntArray("genZhuang");
                var chairGold = new int[gold.Length];
                for (int i = 0; i < gold.Length; i++)
                {
                    var chair = UtilFunc.GetChairId(i);
                    chairGold[chair] = gold[i];
                }
                EventDispatch.Dispatch((int)UIEventId.UserGlodChange, new EventData(chairGold));
            }
        }

        protected override void OnGangGold(ISFSObject data)
        {
            //先判断是否是切猪头
            if (data.ContainsKey("qzt"))
            {
                var goldsSfsObj = data.GetSFSObject("qzt");

                var list = new List<int>();
                foreach (var key in goldsSfsObj.GetKeys())
                {
                    list.Add(goldsSfsObj.GetInt(key));
                }
                var len = list.Count;
                var glodList = new int[len];
                for (int i = 0; i < len; i++)
                {
                    int chair = GetChairId(i);
                    glodList[chair] = list[i];
                }
                EventDispatch.Dispatch((int)UIEventId.UserGlodChange, new EventData(glodList));
            }
            else
            {
                base.OnGangGold(data);
            }
        }

        public override void OnThrowCard(ISFSObject data)
        {
            base.OnThrowCard(data);
            OnGenZhuang(data);
        }

        public override int GetInMahjong
        {
            get
            {
                return base.GetInMahjong;
            }
            set
            {
                _getInMahjong = value;
                //如果有赖子牌
                var data = GetInEventData(_getInMahjong);
                SendGetInData(data, _getInMahjong);
            }
        }


        /// <summary>
        /// 获得getincard事件的处理
        /// </summary>
        private void GetInMahJong(ISFSObject isfdata)
        {
            var opcdValue = isfdata.GetInt(RequestKey.KeyOpCard);
            //如果有赖子牌
            var data = GetInEventData(opcdValue);
            //标记是杠后抓的
            if (isfdata.ContainsKey("gang"))
            {
                data.data5 = 1;
            }
            //不带听玩法时候 tinglist
            if (isfdata.ContainsKey("tingoutlist"))
            {
                var tings = isfdata.GetIntArray("tingoutlist");

                if (null != tings && tings.Length > 0)
                {
                    if (UsingQueryHu)
                    {
                        data.data6 = tings;
                    }
                }
            }
            SendGetInData(data, opcdValue);
        }

        /// <summary>
        /// 获取抓牌数据
        /// </summary>
        /// <param name="cardValue"></param>
        /// <returns></returns>
        private EventData GetInEventData(int cardValue)
        {
            _getInMahjong = cardValue;
            EventData data = new EventData(_getInMahjong);
            if (_laizi != UtilDef.NullMj && _laizi == _getInMahjong)
            {
                //判断赖子是春夏秋冬
                if (_laizi >= 96 && _laizi < 100)
                {
                    if ((_getInMahjong >= 96) && (_getInMahjong < 100))
                        data.data2 = 1;
                }
                else if ((_laizi >= 100) && (_laizi < 104)) //判断赖子是梅兰竹菊
                {
                    if (_getInMahjong >= 100 && _getInMahjong < 104)
                        data.data2 = 1;
                }
                else
                {
                    data.data2 = _laizi;
                }
            }

            //如果有补张数据 加入补张数据
            if (CurrSeat == BuZhangGetInSeat && BuZhangGetIn.Count != 0)
            {
                DVoidInt leaveNum = cardNum => { LeaveMahjongCnt -= cardNum; };
                data.data3 = BuZhangGetIn;
                data.data4 = leaveNum;
            }
            return data;
        }

        /// <summary>
        /// 发送抓牌数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="cardValue"></param>
        private void SendGetInData(EventData data, int cardValue)
        {
            EventDispatch.Dispatch((int)GameEventId.GetInCard, data);
            UserHardCard[_currSeat].Add(cardValue);
        }

        /// <summary>
        /// 清理hulist
        /// </summary>
        public void ClearQueryHuList()
        {
            QueryHuList.ClearCache();
        }

        public override void OnGetInCard(ISFSObject data)
        {
            IsGetInBuZhangFinish = true;
            CurrSeat = data.GetInt(RequestKey.KeySeat);
            GetInMahJong(data);
            if (CurrSeat == BuZhangGetInSeat && BuZhangGetIn.Count != 0)
            {
                var sendObj = data;
                sendObj.PutInt(RequestKey.KeyType, MjRequestData.MJOpreateType);
                EventDispatch.Dispatch((int)NetEventId.OnPushNetRespons,new EventData(data));
            }
            else
            {
                OnOperate(data);
            }

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
            //---------------------------*/
            ClearBuzhangGetIn();
        }

        protected override void InitData()
        {
            base.InitData();
            IsBuZhangFinish = true;
            IsGetInBuZhangFinish = true;
            for (int i = 0; i < UtilDef.GamePlayerCnt; i++)
            {
                BuZhangList[i] = new List<int>();
            }
            OnClearBuZhang();
        }

        public override void GetMahjongInfoFromSFSData(ISFSObject playerData)
        {
            base.GetMahjongInfoFromSFSData(playerData);
            int seat = playerData.GetInt(RequestKey.KeySeat);
            if (playerData.ContainsKey("buHua"))
            {
                var buHua = playerData.GetIntArray("buHua");
                CopyIntArryToMjList(buHua, BuZhangList[seat]);
            }
        }

        public Action<int> OnGetPlayerNum;

        /// <summary>
        /// 是否杠财神的cargs里面的key
        /// </summary>
        private const string Gangcaishen = "-gangcaishen";

        /// <summary>
        /// 分享分数的限制
        /// </summary>
        private const string OnerdHuScore = "-onerdHuScore";

        /// <summary>
        /// 分享连庄的限制
        /// </summary>
        private const string Lianzhuang = "-lianzhuang";

        /// <summary>
        /// 花数
        /// </summary>
        private const string HuaNumKey = "-huaNum";

        /// <summary>
        /// 记录房间信息cargs2
        /// </summary>
        private ISFSObject _cargs2;
        /// <summary>
        /// 胡牌分，分享的分数线
        /// </summary>
        public int ShareHuScoreValue
        {
            get
            {
                if (_cargs2 != null && _cargs2.ContainsKey(OnerdHuScore))
                {
                    return int.Parse(_cargs2.GetUtfString(OnerdHuScore));
                }
                return 500;
            }
        }


        /// <summary>
        /// 胡牌后连庄个数分享的限制
        /// </summary>
        public int LianzhuangNum
        {
            get
            {
                if (_cargs2 != null && _cargs2.ContainsKey(Lianzhuang))
                {
                    return int.Parse(_cargs2.GetUtfString(Lianzhuang));
                }
                return 5;
            }
        }


        /// <summary>
        /// 未胡的花数，分享的分数线
        /// </summary>
        public int ShareHuHuaNumValue
        {
            get
            {
                if (_cargs2 != null && _cargs2.ContainsKey(HuaNumKey))
                {
                    return int.Parse(_cargs2.GetUtfString(HuaNumKey));
                }
                return 25;
            }
        }

        /// <summary>
        /// 设置房间参数
        /// </summary>
        /// <param name="data"></param>
        public override void SetRoomInfo(ISFSObject data)
        {
            base.SetRoomInfo(data);

            if (data.ContainsKey(RequestKeyOther.KeyPlayerNum) && OnGetPlayerNum != null)
            {
                OnGetPlayerNum(data.GetInt(RequestKeyOther.KeyPlayerNum));
            }

            if (data.ContainsKey("cargs2"))
            {
                _cargs2 = data.GetSFSObject("cargs2");

            }
        }

        public override void GetGangData(ISFSObject obj, DVoidSfsObject sendCall)
        {
            List<FindGangData> findList = FindCanGang();

            //如果不能杠财神，则移除可能是财神的牌
            if (_cargs2.ContainsKey(Gangcaishen) && _cargs2.GetUtfString(Gangcaishen) == "0")
            {
                foreach (var findGangData in findList)
                {
                    if (findGangData.cards[0] == Laizi)
                    {
                        findList.Remove(findGangData);
                        break;
                    }
                }
            }

            DVoidInt sendCallArray = index =>
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

        public override void OnHu(ISFSObject data, int type)
        {
            base.OnHu(data, type);
            HideTaiHuaAction();
        }
    }
}
