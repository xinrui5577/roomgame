using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.lyzz2d.Game.Item;
using Assets.Scripts.Game.lyzz2d.Utils;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.lyzz2d.Game.GameCtrl
{
    /// <summary>
    ///     麻将控制玩家（当前玩家）
    /// </summary>
    public class MahjongPlayerCtrl : MahjongPlayer
    {
        /// <summary>
        ///     要检查的牌
        /// </summary>
        protected int _checkCard;

        /// <summary>
        ///     可以杠的牌
        /// </summary>
        protected List<int> _gangList;

        /// <summary>
        ///     手牌数量（实际数量，包括新抓的牌）
        /// </summary>
        private int _handItemNum;

        /// <summary>
        ///     麻将点击事件
        /// </summary>
        protected MahjongClick _mjClick;

        /// <summary>
        ///     已有牌的牌型
        /// </summary>
        protected Dictionary<int, int> _typeDic;

        /// <summary>
        ///     可以吃的牌
        /// </summary>
        protected List<int[]> chiGroup;

        /// <summary>
        ///     默认的超时菜单
        /// </summary>
        protected int DEF_MENU_TIMEOUT = 16; // 菜单选择超时时间

        protected MulSetting Menus = new MulSetting(7, 0);

        /// <summary>
        ///     牌对
        /// </summary>
        protected Dictionary<int, int> TypeDic
        {
            get { return _typeDic; }
        }

        public override void Awake()
        {
            base.Awake();
            _gangList = new List<int>();
            _typeDic = new Dictionary<int, int>();
            chiGroup = new List<int[]>();
        }

        private void Start()
        {
            ShowChiPaiInfo.Instance.OnClickChiEvent = OnClickChiG;
            ShowChiPaiInfo.Instance.OnClickResetEvevt += ReShowMenu;
            ShowGangPaiInfo.Instance.OnClickGangEvent = OnSelectGangItem;
            ShowGangPaiInfo.Instance.OnClickResetEvevt += ReShowMenu;
            ShowTingOutInfo.Instance.OnClickItemEvent = OnSelectTingOutItem;
            ShowTingOutInfo.Instance.OnClickResetEvevt += ReShowMenu;
        }

        public override void Reset()
        {
            base.Reset();
            _handItemNum = 0;
            chiGroup.Clear();
            _gangList.Clear();
            _typeDic.Clear();
            _mjClick = DoThrowOutCardClick;
            ShowChiPaiInfo.Instance.Show(false);
            ClearMenu();
        }

        public override void AllocateCards()
        {
            YxDebug.Log("当前玩家获得分牌，数量是:" + HandCardList.Count);
            HandCardList = GameTools.SortCardWithLaiZi(HandCardList, 0).ToList();
            AddHandCards(HandCardList);
        }

        public override MahjongItem AddHandCard(int value, bool isSingle, bool refresh)
        {
            var item = base.AddHandCard(value, isSingle, refresh);
            AddUserControl(item);
            if (refresh)
            {
                SortHandCard();
            }
            return item;
        }

        protected override MahjongItem OnGetInCard(int value)
        {
            var newItem = base.OnGetInCard(value);
            if (newItem != null)
            {
                if (HasTing)
                {
                    newItem.SetColor(Color.gray);
                }
                _checkCard = value;
            }
            return newItem;
        }

        /// <summary>
        ///     自动打牌
        /// </summary>
        public virtual void ThrowCardAuto()
        {
            if (HasToken)
            {
                var tryThrowCard = _checkCard;
                if (Data.IsInRobot || HasTing)
                {
                    while (tryThrowCard == App.GetGameManager<Lyzz2DGameManager>().LaiZiNum)
                    {
                        var lenth = HandCardList.Count;
                        tryThrowCard = HandCardList[Random.Range(0, lenth - 1)];
                    }
                    var item = GetHandCard(tryThrowCard);
                    if (item != null)
                    {
                        OnMahjongClick(item.transform);
                        _checkCard = tryThrowCard;
                    }
                }
            }
        }

        public override void ShowTingWithEffect(bool withEffect)
        {
            base.ShowTingWithEffect(withEffect);
            MahjongEnv.SetHandCardsColor(Color.grey, true);
        }

        public void SetCards(int[] getIntArray)
        {
            HandCardList = getIntArray.ToList();
        }

        // 收到牌 发牌和重连时使用
        protected void AddUserControl(MahjongItem item)
        {
            var uc = item.GetComponent<UserContorl>() ?? item.gameObject.AddComponent<UserContorl>();
            uc.OnThrowOut = OnMahjongClick;
        }

        /// <summary>
        ///     这里是本地出现相公牌时，强制同步服务器
        /// </summary>
        /// <param name="getIntArray"></param>
        public void SetCheckCards(int[] getIntArray)
        {
            //YxMessageBox.Show("由于您的网络不稳定，导致数据异常，建议您重启游戏");
        }

        public override MahjongItem OnThrowCard(int value)
        {
            var item = base.OnThrowCard(value);
            item.SetColor(Color.white);
            DestroyUserContorl(item);
            SortCardOnSureLaizZi();
            return item;
        }

        /// <summary>
        ///     获得一张手牌
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override MahjongItem GetHandCard(int value)
        {
            var item = MahjongEnv.GetHandCardByValue(value);
            if (item == null)
            {
                YxDebug.LogError(string.Format("玩家{0}要获得的手牌{1}不存在", UserInfo.name, (EnumMahjongValue) value));
            }
            return item;
        }

        /// <summary>
        ///     玩家打牌请求
        /// </summary>
        /// <param name="mjItem"></param>
        private void DoThrowOutCardClick(MahjongItem mjItem)
        {
            SFSObject param;
            if (Data.IsClickTing)
            {
                param = GameTools.getSFSObject((int) EnumRequest.Ting);
                Data.IsClickTing = false;
            }
            else
            {
                param = GameTools.getSFSObject((int) EnumRequest.ThrowOutCard);
            }
            param.PutInt(RequestKey.KeyOpCard, mjItem.Value);
            App.GetGameManager<Lyzz2DGameManager>().SendDataToServer(param);
            App.GetGameManager<Lyzz2DGameManager>().ClearFlagCard();
            _gangList.Clear();
            LostToken();
        }

        protected void FindAnGang()
        {
            foreach (var pair in TypeDic)
            {
                if (!_gangList.Contains(pair.Key))
                {
                    if (pair.Value == 4 && !pair.Key.Equals(App.GetGameManager<Lyzz2DGameManager>().LaiZiNum))
                    {
                        _gangList.Add(pair.Key);
                    }
                }
            }
        }

        protected void FindPengGang()
        {
            foreach (var pair in TypeDic)
            {
                if (!_gangList.Contains(pair.Key))
                {
                    foreach (var mahjongGroup in GroupItems)
                    {
                        // 碰过的牌里头，看看手里是否还有那个牌。有就可以杠 例如碰过4万，如果手里还有四万，那么就可以杠

                        if (mahjongGroup.type == GroupType.Peng && mahjongGroup.values[0] == pair.Key)
                        {
                            _gangList.Add(pair.Key);
                        }
                    }
                }
            }
        }

        public void ShowMenuByCheck(int checkType, int value, int seat)
        {
            HideMenuAndChi();
            _checkCard = value;
            if ((checkType & (int) Enum_CPGHMenuType.Chi) == (int) Enum_CPGHMenuType.Chi)
            {
                CheckChi(value, seat);
            }
            //绝杠特殊显示
            if ((checkType & (int) Enum_CPGHMenuType.Peng) == (int) Enum_CPGHMenuType.Peng)
            {
                if (value.Equals(App.GetGameManager<Lyzz2DGameManager>().FanNum) && Data.IsGangSelect)
                {
                    checkType |= (int) Enum_CPGHMenuType.Peng;
                    checkType ^= (int) Enum_CPGHMenuType.Peng;
                    checkType |= (int) Enum_CPGHMenuType.JueGang;
                }
            }
            if ((checkType & (int) Enum_CPGHMenuType.Gang) == (int) Enum_CPGHMenuType.Gang)
            {
                _gangList.Clear();
                FindAnGang();
                FindPengGang();
            }
            if ((checkType & (int) Enum_CPGHMenuType.JueGang) == (int) Enum_CPGHMenuType.JueGang)
            {
                YxDebug.Log("这里是绝杠");
                checkType |= (int) Enum_CPGHMenuType.Gang;
                checkType ^= (int) Enum_CPGHMenuType.Gang;
            }
            Menus.DisableAll();
            Menus.EnableCondition(checkType);
            InteraptMenu.Instance.ShowMenu(Menus, DEF_MENU_TIMEOUT);
        }

        public void HideMenuAndChi()
        {
            Menus.DisableAll();
            ShowChiPaiInfo.Instance.Show(false);
        }

        // 吃牌判断 不需要吃的玩法可以在子类中重写改函数 吃需要有左右中三种吃提示
        protected virtual void CheckChi(int value, int seat)
        {
            _checkCard = value;
            chiGroup.Clear();
            ///要求从小到大进行显示
            // 往右吃 ，如 value = 35,手牌有33，34 有3条及以上可以被右吃            
            if (TypeDic.ContainsKey(value - 1) && TypeDic.ContainsKey(value - 2))
            {
                var chiCardsCod3 = new int[3];
                chiCardsCod3[0] = value - 2;
                chiCardsCod3[1] = value - 1;
                chiCardsCod3[2] = value;
                if (CheckChiCardsWithLaizi(chiCardsCod3))
                {
                    chiGroup.Add(chiCardsCod3);
                }
            }

            // 中间吃，比如，value =34,手牌有33，35 保证范围不会超出 这类型牌的牌值范围
            if (TypeDic.ContainsKey(value - 1) && TypeDic.ContainsKey(value + 1))
            {
                var chiCardsCod2 = new int[3];
                chiCardsCod2[0] = value - 1;
                chiCardsCod2[1] = value;
                chiCardsCod2[2] = value + 1;
                if (CheckChiCardsWithLaizi(chiCardsCod2))
                {
                    chiGroup.Add(chiCardsCod2);
                }
            }

            // 往左吃，如 value = 39 ,手牌 40，41
            if (TypeDic.ContainsKey(value + 1) && TypeDic.ContainsKey(value + 2))
            {
                var chiCardsCod1 = new int[3];
                chiCardsCod1[0] = value;
                chiCardsCod1[1] = value + 1;
                chiCardsCod1[2] = value + 2;
                if (CheckChiCardsWithLaizi(chiCardsCod1))
                {
                    chiGroup.Add(chiCardsCod1);
                }
            }
        }

        private bool CheckChiCardsWithLaizi(int[] cards)
        {
            var state = true;
            for (var i = 0; i < cards.Length; i++)
            {
                if (cards[i] == App.GetGameManager<Lyzz2DGameManager>().LaiZiNum)
                {
                    state = false;
                    break;
                }
            }
            return state;
        }

        // ----------------------------------- 请求吃碰杠胡出牌
        // 用户点击了吃牌 如果只有一组直接吃 如果有多组弹出选择界面
        public virtual void OnChiClick()
        {
            YxDebug.Log("------> MahjongPlayerControl.OnChiClick() count = " + chiGroup.Count);

            if (chiGroup != null && chiGroup.Count > 0)
            {
                //如果只有一种可能，直接吃
                if (chiGroup.Count == 1)
                {
                    OnClickChiG(ShowChiPaiInfo.Instance.ChiZone[0]);
                    return;
                }

                ShowChiPaiInfo.Instance.SetChiInfos(chiGroup);
            }
            else
            {
                YxDebug.LogError("当前没有可以吃的情况，猜测为癞子牌相关的错误。请咨询服务器");
            }
        }

        //获得要组合吃的那两张牌
        private int[] getChiGpcards(int[] cardsTmp)
        {
            var finalChi = new int[2];
            var j = 0;
            for (var i = 0; i < 3; i++)
            {
                if (finalChi.Length <= j)
                {
                    break;
                }
                if (cardsTmp[i] != _checkCard)
                {
                    finalChi[j] = cardsTmp[i];
                    j++;
                }
            }
            return finalChi;
        }

        public void ReShowMenu()
        {
            Data.IsClickTing = false;
            Manager.ReCheckOp();
        }

        private void OnClickChiG(GameObject button)
        {
            var clickNum = int.Parse(button.name);
            var finalChi = getChiGpcards(chiGroup[clickNum]); // 获得组合吃的那两张牌
            YxDebug.Log(string.Format("用来吃的牌是：{0}", (EnumMahjongValue) _checkCard));
            foreach (var chiitem in finalChi)
            {
                YxDebug.Log((EnumMahjongValue) chiitem);
            }
            YxDebug.Log("吃的牌是：" + (EnumMahjongValue) _checkCard);
            ClearMenu();
            var request = GameTools.getSFSObject((int) EnumRequest.CPHType);
            request.PutInt(RequestKey.KeyTypeType, (int) Enum_CPGType.Chi);
            request.PutInt(RequestKey.KeyOpCard, _checkCard);
            request.PutIntArray(RequestKey.KeyCards, finalChi);
            SendRequest(request);
            ShowChiPaiInfo.Instance.Show(false);
        }

        // 用户点击了碰牌 没有太多操作 直接发送请求
        public void OnPengClick()
        {
            ClearMenu();
            var request = GameTools.getSFSObject((int) EnumRequest.CPHType);
            request.PutInt(RequestKey.KeyTypeType, (int) Enum_CPGType.Peng);
            request.PutInt(RequestKey.KeyOpCard, _checkCard);
            request.PutIntArray(RequestKey.KeyCards, new[] {_checkCard, _checkCard});
            SendRequest(request);
        }

        // 用户点击了杠，杠别人还是杠自己，如果是当前有token,说明是自己抓的，明杠、暗杠
        public void OnGangClick()
        {
            //自己出牌阶段，是明杠或者抓到一个碰过的牌
            if (HasToken)
            {
                //如果不止一个能杠，那么要出菜单选择一个杠的牌 特别的：一个暗杠一个抓杠
                var len = _gangList.Count;
                YxDebug.Log("当前条件下的gang的数量是：" + len);
                if (len > 1)
                {
                    YxDebug.Log(string.Format("当前的杠数量是：{0}", _gangList.Count));
                    foreach (var gang in _gangList)
                    {
                        YxDebug.Log(string.Format("杠的值是：{0}", (EnumMahjongValue) gang));
                    }
                    InteraptMenu.Instance.Revert(); //多选择的杠不清除杠标记状态，可以点击其它位置取消操作
                    ToSelectGang();
                }
                else if (len == 0)
                {
                    YxDebug.LogError("当前没有可以杠的值，请检查客户端代码");
                }
                else
                {
                    //自己手里有
                    SelfGang(_gangList[0]);
                }
            }
            //别人出牌，自然是自己3个杠别人
            else
            {
                var sfsObject = GameTools.getSFSObject((int) EnumRequest.CPHType);
                sfsObject.PutInt(RequestKey.KeyTypeType, (int) Enum_CPGType.PengGang);
                sfsObject.PutInt(RequestKey.KeyOpCard, _checkCard);
                sfsObject.PutIntArray(RequestKey.KeyCards, new[] {_checkCard, _checkCard, _checkCard});
                SendRequest(sfsObject);
            }
        }

        public void OnXuanFengGangClick()
        {
        }

        public void OnJueClick()
        {
            YxDebug.Log("绝杠");
            ClearMenu();
            _checkCard = App.GetGameManager<Lyzz2DGameManager>().FanNum;
            ISFSObject data;
            ;
            if (HasToken)
            {
                data = GameTools.getSFSObject((int) EnumRequest.JueGang);
                data.PutInt(RequestKey.KeyOpCard, _checkCard);
                data.PutIntArray(RequestKey.KeyCards, new[] {_checkCard, _checkCard, _checkCard});
            }
            else
            {
                data = GameTools.getSFSObject((int) EnumRequest.CPHType);
                data.PutIntArray(RequestKey.KeyCards, new[] {_checkCard, _checkCard});
                data.PutInt(RequestKey.KeyTypeType, (int) Enum_CPGType.Peng);
                data.PutInt(RequestKey.KeyOpCard, _checkCard);
            }
            SendRequest(data, false);
        }

        public void OnCancelClick()
        {
            Menus.DisableAll();
            if (Manager.CurrentPosition != UserSeat)
            {
                var sfsObject = GameTools.getSFSObject((int) EnumRequest.CPHType);
                sfsObject.PutInt(RequestKey.KeyTypeType, (int) Enum_CPGType.None);
                SendRequest(sfsObject);
            }
        }

        private void ToSelectGang()
        {
            ShowGangPaiInfo.Instance.ShowInfo(_gangList);
        }

        private void OnSelectGangItem(int value)
        {
            _checkCard = value;
            SelfGang(value);
        }

        private void SelfGang(int card)
        {
            var sfsObject = SFSObject.NewInstance();
            if (_typeDic.ContainsKey(card) && _typeDic[card] > 3)
            {
                sfsObject.PutInt(RequestKey.KeyTypeType, (int) Enum_CPGType.AnGang);
                sfsObject.PutIntArray(RequestKey.KeyCards, new[] {card, card, card, card});
                sfsObject.PutInt(RequestKey.KeyOpCard, card);
            }
            else
            {
                sfsObject.PutInt(RequestKey.KeyTypeType, (int) Enum_CPGType.ZhuaGang);
                sfsObject.PutIntArray(RequestKey.KeyCards, new[] {card});
                sfsObject.PutInt(RequestKey.KeyOpCard, card);
            }
            sfsObject.PutInt(RequestKey.KeyType, (int) EnumRequest.SelfGang);
            SendRequest(sfsObject);
            ClearMenu();
        }

        // 打牌
        protected virtual void OnMahjongClick(Transform item)
        {
            YxDebug.Log("玩家打牌");
            if (!Menus.IsForbiddenAll())
            {
                YxDebug.Log("当前存在菜单操作，不能打牌");
                return;
            }
            if (App.GetGameManager<Lyzz2DGameManager>().GetLeftNum() < 0)
            {
                return;
            }
            var mahjongItem = item.GetComponent<MahjongItem>();
            if (mahjongItem.Value == GetLaiZiValue())
            {
                YxDebug.Log("混牌不能打");
                item.GetComponent<UserContorl>().OnRecoveryPos();
                return;
            }
            if (HasToken)
            {
                _mjClick(mahjongItem);
            }
            else
            {
                item.GetComponent<UserContorl>().OnRecoveryPos();
            }
        }


        // 用户选择胡牌
        public virtual void OnHuClick()
        {
            ClearMenu();
            var type = HasToken ? (int) EnumRequest.ZiMo : (int) EnumRequest.CPHType;
            var sfsObject = GameTools.getSFSObject(type);
            if (!HasToken)
                sfsObject.PutInt(RequestKey.KeyTypeType, (int) EnumRequest.Hu);
            SendRequest(sfsObject);
        }


        protected void SendRequest(ISFSObject request, bool cleanToken = true)
        {
            App.GetGameManager<Lyzz2DGameManager>().SendDataToServer(request);
        }

        // ----------------------------------- 收到吃碰杠胡回包重置按钮状态
        public void DisabelMenu()
        {
            YxDebug.Log("------> MahjongPlayerControl.DisabelMenu()");
            ClearMenu();
            Menus.DisableAll();
        }

        // ----------------------------------- 游戏结束


        public int GetLaiZiValue()
        {
            return App.GetGameManager<Lyzz2DGameManager>().LaiZiNum;
        }

        /// <summary>
        ///     手牌排序
        /// </summary>
        /// <returns></returns>
        protected virtual List<int> SortCardValueList()
        {
            _handItemNum = HandCardList.Count;
            //按照牌值，给牌面排序
            HandCardList = GameTools.SortCardWithLaiZi(HandCardList, Manager.LaiZiNum);
            SortType();
            return HandCardList;
        }

        /// <summary>
        ///     确定会牌后需要重新排序一下SortHandCard
        /// </summary>
        public void SortCardOnSureLaizZi()
        {
            SortHandCard();
        }

        /// <summary>
        ///     设置最后一张抓进手里的牌
        /// </summary>
        /// <param name="lastIn"></param>
        public override void SetLastInCardOnReJoin(int lastIn)
        {
            LastGetValue = lastIn;
            MahjongEnv.SetLastInCardOnReJoin(lastIn);
        }

        protected override void SortHandCard()
        {
            ShowCardList = SortCardValueList().ToList();
            RefresheShow(IsOther);
        }

        /// <summary>
        ///     筛选处理，可以快速的碰和杠
        /// </summary>
        private void SortType()
        {
            _typeDic.Clear();
            var len = HandCardList.Count;
            var singleNum = -1;
            for (var i = 0; i < len; i++)
            {
                if (HandCardList[i] != singleNum)
                {
                    singleNum = HandCardList[i];
                    _typeDic[singleNum] = 1;
                }
                else
                {
                    _typeDic[singleNum] += 1;
                }
            }
        }


        public override void LostToken()
        {
            base.LostToken();
            ClearMenu();
        }

        // sendNoneReq:是否需要向服务端发送取消消息（只有等待自己出牌的时候才不需要发送）
        public void ShowMenu(int timeOut, bool sendNoneReq = true)
        {
            if (Menus.IsForbiddenAll())
            {
                if (sendNoneReq)
                    ResponseOtherThrow();
                return;
            }
            InteraptMenu.Instance.ShowMenu(Menus, timeOut);
        }

        /// <summary>
        ///     过
        /// </summary>
        protected virtual void ResponseOtherThrow()
        {
            var param = GameTools.getSFSObject((int) EnumRequest.CPHType);
            param.PutInt(RequestKey.KeyTypeType, (int) Enum_CPGType.None);
            App.GetRServer<Lyzz2DGameServer>().SendGameRequest(param);
        }

        public void OnTingClick()
        {
            if (App.GetGameManager<Lyzz2DGameManager>().TingOutCards.Count > 0)
            {
                if (HasToken)
                {
                    Data.IsClickTing = true;
                    ToSelectTingOut();
                }
            }
            else
            {
                YxDebug.LogError("tingOut 的数量是0，问问服务器咋回事");
            }
        }

        // 回合结束，清理菜单        
        public void ClearMenu()
        {
            Menus.DisableAll();
            InteraptMenu.Instance.Revert();
        }

        public override void CheckTingState()
        {
            ThrowCardAuto();
        }

        protected virtual void ToSelectTingOut()
        {
            var outList = Manager.TingOutCards;
            ShowTingOutInfo.Instance.ShowInfo(outList);
        }

        private void OnSelectTingOutItem(int value)
        {
            var selectItem = GetHandCard(value);
            if (selectItem != null)
            {
                DoThrowOutCardClick(selectItem);
            }
            else
            {
                YxDebug.LogError("There is not exist such MahjongItem,please checkHandCards");
            }
        }

        protected delegate void MahjongClick(MahjongItem mjItem);
    }
}