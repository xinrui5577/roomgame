using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.Mahjong2D.Common.MahJongCommon;
using Assets.Scripts.Game.Mahjong2D.Common.UI;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Request;
using Assets.Scripts.Game.Mahjong2D.Game.Data;
using Assets.Scripts.Game.Mahjong2D.Game.GameCtrl;
using Assets.Scripts.Game.Mahjong2D.Game.Item;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using Random = UnityEngine.Random;
using UserInfoPanel = Assets.Scripts.Game.Mahjong2D.Common.MahJongCommon.UserInfoPanel;

namespace Assets.Scripts.Game.Mahjong2D.Game.Player
{
    public class MahjongPlayer : MonoBehaviour
    {
        /// <summary>
        /// 庄状态
        /// </summary>
        private bool _isZhuang;
        /// <summary>
        /// 显示座位号
        /// </summary>
        private int _showSeat;
        /// <summary>
        /// 当前消息面板
        /// </summary>
        public UserInfoPanel CurrentInfoPanel;
        /// <summary>
        /// 麻将环境（显示UI，不存储整体数据，只放麻将）
        /// </summary>
        [SerializeField]
        private DefEnvpanel _mahJongEnv;
        /// <summary>
        /// 吃碰杠胡菜单
        /// </summary>
        public CpghChar CpghBehavior;
        /// <summary>
        /// 实际的手牌数据（包括最后一张）
        /// </summary>
        protected List<int> HandCardList = new List<int>();
        /// <summary>
        /// 实际的手牌数据（不包括最后一张）
        /// </summary>
        protected List<int> ShowCardList = new List<int>();
        /// <summary>
        /// 组牌数据
        /// </summary>
        protected List<MahjongGroupData> GroupItems = new List<MahjongGroupData>();
        /// <summary>
        /// 打出的手牌
        /// </summary>
        protected List<int> ThrowOutCards = new List<int>();

        protected bool Token;
        /// <summary>
        ///是否能打牌
        /// </summary>
        [HideInInspector]
        public bool HasToken
        {
            private set
            {
                if (Token!=value)
                {
                    Token = value;
                    OnTokenStateChange(value);
                }
            }
            get
            {
                return Token;
            }
        }


