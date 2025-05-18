using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Request;
using Assets.Scripts.Game.Mahjong2D.fxmj;
using Assets.Scripts.Game.Mahjong2D.Game.Data;
using Assets.Scripts.Game.Mahjong2D.Game.GameCtrl;
using Assets.Scripts.Game.Mahjong2D.Game.Item;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Game.Mahjong2D.Game.Player
{
    /// <summary>
    /// 麻将控制玩家（当前玩家）
    /// </summary>
    public class MahjongPlayerCtrl : MahjongPlayer
    {
        protected delegate void MahjongClick(MahjongItem mjItem);
        /// <summary>
        /// 要检查的牌
        /// </summary>
        protected int CheckCard;
        /// <summary>
        /// 已有牌的牌型
        /// </summary>
        protected Dictionary<int, int> _typeDic;
        /// <summary>
        /// 牌对
        /// </summary>
        protected Dictionary<int, int> TypeDic { get { return _typeDic; } }
        /// <summary>
        /// 麻将点击事件
        /// </summary>
        protected MahjongClick MjClick;
        /// <summary>
        /// 可以杠的牌
        /// </summary>
        protected List<int> GangList;
        /// <summary>
        /// 可以吃的牌
        /// </summary>
        protected List<int[]> ChiGroup;
        /// <summary>
        /// 默认的超时菜单
        /// </summary>
        protected int DEF_MENU_TIMEOUT = 16;                // 菜单选择超时时间
        [Tooltip("上听后等待请求时间")]
        public float AutoWaitRequestTime = 1.5f;
        [Tooltip("托管状态显示的菜单项,默认只显示胡与杠")]
        public List<EnumCpghMenuType> AutoShowTypes=new List<EnumCpghMenuType>()
        {
            EnumCpghMenuType.Hu,EnumCpghMenuType.Gang
        };
        /// <summary>
        /// 特殊杠的牌
        /// </summary>
        private List<int[]> _specialGangList;
        /// <summary>
        /// 是否是打牌牌型
        /// </summary>
        public bool CanThrowCard
        {
            get
            {
                return HandCardList.Count % 3 == 2;
            }
        }
        [Tooltip("当失去token时，是否需要锁定牌")]
        public bool NeedBlockOnLoToken = false;


        private WaitForSeconds _waitForAutoRequest;
        public override void Awake()
        {
            base.Awake();
            GangList = new List<int>();
            _typeDic = new Dictionary<int, int>();
            ChiGroup = new List<int[]>();
            _specialGangList = new List<int[]>();
            _waitForAutoRequest =new WaitForSeconds(AutoWaitRequestTime);
            if (NeedBlockOnLoToken)
            {
                UserContorl.BlockUserControl = true;
            }
        }

        void Start()
        {
            InitListener();
        }

        protected virtual void InitListener()
        {
            ShowChiPaiInfo.Instance.OnClickChiEvent = OnClickChiG;
            ShowGangPaiInfo.Instance.OnClickGangEvent = OnSelectGangItem;
            ShowGangPaiInfo.Instance.OnClickResetEvevt += ReShowMenu;
            ShowChiPaiInfo.Instance.OnClickResetEvevt += ReShowMenu;
            ShowSpecialGangInfo.Instance.OnClickResetEvevt += ReShowMenu;
        }

        protected override void OnTokenStateChange(bool token)
        {
            base.OnTokenStateChange(token);
            if (NeedBlockOnLoToken)
            {
                UserContorl.BlockUserControl = !token;
            }
        }

        public override void Reset()
        {
            base.Reset();
            ChiGroup.Clear();
            GangList.Clear();
            _typeDic.Clear();
            MjClick = DoThrowOutCardClick;
            ShowChiPaiInfo.Instance.Show(false);
            ClearMenu();
        }

        public override void AllocateCards()
        {
            HandCardList = GameTools.SortCardWithLaiZi(HandCardList, 0).ToList();
            AddHandCards(HandCardList);
        }

        public override MahjongItem AddHandCard(int value, bool isSingle, bool refresh)
        {
            MahjongItem item = base.AddHandCard(value, isSingle, refresh);
            AddUserControl(item);
            if (refresh)
            {
                SortHandCard();
            }
            return item;
        }

        protected override MahjongItem OnGetInCard(int value)
        {
            MahjongItem newItem = base.OnGetInCard(value);
            if (newItem != null)
            {
                if (HasTing)
                {
                    newItem.SetColor(Color.gray);
                }
                CheckCard = value;
            }
            YxDebug.LogError("当前玩家存在手牌数量:"+HandCardList.Count);
            return newItem;
        }
        public virtual void ThrowCardAuto()
        {
            if (HasToken)
            {
                int tryThrowCard;
                if (LastGetInCard != null)
                {
                    tryThrowCard = LastGetInCard.Value;
                }
                else
                {
                    return;
                }
                if (Data.IsInRobot || HasTing)
                {
                    while (tryThrowCard == App.GetGameManager<Mahjong2DGameManager>().LaiZiNum)
                    {
                        int lenth = HandCardList.Count;
                        tryThrowCard = HandCardList[Random.Range(0, lenth - 1)];
                    }
                    MahjongItem item = GetHandCard(tryThrowCard);
                    if (item != null)
                    {
                        OnMahjongClick(item.transform);
                        CheckCard = tryThrowCard;
                    }
                }
            }
        }

        public void SetCards(int[] getIntArray)
        {
            HandCardList = getIntArray.ToList();
        }

        // 收到牌 发牌和重连时使用
        protected void AddUserControl(MahjongItem item)
        {
            UserContorl uc = item.GetComponent<UserContorl>() ?? item.gameObject.AddComponent<UserContorl>();
            uc.OnThrowOut = OnMahjongClick;
        }
        protected void AddUserControl(Transform item)
        {
            UserContorl uc = item.GetComponent<UserContorl>() ?? item.gameObject.AddComponent<UserContorl>();
            uc.OnThrowOut = OnMahjongClick;
        }
        /// <summary>
        /// 这里是本地出现相公牌时，强制同步服务器
        /// </summary>
        /// <param name="getIntArray"></param>
        public void SetCheckCards(int[] getIntArray)
        {
            //YxMessageBox.Show("由于您的网络不稳定，导致数据异常，建议您重启游戏");
        }

        public override MahjongItem OnThrowCard(int value, AsyncCallback finishCall = null)
        {
            MahjongItem item = base.OnThrowCard(value, finishCall);
            item.SetColor(Color.white);
            GameTools.DestroyUserContorl(item);
            SortCardOnSureLaizZi();
            return item;
        }
        /// <summary>
        /// 获得一张手牌
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override MahjongItem GetHandCard(int value)
        {
            MahjongItem item = MahjongEnv.GetHandCardByValue(value);
            if (item == null)
            {
                YxDebug.LogError(string.Format("玩家{0}要获得的手牌{1}不存在", UserInfo.name, (EnumMahjongValue)value));
            }
            return item;
        }

        /// <summary>
        /// 玩家打牌请求
        /// </summary>
        /// <param name="mjItem"></param>
        private void DoThrowOutCardClick(MahjongItem mjItem)
        {
            SFSObject param;
            if (Data.IsClickTing || Data.IsXiaoSa)
            {
                param = GameTools.getSFSObject((int)EnumRequest.Ting);
                if (Data.IsXiaoSa)
                {
                    param.PutInt(RequestKey.KeyXs, 1);
                }
            }
            else
            {
                param = GameTools.getSFSObject((int)EnumRequest.ThrowOutCard);
            }
            param.PutInt(RequestKey.KeyOpCard, mjItem.Value);
            param.PutInt(RequestKey.KeySeat, UserSeat);
            App.GetGameManager<Mahjong2DGameManager>().ClearFlagCard();
            GangList.Clear();
            if (Data.IsClickTing || Data.IsXiaoSa)
            {
                Data.IsClickTing = false;
                Data.IsXiaoSa = false;
            }
            else
            {
                if (HasTing || Data.IsInRobot)
                {
                    StartCoroutine(TingWaitRequest(param));
                    return;
                }
            }
            App.GetGameManager<Mahjong2DGameManager>().SendDataToServer(param);
        }

        IEnumerator TingWaitRequest(ISFSObject param)
        {
            yield return _waitForAutoRequest;
            App.GetGameManager<Mahjong2DGameManager>().SendDataToServer(param);
        }


        protected void FindAnGang()
        {
            if (Manager.FilTrateGang&&!Manager.AllowAnGang)
            {
                return;
            }
            foreach (KeyValuePair<int, int> pair in TypeDic)
            {
                if (!GangList.Contains(pair.Key))
                {
                    if ((pair.Value) == 4 && (!pair.Key.Equals(Manager.LaiZiNum)))
                    {
                        GangList.Add(pair.Key);
                    }
                }
            }
        }
        protected void FindZhuaGang()
        {
            if (Manager.FilTrateGang && !Manager.AllowZhuaGang)
            {
                return;
            }
            foreach (KeyValuePair<int, int> pair in TypeDic)
            {
                if (!GangList.Contains(pair.Key))
                {
                    foreach (MahjongGroupData mahjongGroup in GroupItems)
                    {
                        // 碰过的牌里头，看看手里是否还有那个牌。有就可以杠 例如碰过4万，如果手里还有四万，那么就可以杠

                        if (mahjongGroup.type == GroupType.Peng && mahjongGroup.values[0] == pair.Key)
                        {
                            GangList.Add(pair.Key);
                        }
                    }
                }
            }

        }

        public virtual void ShowMenuByCheck(int checkType, int value, int seat)
        {
            if (Data.IsInRobot&&HasTing==false)
            {
                if (CheckAutoMenuState(ref checkType))
                {
                    OnCancelClick();
                    return;
                }
            }
            HideMenu();
            CheckCard = value;
            if (CheckType(checkType, EnumCpghMenuType.Chi))
            {
                CheckChi(value, seat);
            }
            //绝杠特殊显示
            if (CheckType(checkType, EnumCpghMenuType.Peng))
            {
                if (value.Equals(App.GetGameManager<Mahjong2DGameManager>().FanNum) && Data.IsGangSelect)
                {
                    checkType |= (int)EnumCpghMenuType.Peng;
                    checkType ^= (int)EnumCpghMenuType.Peng;
                    checkType |= (int)EnumCpghMenuType.JueGang;
                }
            }
            if (CheckType(checkType, EnumCpghMenuType.LiGang))
            {
                GangList = Manager.LiGangList.ToList();
            }
            if (CheckType(checkType, EnumCpghMenuType.Gang))
            {
                FindGang();
            }
            if (CheckType(checkType,EnumCpghMenuType.JueGang))//如果存在绝杠菜单则隐藏杠按钮（绝杠属于特殊的碰）
            {
                checkType |= (int)EnumCpghMenuType.Gang;
                checkType ^= (int)EnumCpghMenuType.Gang;
            }

            if (Manager.ShowFengWithNum&& CheckType(checkType, EnumCpghMenuType.FengGang))
            {
               checkType |= (int)EnumCpghMenuType.FengGang;
               checkType ^= (int)EnumCpghMenuType.FengGang;
               var fengCards=GetFengHandCards();
               var threeFengList=GetMatchFengList(fengCards,true,3);
               var fourFengList= GetMatchFengList(fengCards, true,4);
               if (threeFengList.Count>0)
               {
                   checkType |= (int)EnumCpghMenuType.ThreeFengGang;
               }
               if (fourFengList.Count > 0)
               {
                   checkType |= (int)EnumCpghMenuType.FourFengGang;
               }
            }
            Menus.EnableCondition(checkType);
            InteraptMenu.Instance.ShowMenu(Menus, DEF_MENU_TIMEOUT);
        }

        /// <summary>
        /// 检测托管状态菜单显示
        /// </summary>
        /// <param name="checkType"></param>
        /// <returns></returns>
        private bool CheckAutoMenuState(ref int checkType)
        {
            var value = 0;
            foreach (var showType in AutoShowTypes)
            {
                if (CheckType(checkType, showType))
                {
                    value |= (int) showType;
                }
            }
            checkType = value;
            return value==0;
        }

        /// <summary>
        /// 检测状态
        /// </summary>
        /// <param name="checkValue">检测值</param>
        /// <param name="checkState">检测状态</param>
        /// <returns></returns>
        private bool CheckType(int checkValue, EnumCpghMenuType checkState)
        {
            var stateVlaue = (int)checkState;
            return (checkValue & stateVlaue) == stateVlaue;
        }

        private void FindGang()
        {
            GangList.Clear();
            FindAnGang();
            FindZhuaGang();
        }

        public void HideMenu()
        {
            Menus.DisableAll();
            ShowChiPaiInfo.Instance.Show(false);
            ShowGangPaiInfo.Instance.Show(false);
        }

        // 吃牌判断 不需要吃的玩法可以在子类中重写改函数 吃需要有左右中三种吃提示
        protected virtual void CheckChi(int value, int seat)
        {
            CheckCard = value;
            ChiGroup.Clear();
            // 往右吃 ，如 value = 35,手牌有33，34 有3条及以上可以被右吃            
            if (TypeDic.ContainsKey(value - 1) && TypeDic.ContainsKey(value - 2))
            {
                int[] chiCardsCod3 = new int[3];
                chiCardsCod3[0] = value - 2;
                chiCardsCod3[1] = value - 1;
                chiCardsCod3[2] = value;
                if (CheckChiCardsWithLaizi(chiCardsCod3))
                {
                    ChiGroup.Add(chiCardsCod3);
                }
            }

            // 中间吃，比如，value =34,手牌有33，35 保证范围不会超出 这类型牌的牌值范围
            if (TypeDic.ContainsKey(value - 1) && TypeDic.ContainsKey(value + 1))
            {
                int[] chiCardsCod2 = new int[3];
                chiCardsCod2[0] = value - 1;
                chiCardsCod2[1] = value;
                chiCardsCod2[2] = value + 1;
                if (CheckChiCardsWithLaizi(chiCardsCod2))
                {
                    ChiGroup.Add(chiCardsCod2);
                }
            }

            // 往左吃，如 value = 39 ,手牌 40，41
            if (TypeDic.ContainsKey(value + 1) && TypeDic.ContainsKey(value + 2))
            {
                int[] chiCardsCod1 = new int[3];
                chiCardsCod1[0] = value;
                chiCardsCod1[1] = value + 1;
                chiCardsCod1[2] = value + 2;
                if (CheckChiCardsWithLaizi(chiCardsCod1))
                {
                    ChiGroup.Add(chiCardsCod1);
                }
            }
        }

        bool CheckChiCardsWithLaizi(int[] cards)
        {
            bool state = true;
            for (int i = 0; i < cards.Length; i++)
            {
                if (cards[i] == App.GetGameManager<Mahjong2DGameManager>().LaiZiNum)
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
            YxDebug.Log("------> MahjongPlayerControl.OnChiClick() count = " + ChiGroup.Count);

            if (ChiGroup != null && ChiGroup.Count > 0)
            {
                //如果只有一种可能，直接吃
                if (ChiGroup.Count == 1)
                {
                    OnClickChiG(ShowChiPaiInfo.Instance.ChiZone[0]);
                    return;
                }

                ShowChiPaiInfo.Instance.SetChiInfos(ChiGroup);
            }
            else
            {
                YxDebug.LogError("当前没有可以吃的情况，猜测为癞子牌相关的错误。请咨询服务器");
            }
        }

        //获得要组合吃的那两张牌
        private int[] GetChiGpcards(int[] cardsTmp)
        {
            int[] finalChi = new int[2];
            int j = 0;
            for (int i = 0; i < 3; i++)
            {
                if (finalChi.Length <= j)
                {
                    break;
                }
                if (cardsTmp[i] != CheckCard)
                {
                    finalChi[j] = cardsTmp[i];
                    j++;
                }
            }
            return finalChi;
        }

        /// <summary>
        /// 取消听选择
        /// </summary>
        public void OnClickTingCancel()
        {
            MahjongEnv.ClearHandFlag();
            AddHandCardsControl();
            ReShowMenu();
            Data.IsClickTing = false;
        }

        protected virtual void ToSelectFengOut(bool widthNum,int num)
        {
            var fengList = GetFengHandCards();
            var list= GetMatchFengList(fengList,widthNum,num);
            if (list.Count>1)
            {
                ShowMessageInfo info=new ShowMessageInfo()
                {
                    ShowType = ShowMessageType.MulTypeGroup,
                    GroupCardsInfo=FengGangHelper.Instance.GetDataByInfos(list)
                };
                InteraptMenu.Instance.Revert();
                ShowInfoWithCard.Instance.ShowMessageWithInfo(info, sure =>
                {
                    var getList= sure as List<int>;
                    if (getList!=null)
                    {
                        FengGangRequest(getList);
                    }
                    else
                    {
                        ReShowMenu();
                    }
                }, delegate
                {
                    ReShowMenu();
                });
            }
            else if (list.Count==1)
            {
                var first= list.First();
                FengGangRequest(FengGangHelper.Instance.GetDataByInfo(first));
            }
        }

        /// <summary>
        /// 获取满足条件风杠的牌
        /// </summary>
        /// <param name="fengHandCards"></param>
        /// <param name="widthNum"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        private List<string> GetMatchFengList(List<int> fengHandCards,bool widthNum, int num)
        {
            return FengGangHelper.Instance.CheckValueType(fengHandCards, widthNum, num);
        }

        /// <summary>
        /// 获取手牌中的风牌
        /// </summary>
        /// <returns></returns>
        private List<int> GetFengHandCards()
        {
            Predicate<int> match = value => value >= (int)EnumMahjongValue.Dong && value <= (int)EnumMahjongValue.Bai;
            var mathchList = HandCardList.FindAll(match);
            mathchList.Sort();
            return mathchList;
        }

        private void FengGangRequest(List<int> gangValues)
        {
            SFSObject request = GameTools.getSFSObject((int)EnumRequest.XFG);
            request.PutIntArray(RequestKey.KeyCards, gangValues.ToArray());
            App.GetGameManager<Mahjong2DGameManager>().SendDataToServer(request);
        }

        protected virtual List<Transform> SetHandCardsValid(Predicate<Transform> match)
        {
            MahjongEnv.SetHandCardsInvalid();
            var matchList=MahjongEnv.HandCards.FindAll(match);
            MahjongEnv.SetItemsColor(Color.white, matchList);
            foreach (var matchItem in matchList)
            {
                AddUserControl(matchItem);
            }
            return matchList;
        }

        public void ReShowMenu()
        {
            Manager.ReCheckOp();
        }

        private void AddHandCardsControl()
        {
            List<Transform> hands = MahjongEnv.HandCards;
            foreach (var trans in hands)
            {
                AddUserControl(trans.GetComponent<MahjongItem>());
            }
        }

        private void OnClickChiG(GameObject button)
        {
            int clickNum = int.Parse(button.name);
            int[] finalChi = GetChiGpcards(ChiGroup[clickNum]);                 // 获得组合吃的那两张牌
            YxDebug.Log(string.Format("用来吃的牌是：{0}", (EnumMahjongValue)CheckCard));
            foreach (var chiitem in finalChi)
            {
                YxDebug.Log((EnumMahjongValue)chiitem);
            }
            YxDebug.Log("吃的牌是：" + (EnumMahjongValue)CheckCard);
            ClearMenu();
            SFSObject request = GameTools.getSFSObject((int)EnumRequest.CPHType);
            request.PutInt(RequestKey.KeyTypeType, (int)EnumCpgType.Chi);
            request.PutInt(RequestKey.KeyOpCard, CheckCard);
            request.PutIntArray(RequestKey.KeyCards, finalChi);
            SendRequest(request);
            ShowChiPaiInfo.Instance.Show(false);
        }

        // 用户点击了碰牌 没有太多操作 直接发送请求
        public void OnPengClick()
        {
            ClearMenu();
            SFSObject request = GameTools.getSFSObject((int)EnumRequest.CPHType);
            request.PutInt(RequestKey.KeyTypeType, (int)EnumCpgType.Peng);
            request.PutInt(RequestKey.KeyOpCard, CheckCard);
            request.PutIntArray(RequestKey.KeyCards, new[] { CheckCard, CheckCard });
            SendRequest(request);
        }

        // 用户点击了杠，杠别人还是杠自己，如果是当前有token,说明是自己抓的，明杠、暗杠
        public void OnGangClick()
        {
            OnGangClick(false);
        }

        private void OnGangClick(bool isLiGang)
        {
            //自己出牌阶段，是明杠或者抓到一个碰过的牌
            if (HasToken)
            {
                //如果不止一个能杠，那么要出菜单选择一个杠的牌 特别的：一个暗杠一个抓杠
                int len = GangList.Count;
                YxDebug.Log("当前条件下的gang的数量是：" + len);
                if (len > 1)
                {
                    YxDebug.Log(string.Format("当前的杠数量是：{0}", GangList.Count));
                    foreach (var gang in GangList)
                    {
                        YxDebug.Log(string.Format("杠的值是：{0}", (EnumMahjongValue)gang));
                    }
                    InteraptMenu.Instance.Revert(); //多选择的杠不清除杠标记状态，可以点击其它位置取消操作
                    ShowGangPaiInfo.Instance.ShowInfo(GangList);
                }
                else if (len == 0)
                {
                    YxDebug.LogError("当前没有可以杠的值，请检查客户端代码");
                }
                else
                {
                    //自己手里有
                    SelfGang(GangList[0], isLiGang);
                }
            }
            //别人出牌，自然是自己3个杠别人
            else
            {
                SFSObject sfsObject = GameTools.getSFSObject((int)EnumRequest.CPHType);
                sfsObject.PutInt(RequestKey.KeyTypeType, (int)EnumCpgType.PengGang);
                sfsObject.PutInt(RequestKey.KeyOpCard, CheckCard);
                sfsObject.PutIntArray(RequestKey.KeyCards, new[] { CheckCard, CheckCard, CheckCard });
                SendRequest(sfsObject);
            }
        }

        public void OnTingClick()
        {
            if (App.GetGameManager<Mahjong2DGameManager>().TingOutCards.Count > 0)
            {
                if (HasToken)
                {
                    Data.IsClickTing = true;
                    ToSelectTingOut();
                }
            }
            else
            {
                Debug.LogError("tingOut 的数量是0，问问服务器咋回事");
            }
        }

        public void OnJueClick()
        {
            YxDebug.Log("绝杠");
            ClearMenu();
            CheckCard = App.GetGameManager<Mahjong2DGameManager>().FanNum;
            ISFSObject data;
            if (HasToken)
            {
                data = GameTools.getSFSObject((int)EnumRequest.JueGang);
                data.PutInt(RequestKey.KeyOpCard, CheckCard);
                data.PutIntArray(RequestKey.KeyCards, new[] { CheckCard, CheckCard, CheckCard });
            }
            else
            {
                data = GameTools.getSFSObject((int)EnumRequest.CPHType);
                data.PutIntArray(RequestKey.KeyCards, new[] { CheckCard, CheckCard });
                data.PutInt(RequestKey.KeyTypeType, (int)EnumCpgType.Peng);
                data.PutInt(RequestKey.KeyOpCard, CheckCard);
            }
            SendRequest(data, false);
        }

        public void OnCancelClick()
        {
            Menus.DisableAll();
            if (Manager.CurrentPosition != UserSeat)
            {
                SFSObject sfsObject = GameTools.getSFSObject((int)EnumRequest.CPHType);
                sfsObject.PutInt(RequestKey.KeyTypeType, (int)EnumCpgType.None);
                SendRequest(sfsObject);
            }
            else
            {
                if (HasTing||Data.IsInRobot)
                {
                    ThrowCardAuto();
                }
            }
        }

        private void OnSelectGangItem(int value)
        {
            CheckCard = value;
            SelfGang(value,false);
        }

        protected virtual void ToSelectTingOut()
        {
            List<int> outList = Manager.TingOutCards;
            var tingCount = outList.Count;
            if (tingCount==1)
            {
                var tingValue = outList[0];
                MahjongItem selectItem = GetHandCard(tingValue);
                if (selectItem)
                {
                    OnMahjongClick(selectItem.transform);
                }
            }
            else
            {
                MahjongEnv.SetHandCardsInvalid();
                InteraptMenu.Instance.Revert();
                InteraptMenu.Instance.ShowTing(true);
                for (int i = 0; i < tingCount; i++)
                {
                    int checkValue = outList[i];
                    MahjongItem selectItem = GetHandCard(checkValue);
                    if (selectItem != null)
                    {
                        selectItem.SetColor(Color.white);
                        AddUserControl(selectItem);
                    }
                    else
                    {
                        YxDebug.LogError(string.Format("TingOut牌中的牌：{0}值是：{1}，不存在于实际手牌中", (EnumMahjongValue)checkValue, checkValue));
                    }
                }
            }

        }
        private void SelfGang(int card,bool isLiGang)
        {
            SFSObject sfsObject = SFSObject.NewInstance();
            if (_typeDic.ContainsKey(card) && _typeDic[card] > 3)
            {
                sfsObject.PutInt(RequestKey.KeyTypeType, (int)EnumCpgType.AnGang);
                sfsObject.PutIntArray(RequestKey.KeyCards, new[] { card, card, card, card });
                sfsObject.PutInt(RequestKey.KeyOpCard, card);
            }
            else
            {
                sfsObject.PutInt(RequestKey.KeyTypeType, (int)EnumCpgType.ZhuaGang);
                sfsObject.PutIntArray(RequestKey.KeyCards, new[] { card });
                sfsObject.PutInt(RequestKey.KeyOpCard, card);
            }
            if (Manager.LiGangList.Count>0&& isLiGang)
            {
                sfsObject.PutInt(RequestKey.KeyType, (int)EnumRequest.LiGang);
                Manager.LiGangList.Clear();
            }
            else
            {
                sfsObject.PutInt(RequestKey.KeyType, (int)EnumRequest.SelfGang);
            }
            SendRequest(sfsObject);
            ClearMenu();
        }
        // 打牌
        protected virtual void OnMahjongClick(Transform item)
        {
            MahjongItem mahjongItem = item.GetComponent<MahjongItem>();
            UserContorl userContorl = item.GetComponent<UserContorl>();
            if (mahjongItem.Value == GetLaiZiValue()
                || App.GetGameManager<Mahjong2DGameManager>().GetLeftNum() < 0
                || !Menus.IsForbiddenAll() && !Data.IsClickTing)
            {
                if (userContorl)
                {
                    userContorl.OnRecoveryPos();
                }          
                return;
            }
            if (HasToken && CanThrowCard)
            {
                MjClick(mahjongItem);
            }
            else
            {
                if (userContorl != null)
                {
                    userContorl.OnRecoveryPos();
                }
            }
        }

        public virtual void OnClickLiGang()
        {
            OnGangClick(true);
        }


        // 用户选择胡牌
        public virtual void OnHuClick()
        {
            ClearMenu();
            int type = HasToken ? (int)EnumRequest.ZiMo : (int)EnumRequest.CPHType;
            SFSObject sfsObject = GameTools.getSFSObject(type);
            if (!HasToken)
                sfsObject.PutInt(RequestKey.KeyTypeType, (int)EnumRequest.Hu);
            SendRequest(sfsObject);
        }


        protected void SendRequest(ISFSObject request, bool cleanToken = true)
        {
            App.GetGameManager<Mahjong2DGameManager>().SendDataToServer(request);
        }


        // ----------------------------------- 游戏结束   
        public int GetLaiZiValue()
        {
            return App.GetGameManager<Mahjong2DGameManager>().LaiZiNum;
        }
        /// <summary>
        /// 手牌排序
        /// </summary>
        /// <returns></returns>
        protected virtual List<int> SortCardValueList()
        {
            //按照牌值，给牌面排序
            HandCardList = GameTools.SortCardWithLaiZi(HandCardList, Manager.LaiZiNum);
            SortType();
            return HandCardList;
        }

        /// <summary>
        /// 确定会牌后需要重新排序一下SortHandCard
        /// </summary>
        public void SortCardOnSureLaizZi()
        {
            SortHandCard();
        }
        /// <summary>
        /// 设置最后一张抓进手里的牌
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
        /// 筛选处理，可以快速的碰和杠
        /// </summary>
        private void SortType()
        {
            _typeDic.Clear();
            int len = HandCardList.Count;
            int singleNum = -1;
            for (int i = 0; i < len; i++)
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

        protected MulSetting Menus = new MulSetting(Enum.GetValues(typeof(EnumCpghMenuType)).Length, 0);

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
        /// 过
        /// </summary>
        protected virtual void ResponseOtherThrow()
        {
            SFSObject param = GameTools.getSFSObject((int)EnumRequest.CPHType);
            param.PutInt(RequestKey.KeyTypeType, (int)EnumCpgType.None);
            App.GetRServer<Mahjong2DGameServer>().SendGameRequest(param);
        }

        public override void ShowTingWithEffect(bool withEffect)
        {
            base.ShowTingWithEffect(withEffect);
            MahjongEnv.SetHandCardsInvalid();
        }

        // 回合结束，清理菜单        
        public void ClearMenu()
        {
            Menus.DisableAll();
            InteraptMenu.Instance.Revert();
        }


        public override void SetHandCardOnTing(bool hasTing)
        {
        }

        public override void AllowRequest(RequestData request, bool showBehavior)
        {
            base.AllowRequest(request, showBehavior);
            if (Manager)
            {
                Manager.ClearFlagCard();
            }
        }

        public override void OnGetXFGang(RequestData requeset)
        {
            base.OnGetXFGang(requeset);
            if (Manager)
            {
                Manager.ClearFlagCard();
            }
        }

        public override void CheckTingState()
        {
            ThrowCardAuto();
        }
        /// <summary>
        /// menu中的点击事件（彩杠）
        /// </summary>
        /// <param name="button"></param>
        public void OnClickCg(GameObject button)
        {
            int clickNum = int.Parse(button.name);
            SFSObject request = GameTools.getSFSObject((int)EnumRequest.CaiGang);
            request.PutIntArray(RequestKey.KeyCards, _specialGangList[clickNum]);
            SendRequest(request);
            _specialGangList.Remove(_specialGangList[clickNum]);
            ShowGangPaiInfo.Instance.Show(false);
            ClearMenu();
        }
        /// <summary>
        /// menu中的点击事件（旋风刚）
        /// </summary>
        /// <param name="button"></param>
        public void OnClickXfg(GameObject button)
        {
            int clickNum = int.Parse(button.name);
            SFSObject request = GameTools.getSFSObject((int)EnumRequest.XFG);
            request.PutIntArray(RequestKey.KeyCards, _specialGangList[clickNum]);
            SendRequest(request);
            _specialGangList.Remove(_specialGangList[clickNum]);
            ShowGangPaiInfo.Instance.Show(false);
            ClearMenu();
        }
        /// <summary>
        /// 彩杠的点击事件
        /// </summary>
        public void OnCaiGangClick()
        {
            int zhongNum = 0;
            int faNum = 0;
            int baiNum = 0;
            _specialGangList.Clear();
            foreach (int t in HandCardList)
            {
                switch (t)
                {
                    case (int)EnumMahjongValue.Zhong:
                        zhongNum++;
                        break;
                    case (int)EnumMahjongValue.Fa:
                        faNum++;
                        break;
                    case (int)EnumMahjongValue.Bai:
                        baiNum++;
                        break;
                }
            }
            if ((zhongNum + faNum + baiNum) == 4)
            {
                if (zhongNum > 1)
                {
                    _specialGangList.Add(ConstantData.CgFourZhong);
                }
                else if (faNum > 1)
                {
                    _specialGangList.Add(ConstantData.CgFourFa);
                }
                else if (baiNum > 1)
                {
                    _specialGangList.Add(ConstantData.CgFourBai);
                }
            }
            else if ((zhongNum + faNum + baiNum) == 5)
            {
                if (zhongNum > 1 && faNum > 1)
                {
                    _specialGangList.Add(ConstantData.CgFourZhong);
                    _specialGangList.Add(ConstantData.CgFourFa);
                }
                else if (faNum > 1 && baiNum > 1)
                {
                    _specialGangList.Add(ConstantData.CgFourFa);
                    _specialGangList.Add(ConstantData.CgFourBai);
                }
                else if (zhongNum > 1 && baiNum > 1)
                {
                    _specialGangList.Add(ConstantData.CgFourZhong);
                    _specialGangList.Add(ConstantData.CgFourBai);
                }
            }
            else if ((zhongNum + faNum + baiNum) >= 6)
            {
                _specialGangList.Add(ConstantData.CgFourZhong);
                _specialGangList.Add(ConstantData.CgFourFa);
                _specialGangList.Add(ConstantData.CgFourBai);
            }
            if (_specialGangList.Count == 1)
            {
                SFSObject request = GameTools.getSFSObject((int)EnumRequest.CaiGang);
                request.PutIntArray(RequestKey.KeyCards, _specialGangList[0]);
                SendRequest(request);
                _specialGangList.Remove(_specialGangList[0]);
                ShowGangPaiInfo.Instance.Show(false);
                ClearMenu();
                YxDebug.Log("单个彩杠直接发送");
            }
            else
            {
                ShowSpecialGangInfo.Instance.ShowSpecialGang(_specialGangList, OnClickCg);//当玩家不点刚一直攒着扛得时候做的特殊处理
            }
        }


        /// <summary>
        /// 点击旋风杠之后的桌面显示(菜单)
        /// </summary>
        public void OnXuanFengGangClick()
        {
            _specialGangList.Clear();
            int laiZiNum = App.GetGameManager<Mahjong2DGameManager>().LaiZiNum;
            TryAddFengList(ConstantData.XfgFour, laiZiNum);
            TryAddFengList(ConstantData.XfgThree, laiZiNum);
            if (_specialGangList.Count == 1)
            {
                SFSObject request = GameTools.getSFSObject((int)EnumRequest.XFG);
                request.PutIntArray(RequestKey.KeyCards, _specialGangList[0]);
                SendRequest(request);
                _specialGangList.Remove(_specialGangList[0]);
                ShowGangPaiInfo.Instance.Show(false);
                ClearMenu();
            }
            else
            {
                ShowSpecialGangInfo.Instance.ShowSpecialGang(_specialGangList, OnClickXfg);
            }
        }

        /// <summary>
        /// 旋风杠手牌处理
        /// </summary>
        public void OnFengGangClick()
        {
            ToSelectFengOut(false,0);
        }

        /// <summary>
        /// 四风杠按钮点击
        /// </summary>
        public void OnSiFengGangClick()
        {
            ToSelectFengOut(true, 4);
        }

        /// <summary>
        /// 三风杠按钮点击
        /// </summary>
        public void OnThreeFengGangClick()
        {
            ToSelectFengOut(true, 3);
        }

        private void TryAddFengList(int[] arr, int laiZiNum)
        {
            bool state = true;
            foreach (var item in arr)
            {
                if (!HandCardList.Contains(item) || item.Equals(laiZiNum))
                {
                    state = false;
                    break;
                }
            }
            if (state)
            {
                _specialGangList.Add(arr);
            }
        }
        /// <summary>
        /// 点击潇洒按钮之后
        /// </summary>
        public void OnXiaoSaClick()
        {
            if (App.GetGameManager<Mahjong2DGameManager>().TingOutCards.Count > 0)
            {
                if (HasToken)
                {
                    Data.IsXiaoSa = true;
                    ToSelectTingOut();
                }
            }
            else
            {
                Debug.LogError("xiaoshatingOut 的数量是0，问问服务器咋回事");
            }
        }

        protected override void GetDealGetInShowItem()
        {
            var laiZiValue = GetLaiZiValue();
            var findIndex = HandCardList.FindIndex(item=>item!=laiZiValue);
            if (findIndex == -1)
            {
                LastGetValue = HandCardList.First();
            }
            else
            {
                LastGetValue = HandCardList[findIndex];
            }
            base.GetDealGetInShowItem();
        }


    }
}
