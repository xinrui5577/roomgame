using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.lyzz2d.Game.Component;
using Assets.Scripts.Game.lyzz2d.Game.Data;
using Assets.Scripts.Game.lyzz2d.Game.Item;
using Assets.Scripts.Game.lyzz2d.Utils;
using Assets.Scripts.Game.lyzz2d.Utils.UI;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.lyzz2d.Game.GameCtrl
{
    public class MahjongPlayer : MonoBehaviour
    {
        /// <summary>
        ///     庄状态
        /// </summary>
        private bool _isZhuang;

        /// <summary>
        ///     麻将环境（显示UI，不存储整体数据，只放麻将）
        /// </summary>
        [SerializeField] private DefEnvpanel _mahJongEnv;

        /// <summary>
        ///     吃碰杠胡菜单
        /// </summary>
        public CpghChar CpghBehavior;

        /// <summary>
        ///     当前消息面板
        /// </summary>
        public UserInfoPanel CurrentInfoPanel;

        /// <summary>
        ///     玩家所在方向
        /// </summary>
        private readonly string[] dnxb = {"dong", "bei", "xi", "nan"};

        /// <summary>
        ///     组牌数据
        /// </summary>
        protected List<MahjongGroupData> GroupItems = new List<MahjongGroupData>();

        /// <summary>
        ///     实际的手牌数据（包括最后一张）
        /// </summary>
        protected List<int> HandCardList = new List<int>();

        /// <summary>
        ///     是否能打牌
        /// </summary>
        [HideInInspector] public bool HasToken;

        [HideInInspector]
        /// <summary>
        /// 获得的最后一张牌
        /// </summary>
        public int LastGetValue;

        /// <summary>
        ///     实际的手牌数据（不包括最后一张）
        /// </summary>
        protected List<int> ShowCardList = new List<int>();

        /// <summary>
        ///     打出的手牌
        /// </summary>
        protected List<int> ThrowOutCards = new List<int>();

        /// <summary>
        ///     庄操作
        /// </summary>
        public bool IsZhuang
        {
            set
            {
                _isZhuang = value;
                CurrentInfoPanel.SetZhuang(value);
            }
            get { return _isZhuang; }
        }

        /// <summary>
        ///     座位号
        /// </summary>
        public int UserSeat
        {
            get { return UserInfo.Seat; }
        }

        /// <summary>
        ///     是否有听
        /// </summary>
        public bool HasTing
        {
            set
            {
                UserInfo.HasTing = value;
                CurrentInfoPanel.SetTing(value);
            }
            get { return UserInfo.HasTing; }
        }


        /// <summary>
        ///     玩家信息
        /// </summary>
        public UserData UserInfo
        {
            set { CurrentInfoPanel.UserInfo = value; }
            get { return CurrentInfoPanel.UserInfo; }
        }

        /// <summary>
        ///     麻将环境
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
        ///     游戏控制器
        /// </summary>
        public Lyzz2DGameManager Manager
        {
            get { return App.GetGameManager<Lyzz2DGameManager>(); }
        }

        public Lyzz2DGlobalData Data
        {
            get { return App.GetGameData<Lyzz2DGlobalData>(); }
        }

        public bool IsOther
        {
            get { return !UserSeat.Equals(App.GetGameManager<Lyzz2DGameManager>().SelfPlayer.UserSeat); }
        }

        /// <summary>
        ///     玩家的打出的牌的状态
        /// </summary>
        public MahjongItem LastOutCard
        {
            get
            {
                var item = MahjongEnv.LastOutMahjong;
                if (item != null)
                {
                    //YxDebug.Log(string.Format("玩家{0}打出的最后一张牌是{1}", UserInfo.name, (EnumMahjongValue)item.Value));
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
        ///     玩家重置操作
        /// </summary>
        public virtual void Reset()
        {
            _isZhuang = false;
            HasTing = false;
            if (CurrentInfoPanel != null && UserInfo != null)
            {
                CurrentInfoPanel.SetGang(UserInfo.Piao);
                CurrentInfoPanel.Reset();
            }
            HandCardList.Clear();
            ShowCardList.Clear();
            ThrowOutCards.Clear();
            GroupItems.Clear();
            MahjongEnv.Reset(IsOther);
        }

        /// <summary>
        ///     加入游戏
        /// </summary>
        /// <param name="user"></param>
        internal MahjongPlayer JoinGame(UserData user)
        {
            UserInfo = user;
            App.GetGameManager<Lyzz2DGameManager>().Players[UserSeat] = this;
            UserInfo.Dnxb = dnxb[UserSeat];
            CurrentInfoPanel.Refresh();
            return this;
        }

        /// <summary>
        ///     重连
        /// </summary>
        /// <param name="data"></param>
        /// <param name="lastIn"></param>
        /// <param name="isCurrent"></param>
        public virtual void OnReJoin(ISFSObject data)
        {
            #region Data

            ISFSArray Groups;
            int[] OutCards;
            int[] HandCards;
            int HandNum;
            bool hasTing;
            GameTools.TryGetValueWitheKey(data, out Groups, RequestKey.KeyGroups);
            GameTools.TryGetValueWitheKey(data, out OutCards, RequestKey.KeyOutCards);
            GameTools.TryGetValueWitheKey(data, out HandCards, RequestKey.KeyHandCards);
            GameTools.TryGetValueWitheKey(data, out HandNum, RequestKey.KeyHandCardsNum);
            GameTools.TryGetValueWitheKey(data, out hasTing, RequestKey.KeyHasTing);
            HasTing = hasTing;
            GroupItems = new List<MahjongGroupData>();
            ThrowOutCards = new List<int>();
            GroupItems.AddRange(GameTools.GetGroupData(Groups));
            ThrowOutCards.AddRange(OutCards.ToList());
            if (HandCards.Length.Equals(0))
            {
                HandCards = new int[HandNum];
            }
            HandCardList = HandCards.ToList();

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

            #endregion
        }

        // ----------------------------------- 开始游戏
        internal void ShowPiao(int p)
        {
            CurrentInfoPanel.SetGang(p);
            if (UserInfo.Piao.Equals(99)) //好吧，没有什么好办法，这里要求一刚到底只有第一次显示动画，其他时候不显示
            {
                return;
            }
            UserInfo.Piao = p;
            CpghBehavior.ShowPiao(p);
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
            ShowCardList = new int[13].ToList();
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
            var item = MahjongEnv.AddHandCard(value, isSingle).GetComponent<MahjongItem>();
            if (item != null)
            {
                return item;
            }
            YxDebug.LogError(string.Format("新增了一张牌，没有创建成功，牌是{0},值是：{1}", (EnumMahjongValue) value, value));
            return null;
        }

        public virtual void TraAddLastGetInCard()
        {
            if (_mahJongEnv.LastGetInMahjong != null)
            {
                var item = _mahJongEnv.LastGetInMahjong;
                if (item != null)
                {
                    MahjongEnv.AddHandCard(item);
                    _mahJongEnv.LastGetInMahjong = null;
                }
            }
        }

        // 玩家收到单张的发牌（Game中已经根据玩家SEAT匹配到这个类中）
        public virtual void GetInCard(int value, ISFSObject param)
        {
            GetToken();
            LastGetValue = value;
            Facade.Instance<MusicManager>().Play(ConstantData.Voice_ZhuaPai);
            OnGetInCard(value);
        }

        protected virtual MahjongItem OnGetInCard(int value)
        {
            HandCardList.Add(value);
            var item = AddHandCard(value, true, true);
            return item;
        }

        /// <summary>
        ///     吃碰杠回包
        /// </summary>
        /// <param name="request"></param>
        /// <param name="data"></param>
        public void AllowRequest(RequestData request, ISFSObject data)
        {
            GetToken();
            int tType;
            int dealCard;
            var randomNum = 0;
            var voiceName = "";
            bool isOk;
            dealCard = request.Card;
            GameTools.TryGetValueWitheKey(data, out tType, RequestKey.KeyTypeType);
            GameTools.TryGetValueWitheKey(data, out isOk, RequestKey.KeyOk);
            if (isOk)
            {
                YxDebug.Log("这个回包中的ok标识是true，不执行操作，因为false时已经执行过了");
                return;
            }
            var needItems = new List<MahjongItem>();
            var groupData = new MahjongGroupData((GroupType) tType);
            var dealCards = new List<int>();
            for (int i = 0, lenth = request.Cards.Length; i < lenth; i++)
            {
                YxDebug.Log((EnumMahjongValue) request.Cards[i]);
            }
            var fromHand = true;
            switch ((Enum_CPGType) tType)
            {
                case Enum_CPGType.None:
                    return;
                case Enum_CPGType.Chi:
                    groupData.AddValue(request.Cards[0]);
                    groupData.AddValue(dealCard);
                    groupData.AddValue(request.Cards[1]);
                    for (int i = 0, lenth = request.Cards.Length; i < lenth; i++)
                    {
                        dealCards.Add(request.Cards[i]);
                    }
                    voiceName = ConstantData.Voice_Chi;
                    randomNum = Random.Range(0, 3);
                    break;
                case Enum_CPGType.Peng:
                    if (dealCard.Equals(App.GetGameManager<Lyzz2DGameManager>().FanNum) && Data.IsGangSelect)
                    {
                        voiceName = ConstantData.Voice_Gang;
                        randomNum = Random.Range(0, 2);
                        tType = (int) Enum_CPGType.ZhuaGang;
                    }
                    else
                    {
                        voiceName = ConstantData.Voice_Peng;
                        randomNum = Random.Range(0, 3);
                    }
                    dealCards = CreateListWithValue(2, dealCard);
                    break;
                case Enum_CPGType.ZhuaGang:
                    voiceName = ConstantData.Voice_Gang;
                    randomNum = Random.Range(0, 2);
                    fromHand = false;
                    dealCards = CreateListWithValue(1, dealCard);
                    break;
                case Enum_CPGType.PengGang:
                case Enum_CPGType.MingGang:
                    dealCards = CreateListWithValue(3, dealCard);
                    voiceName = ConstantData.Voice_Gang;
                    randomNum = Random.Range(0, 2);
                    break;
                case Enum_CPGType.AnGang:
                    dealCards = CreateListWithValue(4, dealCard);
                    voiceName = ConstantData.Voice_Gang;
                    randomNum = Random.Range(0, 2);
                    break;
                case Enum_CPGType.LaiZiGang:
                    break;
            }
            if (!tType.Equals((int) Enum_CPGType.Chi))
            {
                for (int i = 0, lenth = request.Cards.Length; i < lenth; i++)
                {
                    groupData.AddValue(dealCard);
                }
            }
            var lastItem = Manager.GetLastOutCardItem(dealCard);
            if (lastItem != null)
            {
                YxDebug.Log(string.Format("最后打出的一张牌是：{0}", (EnumMahjongValue) lastItem.Value));
                needItems.Add(lastItem);
                if (!tType.Equals((int) Enum_CPGType.Chi))
                {
                    groupData.AddValue(dealCard);
                }
            }
            if (fromHand)
            {
                needItems.AddRange(GetCardsFromHand(dealCards, IsOther));
                foreach (var item in needItems)
                {
                    DestroyUserContorl(item);
                }
                MahjongEnv.AddGroup(groupData, needItems, IsOther);
                GroupItems.Add(groupData);
            }
            else //抓杠特殊处理
            {
                var item = GetHandCard(dealCard);
                if (item != null)
                {
                    DestroyUserContorl(item);
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
                    YxDebug.LogError("抓杠时，找不到那张手牌" + (EnumMahjongValue) dealCard);
                }
            }
            CpghBehavior.SetBehavior((Enum_CPGType) tType);
            Facade.Instance<MusicManager>().Play(GameTools.GetOperationVoice(UserInfo.Sex, voiceName, randomNum));
            SortHandCard();
        }

        private List<int> CreateListWithValue(int num, int value)
        {
            var setList = new List<int>();
            for (var i = 0; i < num; i++)
            {
                setList.Add(value);
            }
            return setList;
        }

        public List<MahjongItem> GetCardsFromHand(List<int> list, bool isOther)
        {
            var needItems = new List<MahjongItem>();
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
            CpghBehavior.SetBehavior(Enum_CPGType.MingGang);

            Facade.Instance<MusicManager>()
                .Play(GameTools.GetOperationVoice(UserInfo.Sex, ConstantData.Voice_Gang, Random.Range(0, 2)));
            var groupData = new MahjongGroupData(GroupType.JueGang);
            foreach (var card in requeset.Cards)
            {
                if (card > 0)
                {
                    groupData.AddValue(card);
                }
            }
            var needItems = new List<MahjongItem>();
            needItems.AddRange(GetCardsFromHand(groupData.values.ToList(), IsOther));
            foreach (var item in needItems)
            {
                DestroyUserContorl(item);
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
        ///     刷新使用
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
                YxDebug.Log(string.Format("玩家{0},name{1}存在token,在打牌时删除一张牌", UserInfo.name, UserInfo.name));
            }
            MahjongEnv.RefreshHandList(ShowCardList);
        }

        public virtual bool OnGetNewCard()
        {
            return _mahJongEnv.OnGetNewCard();
        }

        // 显示吃碰杠胡
        public void ShowCpgh(Enum_CPGType behivor)
        {
            YxDebug.Log("------> MahjongPlayer.ShowCpgh() behivor = " + behivor);
            if (CpghBehavior == null) return;
            CpghBehavior.SetBehavior(behivor);
        }

        public virtual MahjongItem OnThrowCard(int value)
        {
            YxDebug.Log(string.Format("玩家{0}打出一张牌{1}", UserInfo.name, (EnumMahjongValue) value));
            LostToken();
            var item = GetHandCard(value);
            if (IsOther)
            {
                HandCardList.Remove(0);
            }
            else
            {
                HandCardList.Remove(value);
            }
            MahjongEnv.ThrowOutCard(item);
            SortHandCard();
            Facade.Instance<MusicManager>().Play(GameTools.GetOperationVoice(UserInfo.Sex, value.ToString(), 0));
            YxDebug.Log("打牌飞起状态");
            return item;
        }

        public virtual void OnOtherGetCard()
        {
            if (LastOutCard != null)
            {
                YxDebug.Log(string.Format("玩家{0}的牌{1}落下", UserInfo.name, (EnumMahjongValue) LastOutCard.Value));
            }
            MahjongEnv.ThrowOutCardToOutCard();
        }

        /// <summary>
        ///     从手里获得一张牌，任意牌
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual MahjongItem GetHandCard(int value)
        {
            var item = _mahJongEnv.GetHandItem(value);
            item.Value = value;
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

        public virtual void ShowLiangPaiAtGameEnd(List<int> ShowCards, bool zimo, int huCard = 0)
        {
            if (zimo)
            {
                ShowCards.Remove(huCard);
            }
            HandCardList = ShowCards.ToList();
            if (huCard > 0)
            {
                if (OnGetNewCard())
                {
                    MahjongEnv.LastGetInMahjong.Value = huCard;
                }
                else
                {
                    MahjongEnv.AddHandCard(huCard, true, false);
                }
                MahjongEnv.LastGetInMahjong.SetHuTag();
            }
            ShowCardList =
                GameTools.SortCardWithLaiZi(HandCardList, App.GetGameManager<Lyzz2DGameManager>().LaiZiNum).ToList();
            LostToken();
            RefresheShow(false);
            DoShowLiang();
        }

        private void DoShowLiang()
        {
            MahjongEnv.ShowHandCard();
            Facade.Instance<MusicManager>().Play(ConstantData.Voice_LiangPai);
        }

        protected virtual void DestroyUserContorl(MahjongItem item)
        {
            var uc = item.GetComponent<UserContorl>();
            if (uc != null)
                Destroy(uc);
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


        //-------------------------------------社交相关
        public void OnPlayeTalk(AudioClip clip, int lenth)
        {
            CurrentInfoPanel.PlayVoiceChat(clip, lenth);
        }

        public void SetUserHead(bool state, bool showTag)
        {
            CurrentInfoPanel.SetOutLineState(state, showTag);
            var active = !((Manager.IsInit() || Manager.IsWaiting()) && Data.IsFirstTime && !state);
            CurrentInfoPanel.gameObject.SetActive(active);
        }

        /// <summary>
        ///     处理听状态
        /// </summary>
        public virtual void CheckTingState()
        {
        }

        public virtual void ShowTingWithEffect(bool withEffect)
        {
            HasTing = true;
            Manager.IsTingExist = true;
            if (withEffect)
            {
                CpghBehavior.SetBehavior(Enum_CPGType.Ting);
                Facade.Instance<MusicManager>()
                    .Play(GameTools.GetOperationVoice(UserInfo.Sex, ConstantData.Voice_Ting, Random.Range(0, 4)));
            }
        }
    }
}