        /// <summary>
        /// 获得的最后一张牌
        /// </summary>
        [HideInInspector]
        public int LastGetValue;
        /// <summary>
        /// 听牌时放倒手牌
        /// </summary>
        public bool PushHandOnTing = false;
        [Tooltip("根据风杠数量显示特效与音效")]
        public bool ShowFengGangByNum = false;
        /// <summary>
        /// 庄操作
        /// </summary>
        public bool IsZhuang
        {
            set
            {
                _isZhuang = value;
                CurrentInfoPanel.SetZhuang(value);
            }
            get
            {
                return _isZhuang;
            }
        }
        /// <summary>
        /// 是否有听
        /// </summary>
        public bool HasTing
        {
            set
            {
                if (UserInfo!=null)
                {
                    UserInfo.HasTing = value;
                    CurrentInfoPanel.SetTing(value);
                    SetHandCardOnTing(value);
                }
            }
            get
            {
                if (UserInfo!=null)
                {
                    return UserInfo.HasTing;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 真实座位号
        /// </summary>
        public int UserSeat
        {
            get
            {
                if (UserInfo!=null)
                {
                    return UserInfo.Seat;
                }
                return 0;
            }
        }
        /// <summary>
        /// 玩家信息
        /// </summary>
        public UserData UserInfo
        {
            set
            {
                CurrentInfoPanel.UserInfo = value;
            }
            get
            {
                return CurrentInfoPanel.UserInfo;
            }
        }
        /// <summary>
        /// 显示的座位号UI（顺序为：自己，右，对面，左边）
        /// </summary>
        public int ShowSeat
        {
            get
            {
                return _showSeat;
            }
            set { _showSeat = value; }
        }
        /// <summary>
        /// 方向索引
        /// </summary>
        public int ShowDirectionIndex
        {
            set { UserInfo.ShowDirectionIndex = value; }
            get { return UserInfo.ShowDirectionIndex; }
        }

        /// <summary>
        /// 麻将环境
        /// </summary>
        public DefEnvpanel MahjongEnv
        {
            get
            {
                if (_mahJongEnv == null)
                {
                    _mahJongEnv = GetComponentInChildren<DefEnvpanel>();
                }
                return _mahJongEnv;
            }
            set { _mahJongEnv = value; }
        }
        /// <summary>
        /// 游戏控制器
        /// </summary>
        public Mahjong2DGameManager Manager
        {
            get { return App.GetGameManager<Mahjong2DGameManager>(); }
        }
        /// <summary>
        /// 全局数据
        /// </summary>
        public Mahjong2DGameData Data
        {
            get { return App.GetGameData<Mahjong2DGameData>(); }
        }
        /// <summary>
        /// 最后一张抓的牌
        /// </summary>
        public MahjongItem LastGetInCard
        {
            get { return MahjongEnv.LastGetInMahjong; }
        }

        /// <summary>
        /// 玩家的打出的牌的状态
        /// </summary>
        public MahjongItem LastOutCard
        {
            get
            {
                MahjongItem item = MahjongEnv.LastOutMahjong;
                if (item != null)
                {
                    YxDebug.LogError(string.Format("玩家{0}打出的最后一张牌是{1}", UserInfo.name, (EnumMahjongValue)item.Value));
                }
                return item;
            }
            set
            {
                if (MahjongEnv.LastOutMahjong == null)
                {
                    YxDebug.LogError(string.Format("玩家{0}，最后一张牌是空的", UserInfo.name));
                }
                MahjongEnv.LastOutMahjong = value;
            }
        }

        public virtual void Awake()
        {
            _isZhuang = false;
            GroupItems = new List<MahjongGroupData>();
            MahjongEnv.GetComponent<DefEnvpanel>();
        }

        /// <summary>
        /// 玩家重置操作
        /// </summary>
        public virtual void Reset()
        {
            _isZhuang = false;
            HasTing = false;
            if (CurrentInfoPanel!=null&& UserInfo!=null)
            {
                CurrentInfoPanel.SetGang(UserInfo.Piao);
            }
            HandCardList.Clear();
            ShowCardList.Clear();
            ThrowOutCards.Clear();
            GroupItems.Clear();
            if(UserInfo!=null)
            MahjongEnv.Reset(IsOther); 
        }

        protected virtual void OnTokenStateChange(bool token)
        {

        }

        /// <summary>
        /// 加入游戏
        /// </summary>
        /// <param name="user"></param>
        /// <param name="isGameing"></param>
        internal MahjongPlayer JoinGame(UserData user,bool isGameing)
        {
            if (Data)
            {
                user.IsOwner = user.id.Equals(Data.OwnerId);
                UserInfo = user;
                App.GetGameManager<Mahjong2DGameManager>().Players[UserSeat] = this;
                if (!isGameing)
                {
                    FreshReadyState();
                }
                CurrentInfoPanel.Refresh(isGameing);
            }
            return this;
        }

        /// <summary>
        /// 重连
        /// </summary>
        /// <param name="data"></param>
        public virtual bool OnReJoin(ISFSObject data)
        {
            #region Data
            ISFSArray groups;
            int[] outCards;
            int[] handCards;
            int handNum;
            bool hasTing;
            GameTools.TryGetValueWitheKey(data, out groups, RequestKey.KeyGroups);
            GameTools.TryGetValueWitheKey(data, out outCards, RequestKey.KeyOutCards);
            GameTools.TryGetValueWitheKey(data, out handCards, RequestKey.KeyHandCards);
            GameTools.TryGetValueWitheKey(data, out handNum, RequestKey.KeyHandCardsNum);
            GameTools.TryGetValueWitheKey(data, out hasTing, RequestKey.KeyHasTing);
            HasTing = hasTing;
            GroupItems = new List<MahjongGroupData>();
            ThrowOutCards = new List<int>();
            GroupItems.AddRange(GameTools.GetGroupData(groups));
            ThrowOutCards.AddRange(outCards.ToList());
            if (handCards.Length.Equals(0))
            {
                handCards = new int[handNum];
            }
            HandCardList = handCards.ToList();
            #endregion
            #region UI
            MahjongEnv.AddOutCards(ThrowOutCards);
            for (int i = 0, lenth = GroupItems.Count; i < lenth; i++)
            {
                MahjongEnv.AddGroup(GroupItems[i], null, IsOther);
            }
            AddHandCards(HandCardList);
            if (hasTing)
            {
                ShowTingWithEffect(false);
            }
            ShowAutoOnRejoin(UserInfo.IsAuto);
            return UserInfo.IsAuto;

            #endregion
        }

        // ----------------------------------- 开始游戏
        internal void ShowPiao(int p)
        {
            CurrentInfoPanel.SetGang(p);
            if (UserInfo.Piao.Equals(99))//好吧，没有什么好办法，这里要求一刚到底只有第一次显示动画，其他时候不显示
            {
                return;
            }
            UserInfo.Piao = p;
            CpghBehavior.ShowPiao(p);

        }

        internal void FreshReadyState()
        {
            if (UserInfo.isReady)
            {
                ReadyGame();
            }
            else
            {
                HideReady();
            }
        }

        internal void ReadyGame()
        {
            CurrentInfoPanel.GameReady();
            CpghBehavior.ShowReady();
        }

        public void HideReady()
        {
            CpghBehavior.HideReady();
        }

        // 开局发牌
        public virtual void AllocateCards()
        {
            ShowCardList = (new int[13]).ToList();
            HandCardList = ShowCardList.ToList();
            AddHandCards(ShowCardList);
        }

        public virtual void AddHandCards(List<int> list)
        {
            foreach (var value in list)
            {
                AddHandCard(value, false, false);
            }
        }
        public virtual MahjongItem AddHandCard(int value, bool isSingle, bool needRefresh)
        {
            MahjongItem item = MahjongEnv.AddHandCard(value, isSingle).GetComponent<MahjongItem>();          
            if (item!=null)
            {
                return item;
            }
            else
            {
                Debug.LogError(string.Format("新增了一张牌，没有创建成功，牌是{0},值是：{1}",(EnumMahjongValue)value,value));
                return null;
            }
        }

        public virtual void TryAddLastGetInCard()
        {
            if(_mahJongEnv.LastGetInMahjong!=null)
            {
                MahjongItem item=_mahJongEnv.LastGetInMahjong;
                if (item!=null)
                {
                    MahjongEnv.AddHandCard(item);
                    _mahJongEnv.LastGetInMahjong = null;
                }

            }
        }

        // 玩家收到单张的发牌（Game中已经根据玩家SEAT匹配到这个类中）
        public virtual void GetInCard(int value)
        {
            GetToken();
            Facade.Instance<MusicManager>().Play(ConstantData.VoiceZhuaPai);
            LastGetValue = value;
            OnGetInCard(value);
        }
        protected virtual MahjongItem OnGetInCard(int value)
        {
            HandCardList.Add(value);
            MahjongItem item=AddHandCard(value, true, true);
            return item;
        }
        /// <summary>
        /// 是否是其它人
        /// </summary>
        public bool IsOther
        {
            get
            {
                return !UserSeat.Equals(App.GetGameManager<Mahjong2DGameManager>().SelfPlayer.UserSeat);
            }

        }

        /// <summary>
        /// 吃碰杠回包
        /// </summary>
        /// <param name="request"></param>
        /// <param name="showBehavior"></param>
        public  virtual void  AllowRequest(RequestData request,bool showBehavior)
        {
            GetToken();
            int tType;
            int dealCard;
            int randomNum = 0;
            string voiceName = "";
            bool isOk;
            bool isGangBao;
            dealCard = request.Card;
            var data = request.Data;
            GameTools.TryGetValueWitheKey(data, out tType, RequestKey.KeyTypeType);
            GameTools.TryGetValueWitheKey(data, out isOk, RequestKey.KeyOk);
            isGangBao = data.ContainsKey(RequestKey.KeyGangBao);
            List<MahjongItem> needItems = new List<MahjongItem>();
            MahjongGroupData groupData = new MahjongGroupData((GroupType)tType);
            List<int> dealCards=new List<int>();
            if (isGangBao)
            {
                AddHandCard(dealCard, false, false);
                dealCard = request.OpCard;
                if (request.Cards.Length == 0)
                {
                    switch ((GroupType)tType)
                    {
                        case GroupType.ZhuaGang:
                            request.Cards = new[] { dealCard, dealCard, dealCard, dealCard };
                            break;
                        case GroupType.AnGang:
                            request.Cards = new [] { dealCard};
                            break;
                    }
                }
            }
            if (isOk)
            {
                YxDebug.Log("这个回包中的ok标识是true，不执行操作，因为false时已经执行过了");
                return;
            }
            for (int i = 0, lenth = request.Cards.Length; i < lenth; i++)
            {
                YxDebug.Log((EnumMahjongValue)request.Cards[i]);
            }
            bool fromHand = true;
            switch ((EnumCpgType)tType)
            {
                case EnumCpgType.None:
                    return;
                case EnumCpgType.Chi:
                    groupData.AddValue(request.Cards[0]);
                    groupData.AddValue(dealCard);
                    groupData.AddValue(request.Cards[1]);
                    for (int i = 0,lenth= request.Cards.Length; i <lenth; i++)
                    {
                        dealCards.Add(request.Cards[i]);
                    }
                    voiceName = ConstantData.VoiceChi;
                    randomNum = Random.Range(0, 3);
                    break;
                case EnumCpgType.Peng:
                    if (dealCard.Equals(App.GetGameManager<Mahjong2DGameManager>().FanNum)&&Data.IsGangSelect)
                    {
                        voiceName = ConstantData.VoiceGang;
                        randomNum = Random.Range(0, 2);
                        tType = (int) EnumCpgType.ZhuaGang;
                    }
                    else
                    {
                        voiceName = ConstantData.VoicePeng;
                        randomNum = Random.Range(0, 3);
                    }
                    dealCards = CreateListWithValue(2, dealCard);
                    break;
                case EnumCpgType.ZhuaGang:
                    voiceName = ConstantData.VoiceGang;
                    randomNum = Random.Range(0, 2);
                    fromHand = false;
                    dealCards = CreateListWithValue(1, dealCard);
                    break;
                case EnumCpgType.PengGang:       
                case EnumCpgType.MingGang:
                    dealCards = CreateListWithValue(3, dealCard);
                    voiceName = ConstantData.VoiceGang;
                    randomNum = Random.Range(0, 2);
                    break;
                case EnumCpgType.AnGang:
                    dealCards = CreateListWithValue(4, dealCard);
                    voiceName = ConstantData.VoiceGang;
                    randomNum = Random.Range(0, 2);
                    break;
                case EnumCpgType.LaiZiGang:
                    break;
               
            }
            if (!tType.Equals((int)EnumCpgType.Chi))
            {
                for (int i = 0,lenth= request.Cards.Length; i < lenth; i++)
                {
                    groupData.AddValue(dealCard);
                }
            }
            MahjongItem lastItem = Manager.GetLastOutCardItem(dealCard);
            if (lastItem != null)
            {
                YxDebug.Log(string.Format("最后打出的一张牌是：{0}",(EnumMahjongValue)lastItem.Value));
                needItems.Add(lastItem);
                if (!tType.Equals((int)EnumCpgType.Chi))
                {
                    groupData.AddValue(dealCard);
                }
            }
            if (fromHand)
            {
                needItems.AddRange(GetCardsFromHand(dealCards, IsOther));
                foreach (var item in needItems)
                {
                    GameTools.DestroyUserContorl(item);
                }
                MahjongEnv.AddGroup(groupData, needItems, IsOther);
                GroupItems.Add(groupData);
            }
            else//抓杠特殊处理
            {
                MahjongItem item = GetHandCard(dealCard);
                if (item!=null)
                {
                    GameTools.DestroyUserContorl(item);
                    MahjongEnv.ChangeGroup(dealCard, item);
                    if (IsOther)
                    {
                        HandCardList.Remove(0);
                    }
                    else
                    {
                        HandCardList.Remove(dealCard);
                    }
                }
                else
                {
                    if (!isGangBao)
                    {
                        YxDebug.LogError("抓杠时，找不到那张手牌" + (EnumMahjongValue)dealCard);
                    }
                    else
                    {
                        YxDebug.LogError("杠宝时，找不到那张手牌" + (EnumMahjongValue)dealCard);
                    }

                }
               
            }
            if (showBehavior)
            {
                CpghBehavior.SetBehavior((EnumCpgType)tType);
            }
            Facade.Instance<MusicManager>().Play(GameTools.GetOperationVoice(UserInfo.Sex, voiceName, randomNum));
            SortHandCard();
            RecheckLastGetIn();
        }

        private List<int> CreateListWithValue(int num,int value)
        {
            List<int> setList=new List<int>();
            for (int i = 0; i < num; i++)
            {
                setList.Add(value);
            }
            return setList;
        }

        public List<MahjongItem> GetCardsFromHand(List<int> list, bool isOther)
        {
            List<MahjongItem> needItems = new List<MahjongItem>();
            needItems.AddRange(MahjongEnv.GetHandItems(list.Count));
            for (int i = 0, lenth = list.Count; i < lenth; i++)
            {
                if (isOther)
                {
                    HandCardList.Remove(0);
                }
                else
                {
                    HandCardList.Remove(list[i]);
                }
            }
            return needItems;
        }


        public void OnGetJueGang(RequestData requeset)
        {
            CpghBehavior.SetBehavior(EnumCpgType.MingGang);
            Facade.Instance<MusicManager>().Play(GameTools.GetOperationVoice(UserInfo.Sex, ConstantData.VoiceGang, Random.Range(0, 2)));
            MahjongGroupData groupData = new MahjongGroupData(GroupType.JueGang);
            foreach (int card in requeset.Cards)
            {
                if (card > 0)
                {
                    groupData.AddValue(card);
                }
            }
            List<MahjongItem> needItems = new List<MahjongItem>();
            needItems.AddRange(GetCardsFromHand(groupData.values.ToList(), IsOther));
            foreach (var item in needItems)
            {
                GameTools.DestroyUserContorl(item);
            }
            MahjongEnv.AddGroup(groupData, needItems, IsOther);
            GroupItems.Add(groupData);
            SortHandCard();
            RecheckLastGetIn();
        }

        public virtual void OnGetXFGang(RequestData requeset)
        {
            EnumCpgType fengGangBehavior= EnumCpgType.MingGang;
            string fengGangInfo = GameTools.GetOperationVoice(UserInfo.Sex, ConstantData.VoiceGang, Random.Range(0, 2));
            GetToken();
            MahjongGroupData groupData = new MahjongGroupData(GroupType.FengGang);
            if (requeset.Cards.Length == 3)
            {
                if (ShowFengGangByNum)
                {
                    fengGangBehavior = EnumCpgType.ThreeFengGang;
                    fengGangInfo = GameTools.GetOperationVoice(UserInfo.Sex, ConstantData.VoiceThreeFengGang,0);
                }
                groupData.values = new int[3];
            }
            else
            {
                if (ShowFengGangByNum)
                {
                    fengGangBehavior = EnumCpgType.FourFengGang;
                    fengGangInfo = GameTools.GetOperationVoice(UserInfo.Sex, ConstantData.VoiceFourFengGang, 0);
                }
            }
            Facade.Instance<MusicManager>().Play(fengGangInfo);
            CpghBehavior.SetBehavior(fengGangBehavior);
            foreach (int card in requeset.Cards)
            {
                if (card > 0)
                {
                    groupData.AddValue(card);
                }
            }

            List<MahjongItem> needItems = new List<MahjongItem>();
            needItems.AddRange(GetCardsFromHand(groupData.values.ToList(), IsOther));
            foreach (var item in needItems)
            {
                GameTools.DestroyUserContorl(item);
            }
            MahjongEnv.AddGroup(groupData, needItems, IsOther);
            GroupItems.Add(groupData);
            SortHandCard();
            RecheckLastGetIn();
        }
        public void OnGetCaiGang(RequestData requeset)
        {
            GetToken();
            CpghBehavior.SetBehavior(EnumCpgType.CaiGang);
            Facade.Instance<MusicManager>().Play(GameTools.GetOperationVoice(UserInfo.Sex, ConstantData.VoiceGang,
                Random.Range(0, 2)));

            MahjongGroupData groupData = new MahjongGroupData(GroupType.CaiGang);
            foreach (int card in requeset.Cards)
            {
                if (card > 0)
                {
                    groupData.AddValue(card);
                }
            }

            List<MahjongItem> needItems = new List<MahjongItem>();
            needItems.AddRange(GetCardsFromHand(groupData.values.ToList(), IsOther));
            foreach (var item in needItems)
            {
                GameTools.DestroyUserContorl(item);
            }
            MahjongEnv.AddGroup(groupData, needItems, IsOther);
            GroupItems.Add(groupData);
            SortHandCard();
        }

        protected virtual void SortHandCard()
        {
            ShowCardList = HandCardList.ToList();
            RefresheShow(IsOther);
        }

        /// <summary>
        /// 刷新使用
        /// </summary>
        /// <param name="isOther">这个参数可以控制显示UI</param>
        protected virtual void RefresheShow(bool isOther)
        {
            if (HasToken && OnGetNewCard())
            {
                if (isOther)
                {
                    ShowCardList.Remove(0);
                }
                else
                {
                    ShowCardList.Remove(LastGetValue);
                }
            }
            MahjongEnv.RefreshHandList(ShowCardList);
        }

        public virtual bool OnGetNewCard()
        {
            return _mahJongEnv.OnGetNewCard();
        }

        // 显示吃碰杠胡
        public void ShowCpgh(EnumCpgType behivor)
        {
            YxDebug.Log("------> MahjongPlayer.ShowCpgh() behivor = " + behivor);
            if (CpghBehavior == null) return;
            CpghBehavior.SetBehavior(behivor);
        }
        /// <summary>
        /// 打牌
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual MahjongItem OnThrowCard(int value,AsyncCallback finishCall=null)
        {
            MahjongItem item;
            LostToken();
            item = GetHandCard(value);
            if (IsOther)
            {
                HandCardList.Remove(0);
            }
            else
            {
                HandCardList.Remove(value);
            }
            MahjongEnv.ThrowOutCard(item,false,HasTing || App.GetGameData<Mahjong2DGameData>().IsInRobot,0,finishCall);
            SortHandCard();
            Facade.Instance<MusicManager>().Play(GameTools.GetOperationVoice(UserInfo.Sex, value.ToString(), 0));
            return item;
        }

        /// <summary>
        /// 获得一张牌
        /// </summary>
        /// <param name="direct">是否直接落下</param>
        public virtual void OutCardDown(bool direct=false)
        {
            if (LastOutCard!=null)
            {
                MahjongEnv.ThrowOutCardToOutCard(direct,HasTing);
            }
        }
        /// <summary>
        /// 打出宝牌
        /// </summary>
        /// <param name="baoCard">宝牌</param>
        /// <param name="time">移动时间</param>
        public virtual void ThrowBaoCard(int baoCard,float time)
        {
            MahjongItem item = Manager.GetNextMahjong(false);
            item.Value = baoCard;
            item.SelfData.Action=EnumMahJongAction.Lie;
            item.SelfData.Direction=EnumMahJongDirection.Vertical;
            item.SelfData.ShowDirection = EnumShowDirection.Self;
            MahjongEnv.MoveCardToOutPile(item,time);
        }

        /// <summary>
        /// 从手里获得一张牌，任意牌
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual MahjongItem GetHandCard(int value)
        {
            MahjongItem item = _mahJongEnv.GetHandItem(value);
            item.Value=value;
            return item;
        }
        public virtual void SetLastOutMahjongShow(int lastOtCdValue)
        {
            MahjongEnv.SetLastOutMahjongShow(lastOtCdValue);
        }

        public virtual void SetLastInCardOnReJoin(int lastIn)
        {
            MahjongEnv.SetLastInCardOnReJoin(0);
        }
        public virtual void ShowLiangPaiAtGameEnd(List<int> showCards,bool zimo,int huCard=0)
        {
            if (zimo)
            {
                showCards.Remove(huCard);
            }
            HandCardList = showCards.ToList();
            if(huCard>0)
            {
                if (OnGetNewCard())
                {
                    MahjongItem item=MahjongEnv.LastGetInMahjong;
                    item.Value = huCard;
                }
                else
                {
                    MahjongEnv.AddHandCard(huCard, true, false);
                }
                MahjongEnv.LastGetInMahjong.SetHuTag();
            }
            ShowCardList = GameTools.SortCardWithLaiZi(HandCardList, App.GetGameManager<Mahjong2DGameManager>().LaiZiNum).ToList();
            LostToken();
            RefresheShow(false);
            DoShowLiang(zimo);
        }


        private void DoShowLiang(bool zimo)
        {
            MahjongEnv.ShowHandCard();
            MahjongItem item = MahjongEnv.LastGetInMahjong;
            if(item!=null)
            {
                MahjongEnv.LastOutMahjong = item;
                MahjongEnv.LastGetInMahjong = null;
                MahjongEnv.ThrowOutCard(item, !zimo);
            }
            Facade.Instance<MusicManager>().Play(ConstantData.VoiceLiangPai);
        }

        public virtual void SetHandCardOnTing(bool hasTing)
        {
            if (PushHandOnTing)
            {
                MahjongEnv.SetHandCardPush(hasTing);
            }
        }

        public virtual void CheckTingState()
        {

        }

        // ----------------------------------- 对外PUBLIC函数

        public virtual void LostToken()
        {
            HasToken = false;
        }

        public virtual void GetToken()
        {
            HasToken = true;
        }

        public void ChangeAutoState(bool auto)
        {
            if (auto!=UserInfo.IsAuto)
            {
                UserInfo.IsAuto = auto;
                ShowAutoState(auto);
            }
        }

        protected virtual void ShowAutoOnRejoin(bool auto)
        {
            ShowAutoState(auto);
        }

        protected virtual void ShowAutoState(bool auto)
        {
            CurrentInfoPanel.ShowAutoState(auto);
        }


        //-------------------------------------社交相关
        public void OnPlayeTalk(AudioClip clip, int lenth)
        {
            CurrentInfoPanel.PlayVoiceChat(clip, lenth);
        }

        public void SetUserHead(bool state, bool showTag)
        {
            CurrentInfoPanel.SetOutLineState(state,showTag);
            if (Manager)
            {
                bool active = !((Manager.IsInit || Manager.IsWaiting) && Data.IsFirstTime && !state);
                if (CurrentInfoPanel.NoDataHide)
                {
                    CurrentInfoPanel.ShowVisible(active);
                }
            }
        }

        public virtual void ShowTingWithEffect(bool withEffect)
        {
            HasTing = true;
            Manager.IsTingExist = true;
            if (withEffect)
            {
                if (App.GetGameManager<Mahjong2DGameManager>().Xs == 1)
                    CpghBehavior.SetBehavior(EnumCpgType.Xst);
                else
                    CpghBehavior.SetBehavior(EnumCpgType.Ting);
                Facade.Instance<MusicManager>().Play(GameTools.GetOperationVoice(UserInfo.Sex, ConstantData.VoiceTing, Random.Range(0, 4)));
            }
        }
        /// <summary>
        /// 设置打出牌每行的个数
        /// </summary>
        /// <param name="lenth"></param>
        public virtual void SetOutCardsPerLenth(int lenth)
        {
            _mahJongEnv.SetOutCardsWidth(lenth);
        }
        /// <summary>
        /// 设置过蛋状态，抓牌时需要特殊处理
        /// </summary>
        public virtual void SetDanState(int card)
        {
            GuoDanShow.Instance.ShowGuoDanInfo(UserInfo.name,card);
        }

        public virtual void ShowDanEffect(int danNum)
        {
            CpghBehavior.ShowPiaoLabel(danNum);
            CurrentInfoPanel.AddGold(danNum);
        }

        public virtual void OnUserLeave()
        {
            Reset();
            UserInfo = null;
            CpghBehavior.HideReady();
        }

        public void Show(bool showState)
        {
            Reset();
            if (CurrentInfoPanel)
            {
                CurrentInfoPanel.ShowVisible(showState);
            }
        }

        public void RecheckLastGetIn()
        {
            var handCardCount = HandCardList.Count;
            if (handCardCount%3==2&&LastGetInCard==null)
            {
                GetDealGetInShowItem();
            }
        }

        /// <summary>
        ///处理抓牌显示
        /// </summary>
        protected virtual void GetDealGetInShowItem()
        {
            _mahJongEnv.SetItemAsNewGetIn(LastGetValue);
        }

    }
}
